﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Units;
using System.Xml;
using GladiusDataExtract.Extract.Weapons;
using System.CodeDom.Compiler;

namespace GladiusDataExtract.Extract
{

    /// <summary>
    /// Handles extracting the data from the XML files into DTO's.
    /// </summary>
    public class Extractor
    {

		/// <summary>
		/// Returns the DTO units from the data.
		/// </summary>
		/// <param name="localizationFolder">The folder to use for the localization text.  Ex:  "./Warhammer 40000 Gladius - Relics of War/Data/Core/Languages/English/"</param>
		/// <param name="dataFolder">The data folder for the game.
		/// Ex:  ./Warhammer 40000 Gladius - Relics of War/Data</param>
		/// <exception cref="NotImplementedException"></exception>
		public List<Unit> ExtractData(string localizationFolder, string dataFolder)
		{
			//todo:  change to configure in settings or env.


			var extractor = new Extractor();

			LanguageExtract languageExtract = new LanguageExtract();

            Dictionary<string, string> weaponLocalizationText = languageExtract.GetTextStrings(
                Path.Combine(localizationFolder, "Weapons.xml"));

			List<Weapon> weapons = new();

			weapons = extractor.ExtractWeaponInfo(Path.Combine(dataFolder, @"World\Weapons"), weaponLocalizationText);


			Dictionary<string, string> unitLocalizationText = languageExtract.GetTextStrings(
                Path.Combine(dataFolder, @"Core\Languages\English\Units.xml"));

			Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Key);

			List<Unit> gladiusUnits = extractor.ExtractUnitInfo(Path.Combine(dataFolder, @"World\Units"),
				weaponLookup, unitLocalizationText);

            return gladiusUnits;

		}

		public List<Unit> ExtractUnitInfo(string folderName, Dictionary<string, Weapon> weaponLookup, Dictionary<string, string> weaponNameLookup)
        {

            List<Unit> units = new();

            //Get the folder name with a trailing slash



            //Used to trim the start of the path to get the item's key.
            string basePath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folderName)) + "\\";


            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml", SearchOption.AllDirectories))
            {

                try
                {
					//D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\Neutral\Artefacts\VaulSentry.xml
					if (file.Contains(@"\Artefacts\"))
                    {
                        //not units
                        continue;
                    }
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(file);

					string keyName = GetKey(folderName, file);


                    //Weapons don't have sub folders.
                    string name = weaponNameLookup[keyName];


                    //Get the key, which is the relative path with forward slash separators 
                    string unitKey = GetKey(folderName, file);

                    

                    //Model count
                    XmlAttribute? xmlSize = xmlDocument.SelectSingleNode("unit/group")?.Attributes!["size"];

                    int modelCount = xmlSize is null ? 1 : int.Parse(xmlSize.Value);
                    Unit unit = new(name, unitKey, modelCount, new(), new(), new());

                    //--Effects
                    XmlNodeList effectNodes = xmlDocument.SelectNodes("unit/modifiers/modifier/effects/*")!;

                    List<UnitAttribute> attributes = unit.Attributes;

					//todo:  Add cargo info. // <cargoSlots base="6"/> <!-- %cargoSlots base capacity=20 -->

					foreach (XmlNode xmlEffect in effectNodes)
                    {
                        XmlAttributeCollection effectXmlAttributes = xmlEffect.Attributes!;

                        if (effectXmlAttributes.Count > 1)
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

                        attributes.Add(new UnitAttribute(xmlEffect.Name, decimal.Parse(xmlAttributeValue!)));
                    }


                    //--Weapons
                    XmlNodeList weaponNodes = xmlDocument.SelectNodes("unit/weapons/weapon")!;

                    foreach (XmlNode weaponNode in weaponNodes)
                    {
                        unit.Weapons.Add(weaponLookup[weaponNode.Attributes!["name"]!.Value]);
                    }


                    List<Trait> traits = new();

                    //--Traits
                    XmlNodeList traitNodes = xmlDocument.SelectNodes("unit/traits/trait")!;

                    foreach (XmlNode traitNode in traitNodes)
                    {
                        string? requiredUpgrade = traitNode.Attributes?["requiredUpgrade"]?.Value;

                        string trait = traitNode.Attributes!["name"]!.Value;

                        traits.Add(new(trait, requiredUpgrade));
                    }

                    unit.Traits.AddRange(traits.OrderBy(x => x.RequiredUpgrade).ThenBy(x => x.Name));


                    units.Add(unit);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Error parsing {file}: {ex.Message}", ex);
                }
            }

            return units;

        }

        public List<Weapon> ExtractWeaponInfo(string folderName, Dictionary<string, string> weaponLocalizationText)
        {
            List<Weapon> weapons = new();

            StringBuilder sb = new StringBuilder();

            foreach (string file in Directory.EnumerateFiles(folderName, "*.xml"))
            {


                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(file);


                //Range
                int targetRange = 0;
                var targetNode = xmlDocument.SelectSingleNode("weapon/target[@rangeMax]");

                if (targetNode != null)
                {
                    targetRange = int.Parse(targetNode.Attributes!["rangeMax"]!.Value);
                }

				string keyName = GetKey(folderName, file);
				Weapon weapon = new(weaponLocalizationText[keyName], keyName, targetRange, new(), new(), new());


				//----Effects
				XmlNodeList effectNodes = xmlDocument.SelectNodes("weapon/modifiers/modifier/effects/*")!;
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

        private string GetKey(string rootPath, string filePath)
        {
            string key = Path.GetRelativePath(rootPath, filePath)
                .Replace("\\", "/")
                .Replace(".xml", "", StringComparison.OrdinalIgnoreCase);

            return key;

        }



    }
}
