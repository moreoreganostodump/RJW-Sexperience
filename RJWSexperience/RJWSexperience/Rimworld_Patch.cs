using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using rjw;




namespace RJWSexperience
{
    [HarmonyPatch(typeof(MarriageCeremonyUtility), "Married")]
    public static class Rimworld_Patch_Marriage
    {
        public static void Postfix(Pawn firstPawn, Pawn secondPawn)
        {
            if (firstPawn.IsIncest(secondPawn))
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(VariousDefOf.Incestuos_Marriage, firstPawn.Named(HistoryEventArgsNames.Doer)));
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(VariousDefOf.Incestuos_Marriage, secondPawn.Named(HistoryEventArgsNames.Doer)));
            }
        }


    }

    [HarmonyPatch(typeof(Pawn_RelationsTracker), "SecondaryRomanceChanceFactor")]
    public static class Rimworld_Patch_SecondaryRomanceChanceFactor
    {
        public static void Postfix(Pawn otherPawn, Pawn ___pawn, ref float __result)
        {
            Ideo ideo = ___pawn.Ideo;
            if (ideo != null)
            {
                if (ideo.HasPrecept(VariousDefOf.Incestuos_IncestOnly) && ___pawn.IsIncest(otherPawn))
                {
                    __result *= 8f;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Rimworld_Patch_GeneratePawn
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            if (__result != null && !request.Newborn && xxx.is_human(__result))
            {
                float avgsex = -500;
                
                int totalsex = 0;
                float totalbirth = 0;
                if (__result.story != null)
                {
                    float lust;
                    if (xxx.is_nympho(__result)) lust = __result.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, 0);
                    else lust = __result.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, float.MinValue);

                    float sexableage = 0;
                    if (__result.ageTracker.AgeBiologicalYears > RJWSettings.sex_minimum_age)
                    {
                        sexableage = __result.ageTracker.AgeBiologicalYearsFloat - RJWSettings.sex_minimum_age;
                        avgsex = sexableage * Configurations.SexPerYear * __result.LustFactor();
                    }



                    if (xxx.is_rapist(__result))
                    {
                        if (xxx.is_zoophile(__result))
                        {
                            if (__result.Has(Quirk.ChitinLover)) totalsex += (int)__result.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, Configurations.MaxSexCountDeviation);
                            else totalsex += (int)__result.RecordRandomizer(xxx.CountOfRapedAnimals, avgsex, Configurations.MaxSexCountDeviation);
                        }
                        else totalsex += (int)__result.RecordRandomizer(xxx.CountOfRapedHumanlikes, avgsex, Configurations.MaxSexCountDeviation);
                        avgsex /= 4;
                    }

                    if (xxx.is_zoophile(__result))
                    {
                        if (__result.Has(Quirk.ChitinLover)) totalsex += (int)__result.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, Configurations.MaxSexCountDeviation);
                        else totalsex += (int)__result.RecordRandomizer(xxx.CountOfSexWithAnimals, avgsex, Configurations.MaxSexCountDeviation);
                        avgsex /= 2;
                    }
                    else if (xxx.is_necrophiliac(__result))
                    {
                        totalsex += (int)__result.RecordRandomizer(xxx.CountOfSexWithCorpse, avgsex, Configurations.MaxSexCountDeviation);
                        avgsex /= 2;
                    }

                    if (__result.IsSlave)
                    {
                        totalsex += (int)__result.RecordRandomizer(xxx.CountOfBeenRapedByAnimals, Rand.Range(-50,10), Rand.Range(0,10)*sexableage);
                        totalsex += (int)__result.RecordRandomizer(xxx.CountOfBeenRapedByHumanlikes, 0, Rand.Range(0,100)*sexableage);
                    }
                    

                    totalsex += (int)__result.RecordRandomizer(xxx.CountOfSexWithHumanlikes, avgsex, Configurations.MaxSexCountDeviation);
                    if (__result.relations != null && __result.gender == Gender.Female)
                    {
                        totalbirth += __result.relations.ChildrenCount;
                        totalsex += (int)totalbirth;
                        __result.records?.AddTo(xxx.CountOfSexWithHumanlikes, totalbirth);
                        __result.records?.SetTo(xxx.CountOfBirthHuman, totalbirth);
                    }
                }
                __result.records?.SetTo(xxx.CountOfSex, totalsex);
            }
        }
    }

    [HarmonyPatch(typeof(RitualOutcomeEffectWorker_FromQuality), "GiveMemoryToPawn")]
    public static class Rimworld_Patch_GiveMemoryToPawn
    {
        public static bool Prefix(Pawn pawn, ThoughtDef memory, LordJob_Ritual jobRitual)
        {
            if (pawn.IsAnimal()) return false;

            return true;
        }
    }


}
