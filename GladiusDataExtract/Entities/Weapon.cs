using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Weapons;

namespace GladiusDataExtract.Entities
{
	public class Weapon
	{

		public string Name { get; set; } = "";

		public string Key { get; set; } = "";

		public decimal Damage { get; set; } = 0;

		public decimal AttackCount { get; set; } = 0;

		public decimal ArmorPenetration { get; set; } = 0;

		public decimal Accuracy { get; set; } = 0;


		/// <summary>
		/// The range of the weapon.  Will be zero for melee
		/// </summary>
		public int Range { get; set; } = 0;

		public int GetAccuracyChance(int accuracy)
		{
			return (int)(Math.Min((accuracy / 12m), 1m) * 100);
		}


		public List<string> Traits { get; set; } = new();
		public List<Requirement> Requirements { get; set; } = new();
	}
}
