using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;
using UnityEngine;

namespace RJWSexperience.Ideology
{
    public static class Utility
    {

        public static bool IsIncest(this Pawn pawn, Pawn partner)
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

        public static bool IsSubmissive(this Pawn pawn)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null)
            {
                if (ideo.HasPrecept(VariousDefOf.Submissive_Female) && pawn.gender == Gender.Female) return true;
                else if (ideo.HasPrecept(VariousDefOf.Submissive_Male) && pawn.gender == Gender.Male) return true;
            }

            return false;
        }

    }
}
