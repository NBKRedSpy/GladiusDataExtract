using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// An upgrade entry.  For instance, a weapon may require a certain upgrade before 
	/// it is available.  This is the upgrade.
	/// </summary>
	public class Upgrade : ILocalizedStrings
	{
		public string Key { get; set; } = "";
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";

		public string Icon { get; set; } = "";
    }
}
