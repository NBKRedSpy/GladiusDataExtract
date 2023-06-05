using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using GladiusDataExtract.Units;
using GladiusDataExtract.Weapons;
using GladiusStatWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoreLinq;

namespace GladiusStatWeb.Pages
{
	public class UnitsModel : PageModel
	{

		public List<Unit> Units { get; init; }

		/// <summary>
		/// The list of attributes that intersect the Unit's base values and weapon effects.
		/// </summary>
		public static List<string> WeaponIntersectAttributes { get; }


		private static Dictionary<string, int> WeaponAttributeDisplayOrder = new() {
			{"rangedDamage", 1},
			{"meleeDamage", 2},
			{"strengthDamage", 3},
			{"attacks", 4},
			{"rangedArmorPenetration", 5},
			{"meleeArmorPenetration", 6},
			{"Accuracy", 7},
			{"Range", 8 },
		};

		static UnitsModel()
		{
			WeaponIntersectAttributes = new()
			{
				"attacks",
				"meleeAccuracy",
				"meleeAttacks",
				"strengthDamage",
				"rangedDamage",
				"rangedArmorPenetration",
				"meleeArmorPenetration",
			};

		}
		
		public UnitsModel(GladiusDataService dataService)
        {
            Units = dataService.GladiusUnits;
        }


        public List<UnitAttribute> GetDisplayAttributes(Unit unit)
        {
			List<string> attributeNames = new()
				{
					"armor",
					"hitpointsMax",
					"movementMax",
					"biomassCost",
					"productionCost",
					"biomassUpkeep",
					"influenceUpkeep",
				};

			return attributeNames.Join(unit.Attributes, x => x, x => x.Name, (outer, inner) => inner).ToList();
		}


		/// <summary>
		/// Computes the weapon status for a unit's weapon based on a unit.
		/// </summary>
		/// <param name="weapon"></param>
		/// <returns></returns>
		public List<Tuple<string, decimal>> GetWeaponStats(Unit unit, Weapon weapon)
		{

			IEnumerable<(Effect Effect, decimal UnitValue)> weaponUnitAttributes = weapon.Effects.LeftJoin(
				unit.Attributes,
				x => x.Name,
				x => x.Name,
				(left) => (Effect: left, UnitValue: 0m),
				(left, right) => (Effect: left, UnitValue: right.Value));

			List<Tuple<string, decimal>> modifierAppliedWeaponAttributes = weaponUnitAttributes
				.Select(x => new Tuple<string, decimal>(x.Effect.Name, x.Effect.ApplyModifiers(x.UnitValue)))
				.ToList();

			if (weapon.Traits.Contains("Melee"))
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
					UnitsModel.WeaponAttributeDisplayOrder.TryGetValue(x.Item1, out int order) ? order : int.MaxValue)
				.ToList();

			return modifierAppliedWeaponAttributes;
		}

		/// <summary>
		/// Adds the attribute from the unit if the source list doesn't already contain the entry.
		/// </summary>
		/// <param name="sourceList"></param>
		/// <param name="unit"></param>
		/// <param name="attributeName"></param>
		private void AddMissingAttribute(List<Tuple<string,decimal>> sourceList, Unit unit, string attributeName)
		{
			if(sourceList.Any(x=> x.Item1 == attributeName) == false)
			{
				var unitAttribute = unit.Attributes.SingleOrDefault(x => x.Name == attributeName);

				if(unitAttribute != null)
				{
					sourceList.Add( new (unitAttribute.Name, unitAttribute.Value));
				}
			}
		}

		public void OnGet()
        {

        }
    }
}
