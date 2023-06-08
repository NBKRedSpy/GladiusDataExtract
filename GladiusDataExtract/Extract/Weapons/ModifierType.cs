using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Extract.Weapons
{
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
        /// <param name="operation">The operation to use.  Ex:  Add or multiply</param>
        /// <param name="actionOperand">The the data for the operation.  Ex:  2.  Add, 2 means add +2 to the value.</param>
        /// <param name="unitsValue">The unit's value for this attribute.  Used as the starting point for the starting point for the 
        /// operation. Ex:  the unit's strengthDamage. If it was 5, a "mul .5" would result in 2.5.
        /// Ex:  "strengthDamage add 2" on the weapon means "The weapon's strengthDamage is the unit's strengthDamage + 2</param>
        /// <returns></returns>
        public decimal ApplyModifier(string operation, decimal actionOperand, decimal? unitsValue)
        {
            decimal attValue = unitsValue ?? 0;

            switch (operation)
            {
                case "add":
                    return attValue += actionOperand;
                case "base":
                    //I think base means "set".  Oddly in the xml comments there are "set" commands, but maybe this just changed
                    //	over time.
                    return actionOperand;
                case "mul":
                    return (actionOperand + 1) * attValue;
                case "min":
                    //Not sure.  Is it the min between the two?
                    return Math.Min(attValue, actionOperand);
                case "max":
                    //Not sure.  Is it the max between the two?
                    return Math.Max(attValue, actionOperand);
                default:
                    throw new ArgumentException($"Unexpected value for type: '{operation}'", "type");
            }
        }
    }
}
