using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RJWSexperience
{
    public class VariousDefOf
    {
        public static readonly RecordDef NumofEatenCum = DefDatabase<RecordDef>.GetNamed("NumofEatenCum");
        public static readonly RecordDef Lust = DefDatabase<RecordDef>.GetNamed("Lust");
        public static readonly SkillDef SexSkill = DefDatabase<SkillDef>.GetNamed("Sex");
        public static readonly ThoughtDef_Recordbased AteCum = DefDatabase<ThoughtDef_Recordbased>.GetNamed("AteCum");

        [MayRequireIdeology] public static readonly MemeDef Zoophile = DefDatabase<MemeDef>.GetNamed("Zoophile");
        [MayRequireIdeology] public static readonly MemeDef Rapist = DefDatabase<MemeDef>.GetNamed("Rapist");
        [MayRequireIdeology] public static readonly HistoryEventDef SexWithAnimal = DefDatabase<HistoryEventDef>.GetNamed("SexWithAnimal");
        [MayRequireIdeology] public static readonly HistoryEventDef SexWithVeneratedAnimal = DefDatabase<HistoryEventDef>.GetNamed("SexWithVeneratedAnimal");
        [MayRequireIdeology] public static readonly HistoryEventDef Raped = DefDatabase<HistoryEventDef>.GetNamed("Raped");
        [MayRequireIdeology] public static readonly HistoryEventDef RapedSlave = DefDatabase<HistoryEventDef>.GetNamed("RapedSlave");
        [MayRequireIdeology] public static readonly HistoryEventDef RapedPrisoner = DefDatabase<HistoryEventDef>.GetNamed("RapedPrisoner");
        [MayRequireIdeology] public static readonly HistoryEventDef WasRaped = DefDatabase<HistoryEventDef>.GetNamed("WasRaped");
        [MayRequireIdeology] public static readonly HistoryEventDef WasRapedSlave = DefDatabase<HistoryEventDef>.GetNamed("WasRapedSlave");
        [MayRequireIdeology] public static readonly HistoryEventDef WasRapedPrisoner = DefDatabase<HistoryEventDef>.GetNamed("WasRapedPrisoner");
        [MayRequireIdeology] public static readonly HistoryEventDef VaginalSex = DefDatabase<HistoryEventDef>.GetNamed("VaginalSex");
        [MayRequireIdeology] public static readonly HistoryEventDef AnalSex = DefDatabase<HistoryEventDef>.GetNamed("AnalSex");
        [MayRequireIdeology] public static readonly HistoryEventDef OralSex = DefDatabase<HistoryEventDef>.GetNamed("OralSex");
        [MayRequireIdeology] public static readonly HistoryEventDef MiscSex = DefDatabase<HistoryEventDef>.GetNamed("MiscSex");
        [MayRequireIdeology] public static readonly HistoryEventDef PromiscuousSex = DefDatabase<HistoryEventDef>.GetNamed("PromiscuousSex");
        [MayRequireIdeology] public static readonly HistoryEventDef Incestuos_Marriage = DefDatabase<HistoryEventDef>.GetNamed("Incestuos_Marriage");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_Abhorrent = DefDatabase<PreceptDef>.GetNamed("Bestiality_Abhorrent");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_Horrible = DefDatabase<PreceptDef>.GetNamed("Bestiality_Horrible");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_Disapproved = DefDatabase<PreceptDef>.GetNamed("Bestiality_Disapproved");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_Acceptable = DefDatabase<PreceptDef>.GetNamed("Bestiality_Acceptable");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_OnlyVenerated = DefDatabase<PreceptDef>.GetNamed("Bestiality_OnlyVenerated");
        [MayRequireIdeology] public static readonly PreceptDef Bestiality_Honorable = DefDatabase<PreceptDef>.GetNamed("Bestiality_Honorable");
        [MayRequireIdeology] public static readonly PreceptDef Rape_Abhorrent = DefDatabase<PreceptDef>.GetNamed("Rape_Abhorrent");
        [MayRequireIdeology] public static readonly PreceptDef Rape_Horrible = DefDatabase<PreceptDef>.GetNamed("Rape_Horrible");
        [MayRequireIdeology] public static readonly PreceptDef Rape_Disapproved = DefDatabase<PreceptDef>.GetNamed("Rape_Disapproved");
        [MayRequireIdeology] public static readonly PreceptDef Rape_Acceptable = DefDatabase<PreceptDef>.GetNamed("Rape_Acceptable");
        [MayRequireIdeology] public static readonly PreceptDef Rape_Honorable = DefDatabase<PreceptDef>.GetNamed("Rape_Honorable");
        [MayRequireIdeology] public static readonly PreceptDef Sex_Free = DefDatabase<PreceptDef>.GetNamed("Sex_Free");
        [MayRequireIdeology] public static readonly PreceptDef Sex_VaginalOnly = DefDatabase<PreceptDef>.GetNamed("Sex_VaginalOnly");
        [MayRequireIdeology] public static readonly PreceptDef Sex_AnalOnly = DefDatabase<PreceptDef>.GetNamed("Sex_AnalOnly");
        [MayRequireIdeology] public static readonly PreceptDef Sex_OralOnly = DefDatabase<PreceptDef>.GetNamed("Sex_OralOnly");
        [MayRequireIdeology] public static readonly PreceptDef Sex_Promiscuous = DefDatabase<PreceptDef>.GetNamed("Sex_Promiscuous");
        [MayRequireIdeology] public static readonly PreceptDef Incestuos_Free = DefDatabase<PreceptDef>.GetNamed("Incestuos_Free");
        [MayRequireIdeology] public static readonly PreceptDef Incestuos_Disapproved = DefDatabase<PreceptDef>.GetNamed("Incestuos_Disapproved");
        [MayRequireIdeology] public static readonly PreceptDef Incestuos_Forbidden = DefDatabase<PreceptDef>.GetNamed("Incestuos_Forbidden");
        [MayRequireIdeology] public static readonly PreceptDef Incestuos_IncestOnly = DefDatabase<PreceptDef>.GetNamed("Incestuos_IncestOnly");
        [MayRequireIdeology] public static readonly PreceptDef Incestuos_Disapproved_CloseOnly = DefDatabase<PreceptDef>.GetNamed("Incestuos_Disapproved_CloseOnly");
    }
}
