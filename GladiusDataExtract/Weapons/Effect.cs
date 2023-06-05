using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Weapons
{
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
}
