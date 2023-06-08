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

				var attributes = dtoUnit.Attributes.ToDictionary(x => x.Name);

				unit.Armor = (int)attributes["armor"].Value;
				unit.Morale = (int)attributes["morale"].Value;
				unit.ProductionCost = (int)attributes["productionCost"].Value;

				unit.ProductionResources = ConvertResources(attributes, "Cost");

				unit.Hitpoints = (int)attributes["hitpointsMax"].Value;
				unit.Movement = (int)attributes["movementMax"].Value;
			}

			return units;
		}

		private Resources ConvertResources(Dictionary<string, du.UnitAttribute> attributes, string suffix)
		{
			Resources resources = new Resources();
			//resources.Influence = attributes[""]

			resources.BioMass = attributes.GetValueOrDefault("biomass" + suffix);

			/*
			 * 	food
	ore
	energy
	influence
	biomass

		}
	}
}
