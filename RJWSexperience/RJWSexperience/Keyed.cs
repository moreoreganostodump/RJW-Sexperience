using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
using UnityEngine;

namespace RJWSexperience
{
    public static class Keyed
    {
        public static string LustStatFactor(string value) => "LustStatFactor".Translate(value);
        public static string SlaveStatFactor(string value) => "SlaveStatFactor".Translate(value);
        public static string MemeStatFactor(string value) => "MemeStatFactor".Translate(value);
        public static string RS_LostVirgin(string pawn, string partner) => "RS_LostVirgin".Translate(pawn.Colorize(Color.yellow),partner.Colorize(Color.yellow));
        public static string RS_Sex_Info(string sextype, string sexcount) => "RS_Sex_Info".Translate(sextype, sexcount);
        public static string RS_SAT_AVG(string avgsat) => "RS_SAT_AVG".Translate(avgsat);

        public static readonly string Mod_Title = "RS_Mod_Title".Translate();
        public static readonly string SlaveStatFactorDefault = "SlaveStatFactorDefault".Translate();
        public static readonly string RSVictimCondition = "RSVictimCondition".Translate();
        public static readonly string RSBreederCondition = "RSBreederCondition".Translate();
        public static readonly string RSNotHuman = "RSNotHuman".Translate();
        public static readonly string RSNotAnimal = "RSNotAnimal".Translate();
        public static readonly string RSShouldCanFuck = "RSShouldCanFuck".Translate();
        public static readonly string RSTotalGatheredCum = "RSTotalGatheredCum".Translate();
        public static readonly string RS_FloatMenu_CleanSelf = "RS_FloatMenu_CleanSelf".Translate();
        public static readonly string RS_Best_Sextype = "RS_Best_Sextype".Translate();
        public static readonly string RS_Recent_Sextype = "RS_Recent_Sextype".Translate();
        public static readonly string RS_Sex_Partners = "RS_Sex_Partners".Translate();
        public static readonly string RS_Cum_Swallowed = "RS_Cum_Swallowed".Translate();
        public static readonly string RS_Selected_Partner = "RS_Selected_Partner".Translate();
        public static readonly string RS_Sex_Count = "RS_Sex_Count".Translate();
        public static readonly string RS_Orgasms = "RS_Orgasms".Translate();
        public static readonly string RS_Recent_Sex_Partner = "RS_Recent_Sex_Partner".Translate();
        public static readonly string RS_First_Sex_Partner = "RS_First_Sex_Partner".Translate();
        public static readonly string RS_Most_Sex_Partner = "RS_Most_Sex_Partner".Translate();
        public static readonly string RS_Best_Sex_Partner = "RS_Best_Sex_Partner".Translate();
        public static readonly string RS_VirginsTaken = "RS_VirginsTaken".Translate();
        public static readonly string RS_TotalSexHad = "RS_TotalSexHad".Translate();
        public static readonly string RS_Recent_Sex_Partner_ToolTip = "RS_Recent_Sex_Partner_ToolTip".Translate();
        public static readonly string RS_First_Sex_Partner_ToolTip = "RS_First_Sex_Partner_ToolTip".Translate();
        public static readonly string RS_Most_Sex_Partner_ToolTip = "RS_Most_Sex_Partner_ToolTip".Translate();
        public static readonly string RS_Best_Sex_Partner_ToolTip = "RS_Best_Sex_Partner_ToolTip".Translate();
        public static readonly string RS_VirginsTaken_ToolTip = "RS_VirginsTaken_ToolTip".Translate();
        public static readonly string RS_TotalSexHad_ToolTip = "RS_TotalSexHad_ToolTip".Translate();
        public static readonly string RS_Raped = "RS_Raped".Translate();
        public static readonly string RS_RapedMe = "RS_RapedMe".Translate();
        public static readonly string RS_Sex_History = "RS_Sex_History".Translate();
        public static readonly string RS_Statistics = "RS_Statistics".Translate();
        public static readonly string RS_PartnerList = "RS_PartnerList".Translate();
        public static readonly string RS_Sexuality = "RS_Sexuality".Translate();
        public static readonly string RS_BeenRaped = "RS_BeenRaped".Translate();
        public static readonly string RS_RapedSomeone = "RS_RapedSomeone".Translate();
        public static readonly string RS_PreferRace = "RS_PreferRace".Translate();
        public static readonly string Virgin = "Virgin".Translate();
        public static readonly string Lust = "Lust".Translate();
        public static readonly string Unknown = "Unknown".Translate();
        public static readonly string Incest = "Incest".Translate();
        public static readonly string None = "None".Translate();
        public static readonly string RS_Bestiality = "RS_Bestiality".Translate();
        public static readonly string RS_Interspecies = "RS_Interspecies".Translate();
        public static readonly string RS_Normal = "RS_Normal".Translate();
        public static readonly string RS_Necrophile = "RS_Necrophile".Translate();
        public static readonly string RS_GatherCum = "RS_GatherCum".Translate();
        public static readonly string RS_SexSkill = "RS_SexSkill".Translate();
        public static readonly string RS_CumAddiction = "RS_CumAddiction".Translate();
        public static readonly string RS_CumAddiction_Tooltip = "RS_CumAddiction_Tooltip".Translate();
        public static readonly string RS_CumAddictiveness = "RS_CumAddictiveness".Translate();
        public static readonly string RS_CumAddictiveness_Tooltip = "RS_CumAddictiveness_Tooltip".Translate();
        public static readonly string RS_NumofTimes = "RS_NumofTimes".Translate();
        

