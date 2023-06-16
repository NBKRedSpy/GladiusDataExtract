using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Entities;
using en = GladiusDataExtract.Entities;

namespace GladiusDataExtract.Extract
{

    /// <summary>
    /// A trait of the unit.
    /// </summary>
    /// <param name="icon">The icon to show</param>
    /// <param name="Key">The key for the item.  Ex: Traits/MeleeDamage</param>
    /// <param name="Name">The friendly name for display.  EG:  Heavy Bolter</param>
    /// <param name="description">The description of the trait.</param>
    ///  <param name="Effects">The effects of the trait (if any).  Ex:  add 1 armorPenetration</param>
    /// <param name="RequiredUpgrade">If not null, this is a trait that occurs after an upgrade.</param>
    /// 

    [DebuggerDisplay("Trait = {Key}")]
	internal class Trait : en.ILocalizedStrings
	{
		public string Key { get; set; } = "";
		public string Name { get; set; } = "";
		public string Description { get; set; } = "";
		public string Icon { get; set; } = "";
		public List<Effect> Effects { get; set; } = new List<Effect>();
		public Upgrade? RequiredUpgrade { get; set; }
	}
}
