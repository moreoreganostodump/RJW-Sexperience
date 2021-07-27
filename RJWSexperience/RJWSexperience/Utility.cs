using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;
using UnityEngine;

namespace RJWSexperience
{
    public static class Utility
    {
        public static System.Random random = new System.Random(Environment.TickCount);

        public static float RandGaussianLike(float min, float max, int iterations = 3)
        {
            double res = 0;
            for (int i = 0; i < iterations; i++)
            {
                res += random.NextDouble();
            }
            res = res / iterations;

            return (float)res * (max - min) + min;
        }

        public static void SetTo(this Pawn_RecordsTracker records, RecordDef record ,float value)
        {
            float recordval = records.GetValue(record);
            records.AddTo(record, value - recordval);
        }

        public static bool IsIncest(this Pawn pawn, Pawn partner)
        {
            IEnumerable<PawnRelationDef> relations = pawn.GetRelations(partner);
            Ideo ideo = pawn.Ideo;
            bool wide = false;
            if (ideo != null) wide = ideo.HasPrecept(VariousDefOf.Incestuos_Disapproved_CloseOnly);
            if (!relations.EnumerableNullOrEmpty()) foreach(PawnRelationDef relation in relations)
            {
                if(wide)
                {
                    if (relation.incestOpinionOffset < 0) return true;
                }
                else if (relation.familyByBloodRelation) return true;
            }
            return false;
        }
        
        public static float RecordRandomizer(this Pawn pawn, RecordDef record, float avg, float dist, float min = 0, float max = float.MaxValue)
        {
            float value = Mathf.Clamp(RandGaussianLike(avg - dist,avg + dist),min,max);
            float recordvalue = pawn.records.GetValue(record);
            pawn.records.AddTo(record, value - recordvalue);

            return value;
        }



    }
}
