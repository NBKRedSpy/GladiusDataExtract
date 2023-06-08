using System.Runtime.InteropServices;
using GladiusDataExtract;
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


            LanguageExtract languageExtract = new LanguageExtract();

            Dictionary<string,string> weaponLocalizationText = languageExtract.GetTextStrings("D:\\Games\\Steam\\steamapps\\common\\Warhammer 40000 Gladius - Relics of War\\Data\\Core\\Languages\\English\\Weapons.xml");

            List<Weapon> weapons = new();
            
            weapons = gde.Program.ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons", weaponLocalizationText);


			Dictionary<string, string> unitLocalizationText = languageExtract.GetTextStrings("D:\\Games\\Steam\\steamapps\\common\\Warhammer 40000 Gladius - Relics of War\\Data\\Core\\Languages\\English\\Units.xml");

			Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Key );

            GladiusUnits = gde.Program.ExtractUnitInfo(
                @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units",
                weaponLookup, unitLocalizationText);
        }
    }
}
