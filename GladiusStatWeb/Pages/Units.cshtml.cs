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
		public static List<string> WeaponIntersectAttributes { get;}
        

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
		/// Computes the weapon status for a unit's weapon.
		/// </summary>
		/// <param name="weapon"></param>
		/// <returns></returns>
		public List<Tuple<string, decimal>> GetWeaponStats(Unit unit, Weapon weapon)
		{



			List<Tuple<string, decimal>> weaponValues = new();


			var weaponUnitAttributes = weapon.Effects.LeftJoin(
				unit.Attributes,
				x => x.Name,
				x => x.Name,
				(left) => (Effect: left, UnitValue: 0m),
				(left, right) => (Effect: left, UnitValue: right.Value));

			return weaponUnitAttributes
				.Select(x => new Tuple<string,decimal>(x.Effect.Name, x.Effect.ApplyModifiers(x.UnitValue)))
				.ToList();

		}

		public void OnGet()
        {

        }
    }
}
