using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Entities;
using du = GladiusDataExtract.Extract.Units;
using dw = GladiusDataExtract.Extract.Weapons;

namespace GladiusDataExtract.Extract
{
	/// <summary>
	/// A class to convert DTO's to project entities.
	/// </summary>
	public class Converter
	{

		
		/// <summary>
		/// Converts the DTO's data to Entity data.
		/// </summary>
		/// <param name="dtoUnits"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public List<Unit> ConvertUnits(List<du.Unit> dtoUnits)
		{

			List<Unit> units = new List<Unit>();
			
			foreach (du.Unit dtoUnit in dtoUnits)
			{

				Unit unit = new();

				unit.Name = dtoUnit.Name;
				unit.Key = dtoUnit.Key;
				unit.ModelCount = dtoUnit.ModelCount;

				var attributes = new Dictionary<string, decimal>(
					dtoUnit.Attributes
						.Select(x => new KeyValuePair<string,decimal>(x.Name, x.Value)));

				unit.Armor = (int)attributes["armor"];
				unit.Morale = (int)attributes["morale"];
				unit.ProductionCost = (int)attributes["productionCost"];

				unit.ProductionResources = ConvertResources(attributes, "Cost");

				unit.Hitpoints = (int)attributes["hitpointsMax"];
				unit.Movement = (int)attributes["movementMax"];

				unit.Traits = dtoUnit.Traits.Select(x => new Trait(x.Name, x.RequiredUpgrade)).ToList();
			}

			return units;
		}

		private Resources ConvertResources(Dictionary<string, decimal> attributes, string suffix)
		{
			Resources resources = new Resources();

			resources.BioMass = attributes.GetValueOrDefault("biomass" + suffix);
			resources.Energy = attributes.GetValueOrDefault("energy" + suffix);
			resources.Food = attributes.GetValueOrDefault("food" + suffix);
			resources.Influence = attributes.GetValueOrDefault("influence" + suffix);
			resources.Ore = attributes.GetValueOrDefault("ore" + suffix);

			return resources;
		}
	}
}
