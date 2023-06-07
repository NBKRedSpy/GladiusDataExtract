using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Weapons;

namespace GladiusDataExtract.Units
{

    /// <summary>
    /// Ex:  Armor: 6
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Value"></param>
    public record UnitAttribute(string Name, decimal Value);



    /// <summary>
    /// A trait of the unit.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="RequiredUpgrade">If not null, this is a trait that occurs after an upgrade.</param>
    public record Trait(string Name, string? RequiredUpgrade);


    /// <summary>
    /// A unit
    /// </summary>
    /// <param name="Key">The key for this object.  For example "Tyranid/Hormagaunt"</param>
    /// <param name="Name"></param>
    /// <param name="ModelCount">The number of members in the unit</param>
    /// <param name="Attributes">Armor, hitpoints, etc.</param>
    /// <param name="Weapons">List of possible weapons for this unit</param>
    public record Unit(string Name, string Key, int ModelCount, List<UnitAttribute> Attributes, List<Weapon> Weapons, List<Trait> Traits);


}
