using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Extract.Weapons
{
	/// <summary>
	/// The requirement for a weapon to be available to the unit.
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="Requires"></param>
	internal record Requirement(string Name, string Requires)
    {

        public string FormatRequirements()
        {
            return Name == Requires ? Name : $"{Name} -> {Requires}";
        }
    }
}
