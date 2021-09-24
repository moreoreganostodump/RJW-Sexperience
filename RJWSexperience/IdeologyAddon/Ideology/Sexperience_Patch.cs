using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using rjw;
using RJWSexperience;
using Verse;
using RimWorld;


namespace RJWSexperience.Ideology
{
    [HarmonyPatch(typeof(RJWUtility), "ThrowVirginHIstoryEvent")]
    public static class Sexperience_Patch_ThrowVirginHIstoryEvent
    {
        public static void Postfix(Pawn pawn, Pawn partner, SexProps props, int degree)
        {
            string tag = "";
            if (props.isRape)
            {
                if (pawn == props.pawn && props.isRapist) tag += HETag.Rape;
                else tag += HETag.BeenRaped;
            }
            if (!pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, partner))
            {
                tag += HETag.NotSpouse;
            }
            
            
            if (pawn.gender == Gender.Male)
            {
                if (degree > 1) Find.HistoryEventsManager.RecordEvent(VariousDefOf.Virgin_TakenM.TaggedEvent(pawn, tag + HETag.Gender(pawn), partner));
                Find.HistoryEventsManager.RecordEvent(VariousDefOf.Virgin_TookM.TaggedEvent(partner, tag + HETag.Gender(pawn), pawn));
            }
            else
            {
                if (degree > 1) Find.HistoryEventsManager.RecordEvent(VariousDefOf.Virgin_TakenF.TaggedEvent(pawn, tag + HETag.Gender(pawn), partner));
                Find.HistoryEventsManager.RecordEvent(VariousDefOf.Virgin_TookF.TaggedEvent(partner, tag + HETag.Gender(pawn), pawn));
            }

            

        }
    }

    [HarmonyPatch(typeof(Utility), "IsIncest")]
    public static class Sexperience_Patch_IsIncest
    {
        public static bool Prefix(Pawn pawn, Pawn otherpawn, ref bool __result)
        {
            __result = IsIncest(pawn, otherpawn);
            return false;
        }

        private static bool IsIncest(Pawn pawn, Pawn partner)
        {
            IEnumerable<PawnRelationDef> relations = pawn.GetRelations(partner);
            Ideo ideo = pawn.Ideo;
            bool wide = false;
            if (ideo != null) wide = ideo.HasPrecept(VariousDefOf.Incestuos_Disapproved_CloseOnly);
            if (!relations.EnumerableNullOrEmpty()) foreach (PawnRelationDef relation in relations)
                {
                    if (wide)
                    {
                        if (relation.incestOpinionOffset < 0) return true;
                    }
                    else if (relation.familyByBloodRelation) return true;
                }
            return false;
        }
    }

}
