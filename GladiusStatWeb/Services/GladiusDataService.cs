using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using GladiusDataExtract.Entities;

namespace GladiusStatWeb.Services
{
    public class GladiusDataService
    {


		
		public List<Unit> GladiusUnits { get; init; }

        public GladiusDataService()
        {

			//GladiusUnits = LoadFromSourceXml();
			GladiusUnits = LoadFromXml("Data/GladiusUnits.xml");

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

		private List<Unit> LoadFromSourceXml()
        {
			var converter = new GladiusDataExtract.Extract.Converter();

			return converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);

		}
	}
}
