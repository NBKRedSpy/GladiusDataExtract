using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using GladiusDataExtract.Entities;
using GladiusStatWeb.Pages;

namespace GladiusStatWeb.Services
{
    public class GladiusDataService
    {


		
		/// <summary>
		/// All of the units
		/// </summary>
		private List<Unit> GladiusUnits { get; init; }

		/// <summary>
		/// The data grouped by factions
		/// </summary>
		private List<IGrouping<Faction,Unit>> GladiusFactionUnits { get; init; }

        public GladiusDataService()
        {
			GladiusUnits = LoadFromXml("Data/GladiusUnits.xml");

			//DEBUG:
			//GladiusUnits = LoadFromSourceXml();

			GladiusFactionUnits = GladiusUnits
				.GroupBy(x => x.Faction)
				.ToList();
		}

		/// <summary>
		/// All the units for a specific faction.
		/// </summary>
		/// <param name="faction"></param>
		/// <returns></returns>
		public IEnumerable<Unit> GetUnits(Faction faction)
		{
			return GladiusFactionUnits.Where(x => x.Key == faction).SelectMany(x => x);
		}

		/// <summary>
		/// All units, regardless of factions.
		/// </summary>
		/// <param name="faction"></param>
		/// <returns></returns>
		public IEnumerable<Unit> GetAllUnits()
		{
			return GladiusUnits;
		}

		/// <summary>
		/// Loads the entity Unit data from the converted game's XML data.
		/// </summary>
		/// <param name="xmlFilePath"></param>
		/// <returns></returns>
		private List<Unit> LoadFromXml(string xmlFilePath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Unit>));
			
			using (StreamReader reader = new StreamReader(xmlFilePath))
			{
				return (List<Unit>)serializer.Deserialize(reader)!;
			}
		}


		/// <summary>
		/// Only used for debugging.  Parses the game's original files instead of using the pre-processed GladiusUnis data.
		/// </summary>
		/// <returns></returns>
		private List<Unit> LoadFromSourceXml()
        {
			var converter = new GladiusDataExtract.Extract.Converter();

			return converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);

		}
	}
}
