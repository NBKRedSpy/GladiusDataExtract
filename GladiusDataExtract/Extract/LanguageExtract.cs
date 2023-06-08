using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GladiusDataExtract.Extract
{
    public class LanguageExtract
    {

        /// <summary>
        /// Parses a localization text file in the "&ltentry name= value=" format
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetTextStrings(string path)
        {

            string xml = File.ReadAllText(path);

            //Some lines are not formatted correctly.  Sometimes in flavor text or not, but it seems to always follow
            //this format:
            //<entry name="AstraMilitarum/Headquarters" value="<string name='Buildings/AstraMilitarum/Headquarters'/>"/>
            //A hack is to replace the start and end of the bad text.

            xml = xml
                .Replace("value=\"<string name='", "value=\"") //Start
                .Replace("'/>\"/>", "\"/>"); //End


            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(xml);

            Dictionary<string, string> textLookup = new();

            foreach (XmlNode xmlNode in xmlDocument.SelectNodes("language/entry")!)
            {
                //For some reason, there are duplicates. 
                //The earlier items seem to be text, while the latter are keys.
                //Examples:
                //	<entry name="FlamerFlavor" value="Flamers are short-ranged weapons that spew out highly volatile clouds of liquid chemicals that ignite on contact with air. They are primarily used to scour the enemy from defended positions, as walls are of no defence against blasts of superheated vapour."/>
                //
                //<entry name="BaleflamerFlavor" value="<string name='Weapons/FlamerFlavor'/>"/>
                textLookup.TryAdd(xmlNode.Attributes!["name"]!.Value, xmlNode.Attributes!["value"]!.Value);
            }

            return textLookup;
        }
    }
}