        public static readonly string Option_1_Label = "RSOption_1_Label".Translate();
        public static readonly string Option_1_Desc  = "RSOption_1_Desc".Translate();
        public static readonly string Option_2_Label = "RSOption_2_Label".Translate();
        public static readonly string Option_2_Desc = "RSOption_2_Desc".Translate();
        public static readonly string Option_3_Label = "RSOption_3_Label".Translate();
        public static readonly string Option_3_Desc = "RSOption_3_Desc".Translate();
        public static readonly string Option_4_Label = "RSOption_4_Label".Translate();
        public static readonly string Option_4_Desc = "RSOption_4_Desc".Translate();
        public static readonly string Option_5_Label = "RSOption_5_Label".Translate();
        public static readonly string Option_5_Desc = "RSOption_5_Desc".Translate();
        public static readonly string Option_6_Label = "RSOption_6_Label".Translate();
        public static readonly string Option_6_Desc = "RSOption_6_Desc".Translate();
        public static readonly string Option_7_Label = "RSOption_7_Label".Translate();
        public static readonly string Option_7_Desc = "RSOption_7_Desc".Translate();
        public static readonly string Option_8_Label = "RSOption_8_Label".Translate();
        public static readonly string Option_8_Desc = "RSOption_8_Desc".Translate();
        public static readonly string Option_9_Label = "RSOption_9_Label".Translate();
        public static readonly string Option_9_Desc = "RSOption_9_Desc".Translate();
        public static readonly string Option_10_Label = "RSOption_10_Label".Translate();
        public static readonly string Option_10_Desc = "RSOption_10_Desc".Translate();
        public static readonly string Option_11_Label = "RSOption_11_Label".Translate();
        public static readonly string Option_11_Desc = "RSOption_11_Desc".Translate();

        
        public static readonly string[] Sextype =
        {
            ((xxx.rjwSextype)0).ToString().Translate(),
            ((xxx.rjwSextype)1).ToString().Translate(),
            ((xxx.rjwSextype)2).ToString().Translate(),
            ((xxx.rjwSextype)3).ToString().Translate(),
            ((xxx.rjwSextype)4).ToString().Translate(),
            ((xxx.rjwSextype)5).ToString().Translate(),
            ((xxx.rjwSextype)6).ToString().Translate(),
            ((xxx.rjwSextype)7).ToString().Translate(),
            ((xxx.rjwSextype)8).ToString().Translate(),
            ((xxx.rjwSextype)9).ToString().Translate(),
            ((xxx.rjwSextype)10).ToString().Translate(),
            ((xxx.rjwSextype)11).ToString().Translate(),
            ((xxx.rjwSextype)12).ToString().Translate(),
            ((xxx.rjwSextype)13).ToString().Translate(),
            ((xxx.rjwSextype)14).ToString().Translate(),
            ((xxx.rjwSextype)15).ToString().Translate(),
            ((xxx.rjwSextype)16).ToString().Translate(),
            ((xxx.rjwSextype)17).ToString().Translate(),
            ((xxx.rjwSextype)18).ToString().Translate(),
            ((xxx.rjwSextype)19).ToString().Translate(),
            ((xxx.rjwSextype)20).ToString().Translate()
        };

        public static readonly string[] Sexuality =
        {
            ((Orientation)0).ToString().Translate(),
            ((Orientation)1).ToString().Translate(),
            ((Orientation)2).ToString().Translate(),
            ((Orientation)3).ToString().Translate(),
            ((Orientation)4).ToString().Translate(),
            ((Orientation)5).ToString().Translate(),
            ((Orientation)6).ToString().Translate(),
            ((Orientation)7).ToString().Translate(),
            ((Orientation)8).ToString().Translate(),
            ((Orientation)9).ToString().Translate(),
            ((Orientation)10).ToString().Translate()
        };

    }
}
