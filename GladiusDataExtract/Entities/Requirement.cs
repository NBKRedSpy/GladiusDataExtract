using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// A requirement for a trait.  
	/// Ex:  Enhanced damage for Fleshborer requires the Tyranids/ShortRangedDamage upgrade.
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="RequiredUpgrade">If not null, this is a trait that occurs after an upgrade.</param>
	public class Requirement
	{
        public Requirement()
        {
			Name = "";
			RequiredUpgrade = "";
		}


		public Requirement(string name, string requiredUpgrade)
		{
			Name = name;
			RequiredUpgrade = requiredUpgrade;
		}

		public string Name { get; set; }
		public string RequiredUpgrade { get; set; }
	}
}
