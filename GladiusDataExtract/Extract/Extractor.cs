using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Extract.Units;
using System.Xml;
using GladiusDataExtract.Extract.Weapons;
using e = GladiusDataExtract.Entities;
using MoreLinq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace GladiusDataExtract.Extract
{

	/// <summary>
	/// Handles extracting the data from the XML files into DTO's.
	/// </summary>
	public class Extractor
	{

		private static IReadOnlySet<string> DataFolderNames { get; }


        static Extractor()
        {
			
			DataFolderNames = new HashSet<string>
			{
				"Actions",
				"Attributes",
				"Buildings",
				"Factions",
				"Features",
				"Items",
				"Notifications",
				"Overlay",
				"Quests",
				"Traits",
				"Units",
				"Upgrades",
				"Weapons",
			};
		}

        /// <summary>
        /// Returns the DTO units from the data.
        /// </summary>
        /// <param name="localizationFolder">The folder to use for the localization text.  Ex:  "./Warhammer 40000 Gladius - Relics of War/Data/Core/Languages/English/"</param>
        /// <param name="dataFolder">The data folder for the game.
        /// Ex:  ./Warhammer 40000 Gladius - Relics of War/Data</param>
        /// <exception cref="NotImplementedException"></exception>
        internal List<Unit> ExtractData(string dataFolder, 
			out Dictionary<string, string> factionLocallization )
		{
			//todo:  change to configure in settings or env.


			var extractor = new Extractor();

			//Languages
			Dictionary<string, string> weaponLocalizationText, traitsText, unitLocalizationText, upgradesLocalizationText;
			ExtractLanguages(dataFolder, out factionLocallization, out weaponLocalizationText, out traitsText, 
				out unitLocalizationText, out upgradesLocalizationText);


			//---Unit components

			Dictionary<string, Trait> traitsLookup = GetTraits(Path.Combine(dataFolder, @"World\Traits"), traitsText);

			Dictionary<string, e.Upgrade> upgradeLookup = GetUpgrades(Path.Combine(dataFolder, @"World\Upgrades"), upgradesLocalizationText);

			List<Weapon> weapons = new();
			weapons = extractor.ExtractWeaponInfo(Path.Combine(dataFolder, @"World\Weapons"), weaponLocalizationText, 
				upgradeLookup, traitsLookup);

			Dictionary<string, Weapon> weaponLookup = weapons.ToDictionary(x => x.Key);

			List<Unit> gladiusUnits = extractor.ExtractUnitInfo(Path.Combine(dataFolder, @"World\Units"),
				weaponLookup, unitLocalizationText, upgradeLookup, traitsLookup);

			return gladiusUnits;

		}

		private Dictionary<string, e.Upgrade> GetUpgrades(string dataFolder, Dictionary<string, string> upgradesLocalizationText)
		{
			//Get the folder name with a trailing slash

			//Used to trim the start of the path to get the item's key.
			string basePath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(dataFolder)) + "\\";

			Dictionary<string, e.Upgrade> upgrades = new Dictionary<string, e.Upgrade>();
			Regex iconRegEx = new Regex(@"icon="".+?""");
			///There are two icons in some files.
			foreach (string file in Directory.EnumerateFiles(basePath, "*.xml", SearchOption.AllDirectories))
			{
				try
				{

					string xml = File.ReadAllText(file);

					//--Fix multiple icons in some XML
					var iconMatches = iconRegEx.Matches(xml);

					if(iconMatches.Count > 1)
					{
						//For now, remove the first match.
						xml = xml.Replace(iconMatches[0].Value, "");
					}


					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(xml);

					e.Upgrade upgrade = new e.Upgrade();

					upgrade.Key = GetKey(basePath, file);

					//Todo:  two icons.... D:\Games\Steam\steamapps\common\Warhammer 40000 Gladius - Relics of War\Data\World\Upgrades\Tau\RipykaVa.xml

					var upgradeNode = xmlDocument.SelectSingleNode("upgrade[@icon]");

					upgrade.Icon = GetPathedIcon(upgradeNode?.Attributes?["icon"]?.Value, "Upgrades", upgrade.Key);

					SetLocalizedStrings(upgrade, upgradesLocalizationText);

					upgrades.Add(upgrade.Key, upgrade);

				}
				catch (Exception ex)
				{
					throw new ApplicationException($"Error parsing {file}: {ex.Message}", ex);
				}
			}

			return upgrades;
		}

		private void SetLocalizedStrings(e.ILocalizedStrings target, Dictionary<string, string> localizationLookup)
		{
			target.Name = localizationLookup.GetValueOrDefault(target.Key, "");
			target.Description = localizationLookup.GetValueOrDefault(target.Key + "Description" , "");

			if (target.Name == "") target.Name = target.Key;

		}

		private static void ExtractLanguages(string dataFolder, 
			out Dictionary<string, string> factionLocallization, 
			out Dictionary<string, string> weaponLocalizationText, 
			out Dictionary<string, string> traitsText, 
			out Dictionary<string, string> unitLocalizationText,
			out Dictionary<string, string> upgradesLocalizationText)
		{
			LanguageExtract languageExtract = new LanguageExtract();


			traitsText = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Traits.xml"));

			weaponLocalizationText = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Weapons.xml"));

			factionLocallization = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Factions.xml"));

			unitLocalizationText = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Units.xml"));

			upgradesLocalizationText = languageExtract.GetTextStrings(
				Path.Combine(dataFolder, @"Core\Languages\English\Upgrades.xml"));


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

					Trait trait = new Trait();

					trait.Key = GetKey(basePath, file);

					XmlNode traitNode = xmlDocument.SelectSingleNode("trait")!;

					//Some items do not have an icon. Mostly buildings.
					//Not sure why, but the icon for "InstinctiveBehavior" isn't mapped.
					//		It appears to be "Traits\Tyranids\InstinctiveBehaviourLurk.png"

					switch (trait.Key)
					{
						case @"Traits/Tyranids/InstinctiveBehaviour":
							trait.Icon = @"Traits/Tyranids/InstinctiveBehaviourLurk";
							break;
						case @"Suicider":
							//Can't find an icon.  I don't have DLC4 so currently can't see what it looks like.
							trait.Icon = "Missing";
							break;

					default:
							trait.Icon = GetPathedIcon(traitNode.Attributes!["icon"]?.Value, "Traits", trait.Key);
							break;
					}

					SetLocalizedStrings(trait, traitsText);
					trait.Effects = ParseEffects(xmlDocument, "/trait/modifiers/modifier/effects/*");

					//Not parsing required upgrades since they are actually part of modifier effects.
					//	that would require modeling the modifer and their effects.
					///	XPath: trait/modifiers/modifier/effects/production
					//	<modifier visible="0" requiredUpgrade="Orks/CreatePermanentOrkoidFungusOnDeath">

					trait.RequiredUpgrade = null;

					traits.Add(trait.Key, trait);
				}
				catch (Exception ex)
				{
					throw new ApplicationException($"Error parsing {file}: {ex.Message}", ex);
				}
			}
			return traits;

		}


		/// <summary>
		/// Returns a consistent path to an icon.
		/// For example, an icon may or may not have a folder prefix ("Tyranids/Biomorph")
		/// Required since the data is inconsistent.
		/// </summary>
		/// <param name="iconPath">The icon path as set from the extractor.</param>
		/// <param name="defaultFolder">The root folder to use if the path does not contain a root folder or no icon data</param>
		/// <param name="itemKey">The trait's key.  Used as the default icon name if icon is not set.</param>
		/// <returns></returns>
		private string GetPathedIcon(string? iconPath, string defaultFolder, string itemKey)
		{
			if(!string.IsNullOrEmpty(iconPath))
			{
				//Sometimes the folders are not set and assumes the context's folder (Weapon or Action, etc.)
				string[] paths = iconPath.Split("/");

				if(paths.Length == 1)
				{
					//Assume the root folder is not set.
					return String.Join("/", defaultFolder, iconPath);
				}
				else
				{
					if (DataFolderNames.Contains(paths[0]))
					{
						//Already has a "root" folder path
						return iconPath;
					}
					else
					{
						//Add path
						return String.Join("/", defaultFolder, itemKey);
					}
				}
			}
			else
			{
				//path is not set.
				return String.Join("/", defaultFolder, itemKey);
			}
		}

		internal List<Unit> ExtractUnitInfo(string folderName, Dictionary<string, Weapon> weaponLookup, 
			Dictionary<string, string> weaponNameLookup, Dictionary<string, e.Upgrade> upgradeLookup, 
			Dictionary<string, Trait> traitLookup)
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
						string requiredUpgradeKey = weaponNode.Attributes!["requiredUpgrade"]?.Value ?? "";

						e.Upgrade? requiredUpgrade = null;

						if (requiredUpgradeKey != "") requiredUpgrade = upgradeLookup[requiredUpgradeKey];

						unit.Weapons.Add(new UnitWeapon(weaponLookup[weaponNode.Attributes!["name"]!.Value],
							requiredUpgrade));
					}

					GetSecondaryRequirements(xmlDocument, unit, upgradeLookup);


					//--Traits
					List<Trait> traits = new();

					//--Traits
					XmlNodeList traitNodes = xmlDocument.SelectNodes("unit/traits/trait")!;

					foreach (XmlNode traitNode in traitNodes)
					{
						string? requiredUpgrade = traitNode.Attributes?["requiredUpgrade"]?.Value;

						string trait = traitNode.Attributes!["name"]!.Value;



						traits.Add(traitLookup[trait]);
					}

					unit.Traits.AddRange(traits.OrderBy(x => x.Name).ThenBy(x => x.RequiredUpgrade?.Name));


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
		private static void GetSecondaryRequirements(XmlDocument xmlDocument, Unit unit, Dictionary<string, e.Upgrade> upgradeLookup)
		{

			//All actions that have both a required upgrade and a weapon key
			XmlNodeList actionRequirementNodes = xmlDocument.SelectNodes("unit/actions/*[@requiredUpgrade and @weaponSlotName]")!;

			var actionRequirements = actionRequirementNodes.Cast<XmlNode>()
				.Select(x => new
				{
					weaponKey = x.Attributes!["weaponSlotName"]!.Value,
					upgrade = x.Attributes!["requiredUpgrade"]!.Value
				}
			);

			//The requirements that come from the weapon's actions.
			//	Should be only if the weapon's upgrade isn't already set.
			var actionWeaponJoinList = unit.Weapons
				.Join(actionRequirements, 
					x => x.Weapon.Key,
					x => x.weaponKey, 
					(unitWeapon, actionRequirement) => new { unitWeapon, upgradeKey = actionRequirement.upgrade });


			//Validate and set
			foreach (var actionWeaponJoin in actionWeaponJoinList)
			{
				if (actionWeaponJoin.unitWeapon.RequiredUpgrade is null)
				{
					//Only set if it is not already set.  It seems that only the unit's requirement for the upgrade
					//	is set or the action's upgrade is set.
					actionWeaponJoin.unitWeapon.RequiredUpgrade = upgradeLookup[actionWeaponJoin.upgradeKey];
				}
				else
				{
					//Data validation.  If it is set, check if it is not the same.
					if (actionWeaponJoin.unitWeapon.RequiredUpgrade!.Key != actionWeaponJoin.upgradeKey)
					{
						throw new InvalidDataException($"Adding weapon upgrade requirements from actions.  '{actionWeaponJoin.unitWeapon.Weapon.Name} has an action adn weapon required upgrade and they are of different types.");
					}
				}
			}
		}

		internal List<Weapon> ExtractWeaponInfo(string folderName, Dictionary<string, string> weaponLocalizationText, 
			Dictionary<string, e.Upgrade> upgradeLookup, Dictionary<string, Trait> traitsLookup)
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

				//TODO:  upgrades must require a trait and the required upgrade as a pair.
				//Should just change traits instead of having "requirements"


				foreach (XmlNode requirementNode in requireNodes)
				{
					List<e.Upgrade> requirements = weapon.Requirements;

					string requirementName = requirementNode.Attributes!["name"]!.Value;
					string requiredUpgrade = requirementNode.Attributes["requiredUpgrade"]!.Value;

					requirements.Add(upgradeLookup[requiredUpgrade]);

				}

				//----Traits
				XmlNodeList traitNodes = xmlDocument.SelectNodes(@"/weapon/traits/trait[not(@requiredUpgrade)]")!;

				if (traitNodes.Count > 0)
				{

					foreach (XmlNode traitNode in traitNodes)
					{
						weapon.Traits.Add(traitsLookup[traitNode.Attributes!["name"]!.Value]);
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
