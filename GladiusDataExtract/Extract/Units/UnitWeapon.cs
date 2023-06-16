using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Weapons;
using e = GladiusDataExtract.Entities;

namespace GladiusDataExtract.Extract.Units
{
    internal class UnitWeapon
	{
        public Weapon Weapon { get; set; }
        public e.Upgrade? RequiredUpgrade { get; set; }

		public UnitWeapon(Weapon weapon, e.Upgrade? requiredUpgrade)
		{
			Weapon = weapon;
			RequiredUpgrade = requiredUpgrade;
		}
    }
}
