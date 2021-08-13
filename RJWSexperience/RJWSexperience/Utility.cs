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

        public static bool ContainAll(this string str, string[] tags)
        {
            string lstr = str.ToLower();
            if (!tags.NullOrEmpty()) for (int i=0; i< tags.Count(); i++)
                {
                    if (!lstr.Contains('[' + tags[i].ToLower() + ']')) return false;
                }
            return true;
        }


        public static float LustFactor(this Pawn pawn)
        {
            float lust = pawn.records.GetValue(VariousDefOf.Lust) * Configurations.LustEffectPower;
            if (lust < 0)
            {
                lust = Mathf.Exp((lust + 200f * Mathf.Log(10f)) / 100f) - 100f;
            }
            else
            {
                lust = Mathf.Sqrt(100f*(lust + 25f)) - 50f;
            }
            
            return 1 + lust / 100f;
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

        public static Building GetAdjacentBuilding<T>(this Pawn pawn) where T : Building 
        {

            if (pawn.Spawned)
            {
                EdificeGrid edifice = pawn.Map.edificeGrid;
                if (edifice[pawn.Position] is T) return edifice[pawn.Position];
                IEnumerable<IntVec3> adjcells = GenAdjFast.AdjacentCells8Way(pawn.Position);
                foreach(IntVec3 pos in adjcells)
                {
                    if (edifice[pos] is T) return edifice[pos];
                }
            }
            return null;
        }


        public static float GetCumVolume(this Pawn pawn)
        {
            List<Hediff> hediffs = Genital_Helper.get_PartsHediffList(pawn, Genital_Helper.get_genitalsBPR(pawn));
            if (hediffs.NullOrEmpty()) return 0;
            else return pawn.GetCumVolume(hediffs);
        }

        public static float GetCumVolume(this Pawn pawn, List<Hediff> hediffs)
        {
            CompHediffBodyPart part = hediffs?.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("penis")).InRandomOrder().FirstOrDefault()?.TryGetComp<CompHediffBodyPart>();
            if (part == null) part = hediffs?.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("ovipositorf")).InRandomOrder().FirstOrDefault()?.TryGetComp<CompHediffBodyPart>();
            if (part == null) part = hediffs?.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("ovipositorm")).InRandomOrder().FirstOrDefault()?.TryGetComp<CompHediffBodyPart>();
            if (part == null) part = hediffs?.FindAll((Hediff hed) => hed.def.defName.ToLower().Contains("tentacle")).InRandomOrder().FirstOrDefault()?.TryGetComp<CompHediffBodyPart>();


            return pawn.GetCumVolume(part);
        }


        public static float GetCumVolume(this Pawn pawn, CompHediffBodyPart part)
        {
            float res;

            try
            {
                res = part.FluidAmmount * part.FluidModifier * pawn.BodySize / pawn.RaceProps.baseBodySize * Rand.Range(0.8f, 1.2f) * RJWSettings.cum_on_body_amount_adjust * 0.3f;
            }
            catch (NullReferenceException)
            {
                res = 0.0f;
            }
            if (pawn.Has(Quirk.Messy)) res *= Rand.Range(4.0f, 8.0f);

            return res;
        }

        public static void CumDrugEffect(this Pawn pawn)
        {
            Need need = pawn.needs?.TryGetNeed(VariousDefOf.Chemical_Cum);
            if (need != null) need.CurLevel += VariousDefOf.CumneedLevelOffset;
            Hediff addictive = HediffMaker.MakeHediff(VariousDefOf.CumTolerance, pawn);
            addictive.Severity = 0.032f;
            pawn.health.AddHediff(addictive);
            Hediff addiction = pawn.health.hediffSet.GetFirstHediffOfDef(VariousDefOf.CumAddiction);
            if (addiction != null) addiction.Severity += VariousDefOf.CumexistingAddictionSeverityOffset;

            pawn.needs?.mood?.thoughts?.memories?.TryGainMemoryFast(VariousDefOf.AteCum);
        }

    }
}
