using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladiusDataExtract.Entities
{
	public enum Faction
	{
		Invalid = 0,
		AdeptusMechanicus,
		AstraMilitarum,
		ChaosSpaceMarines,
		Eldar,
		Necrons,
		Neutral,
		[Description("Orks")]
		Orks,
		SistersOfBattle,
		SpaceMarines,
		Tau,
		Tyranids
	}
}
