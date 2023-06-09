using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Extract.Weapons
{
	/// <summary>
	/// The effect of a weapon.  Ex:  attacks
	/// </summary>
	/// <param name="Name"></param>
	/// <param name="Modifiers"></param>
	internal record Effect(string Name, List<ModifierType> Modifiers)
    {
        /// <summary>
        /// Runs all the modifiers for this effect.
        /// </summary>
        /// <param name="unitAttributeValue">The unit's base value to apply this effect to.  
        /// Ex:  "strengthDamage add 2" on the weapon means "The weapon's strengthDamage is the unit's strengthDamage + 2</param>
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
}
