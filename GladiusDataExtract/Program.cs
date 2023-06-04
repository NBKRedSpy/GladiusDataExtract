using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using GladiusDataExtract.Weapons;

namespace GladiusDataExtract
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExtractUnitInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
                @"c:\work\UnitInfo.txt");

            ExtractWeaponInfoText(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons", 
                @"c:\work\WeaponInfo.txt");

            List<Weapon> weapons = new();
            weapons = ExtractWeaponWeapons(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons");


        }

        private static List<Weapon> ExtractWeaponWeapons(string folderName) 
        {
            List<Weapon> weapons = new();

            StringBuilder sb = new StringBuilder();

            IndentedTextWriter tabbedWriter = new IndentedTextWriter(new StringWriter(sb));

            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml"))
            {
                string name = Path.GetFileName(file).Replace(".xml", "");

                Weapon weapon = new(name, new(), new(), new());

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file);

                XmlNodeList effectNodes = xmlDocument.SelectNodes("weapon/modifiers/modifier/effects/*")!;

                //----Effects
                List<Effect> effects = weapon.Effects;  //Ex: meleeArmorPenetration

                foreach (XmlNode effectNode in effectNodes)
                {
                    Effect effect = new(effectNode.Name, new());  //Ex: attacks

                    List<ModifierType> modifiers = effect.Modifiers;

                    foreach (XmlAttribute attribute in effectNode.Attributes!)
                    {
                        modifiers.Add(new(attribute.Name, decimal.Parse(attribute.Value)));
                    }
                }

                //----Requirements
                XmlNodeList requireNodes = xmlDocument.SelectNodes(@"/weapon/traits/trait[@requiredUpgrade]")!;

                foreach (XmlNode requirementNode in requireNodes)
                {
                    List<Requirement> requirements = weapon.Requirements;

                    string requirementName = requirementNode.Attributes!["name"]!.Value;
                    string requiredUpgrade = requirementNode.Attributes["requiredUpgrade"]!.Value;

                    requirements.Add(new(requirementName, requiredUpgrade));

                }

                //----Traits
                XmlNodeList traitNodes = xmlDocument.SelectNodes(@"/weapon/traits/trait[not(@requiredUpgrade)]")!;

                if (traitNodes.Count > 0)
                {

                    foreach (XmlNode traitNode in traitNodes)
                    {
                        weapon.Traits.Add(traitNode.Attributes!["name"]!.Value);
                    }

                }

                weapons.Add(weapon);

            }

            return weapons;
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