using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Weapons
{
    /// <summary>
    /// The requirement for a weapon to be available to the unit.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Requires"></param>
    public record Requirement(string Name, string Requires)
    {

        public string FormatRequirements()
        {
            return Name == Requires ? Name : $"{Name} -> {Requires}";
        }
    }

    /// <summary>
    /// The effect of a weapon.  Ex:  attacks
    /// </summary>
    /// <param name="name"></param>
    /// <param name="Modifiers"></param>
    public record Effect(string name, List<ModifierType> Modifiers);
    
    
    /// <summary>
    /// A modifier for an effect.  For example, add 2 more attacks or add 12 range damage.
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Value"></param>
    public record ModifierType(string Type, decimal Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Effects"></param>
    /// <param name="Requirements"></param>
    /// <param name="Traits">The traits for the weapon.  For example, Melee or IgnoresCover</param>
    public record Weapon(string Name, List<Effect> Effects, List<Requirement> Requirements,
        List<string> Traits);



}
