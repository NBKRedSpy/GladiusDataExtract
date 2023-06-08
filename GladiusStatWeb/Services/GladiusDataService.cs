using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using GladiusDataExtract;
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
            //todo:  change to configure in settings or env.


            var extractor = new Extractor();


            LanguageExtract languageExtract = new LanguageExtract();

            Dictionary<string,string> weaponLocalizationText = languageExtract.GetTextStrings("D:\\Games\\Steam\\steamapps\\common\\Warhammer 40000 Gladius - Relics of War\\Data\\Core\\Languages\\English\\Weapons.xml");

            List<Weapon> weapons = new();
            
            weapons = extractor.ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons", weaponLocalizationText);


			Dictionary<string, string> unitLocalizationText = languageExtract.GetTextStrings("D:\\Games\\Steam\\steamapps\\common\\Warhammer 40000 Gladius - Relics of War\\Data\\Core\\Languages\\English\\Units.xml");

			Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Key );

            GladiusUnits = extractor.ExtractUnitInfo(
                @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units",
                weaponLookup, unitLocalizationText);
        }
    }
}
