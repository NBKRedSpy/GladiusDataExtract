using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GladiusDataExtract
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
			XmlDocument xmlDocument = new XmlDocument();

			xmlDocument.Load(path);

			Dictionary<string, string> textLookup = new();

			foreach (XmlNode xmlNode in xmlDocument.SelectNodes("entry")!)
			{
				textLookup.Add(xmlNode.Attributes!["name"]!.Value, xmlNode.Attributes!["value"]!.Value);
			}

			return textLookup;
		}
	}
}
