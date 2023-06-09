using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
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

		public void OnGet()
        {

        }
    }
}
