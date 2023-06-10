using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Text;
using GladiusDataExtract.Entities;
using GladiusStatWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoreLinq;

namespace GladiusStatWeb.Pages
{
    public class UnitsModel : PageModel
	{

        public List<Unit> Units { get; init; }

		static UnitsModel()
		{

		}
		
		public UnitsModel(GladiusDataService dataService)
        {
            Units = dataService.GladiusUnits;
        }

        public string? Faction { get; private set; }

        public void OnGet(string? faction)
        {
			Faction = faction;
        }

		public string WeaponDamageFormat(Weapon weapon, Unit unit)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(weapon.Damage.ToString("#.##"));

			decimal multiplier = unit.ModelCount * weapon.AttackCount;

			if (multiplier != 1)
			{
				sb.Append($" (~ {(weapon.Damage * multiplier):#.##})");
			}

			return sb.ToString();
		}


		public string AttackFormat(Weapon weapon, Unit unit)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(weapon.AttackCount.ToString("#.##"));

			if (unit.ModelCount > 1)
			{
				sb.Append($" ( x{unit.ModelCount:#.##})");
			}

			return sb.ToString();
		}

		public string HitPointsFormat(Unit unit)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(unit.Hitpoints);

			if (unit.ModelCount > 1)
			{

				sb.Append($" ({unit.ModelCount * unit.Hitpoints})");
			}

			return sb.ToString();
		}

	}
}
