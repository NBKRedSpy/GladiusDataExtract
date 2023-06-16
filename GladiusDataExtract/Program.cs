using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;
using GladiusDataExtract.Extract;
using GladiusDataExtract.Extract.Units;

namespace GladiusDataExtract
{
    public class Program
    {
        static void Main(string[] args)
        {
            //ExtractUnitInfoText(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
            //    @"c:\work\UnitInfo.txt");

            //ExtractWeaponInfoText(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons",
            //    @"c:\work\WeaponInfo.txt");

            //List<Weapon> weapons = new();
            //weapons = ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons");


            //Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Name);

            //List<Unit> units = ExtractUnitInfo(
            //    @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units",
            //    weaponLookup);

            //ExportUnitInfo(units, @"C:\work\UnitsJoined.txt");

            SaveData(args[0]);
        }

		private static void SaveData(string outputFilePath)
		{
			var converter = new GladiusDataExtract.Extract.Converter();

			List<Entities.Unit> units = converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);


            var writer = new XmlSerializer(typeof(List<Entities.Unit>));


			using (var streamWriter = new StreamWriter(outputFilePath))
            {

                writer.Serialize(streamWriter, units);
			}


			//var reader = new XmlSerializer(typeof(List<Entities.Unit>));
   //         List<Entities.Unit> importedList = (List <Entities.Unit>)reader.Deserialize(new StreamReader(outputFilePath))!;
            

		}


    }
}