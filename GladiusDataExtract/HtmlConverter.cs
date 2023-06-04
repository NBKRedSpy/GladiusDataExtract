using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladiusDataExtract.Units;

namespace GladiusDataExtract
{
    public class HtmlConverter
    {

        public string Convert(List<Unit> units)
        {


            StringBuilder sb = new StringBuilder();



            foreach (Unit unit in units)
            {
                sb.AppendLine($"<div>{unit.Name}</div>");
                sb.AppendLine($"<div>");

                List<string> attributeNames = new()
                {
                    "armor",
                    "hitpointsMax",
                    "movementMax",
                    "biomassCost",
                    "productionCost",
                    "biomassUpkeep",
                    "influenceUpkeep",
                };

                IEnumerable<UnitAttribute> filteredAttributes = attributeNames.Join(unit.Attributes, x => x, x => x.Name, (outer, inner) => inner);

                foreach (var attribute in filteredAttributes)
                {
                    sb.AppendLine($"<span>{attribute.Name} {attribute.Value:#.##}</span");
                }

				//sb.Append($"<span>{unit.Attributes.armo})
			//<effects>
			//	<armor base="8"/> <!-- %armor base armor=3+ -->
			//	<biomassUpkeep base="4.0"/> <!-- %biomassUpkeep base tier=8 factor=1 -->
			//	<biomassCost base="80.0"/> <!-- %biomassCost base tier=8 factor=1 -->
			//	<hitpointsMax base="42.0"/> <!-- %hitpointsMax base toughness=6 wounds=7 -->
			//	<influenceUpkeep base="6.0"/> <!-- %influenceUpkeep base tier=8 factor=1.5 -->
			//	<influenceCost base="120.0"/> <!-- %influenceCost base tier=8 factor=1.5 -->
			//	<itemSlots base="6"/>
			//	<levelMax base="9"/>
			//	<meleeAccuracy base="6"/> <!-- %meleeAccuracy base weaponSkill=3 -->
			//	<meleeAttacks base="2"/>
			//	<strengthDamage base="2"/> <!-- %strengthDamage base strength=5 -->
			//	<moraleMax base="12"/> <!-- %moraleMax base leadership=9 -->
			//	<movementMax base="3"/>
			//	<productionCost base="48.0"/> <!-- %productionCost base tier=8 factor=1 -->
			//	<rangedAccuracy base="6"/> <!-- %rangedAccuracy base ballisticSkill=3 -->
			//</effects>
                //unit.Attributes
                //sb.Append("<span>{unit.Name}</div>\")




                break;  //Debug - only do one for now.
            }

//            sb.Append(@"
//<!DOCTYPE html>
//<html lang=""en"">
//<head>
//    <meta charset=""UTF-8"">
//    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
//    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
//    <title>Document</title>
//</head>
//<body>
//<<<<<<<Insert html here>>>>>>>>>>>>>>>
//</body>
//</html>");








            throw new NotImplementedException();
        }


    }
}
