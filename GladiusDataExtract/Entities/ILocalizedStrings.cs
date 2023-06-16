using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	public interface ILocalizedStrings
	{
		public string Key { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }		
    }
}
