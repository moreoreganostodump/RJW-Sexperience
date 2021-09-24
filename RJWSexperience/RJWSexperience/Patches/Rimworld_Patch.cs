using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using rjw;
using UnityEngine;




namespace RJWSexperience
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public static class Rimworld_Patch_GeneratePawn
    {
        public static void Postfix(PawnGenerationRequest request, ref Pawn __result)
        {
            if (Configurations.EnableRecordRandomizer && __result != null && !request.Newborn && xxx.is_human(__result))
            {
                int avgsex = -500;
                bool isvirgin = Rand.Chance(Configurations.VirginRatio);
                int totalsex = 0;
                int totalbirth = 0;
                int deviation = (int)Configurations.MaxSexCountDeviation;
                if (__result.story != null)
                {
                    float lust;
                    if (xxx.is_nympho(__result)) lust = __result.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, 0);
                    else lust = __result.RecordRandomizer(VariousDefOf.Lust, Configurations.AvgLust, Configurations.MaxLustDeviation, float.MinValue);

                    int sexableage = 0;
                    int minsexage = (int)(__result.RaceProps.lifeExpectancy * Configurations.MinSexablePercent);
                    if (__result.ageTracker.AgeBiologicalYears > minsexage)
                    {
                        sexableage = __result.ageTracker.AgeBiologicalYears - minsexage;
                        avgsex = (int)(sexableage * Configurations.SexPerYear * __result.LustFactor());
                    }


                    if (__result.relations != null && __result.gender == Gender.Female)
                    {
                        totalbirth += __result.relations.ChildrenCount;
                        totalsex += totalbirth;
                        __result.records?.AddTo(xxx.CountOfSexWithHumanlikes, totalbirth);
                        __result.records?.SetTo(xxx.CountOfBirthHuman, totalbirth);
                        if (totalbirth > 0) isvirgin = false;
                    }
                    if (!isvirgin)
                    {
                        if (xxx.is_rapist(__result))
                        {
                            if (xxx.is_zoophile(__result))
                            {
                                if (__result.Has(Quirk.ChitinLover)) totalsex += __result.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, deviation);
                                else totalsex += __result.RecordRandomizer(xxx.CountOfRapedAnimals, avgsex, deviation);
                            }
                            else totalsex += __result.RecordRandomizer(xxx.CountOfRapedHumanlikes, avgsex, deviation);
                            avgsex /= 8;
                        }

                        if (xxx.is_zoophile(__result))
                        {
                            if (__result.Has(Quirk.ChitinLover)) totalsex += __result.RecordRandomizer(xxx.CountOfRapedInsects, avgsex, deviation);
                            else totalsex += __result.RecordRandomizer(xxx.CountOfSexWithAnimals, avgsex, deviation);
                            avgsex /= 10;
                        }
                        else if (xxx.is_necrophiliac(__result))
                        {
                            totalsex += __result.RecordRandomizer(xxx.CountOfSexWithCorpse, avgsex, deviation);
                            avgsex /= 4;
                        }

                        if (__result.IsSlave)
                        {
                            totalsex += __result.RecordRandomizer(xxx.CountOfBeenRapedByAnimals, Rand.Range(-50, 10), Rand.Range(0, 10) * sexableage);
                            totalsex += __result.RecordRandomizer(xxx.CountOfBeenRapedByHumanlikes, 0, Rand.Range(0, 100) * sexableage);
                        }


                        totalsex += __result.RecordRandomizer(xxx.CountOfSexWithHumanlikes, avgsex, deviation);
                        
                        if (totalsex > 0) __result.records.AddTo(VariousDefOf.SexPartnerCount, Math.Max(1, Rand.Range(0, totalsex/7)));
                    }
                }
                __result.records?.SetTo(xxx.CountOfSex, totalsex);
                RJWUtility.GenerateSextypeRecords(__result, totalsex);
            }
            if (__result.story?.traits != null)
            {
                if (__result.IsVirgin())
                {
                    int degree = 0;
                    if (__result.gender == Gender.Female) degree = 2;
                    Trait virgin = new Trait(VariousDefOf.Virgin, degree ,true);
                    __result.story.traits.GainTrait(virgin);
                }
                else if (__result.gender == Gender.Female && Rand.Chance(0.05f))
                {
                    Trait virgin = new Trait(VariousDefOf.Virgin, 1, true);
                    __result.story.traits.GainTrait(virgin);
                }
            }
        }
    }



    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    public class HumanlikeOrder_Patch
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            var targets = GenUI.TargetsAt(clickPos, TargetingParameters.ForBuilding());

            if (pawn.health.hediffSet.HasHediff(RJW_SemenoOverlayHediffDefOf.Hediff_Bukkake))
            foreach (LocalTargetInfo t in targets)
            {
                Building building = t.Thing as Building;
                if (building != null)
                {
                    if (building is Building_CumBucket)
                    {
                        opts.AddDistinct(MakeMenu(pawn, building));
                        break;
                    }
                }
            }
        }
        
        public static FloatMenuOption MakeMenu(Pawn pawn, LocalTargetInfo target)
        {
            FloatMenuOption option = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(Keyed.RS_FloatMenu_CleanSelf, delegate ()
            {
                pawn.jobs.TryTakeOrderedJob(new Verse.AI.Job(VariousDefOf.CleanSelfwithBucket, null, target, target.Cell));
            }, MenuOptionPriority.Low), pawn, target);

            return option;
        }
    }

}
