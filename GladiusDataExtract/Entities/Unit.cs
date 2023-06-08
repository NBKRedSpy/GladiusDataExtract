using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// A game unit such as Hormagaunts
	/// </summary>
	public class Unit
	{
		string Name { get; set; } = "";
		string Key { get; set; } = "";

		int Armor { get; set; } = 0;
		int Hitpoints { get; set; } = 0;
		int Movement { get; set; } = 0;
		int ProductionCost { get; set; } = 0;
		int ModelCount { get; set; } = 0;
	}



}
