﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Units;
using MoreLinq;

namespace GladiusDataExtract.Weapons
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="Effects"></param>
	/// <param name="Requirements"></param>
	/// <param name="Traits">The traits for the weapon.  For example, Melee or IgnoresCover</param>
	public record Weapon(string Name, int targetRange, List<Effect> Effects, List<Requirement> Requirements,
		List<string> Traits)
	{
		private static Dictionary<string, int> WeaponAttributeDisplayOrder = new() {
			{"rangedDamage", 1},
			{"meleeDamage", 2},
			{"strengthDamage", 3},
			{"attacks", 4},
			{"meleeAttacks", 4},
			{"rangedArmorPenetration", 5},
			{"meleeArmorPenetration", 6},
			{"Accuracy", 7},
			{"meleeAccuracy",8},
			{"Range", 9 },
		};

		/// <summary>
		/// Computes the stats for a weapon for a unit.
		/// </summary>
		/// <param name="weapon"></param>
		/// <returns></returns>
		public List<Tuple<string, decimal>> GetWeaponStats(Unit unit)
		{

			IEnumerable<(Effect Effect, decimal UnitValue)> weaponUnitAttributes = Effects.LeftJoin(
				unit.Attributes,
				x => x.Name,
				x => x.Name,
				(left) => (Effect: left, UnitValue: 0m),
				(left, right) => (Effect: left, UnitValue: right.Value));

			List<Tuple<string, decimal>> modifierAppliedWeaponAttributes = weaponUnitAttributes
				.Select(x => new Tuple<string, decimal>(x.Effect.Name, x.Effect.ApplyModifiers(x.UnitValue)))
				.ToList();

			if (Traits.Contains("Melee"))
			{
				AddMissingAttribute(modifierAppliedWeaponAttributes, unit, "meleeAccuracy");
				AddMissingAttribute(modifierAppliedWeaponAttributes, unit, "meleeAttacks");
				AddMissingAttribute(modifierAppliedWeaponAttributes, unit, "strengthDamage");
			}
			else
			{
				//Assume range - the weapon doesn't seem to have a range specific trait.

				AddMissingAttribute(modifierAppliedWeaponAttributes, unit, "rangedArmorPenetration");
				AddMissingAttribute(modifierAppliedWeaponAttributes, unit, "rangedDamage");
			}

			//Sort to match the UI's order.
			modifierAppliedWeaponAttributes = modifierAppliedWeaponAttributes
				.OrderBy(x =>
					WeaponAttributeDisplayOrder.TryGetValue(x.Item1, out int order) ? order : int.MaxValue)
				.ToList();

			return modifierAppliedWeaponAttributes;
		}

		/// <summary>
		/// Adds the attribute from the unit if the source list doesn't already contain the entry.
		/// </summary>
		/// <param name="sourceList"></param>
		/// <param name="unit"></param>
		/// <param name="attributeName"></param>
		private void AddMissingAttribute(List<Tuple<string, decimal>> sourceList, Unit unit, string attributeName)
		{
			if (sourceList.Any(x => x.Item1 == attributeName) == false)
			{
				var unitAttribute = unit.Attributes.SingleOrDefault(x => x.Name == attributeName);

				if (unitAttribute != null)
				{
					sourceList.Add(new(unitAttribute.Name, unitAttribute.Value));
				}
			}
		}

	}

}
