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
    public static class IdeoUtility
    {


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
