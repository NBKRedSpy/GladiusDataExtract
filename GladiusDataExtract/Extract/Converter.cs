using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Entities;
using du = GladiusDataExtract.Extract.Units;
using dw = GladiusDataExtract.Extract.Weapons;

namespace GladiusDataExtract.Extract
{
	/// <summary>
	/// A class to convert DTO's to project entities.
	/// </summary>
	public class Converter
	{

		
		/// <summary>
		/// Converts the DTO's data to Entity data.
		/// </summary>
		/// <param name="dtoUnits"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public List<Unit> ConvertUnits(List<du.Unit> dtoUnits, List<dw.Weapon> weapons)
		{

			Dictionary<string, dw.Weapon> weaponLookup = weapons.ToDictionary(x => x.Name);

			List<Unit> units = new List<Unit>();
			
			foreach (du.Unit dtoUnit in dtoUnits)
			{

				Unit unit = new();

				unit.Name = dtoUnit.Name;
				unit.Key = dtoUnit.Key;
				unit.ModelCount = dtoUnit.ModelCount;

				var attributes = new Dictionary<string, decimal>(
					dtoUnit.Attributes
						.Select(x => new KeyValuePair<string,decimal>(x.Name, x.Value)));

				unit.Armor = (int)attributes["armor"];
				unit.Morale = (int)attributes["morale"];
				unit.ProductionCost = (int)attributes["productionCost"];

				unit.ProductionResources = ConvertResources(attributes, "Cost");

				unit.Hitpoints = (int)attributes["hitpointsMax"];
				unit.Movement = (int)attributes["movementMax"];

				unit.Traits = dtoUnit.Traits.Select(x => new Requirement(x.Name, x.RequiredUpgrade!)).ToList();

				unit.Weapons = GetWeapons(unit, dtoUnit);
				
			}

			return units;
		}

		private List<Weapon> GetWeapons(Unit unit, du.Unit dtoUnit)
		{
			foreach (dw.Weapon dtoWeapon in dtoUnit.Weapons)
			{
				Dictionary<string, decimal> unitWeaponStats = dtoWeapon.GetWeaponStats(dtoUnit)
					.ToDictionary(x => x.Item1, x => x.Item2);

				Weapon weapon = new();
				weapon.Name = dtoWeapon.Name;
				weapon.Key = dtoWeapon.Key;
				weapon.Range = dtoWeapon.weaponRange;
				weapon.AttackCount = unitWeaponStats["attacks"];


				//melee or range stats.
				if(weapon.IsRangedWeapon)
				{
					weapon.Accuracy = unitWeaponStats["rangedAccuracy"];
					weapon.ArmorPenetration = unitWeaponStats["rangedArmorPenetration"];
					weapon.Damage = unitWeaponStats["rangedDamage"];
				}
				else
				{
					decimal strengthDamage = unitWeaponStats.GetValueOrDefault("strengthDamage");

					//Damage - for melee strengthDamage is supposed to be the preferred stat,
					//	but some legacy entries still use meleeDamage and set the strenghDamage to zero.
					if(strengthDamage == 0)
					{
						weapon.Damage = unitWeaponStats["meleeDamage"];
					}
					else
					{
						weapon.Damage = strengthDamage;
					}

					weapon.Accuracy = unitWeaponStats["meleeAccuracy"];
					weapon.ArmorPenetration = unitWeaponStats["meleeArmorPenetration"];
				}

				weapon.Traits = dtoWeapon.Traits;
				weapon.Requirements = dtoWeapon.Requirements.Select(x => new Requirement(x.Name, x.Requires))
					.ToList();

				//if((var attribute = unitWeaponStats.SingleOrDefault(x=> x.Item1)
			}

			throw new NotImplementedException();
		}



		private Resources ConvertResources(Dictionary<string, decimal> attributes, string suffix)
		{
			Resources resources = new Resources();

			resources.BioMass = attributes.GetValueOrDefault("biomass" + suffix);
			resources.Energy = attributes.GetValueOrDefault("energy" + suffix);
			resources.Food = attributes.GetValueOrDefault("food" + suffix);
			resources.Influence = attributes.GetValueOrDefault("influence" + suffix);
			resources.Ore = attributes.GetValueOrDefault("ore" + suffix);

			return resources;
		}
	}
}
