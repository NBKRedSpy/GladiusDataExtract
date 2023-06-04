using System.CodeDom.Compiler;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using GladiusDataExtract.Units;
using GladiusDataExtract.Weapons;

namespace GladiusDataExtract
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExtractUnitInfoText(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
                @"c:\work\UnitInfo.txt");

            ExtractWeaponInfoText(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons", 
                @"c:\work\WeaponInfo.txt");

            List<Weapon> weapons = new();
            weapons = ExtractWeaponInfo(@"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Weapons");


            Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Name);

            List<Unit> units = ExtractUnitInfo(
                @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Tyranids",
                weaponLookup);

            ExportUnitInfo(units, @"C:\work\UnitsJoined.txt");
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

                foreach (var weapon in unit.Weapons)
                {
                    writer.WriteLine(weapon.Name);

                    writer.Indent++;

                    writer.WriteLine("Effects");

                    writer.Indent++;

                    foreach (Effect effect in weapon.Effects)
                    {
                        writer.WriteLine(effect.name);

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

        private static List<Unit> ExtractUnitInfo(string folderName, Dictionary<string, Weapon> weaponLookup)
        {

            List<Unit> units = new();

            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml"))
            {
                try
                {
                    string name = Path.GetFileName(file).Replace(".xml", "");



                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(file);

                    //Model count
                    XmlAttribute? xmlSize = xmlDocument.SelectSingleNode("unit/group")?.Attributes!["size"];

                    int modelCount = xmlSize is null ? 1 : int.Parse(xmlSize.Value);

                    Unit unit = new(name, modelCount, new(), new(), new());

                    //--Effects
                    XmlNodeList effectNodes = xmlDocument.SelectNodes("unit/modifiers/modifier/effects/*")!;

                    List<UnitAttribute> attributes = unit.Attributes;

                    foreach (XmlNode xmlEffect in effectNodes)
                    {
                        XmlAttributeCollection effectXmlAttributes = xmlEffect.Attributes!;

                        if(effectXmlAttributes.Count  > 1)
                        {
                            throw new InvalidDataException($"Attribute {xmlEffect.Name} has more than one attribute");
                        }

                        string? xmlAttributeValue;


                        //Should only be base or max
                        if ((xmlAttributeValue = effectXmlAttributes["base"]?.Value) is null)
                        {
                            //Will be max
                            xmlAttributeValue = effectXmlAttributes["max"]?.Value;
                        }

                        attributes.Add(new UnitAttribute(xmlEffect.Name, Decimal.Parse(xmlAttributeValue!)));
                    }


                    //--Weapons
                    XmlNodeList weaponNodes = xmlDocument.SelectNodes("unit/weapons/weapon")!;

                    foreach (XmlNode weaponNode in weaponNodes)
                    {
                        unit.Weapons.Add(weaponLookup[weaponNode.Attributes!["name"]!.Value]);
                    }


                    List<Trait> traits = new();

                    //--Traits
                    XmlNodeList traitNodes= xmlDocument.SelectNodes("unit/traits/trait")!;

                    foreach (XmlNode traitNode in traitNodes)
                    {
                        string? requiredUpgrade = traitNode.Attributes?["requiredUpgrade"]?.Value;

                        string trait = traitNode.Attributes!["name"]!.Value;

                        traits.Add(new(trait, requiredUpgrade));
                    }

                    unit.Traits.AddRange(traits.OrderBy(x => x.RequiredUpgrade).ThenBy(x=> x.Name));


                    units.Add(unit);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Error parsing {file}: {ex.Message}", ex);
                }
            }

            return units;

        }

        private static List<Weapon> ExtractWeaponInfo(string folderName) 
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

                    weapon.Effects.Add(effect);
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