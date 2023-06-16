using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract;

namespace GladiusDataExtract.Entities
{
	[DebuggerDisplay("Trait = {Key}")]
	public class Trait : ILocalizedStrings
	{
		public string Key { get; set; } = "";
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string Icon { get; set; } = "";

		public Upgrade?  RequiredUpgrade { get; set; }
	}
}
