using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Units;
using System.Xml;
using GladiusDataExtract.Extract.Weapons;
using System.CodeDom.Compiler;
using MoreLinq;
using System.IO;
using System.Diagnostics;

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
		internal List<Unit> ExtractData(string localizationFolder, string dataFolder, out Dictionary<string, string> factionLocallization )
		{
			//todo:  change to configure in settings or env.


			var extractor = new Extractor();

			LanguageExtract languageExtract = new LanguageExtract();

            Dictionary<string, string> weaponLocalizationText = languageExtract.GetTextStrings(
                Path.Combine(localizationFolder, "Weapons.xml"));

			List<Weapon> weapons = new();

			Dictionary<string, string> traitsText = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Traits.xml"));

			Dictionary<string, Trait> traitsTraits = GetTraits(Path.Combine(dataFolder, @"World\Traits"), traitsText);

			weapons = extractor.ExtractWeaponInfo(Path.Combine(dataFolder, @"World\Weapons"), weaponLocalizationText);

            factionLocallization = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Factions.xml"));

			Dictionary<string, string> unitLocalizationText = languageExtract.GetTextStrings(
                Path.Combine(dataFolder, @"Core\Languages\English\Units.xml"));

			Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Key);

			List<Unit> gladiusUnits = extractor.ExtractUnitInfo(Path.Combine(dataFolder, @"World\Units"),
				weaponLookup, unitLocalizationText);

            return gladiusUnits;

		}

		private Dictionary<string, Trait> GetTraits(string dataFolder, Dictionary<string, string> traitsText)
		{
			//Get the folder name with a trailing slash

			//Used to trim the start of the path to get the item's key.
			string basePath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(dataFolder)) + "\\";

			Dictionary<string, Trait> traits = new Dictionary<string, Trait>();

			foreach (string file in Directory.EnumerateFiles(basePath, "*.xml", SearchOption.AllDirectories))
			{



				try
				{
					//OrkoidFungusFood is commented out.
					if (file.Contains(@"\Artefacts\") || file.Contains(@"\Items\") || file.Contains(@"OrkoidFungusFood"))
					{
						//Ignore some traits.
						continue;
					}


					string xml = File.ReadAllText(file);

					//Fix some of the bad XML manually.

					xml = xml.Replace(@"<trait alwaysVisible=""1""category=""Buff"">", @"<trait alwaysVisible=""1"" category=""Buff"">");

					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xml);

					//debug
					//try
					//{
					//	xmlDocument.Load(file);
					//}
					//catch(XmlException ex) when (ex.Message == "Root element is missing.")
					//{
					//	//Some files are commented out minus the leading XML.  Skip
					//	continue;
					//}

					Trait trait = new Trait();

					trait.Key = GetKey(basePath, file);

					XmlNode traitNode = xmlDocument.SelectSingleNode("trait")!;

					//Some items do not have an icon. Mostly buildings.
					trait.Icon = traitNode.Attributes!["icon"]?.Value ?? "";
					trait.Name = traitsText[trait.Key];
					trait.Description = traitsText.GetValueOrDefault(trait.Key + "Description", "");
					trait.Effects = ParseEffects(xmlDocument, "/trait/modifiers/modifier/effects/*");

					//Not parsing required upgrades since they are actually part of modifier effects.
					//	that would require modeling the modifer and their effects.
					///	XPath: trait/modifiers/modifier/effects/production
					//	<modifier visible="0" requiredUpgrade="Orks/CreatePermanentOrkoidFungusOnDeath">

					trait.RequiredUpgrade = "";

					traits.Add(trait.Key, trait);
				}
				catch (Exception ex)
				{
					throw new ApplicationException($"Error parsing {file}: {ex.Message}", ex);
				}
			}
			return traits;

		}

		internal List<Unit> ExtractUnitInfo(string folderName, Dictionary<string, Weapon> weaponLookup, Dictionary<string, string> weaponNameLookup)
        {

            List<Unit> units = new();

            //Get the folder name with a trailing slash



            //Used to trim the start of the path to get the item's key.
            string basePath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(folderName)) + "\\";




			//Debug
			//            foreach (string file in new[] { @"D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Units\AstraMilitarum\Guardsman.xml"})
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
						string requiredUpgrade = weaponNode.Attributes!["requiredUpgrade"]?.Value ?? "";

						unit.Weapons.Add(new UnitWeapon(weaponLookup[weaponNode.Attributes!["name"]!.Value],
							requiredUpgrade));
					}

					GetSecondaryRequirements(xmlDocument, unit);


					//--Traits
					List<Trait> traits = new();

					//--Traits
					XmlNodeList traitNodes = xmlDocument.SelectNodes("unit/traits/trait")!;

					foreach (XmlNode traitNode in traitNodes)
					{
						string? requiredUpgrade = traitNode.Attributes?["requiredUpgrade"]?.Value;

						string trait = traitNode.Attributes!["name"]!.Value;

						traits.Add(new Trait() 
						{ 
							Key = trait, 
							Name = trait,
							Icon = trait,
							RequiredUpgrade = requiredUpgrade 
						}
						);
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

		/// <summary>
		/// Additional unit requirements for the unit that are stored in the actions.
		///  Oddly there are also requirements on a unit's actions.
		///  I'm guessing that since they are actions and they are hidden without the upgrade, 
		///  the devs chose not to put it on the weapon.
		/// </summary>
		/// <param name="xmlDocument"></param>
		/// <param name="unit"></param>
		/// <exception cref="InvalidDataException"></exception>
		private static void GetSecondaryRequirements(XmlDocument xmlDocument, Unit unit)
		{

			//All actions that have both a reuqired upgrade and a weapon key
			XmlNodeList actionRequirementNodes = xmlDocument.SelectNodes("unit/actions/*[@requiredUpgrade and @weaponSlotName]")!;

			var actionRequirements = actionRequirementNodes.Cast<XmlNode>()
				.Select(x => new
				{
					weaponKey = x.Attributes!["weaponSlotName"]!.Value,
					upgrade = x.Attributes!["requiredUpgrade"]!.Value
				}
			);

			//unit weapon to upgrade.
			var actionWeaponJoinList = unit.Weapons.Join(actionRequirements, x => x.Weapon.Key,
				x => x.weaponKey, (unitWeapon, actionRequirement) => new { unitWeapon, actionRequirement.upgrade });


			//Validate and set
			foreach (var actionWeaponJoin in actionWeaponJoinList)
			{
				if (actionWeaponJoin.unitWeapon.RequiredUpgrade == "")
				{
					actionWeaponJoin.unitWeapon.RequiredUpgrade = actionWeaponJoin.upgrade;
				}
				else
				{
					//Validation
					if (actionWeaponJoin.unitWeapon.RequiredUpgrade != actionWeaponJoin.upgrade)
					{
						throw new InvalidDataException($"Weapon '{actionWeaponJoin.unitWeapon.Weapon.Name} already has a required upgrade");
					}

				}
			}
		}

		internal List<Weapon> ExtractWeaponInfo(string folderName, Dictionary<string, string> weaponLocalizationText)
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

				weapon.Effects.AddRange(ParseEffects(xmlDocument, "weapon/modifiers/modifier/effects/*"));

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

		private static List<Effect> ParseEffects(XmlDocument xmlDocument, string xmlPath)
		{
			XmlNodeList effectNodes = xmlDocument.SelectNodes(xmlPath)!;

			List<Effect> effects = new List<Effect>();

			//Note: This intentionally does not parse Trait effect's conditions.
			//	may be added later.
			foreach (XmlNode effectNode in effectNodes)
			{
				//HACK: Ignore addTrait for now.  
				//Example:  <addTrait duration="1" name="SistersOfBattle/UsedActOfFaith"/>
				if (effectNode.Name == "addTrait")
				{
					continue;
				}

				Effect effect = new(effectNode.Name, new());  //Ex: attacks

				List<ModifierType> modifiers = effect.Modifiers;

				foreach (XmlAttribute attribute in effectNode.Attributes!)
				{
					modifiers.Add(new(attribute.Name, decimal.Parse(attribute.Value)));
				}

				effects.Add(effect);
			}

			return effects;
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
