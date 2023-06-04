using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace GladiusDataExtract
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExtractUnitInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
                @"c:\work\UnitInfo.txt");

            ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons", 
                @"c:\work\WeaponInfo.txt");
        }


        public static void ExtractWeaponInfo(string folderName, string outputFile)
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

            }

            File.WriteAllText(outputFile, sb.ToString());
        }

        public static void ExtractUnitInfo(string folderName, string outputFile)
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
                    tabbedWriter.WriteLine(weaponNode.Attributes!["name"].Value);
                }

                tabbedWriter.Indent--;
                tabbedWriter.Indent--;

                //--
            }

            File.WriteAllText(outputFile, sb.ToString());
        }

    }
}