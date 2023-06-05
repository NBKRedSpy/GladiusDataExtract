using GladiusDataExtract.Units;
using GladiusDataExtract.Weapons;
using gde = GladiusDataExtract;

namespace GladiusStatWeb.Services
{
    public class GladiusDataService
    {

        public List<Unit> GladiusUnits { get; init; }

        public GladiusDataService()
        {
            //todo:  change to configure in settings or env.

            List<Weapon> weapons = new();
            weapons =  gde.Program.ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons");


            Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Name);

            GladiusUnits = gde.Program.ExtractUnitInfo(
                @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
                weaponLookup);
        }
    }
}
