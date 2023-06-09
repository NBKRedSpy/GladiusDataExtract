using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using GladiusDataExtract.Entities;

namespace GladiusStatWeb.Services
{
    public class GladiusDataService
    {


		
		public List<Unit> GladiusUnits { get; init; }

        public GladiusDataService()
        {

            //Debug
            var converter= new GladiusDataExtract.Extract.Converter();  

            GladiusUnits = converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);


        }
    }
}
