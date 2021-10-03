using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using rjw;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;


namespace RJWSexperience
{



    [HarmonyPatch(typeof(JobDriver_Sex), "Orgasm")]
    public static class RJW_Patch_Orgasm
    {
        public static void Postfix(JobDriver_Sex __instance)
        {
            if (__instance.Sexprops.sexType != xxx.rjwSextype.Masturbation && !(__instance is JobDriver_Masturbate))
            {
                if (__instance.Sexprops.isRape)
                {
                    __instance.pawn?.skills?.Learn(VariousDefOf.SexSkill, 0.05f, true);
                }
                {   
                    __instance.pawn?.skills?.Learn(VariousDefOf.SexSkill, 0.35f, true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(WhoringHelper), "WhoreAbilityAdjustmentMin")]
    public static class RJW_Patch_WhoreAbilityAdjustmentMin
    {
        public static void Postfix(Pawn whore, ref float __result)
        {
            __result *= whore.GetSexStat();
        }
    }

    [HarmonyPatch(typeof(WhoringHelper), "WhoreAbilityAdjustmentMax")]
    public static class RJW_Patch_WhoreAbilityAdjustmentMax
    {
        public static void Postfix(Pawn whore, ref float __result)
        {
            __result *= whore.GetSexStat();
        }
    }

    [HarmonyPatch(typeof(SexUtility), "SatisfyPersonal")]
    public static class RJW_Patch_SatisfyPersonal
    {
        private const float base_sat_per_fuck = 0.4f;

        public static void Prefix(SexProps props, ref float satisfaction)
        {
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;
            satisfaction = Mathf.Max(base_sat_per_fuck, satisfaction * partner.GetSexStat());
        }

        public static void Postfix(SexProps props, ref float satisfaction)
        {
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;
            float? lust = pawn.records?.GetValue(VariousDefOf.Lust);
            xxx.rjwSextype sextype = props.sexType;
            if (lust != null)
            {
                if (sextype != xxx.rjwSextype.Masturbation || partner != null) pawn.records.AddTo(VariousDefOf.Lust, Mathf.Clamp((satisfaction - base_sat_per_fuck) * RJWUtility.LustIncrementFactor(lust ?? 0), -0.5f, 0.5f)); // If the sex is satisfactory, lust grows up. Declines at the opposite.
                else pawn.records.AddTo(VariousDefOf.Lust, Mathf.Clamp(satisfaction * satisfaction * RJWUtility.LustIncrementFactor(lust ?? 0), 0, 0.5f));                                             // Masturbation always increases lust.
            }

            if (sextype == xxx.rjwSextype.Masturbation || partner == null)
            {
                Building_CumBucket cumbucket = (Building_CumBucket)pawn.GetAdjacentBuilding<Building_CumBucket>();
                if (cumbucket != null)
                {
                    cumbucket.AddCum(pawn.GetCumVolume());
                }
            }

            RJWUtility.UpdateSatisfactionHIstory(pawn, partner, props, satisfaction);
            pawn.records?.Increment(VariousDefOf.OrgasmCount);

        }




    }

    [HarmonyPatch(typeof(SexUtility), "TransferNutrition")]
    public static class RJW_Patch_TransferNutrition
    {
        public static void Postfix(SexProps props)
        {
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;
            xxx.rjwSextype sextype = props.sexType;
            Pawn giver = null;
            Pawn receiver = null;

            if (Genital_Helper.has_penis_fertile(pawn))
            {
                giver = pawn;
                receiver = partner;
            }
            else if (Genital_Helper.has_penis_fertile(partner))
            {
                giver = partner;
                receiver = pawn;
            }
    
            if (receiver != null && (
                sextype == xxx.rjwSextype.Oral ||
                sextype == xxx.rjwSextype.Fellatio ||
                sextype == xxx.rjwSextype.Sixtynine))
            {
                receiver.AteCum(giver.GetCumVolume(), true);
            }
        }
    }

    [HarmonyPatch(typeof(Nymph_Generator), "set_skills")]
    public static class RJW_Patch_Nymph_set_skills
    {
        public static void Postfix(Pawn pawn)
        {
            SkillRecord sexskill = pawn.skills.GetSkill(VariousDefOf.SexSkill);
            if (sexskill != null)
            {
                sexskill.passion = Passion.Major;
                sexskill.Level = (int)Utility.RandGaussianLike(7f, 20.99f);
                sexskill.xpSinceLastLevel = sexskill.XpRequiredForLevelUp * Rand.Range(0.10f, 0.90f);
            }
        }
    }

    [HarmonyPatch(typeof(AfterSexUtility), "UpdateRecords")]
    public static class RJW_Patch_UpdateRecords
    {
        public static void Postfix(SexProps props)
        {
            RJWUtility.UpdateSextypeRecords(props);
            RJWUtility.UpdatePartnerHistory(props.pawn, props.partner, props);
            RJWUtility.UpdatePartnerHistory(props.partner, props.pawn, props);
        }

    }

    [HarmonyPatch(typeof(JobDriver_SexBaseInitiator), "Start")]
    public static class RJW_Patch_LogSextype
    {
        public static void Postfix(JobDriver_SexBaseInitiator __instance)
        {
            if (__instance.Partner != null)
            {
                __instance.pawn.PoptheCherry(__instance.Partner, __instance.Sexprops);
                __instance.Partner.PoptheCherry(__instance.pawn, __instance.Sexprops);
            }
        }
    }


    [HarmonyPatch(typeof(WorkGiver_CleanSelf), "JobOnThing")]
    public static class RJW_Patch_CleanSelf_JobOnThing
    {
        public static bool Prefix(Pawn pawn, Thing t, bool forced, ref Job __result)
        {
            Building_CumBucket bucket = pawn.GetAdjacentBuilding<Building_CumBucket>();
            if (bucket == null) bucket = pawn.FindClosestBucket();
            if (bucket != null)
            {
                __result = JobMaker.MakeJob(VariousDefOf.CleanSelfwithBucket, null, bucket, bucket.Position);
                return false;
            }


            return true;
        }
    }

    [HarmonyPatch(typeof(JobGiver_Masturbate), "TryGiveJob")]
    public static class RJW_Patch_Masturabte_TryGiveJob
    {
        public static void Postfix(Pawn pawn, ref Job __result)
        {
            if (RJWPreferenceSettings.FapEverywhere && (pawn.Faction?.IsPlayer ?? false) && __result != null)
            {
                Building_CumBucket bucket = pawn.FindClosestBucket();
                if (bucket != null)
                {
                    __result.Clear();
                    __result = JobMaker.MakeJob(xxx.Masturbate, null, null, bucket.RandomAdjacentCell8Way());
                }
            }

        }
    }

}
