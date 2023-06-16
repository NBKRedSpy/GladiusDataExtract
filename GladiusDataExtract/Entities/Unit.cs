using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	/// <summary>
	/// A game unit such as Hormagaunts
	/// </summary>
	public class Unit
	{
		/// <summary>
		/// Friendly name of the unit
		/// </summary>
		public string Name { get; set; } = "";


        public bool IsFortification { get; set; }

        public Faction Faction { get; set; }

        /// <summary>
        /// The internal key of the unit.  Ex:  Tyranids/Hormagaunt
        /// </summary>
        public string Key { get; set; } = "";

		public int Armor { get; set; } = 0;

		/// <summary>
		/// The hit points per model.  Ex: a 8 model unit at 2 hp.
		/// </summary>
		public int Hitpoints { get; set; } = 0;
		public int Movement { get; set; } = 0;

        public int Morale { get; set; }

        /// <summary>
        /// The number of models in a unit.  Ex:  A unit may have 8 soldiers.
        /// </summary>
        public int ModelCount { get; set; } = 0;

		public int ProductionCost { get; set; } = 0;

		public Resources ProductionResources { get; set; } = new Resources();

		public Resources UpkeepResources { get; set; } = new Resources();

		public List<Trait> Traits { get; set; } = new();

		public List<Weapon> Weapons { get; set; } = new List<Weapon>();

    }



}
