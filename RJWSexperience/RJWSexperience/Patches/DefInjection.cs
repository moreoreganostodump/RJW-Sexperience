using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;


namespace RJWSexperience
{
    [StaticConstructorOnStartup]
    public static class DefInjection
    {
        static DefInjection()
        {
            InjectRaces();
        }

        private static void InjectRaces()
        {
            List<ThingDef> PawnDefs = DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && !x.race.IsMechanoid).ToList();
            InjectComp(PawnDefs);
        }

        private static void InjectComp(List<ThingDef> PawnDefs)
        {
            CompProperties comp = new CompProperties(typeof(SexPartnerHistory));
            if (!PawnDefs.NullOrEmpty()) foreach(ThingDef def in PawnDefs)
            {
                    def.comps.Add(comp);
            }
        }
    }
}
