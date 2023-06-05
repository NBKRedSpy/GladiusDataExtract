using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Weapons
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
}
