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

        public static bool IsIncest(this Pawn pawn, Pawn otherpawn)
        {
            if (otherpawn != null)
            {
                IEnumerable<PawnRelationDef> relations = pawn.GetRelations(otherpawn);
                if (!relations.EnumerableNullOrEmpty()) foreach (PawnRelationDef relation in relations)
                    {
                        if (relation.incestOpinionOffset < 0) return true;
                    }
            }
            return false;
        }

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

        
        public static float RecordRandomizer(this Pawn pawn, RecordDef record, float avg, float dist, float min = 0, float max = float.MaxValue)
        {
            float value = Mathf.Clamp(RandGaussianLike(avg - dist,avg + dist),min,max);
            float recordvalue = pawn.records.GetValue(record);
            pawn.records.AddTo(record, value - recordvalue);

            return value;
        }

        public static int RecordRandomizer(this Pawn pawn, RecordDef record, int avg, int dist, int min = 0, int max = int.MaxValue)
        {
            int value = (int)Mathf.Clamp(RandGaussianLike(avg - dist, avg + dist), min, max);
            int recordvalue = pawn.records.GetAsInt(record);
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


        public static T GetAdjacentBuilding<T>(this Pawn pawn) where T : Building 
        {

            if (pawn.Spawned)
            {
                EdificeGrid edifice = pawn.Map.edificeGrid;
                if (edifice[pawn.Position] is T) return (T)edifice[pawn.Position];
                IEnumerable<IntVec3> adjcells = GenAdjFast.AdjacentCells8Way(pawn.Position);
                foreach(IntVec3 pos in adjcells)
                {
                    if (edifice[pos] is T) return (T)edifice[pos];
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


        public static float Normalization(this float num, float min, float max)
        {

            return (num - min)/(max - min);
        }

        public static float Denormalization(this float num, float min, float max)
        {
            return num * (max - min) + min;
        }

        public static void ResetRecord(this Pawn pawn, bool allzero)
        {
            if (!allzero)
            {
                if (Configurations.EnableRecordRandomizer && pawn != null && xxx.is_human(pawn))
                {
                    int avgsex = -500;
                    bool isvirgin = Rand.Chance(Configurations.VirginRatio);
                    int totalsex = 0;
                    int totalbirth = 0;
                    int deviation = (int)Configurations.MaxSexCountDeviation;
                    if (pawn.story != null)
                    {
                        float lust;
                        if (xxx.is_nympho(pawn)) lust = pawn.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, 0);
                        else lust = pawn.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, float.MinValue);

                        int sexableage = 0;
                        int minsexage = (int)(pawn.RaceProps.lifeExpectancy * Configurations.MinSexablePercent);
                        if (pawn.ageTracker.AgeBiologicalYears > minsexage)
                        {
                            sexableage = pawn.ageTracker.AgeBiologicalYears - minsexage;
                            avgsex = (int)(sexableage * Configurations.SexPerYear * pawn.LustFactor());
                        }


                        if (pawn.relations != null && pawn.gender == Gender.Female)
                        {
                            totalbirth += pawn.relations.ChildrenCount;
                            totalsex += totalbirth;
                            pawn.records?.AddTo(xxx.CountOfSexWithHumanlikes, totalbirth);
                            pawn.records?.SetTo(xxx.CountOfBirthHuman, totalbirth);
                            if (totalbirth > 0) isvirgin = false;
                        }
                        if (!isvirgin)
                        {
                            if (xxx.is_rapist(pawn))
                            {
                                if (xxx.is_zoophile(pawn))
                                {
                                    if (pawn.Has(Quirk.ChitinLover)) totalsex += pawn.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, deviation);
                                    else totalsex += pawn.RecordRandomizer(xxx.CountOfRapedAnimals, avgsex, deviation);
                                }
                                else totalsex += pawn.RecordRandomizer(xxx.CountOfRapedHumanlikes, avgsex, deviation);
                                avgsex /= 8;
                            }

                            if (xxx.is_zoophile(pawn))
                            {
                                if (pawn.Has(Quirk.ChitinLover)) totalsex += pawn.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, deviation);
                                else totalsex += pawn.RecordRandomizer(xxx.CountOfSexWithAnimals, avgsex, deviation);
                                avgsex /= 10;
                            }
                            else if (xxx.is_necrophiliac(pawn))
                            {
                                totalsex += pawn.RecordRandomizer(xxx.CountOfSexWithCorpse, avgsex, deviation);
                                avgsex /= 4;
                            }

                            if (pawn.IsSlave)
                            {
                                totalsex += pawn.RecordRandomizer(xxx.CountOfBeenRapedByAnimals, Rand.Range(-50, 10), Rand.Range(0, 10) * sexableage);
                                totalsex += pawn.RecordRandomizer(xxx.CountOfBeenRapedByHumanlikes, 0, Rand.Range(0, 100) * sexableage);
                            }


                            totalsex += pawn.RecordRandomizer(xxx.CountOfSexWithHumanlikes, avgsex, deviation);

                            if (totalsex > 0) pawn.records.AddTo(VariousDefOf.SexPartnerCount, Math.Max(1, Rand.Range(0, totalsex / 7)));
                        }
                    }
                    pawn.records?.SetTo(xxx.CountOfSex, totalsex);
                    RJWUtility.GenerateSextypeRecords(pawn, totalsex);
                }
                if (pawn.story?.traits != null)
                {
                    if (pawn.IsVirgin())
                    {
                        int degree = 0;
                        if (pawn.gender == Gender.Female) degree = 2;
                        Trait virgin = new Trait(VariousDefOf.Virgin, degree, true);
                        pawn.story.traits.GainTrait(virgin);
                    }
                    else if (pawn.gender == Gender.Female && Rand.Chance(0.05f))
                    {
                        Trait virgin = new Trait(VariousDefOf.Virgin, 1, true);
                        pawn.story.traits.GainTrait(virgin);
                    }
                }
            }
            else
            {
                pawn.records.SetTo(VariousDefOf.Lust, 0);
                pawn.records.SetTo(VariousDefOf.NumofEatenCum, 0);
                pawn.records.SetTo(VariousDefOf.AmountofEatenCum, 0);
                pawn.records.SetTo(VariousDefOf.VaginalSexCount, 0);
                pawn.records.SetTo(VariousDefOf.AnalSexCount, 0);
                pawn.records.SetTo(VariousDefOf.OralSexCount, 0);
                pawn.records.SetTo(VariousDefOf.BlowjobCount, 0);
                pawn.records.SetTo(VariousDefOf.CunnilingusCount, 0);
                pawn.records.SetTo(VariousDefOf.GenitalCaressCount, 0);
                pawn.records.SetTo(VariousDefOf.HandjobCount, 0);
                pawn.records.SetTo(VariousDefOf.FingeringCount, 0);
                pawn.records.SetTo(VariousDefOf.FootjobCount, 0);
                pawn.records.SetTo(VariousDefOf.MiscSexualBehaviorCount, 0);
                pawn.records.SetTo(VariousDefOf.SexPartnerCount, 0);
                pawn.records.SetTo(VariousDefOf.OrgasmCount, 0);
                pawn.records.SetTo(xxx.CountOfBeenRapedByAnimals, 0);
                pawn.records.SetTo(xxx.CountOfBeenRapedByHumanlikes, 0);
                pawn.records.SetTo(xxx.CountOfBeenRapedByInsects, 0);
                pawn.records.SetTo(xxx.CountOfBeenRapedByOthers, 0);
                pawn.records.SetTo(xxx.CountOfBirthAnimal, 0);
                pawn.records.SetTo(xxx.CountOfBirthEgg, 0);
                pawn.records.SetTo(xxx.CountOfBirthHuman, 0);
                pawn.records.SetTo(xxx.CountOfFappin, 0);
                pawn.records.SetTo(xxx.CountOfRapedAnimals, 0);
                pawn.records.SetTo(xxx.CountOfRapedHumanlikes, 0);
                pawn.records.SetTo(xxx.CountOfRapedInsects, 0);
                pawn.records.SetTo(xxx.CountOfRapedOthers, 0);
                pawn.records.SetTo(xxx.CountOfSex, 0);
                pawn.records.SetTo(xxx.CountOfSexWithAnimals, 0);
                pawn.records.SetTo(xxx.CountOfSexWithCorpse, 0);
                pawn.records.SetTo(xxx.CountOfSexWithHumanlikes, 0);
                pawn.records.SetTo(xxx.CountOfSexWithInsects, 0);
                pawn.records.SetTo(xxx.CountOfSexWithOthers, 0);
                pawn.records.SetTo(xxx.CountOfWhore, 0);
            }
        }

    }
}
