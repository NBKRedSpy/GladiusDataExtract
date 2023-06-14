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
using GladiusDataExtract.Extract.Weapons;

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

			List<Entities.Unit> units = converter.ConvertData(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\Core\Languages\English\",
				@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data"
				);


            var writer = new XmlSerializer(typeof(List<Entities.Unit>));


			using (var streamWriter = new StreamWriter(outputFilePath))
            {

                writer.Serialize(streamWriter, units);
			}


			//var reader = new XmlSerializer(typeof(List<Entities.Unit>));
   //         List<Entities.Unit> importedList = (List <Entities.Unit>)reader.Deserialize(new StreamReader(outputFilePath))!;
            

		}

		private static void ExportUnitInfo(List<Unit> units, string outputFile)
        {

            StringBuilder sb = new StringBuilder();

            IndentedTextWriter writer = new IndentedTextWriter(new StringWriter(sb));

            foreach (Unit unit in units)
            {
                writer.WriteLine(unit.Name);


                writer.Indent++;
                writer.WriteLine($"Models: {unit.ModelCount}");
                writer.WriteLine("Attributes");

                writer.Indent++;

                foreach (UnitAttribute attribute in unit.Attributes)
                {
                    writer.WriteLine($"{attribute.Name}: {attribute.Value:#.#}");
                }

                writer.Indent--;
                writer.WriteLine("Traits");
                writer.Indent++;
                foreach (var trait in unit.Traits)
                {
                    //writer.WriteLine($"{trait.Name} {trait.RequiredUpgrade is null ? "xxx" : ""}{trait.RequiredUpgrade}");
                    writer.WriteLine($"{trait.Name}{(trait.RequiredUpgrade is null ? "" : " -> ") }{trait.RequiredUpgrade}");
                }

                writer.Indent--;



                //Weapons
                writer.WriteLine("Weapons");
                
                writer.Indent++;

                foreach (var unitWeapon in unit.Weapons)
                {
                    var weapon = unitWeapon.Weapon;

                    writer.WriteLine(weapon.Name);

                    writer.Indent++;
                    
                    if(unitWeapon.RequiredUpgrade != "")
                    {
                        writer.WriteLine($"Requires {unitWeapon.RequiredUpgrade}");
                    }
                    writer.WriteLine("Effects");

                    writer.Indent++;

                    foreach (Effect effect in weapon.Effects)
                    {
                        writer.WriteLine(effect.Name);

                        writer.Indent++;

                        foreach (var modifier in effect.Modifiers)
                        {
                            writer.WriteLine($"{modifier.Type}: {modifier.Value}");
                        }

                        writer.Indent--;
                    }

                    writer.Indent--;

                    //weapon.Requirements;
                    writer.WriteLine("Requirements");
                    writer.Indent++;

                    foreach (Requirement requirement in weapon.Requirements)
                    {
                        writer.WriteLine(requirement.FormatRequirements());
                    }
                    writer.Indent--;

                    //weapon.Traits;
                    writer.WriteLine("Traits");

                    writer.Indent++;

                    foreach (string trait in weapon.Traits)
                    {
                        writer.WriteLine(trait);
                    }

                    writer.Indent--;
                    writer.Indent--;
                }

                writer.Indent--;
                writer.Indent--;

            }

            File.WriteAllText(outputFile, sb.ToString());

        }



        public static void ExtractWeaponInfoText(string folderName, string outputFile)
        {

            StringBuilder sb = new StringBuilder();

            IndentedTextWriter tabbedWriter = new IndentedTextWriter(new StringWriter(sb));

            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml"))
            {
                string name = Path.GetFileName(file).Replace(".xml", "");
                tabbedWriter.WriteLine(name);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file);

                XmlNodeList effectNodes = xmlDocument.SelectNodes("weapon/modifiers/modifier/effects/*")!;

                //----Effects
                tabbedWriter.Indent++;
                tabbedWriter.WriteLine("Effects");
                tabbedWriter.Indent++;

                foreach (XmlNode effect in effectNodes)
                {
                    tabbedWriter.WriteLine(" " + effect.Name);

                    tabbedWriter.Indent++;

                    foreach (XmlAttribute attribute in effect.Attributes!)
                    {
                        tabbedWriter.Write(" " + attribute.Name + " ");
                        tabbedWriter.WriteLine(attribute.Value);
                    }

                    tabbedWriter.Indent--;
 
                }

                tabbedWriter.Indent--;


                //----Requirements
                XmlNodeList requireNodes = xmlDocument.SelectNodes(@"/weapon/traits/trait[@requiredUpgrade]")!;

                if(requireNodes.Count > 0) {

                    //----Requirements
                    tabbedWriter.WriteLine("Requirements");

                    tabbedWriter.Indent++;

                    foreach (XmlNode requirementNode in requireNodes)
                    {

                        string requirementName =  requirementNode.Attributes!["name"]!.Value;
                        string requiredUpgrade = requirementNode.Attributes["requiredUpgrade"]!.Value;


                        tabbedWriter.Write(requirementName);
                        tabbedWriter.Write(" ");
                        tabbedWriter.WriteLine(requiredUpgrade);

                    }

                    tabbedWriter.Indent--;
                }


                //----Traits
                XmlNodeList traitNodes= xmlDocument.SelectNodes(@"/weapon/traits/trait[not(@requiredUpgrade)]")!;

                if (traitNodes.Count > 0)
                {

                    //----Requirements
                    tabbedWriter.WriteLine("Traits");

                    tabbedWriter.Indent++;

                    foreach (XmlNode traitNode in traitNodes)
                    {

                        string requirementName = traitNode.Attributes!["name"]!.Value;

                        tabbedWriter.WriteLine(requirementName);
                    }

                    tabbedWriter.Indent--;
                }

                tabbedWriter.Indent--;

            }

            File.WriteAllText(outputFile, sb.ToString());
        }

        public static void ExtractUnitInfoText(string folderName, string outputFile)
        {

            StringBuilder sb = new StringBuilder();

            IndentedTextWriter tabbedWriter = new IndentedTextWriter(new StringWriter(sb));

            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml"))
            {
                string name = Path.GetFileName(file).Replace(".xml", "");
                tabbedWriter.WriteLine(name);

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file);

                //--Effects
                XmlNodeList effectNodes = xmlDocument.SelectNodes("unit/modifiers/modifier/effects/*")!;

                tabbedWriter.Indent++;

                tabbedWriter.WriteLine("Effects");

                tabbedWriter.Indent++;
                foreach (XmlNode effect in effectNodes)
                {
                    tabbedWriter.WriteLine($"{effect.Name} {effect.Attributes?["base"]?.Value}");
                }
                tabbedWriter.Indent--;

                //--Weapons
                tabbedWriter.WriteLine("Weapons");

                tabbedWriter.Indent++;

                XmlNodeList weaponNodes = xmlDocument.SelectNodes("unit/weapons/weapon")!;

                foreach  (XmlNode weaponNode in weaponNodes)
                {
                    tabbedWriter.WriteLine(weaponNode.Attributes!["name"]!.Value);
                }

                tabbedWriter.Indent--;
                tabbedWriter.Indent--;

                //--
            }

            File.WriteAllText(outputFile, sb.ToString());
        }


    }
}