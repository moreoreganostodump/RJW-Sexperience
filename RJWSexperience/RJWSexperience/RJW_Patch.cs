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

        public static float LustIncrementFactor(float lust)
        {
            return Mathf.Exp(-Mathf.Pow(lust / Configurations.LustLimit, 2));
        }

        /// <summary>
        /// If the pawn is virgin, return true.
        /// </summary>
        public static bool IsVirgin(this Pawn pawn)
        {
            if (pawn.records.GetValue(xxx.CountOfSex) == 0) return true;
            return false;
        }

        /// <summary>
        /// If pawn is virgin, lose his/her virginity.
        /// </summary>
        public static void PoptheCherry(this Pawn pawn, Pawn partner, bool violent)
        {
            if (pawn.IsVirgin())
            {
                Messages.Message(Keyed.RS_LostVirgin(pawn.LabelShort, partner.LabelShort), MessageTypeDefOf.NeutralEvent, true);
                RemoveVirginTrait(pawn);
                FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Blood, 1, FilthSourceFlags.Pawn);
            }
        }
        
        public static void RemoveVirginTrait(Pawn pawn)
        {
            Trait virgin = pawn.story?.traits?.GetTrait(VariousDefOf.Virgin);
            if (virgin != null)
            {
                pawn.story.traits.RemoveTrait(virgin);
            }
        }

        public static void UpdateSextypeRecords(SexProps props)
        {
            xxx.rjwSextype sextype = props.sexType;
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;
            Pawn receiver = props.reciever;
            Pawn giver = props.giver;

            if (partner != null)
            {

                switch (sextype)
                {
                    case xxx.rjwSextype.Vaginal:
                    case xxx.rjwSextype.Scissoring:
                        IncreaseSameRecords(pawn, partner, VariousDefOf.VaginalSexCount);
                        break;
                    case xxx.rjwSextype.Anal:
                        IncreaseSameRecords(pawn, partner, VariousDefOf.AnalSexCount);
                        break;
                    case xxx.rjwSextype.Oral:
                    case xxx.rjwSextype.Fellatio:
                        if (Genital_Helper.has_penis_fertile(giver) || Genital_Helper.has_penis_infertile(giver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.OralSexCount, VariousDefOf.BlowjobCount);
                        }
                        else if (Genital_Helper.has_penis_infertile(receiver) || Genital_Helper.has_penis_infertile(receiver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.BlowjobCount, VariousDefOf.OralSexCount);
                        }
                        break;
                    case xxx.rjwSextype.Sixtynine:
                        IncreaseSameRecords(pawn, partner, VariousDefOf.OralSexCount);
                        RecordDef recordpawn, recordpartner;
                        if (Genital_Helper.has_penis_fertile(pawn) || Genital_Helper.has_penis_infertile(pawn))
                        {
                            recordpartner = VariousDefOf.BlowjobCount;
                        }
                        else
                        {
                            recordpartner = VariousDefOf.CunnilingusCount;
                        }

                        if (Genital_Helper.has_penis_fertile(partner) || Genital_Helper.has_penis_infertile(partner))
                        {
                            recordpawn = VariousDefOf.BlowjobCount;
                        }
                        else
                        {
                            recordpawn = VariousDefOf.CunnilingusCount;
                        }
                        IncreaseRecords(pawn, partner, recordpawn, recordpartner);
                        break;
                    case xxx.rjwSextype.Cunnilingus:
                        if (Genital_Helper.has_vagina(giver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.OralSexCount, VariousDefOf.CunnilingusCount);
                        }
                        else if (Genital_Helper.has_vagina(receiver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.CunnilingusCount, VariousDefOf.OralSexCount);
                        }
                        break;
                    case xxx.rjwSextype.Masturbation:
                        break;
                    case xxx.rjwSextype.Handjob:
                        if (Genital_Helper.has_penis_fertile(giver) || Genital_Helper.has_penis_infertile(giver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.GenitalCaressCount, VariousDefOf.HadnjobCount);
                        }
                        else
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.HadnjobCount, VariousDefOf.GenitalCaressCount);
                        }
                        break;
                    case xxx.rjwSextype.Fingering:
                    case xxx.rjwSextype.Fisting:
                        if (Genital_Helper.has_vagina(giver))
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.GenitalCaressCount, VariousDefOf.FingeringCount);
                        }
                        else
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.FingeringCount, VariousDefOf.GenitalCaressCount);
                        }
                        break;
                    case xxx.rjwSextype.Footjob:
                        IncreaseSameRecords(pawn, partner, VariousDefOf.FootjobCount);
                        break;
                    default:
                        IncreaseSameRecords(pawn, partner, VariousDefOf.MiscSexualBehaviorCount);
                        break;
                }
            }
        }

        public static void UpdatePartnerHistory(Pawn pawn, Pawn partner, SexProps props, float satisfaction)
        {
            SexPartnerHistory pawnshistory = pawn.GetComp<SexPartnerHistory>();
            pawnshistory.RecordHistory(partner, props);
        }

        public static void IncreaseSameRecords(Pawn pawn, Pawn partner, RecordDef record)
        {
            pawn.records?.AddTo(record, 1);
            partner.records?.AddTo(record, 1);
        }

        public static void IncreaseRecords(Pawn pawn, Pawn partner, RecordDef recordforpawn, RecordDef recordforpartner)
        {
            pawn.records?.AddTo(recordforpawn, 1);
            partner.records?.AddTo(recordforpartner, 1);
        }

    }



    [HarmonyPatch(typeof(JobDriver_Sex), "Orgasm")]
    public static class RJW_Patch_Orgasm
    {
        public static void Postfix(JobDriver_Sex __instance)
        {
            if (__instance.Sexprops.sexType != xxx.rjwSextype.Masturbation && !(__instance is JobDriver_Masturbate))
            {
                if (__instance.isRape)
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
            Pawn partner = props.partner;
            satisfaction = Mathf.Max(base_sat_per_fuck, satisfaction * partner.GetSexStat());
        }

        public static void Postfix(SexProps props, float satisfaction)
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

    [HarmonyPatch(typeof(AfterSexUtility), "UpdateRecords")]
    public static class RJW_Patch_UpdateRecords
    {
        public static void Postfix(SexProps props)
        {
            RJWUtility.UpdateSextypeRecords(props);
        }

    }



}
