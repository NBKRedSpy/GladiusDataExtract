using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        public static Dictionary<string,Faction> FactionLookup{get;}


        static Converter()
        {
			FactionLookup = Enum.GetNames<Faction>()
				.Zip(Enum.GetValues<Faction>())
				.Where(x=> x.Second != Faction.Invalid)
				.ToDictionary(x => x.First, x => x.Second);
		}


		/// <summary>
		/// Returns the Units from the game's data.
		/// </summary>
		/// <param name="localizationFolder">The folder to use for the localization text.  Ex:  "[steam dir]/Warhammer 40000 Gladius - Relics of War/Data/Core/Languages/English/"</param>
		/// <param name="dataFolder">The data folder for the game.
		/// Ex:  [steam dir]/Warhammer 40000 Gladius - Relics of War/Data</param>
		/// <exception cref="NotImplementedException"></exception>
		public List<Unit> ConvertData(string localizationFolder, string dataFolder)
		{
			Extractor extractor = new Extractor();
			Dictionary<string, string> factionLocallization;
			List<du.Unit> units = extractor.ExtractData(localizationFolder, dataFolder, out factionLocallization);

			return ConvertUnits(units);
		}


		/// <summary>
		/// Converts the DTO's data to Entity data.
		/// </summary>
		/// <param name="dtoUnits"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private List<Unit> ConvertUnits(List<du.Unit> dtoUnits)
		{

			List<Unit> units = new List<Unit>();
			
			foreach (du.Unit dtoUnit in dtoUnits)
			{

				if(dtoUnit.Traits.Any(x=> x.Name == "Artefact"))
				{
					continue;
				}

				Unit unit = new();

				unit.Name = dtoUnit.Name;
				unit.Key = dtoUnit.Key;
				unit.ModelCount = dtoUnit.ModelCount;

				unit.Faction = GetFaction(unit.Key);

				var attributes = new Dictionary<string, decimal>(
					dtoUnit.Attributes
						.Select(x => new KeyValuePair<string,decimal>(x.Name, x.Value)));

				unit.Armor = (int)attributes["armor"];

				unit.Morale = (int)attributes["moraleMax"];

				unit.IsFortification = dtoUnit.Traits.Any(x=> x.Name =="Fortification");

				if(!unit.IsFortification)
				{
					unit.ProductionCost = (int)attributes["productionCost"];
					unit.ProductionResources = ConvertResources(attributes, "Cost");
					unit.UpkeepResources = ConvertResources(attributes, "Upkeep");
					unit.Hitpoints = (int)attributes["hitpointsMax"];
				}

				unit.Movement = (int)attributes["movementMax"];

				unit.Traits = dtoUnit.Traits.Where(x => x.RequiredUpgrade is null)
					.Select(x => x.Name).ToList();

				unit.Upgrades = dtoUnit.Traits.Where(x => x.RequiredUpgrade is not null)
					.Select(x => new Requirement(x.Name, x.RequiredUpgrade!)).ToList();

				unit.Weapons = GetWeapons(dtoUnit);

				units.Add(unit);
				
			}

			return units;
		}

		private Faction GetFaction(string key)
		{
			string factionKey = key.Split("/")[0];

			if (FactionLookup.TryGetValue(factionKey, out Faction faction))
			{
				return faction;
			}
			else
			{
				throw new ArgumentOutOfRangeException($"Unexpected value '{factionKey}' for Faction");
			}
		}


		private List<Weapon> GetWeapons(du.Unit dtoUnit)
		{

			List<Weapon> weapons = new();

			foreach (du.UnitWeapon dtoUnitWeapon in dtoUnit.Weapons)
			{

				dw.Weapon dtoWeapon = dtoUnitWeapon.Weapon;

				//Ignore the "None" weapon.  Might just be to put a default weapon on the 3d model.
				if (dtoWeapon.Name == "None")
				{
					continue;
				}


				Weapon weapon = new();
				weapon.Name = dtoWeapon.Name;
				weapon.Key = dtoWeapon.Key;
				weapon.Range = dtoWeapon.weaponRange;
				weapon.RequiredUpgrade = dtoUnitWeapon.RequiredUpgrade;

				dtoWeapon.GetWeaponStats(dtoUnit, out List<Tuple<string, decimal>> unitWeaponStatsList , out bool isRanged);

				Dictionary<string, decimal> unitWeaponStats = unitWeaponStatsList.ToDictionary(x => x.Item1, x => x.Item2);

				weapon.AttackCount = unitWeaponStats.GetValueOrDefault("attacks", 1);


				//There are range 1 ranged weapons. Only way to tell is that 
				//there should be a ranged stat of some sort.
				if (isRanged)
				{
					weapon.Accuracy = unitWeaponStats.GetValueOrDefault("rangedAccuracy");

					//For some reason, some have no accuracy.  Seems to use melee Accuracy
					if(weapon.Accuracy == 0)
					{
						//Get from the unit's attributes since there were no operators.
						weapon.Accuracy = dtoUnit.Attributes.First(x => x.Name == "meleeAccuracy").Value;
					}
					weapon.ArmorPenetration = unitWeaponStats.GetValueOrDefault("rangedArmorPenetration", -1);
					if (weapon.ArmorPenetration == 0) weapon.ArmorPenetration = 1;	//Seems to default to 1.

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
					weapon.ArmorPenetration = unitWeaponStats.GetValueOrDefault("meleeArmorPenetration");
				}

				weapon.Traits = dtoWeapon.Traits;
				weapon.Requirements = dtoWeapon.Requirements.Select(x => new Requirement(x.Name, x.Requires))
					.ToList();

				weapons.Add(weapon);
			}

			return weapons;
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
