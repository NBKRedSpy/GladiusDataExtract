using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using GladiusDataExtract.Extract;
using GladiusDataExtract.Extract.Units;
using GladiusDataExtract.Extract.Weapons;
using gde = GladiusDataExtract;

namespace GladiusStatWeb.Services
{
    public class GladiusDataService
    {


		
		public List<Unit> GladiusUnits { get; init; }

        public GladiusDataService()
        {

            var extractor = new Extractor();
			GladiusUnits = extractor.ExtractData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);

            //Debug
            Converter converter= new Converter();  
            converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);


        }
    }
}
