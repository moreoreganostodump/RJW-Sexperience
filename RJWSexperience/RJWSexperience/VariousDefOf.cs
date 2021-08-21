using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RJWSexperience
{
    public static class VariousDefOf
    {
        public static readonly RecordDef NumofEatenCum = DefDatabase<RecordDef>.GetNamed("NumofEatenCum");
        public static readonly RecordDef Lust = DefDatabase<RecordDef>.GetNamed("Lust");
        public static readonly SkillDef SexSkill = DefDatabase<SkillDef>.GetNamed("Sex");
        public static readonly ThoughtDef_Recordbased AteCum = DefDatabase<ThoughtDef_Recordbased>.GetNamed("AteCum");
        public static readonly PawnRelationDef Bastard = DefDatabase<PawnRelationDef>.GetNamed("Bastard");
        public static readonly ThingDef GatheredCum = DefDatabase<ThingDef>.GetNamed("GatheredCum");
        public static readonly HediffDef CumAddiction = DefDatabase<HediffDef>.GetNamed("CumAddiction");
        public static readonly HediffDef CumTolerance = DefDatabase<HediffDef>.GetNamed("CumTolerance");
        public static readonly ChemicalDef Cum = DefDatabase<ChemicalDef>.GetNamed("Cum");
        public static readonly NeedDef Chemical_Cum = DefDatabase<NeedDef>.GetNamed("Chemical_Cum");
        
        public static float CumneedLevelOffset
        {
            get
            {
                if (cumneedLevelOffsetcache == null)
                {
                    CreateCumCompCache();
                }
                return cumneedLevelOffsetcache ?? 1.0f;
            }
        }
        public static float CumexistingAddictionSeverityOffset
        {
            get
            {
                if (cumexistingAddictionSeverityOffsetcache == null)
                {
                    CreateCumCompCache();
                }
                return cumexistingAddictionSeverityOffsetcache ?? 1.0f;
            }
        }

        private static void CreateCumCompCache()
        {
            CompProperties_Drug comp = (CompProperties_Drug)GatheredCum.comps.FirstOrDefault(x => x is CompProperties_Drug);
            cumneedLevelOffsetcache = comp.needLevelOffset;
            cumexistingAddictionSeverityOffsetcache = comp.existingAddictionSeverityOffset;
        }


        private static float? cumneedLevelOffsetcache = null;
        private static float? cumexistingAddictionSeverityOffsetcache = null;
    }
}
