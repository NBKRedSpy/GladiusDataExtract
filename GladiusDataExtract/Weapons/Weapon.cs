using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Units;

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
    /// <param name="Name"></param>
    /// <param name="Modifiers"></param>
    public record Effect(string Name, List<ModifierType> Modifiers)
    { 
        /// <summary>
        /// Runs all the modifiers for this effect.
        /// </summary>
        /// <param name="unitAttributeValue"></param>
        /// <returns></returns>
        public decimal ApplyModifiers(decimal unitAttributeValue)
        {
            decimal result = unitAttributeValue;

			//Note - Almost all the modifiers have a single operation.  
			//  However, ClusterMines.xml has a min and a max on the same effect.
			foreach (ModifierType modifier in Modifiers)
            {
                 result = modifier.ApplyModifier(result);
            }

            return result;
        }
    }




    /// <summary>
    /// A modifier for an effect.  For example, add 2 more attacks or add 12 range damage.
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Value"></param>
    public record ModifierType(string Type, decimal Value)
    {
        
        public decimal ApplyModifier(decimal? attributeValue)
        {
            return ApplyModifier(Type, Value, attributeValue);
        }

        /// <summary>
        /// Applies a modfier command to a value.  For example:  "Add 2" to the value.
        /// Ex: add 2 to value.
        /// </summary>
        /// 
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public decimal ApplyModifier(string type, decimal sourceValue, decimal? attributeValue)
        {
            decimal attValue = attributeValue ?? 0;

            switch (type)
            {
                case "add":
                    return attValue += sourceValue;
                case "base":
                    //Don't know what this is.  It seems to always be zero.
                    //For now, returning the unit's value.
                    return attValue;
				case "mul":
                    return (sourceValue + 1) * attValue;
				case "min":
					//Not sure.  Is it the min between the two?
					return Math.Min(attValue, sourceValue);    
				case "max":
					//Not sure.  Is it the max between the two?
					return Math.Max(attValue, sourceValue);
				default:
                    throw new ArgumentException($"Unexpected value for type: '{type}'", "type");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Effects"></param>
    /// <param name="Requirements"></param>
    /// <param name="Traits">The traits for the weapon.  For example, Melee or IgnoresCover</param>
    public record Weapon(string Name, int targetRange, List<Effect> Effects, List<Requirement> Requirements,
        List<string> Traits);



}
