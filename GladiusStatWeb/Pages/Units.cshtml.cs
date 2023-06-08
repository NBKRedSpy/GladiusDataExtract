using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using GladiusDataExtract.Extract.Units;
using GladiusDataExtract.Extract.Weapons;
using GladiusStatWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoreLinq;

namespace GladiusStatWeb.Pages
{
    public class UnitsModel : PageModel
	{

		public List<Unit> Units { get; init; }

		private static List<string> UnitMainAttributeFilter = new ()
				{
					"armor",
					"hitpointsMax",
					"movementMax",
					"biomassCost",
					"productionCost",
					"biomassUpkeep",
					"influenceUpkeep",
				};

		static UnitsModel()
		{

		}
		
		public UnitsModel(GladiusDataService dataService)
        {
            Units = dataService.GladiusUnits;
        }


        public List<UnitAttribute> GetDisplayAttributes(Unit unit)
        {
			return UnitsModel.UnitMainAttributeFilter.Join(unit.Attributes, x => x, x => x.Name, (outer, inner) => inner).ToList();
		}


		

		public int GetAccuracyChance(int accuracy)
		{
			return (int)(Math.Min((accuracy / 12m), 1m) * 100);
		}


		public void OnGet()
        {

        }
    }
}
