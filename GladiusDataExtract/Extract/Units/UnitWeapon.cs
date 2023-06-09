using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Weapons;

namespace GladiusDataExtract.Extract.Units
{
	internal class UnitWeapon
	{
        public Weapon Weapon { get; set; }
        public string RequiredUpgrade { get; set; }

		public UnitWeapon(Weapon weapon, string requiredUpgrade = "")
		{
			Weapon = weapon;
			RequiredUpgrade = requiredUpgrade;
		}
    }
}
