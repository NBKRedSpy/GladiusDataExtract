using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// A trait of the unit.
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="RequiredUpgrade">If not null, this is a trait that occurs after an upgrade.</param>
	public class Trait
	{
        public Trait()
        {
            
        }
        public Trait(string name, string? requiredUpgrade)
		{
			Name = name;
			RequiredUpgrade = requiredUpgrade;
		}

		public string Name { get; set; } = "";
		public string? RequiredUpgrade { get; set; }
	}
}
