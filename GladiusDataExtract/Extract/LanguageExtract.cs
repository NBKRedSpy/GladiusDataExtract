﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

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
			Regex regEx = new Regex(@"^(\s+<entry name="".+"" value="")(<string name='(.+)'/>)(""/>)\s*$", RegexOptions.Multiline);

            var matcher = (Match m) =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(m.Groups[1]);    //entry name start

                builder.Append(m.Groups[3].Value);  
				builder.Append(m.Groups[4]);

                return builder.ToString();

            };

			xml = regEx.Replace(xml, new MatchEvaluator(matcher));

            //Some value strings have invalid characters.  
            // Example - Has &:  <entry name="HitAndRun" value="Hit & Run"/>
            Regex valueRegex = new Regex(@"^(\s*<entry name="".+"" value="")(.+)(""/>)\s*$", RegexOptions.Multiline);

			var valueMatcher = (Match m) =>
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(m.Groups[1]);    //entry name start

                builder.Append(HtmlEncoder.Default.Encode(m.Groups[2].Value));
				builder.Append(m.Groups[3]);

				return builder.ToString();
			};


			xml = valueRegex.Replace(xml, new MatchEvaluator(valueMatcher));


            //Debug, may not be needed.
//			xml = xml.Replace("<br/>", HtmlEncoder.Default.Encode("<br/>"));  //Some entries have an HTML line break.  Preserve for rendering.

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
