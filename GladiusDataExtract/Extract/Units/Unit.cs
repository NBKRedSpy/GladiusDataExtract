using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Weapons;

namespace GladiusDataExtract.Extract.Units
{

	/// <summary>
	/// Ex:  Armor: 6
	/// </summary>
	/// <param name="Name">The key for the attribute.  Ex:  meleeDamage</param>
	/// <param name="Value">The value for the attribute.  Ex: 1.5</param>
	internal record UnitAttribute(string Name, decimal Value);





    /// <summary>
    /// A unit
    /// </summary>
    /// <param name="Key">The key for this object.  For example "Tyranid/Hormagaunt"</param>
    /// <param name="Name"></param>
    /// <param name="ModelCount">The number of members in the unit</param>
    /// <param name="Attributes">Armor, hitpoints, etc.</param>
    /// <param name="Weapons">List of possible weapons for this unit</param>
    internal record Unit(string Name, string Key, int ModelCount, List<UnitAttribute> Attributes, List<UnitWeapon> Weapons, List<Trait> Traits);


}
