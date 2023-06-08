using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public List<Trait> Traits { get; set; } = new();
	}
}
