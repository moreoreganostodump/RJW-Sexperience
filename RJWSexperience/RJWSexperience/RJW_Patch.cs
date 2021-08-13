using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using rjw;
using RimWorld;
using Verse;
using UnityEngine;


namespace RJWSexperience
{
    public static class RJWUtility
    {
        public static float GetSexStat(this Pawn pawn)
        {
            if (xxx.is_human(pawn) && !pawn.Dead)
            {
                return pawn.GetStatValue(xxx.sex_stat);
            }
            else return 1.0f;
        }

        public static HistoryEvent TaggedEvent(this HistoryEventDef def ,Pawn pawn, string tag, Pawn partner)
        {
            return new HistoryEvent(def, pawn.Named(HistoryEventArgsNames.Doer), tag.Named(HistoryEventArgsNamesCustom.Tag), partner.Named(HistoryEventArgsNamesCustom.Partner));
        }

        public static Faction GetFactionUsingPrecept(this Pawn baby, out Ideo ideo)
        {
            Faction playerfaction = Find.FactionManager.OfPlayer;
            Ideo mainideo = playerfaction.ideos.PrimaryIdeo;
            if (mainideo != null)
            {
                if (mainideo.HasPrecept(VariousDefOf.BabyFaction_AlwaysFather))
                {
                    Pawn parent = baby.GetFather();
                    if (parent == null) baby.GetMother();

                    ideo = parent.Ideo;
                    return parent.Faction;
                }
                else if (mainideo.HasPrecept(VariousDefOf.BabyFaction_AlwaysColony))
                {
                    ideo = mainideo;
                    return playerfaction;
                }
            }
            Pawn mother = baby.GetMother();
            ideo = mother?.Ideo;
            return mother?.Faction ?? baby.Faction;
        }


    }



    [HarmonyPatch(typeof(JobDriver_Sex), "Orgasm")]
    public static class RJW_Patch_Orgasm
    {
        public static void Postfix(JobDriver_Sex __instance)
        {
            if (__instance.sexType != xxx.rjwSextype.Masturbation && !(__instance is JobDriver_Masturbate))
            {
                if (__instance.isRape)
                {
                    __instance.pawn?.skills?.Learn(VariousDefOf.SexSkill, 0.05f, true);
                }
                else
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

        public static void Prefix(Pawn pawn, Pawn partner, xxx.rjwSextype sextype, bool violent, bool pawn_is_raping, ref float satisfaction)
        {
            satisfaction *= partner.GetSexStat();
        }

        public static void Postfix(Pawn pawn, Pawn partner, xxx.rjwSextype sextype, bool violent, bool pawn_is_raping, float satisfaction)
        {
            float? lust = pawn.records?.GetValue(VariousDefOf.Lust);
            if (lust != null)
            {
                if (sextype != xxx.rjwSextype.Masturbation || partner != null) pawn.records.AddTo(VariousDefOf.Lust, Mathf.Clamp((satisfaction - base_sat_per_fuck) * LustIncrementFactor(lust ?? 0), -0.5f, 0.5f)); // If the sex is satisfactory, lust grows up. Declines at the opposite.
                else pawn.records.AddTo(VariousDefOf.Lust, Mathf.Clamp(satisfaction * satisfaction * LustIncrementFactor(lust ?? 0), 0, 0.5f));                                             // Masturbation always increases lust.
            }

            if (sextype == xxx.rjwSextype.Masturbation || partner == null)
            {
                Building_CumBucket cumbucket = (Building_CumBucket)pawn.GetAdjacentBuilding<Building_CumBucket>();
                if (cumbucket != null)
                {
                    cumbucket.AddCum(pawn.GetCumVolume());
                }
            }
        }

        public static float LustIncrementFactor(float lust)
        {
            return Mathf.Exp(-Mathf.Pow(lust / Configurations.LustLimit, 2));
        }



    }

    [HarmonyPatch(typeof(xxx), "TransferNutrition")]
    public static class RJW_Patch_TransferNutrition
    {
        public static void Postfix(Pawn pawn, Pawn partner, xxx.rjwSextype sextype)
        {
            Pawn receiver = null;
            if (Genital_Helper.has_penis_fertile(pawn)) receiver = partner;
            else if (Genital_Helper.has_penis_fertile(partner)) receiver = pawn;
    
            if (receiver != null && (
                sextype == xxx.rjwSextype.Oral ||
                sextype == xxx.rjwSextype.Fellatio ||
                sextype == xxx.rjwSextype.Sixtynine))
            {
                receiver.CumDrugEffect();
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



}
