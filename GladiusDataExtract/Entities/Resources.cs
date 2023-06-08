using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// The resources required.  Used for building and upkeep info.
	/// </summary>
	public class Resources
	{
		public decimal BioMass { get; set; } = 0;
		public decimal Energy { get; set; } = 0;
		public decimal Ore { get; set; } = 0;
		public decimal Food { get; set; } = 0;
		public decimal Influence { get; set; } = 0;

	}
}
