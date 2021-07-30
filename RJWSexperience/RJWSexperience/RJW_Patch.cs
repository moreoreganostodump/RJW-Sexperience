using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using rjw;
using RimWorld;
using Verse;


namespace RJWSexperience
{
    public static class RJWUtility
    {
        public static float GetSexStat(this Pawn pawn)
        {
            if (xxx.is_human(pawn))
            {
                return pawn.GetStatValue(xxx.sex_stat);
            }
            else return 1.0f;
        }

        public static HistoryEvent TaggedEvent(this HistoryEventDef def ,Pawn pawn, string tag, Pawn partner)
        {
            return new HistoryEvent(def, pawn.Named(HistoryEventArgsNames.Doer), tag.Named(HistoryEventArgsNamesCustom.Tag), partner.Named(HistoryEventArgsNamesCustom.Partner));
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
    public static class RJW_Patch_SatisfyPersonal_Pre
    {
        public static void Prefix(Pawn pawn, Pawn partner, xxx.rjwSextype sextype, bool violent, bool pawn_is_raping, ref float satisfaction)
        {
            satisfaction *= partner.GetSexStat();
        }
    }


    [HarmonyPatch(typeof(SexUtility), "SatisfyPersonal")]
    public static class RJW_Patch_SatisfyPersonal_Post
    {
        private const float base_sat_per_fuck = 0.3f;
        public static void Postfix(Pawn pawn, Pawn partner, xxx.rjwSextype sextype, bool violent, bool pawn_is_raping, float satisfaction)
        {
            float? lust = pawn.records?.GetValue(VariousDefOf.Lust);
            if (lust != null)
            {
                if (sextype != xxx.rjwSextype.Masturbation) pawn.records.AddTo(VariousDefOf.Lust, satisfaction - base_sat_per_fuck); // If the sex is satisfactory, lust grows up. Declines at the opposite.
                else pawn.records.AddTo(VariousDefOf.Lust, satisfaction * satisfaction);                                             // Masturbation always increases lust.
            }

        }
    }

    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_Bestiality), "MtbHours")]
    public static class RJW_Patch_ChancePerHour_Bestiality
    {
        public static void Postfix(Pawn pawn, ref float __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null) __result = BestialityByPrecept(ideo); // ideo is null if don't have dlc
        }

        public static float BestialityByPrecept(Ideo ideo)
        {
            if (ideo.HasPrecept(VariousDefOf.Bestiality_Honorable)) return 0.3f;
            else if (ideo.HasPrecept(VariousDefOf.Bestiality_OnlyVenerated)) return 0.6f;
            else if (ideo.HasPrecept(VariousDefOf.Bestiality_Acceptable)) return 0.75f;
            else if (ideo.HasPrecept(VariousDefOf.Bestiality_Disapproved)) return 1.0f;
            else return 5f;
        }
    }

    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_RapeCP), "MtbHours")]
    public static class RJW_Patch_ChancePerHour_RapeCP
    {
        public static void Postfix(Pawn pawn, ref float __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null) __result *= RapeByPrecept(ideo); // ideo is null if don't have dlc
        }

        public static float RapeByPrecept(Ideo ideo)
        {
            if (ideo.HasPrecept(VariousDefOf.Rape_Honorable)) return 0.25f;
            else if (ideo.HasPrecept(VariousDefOf.Rape_Acceptable)) return 0.5f;
            else if (ideo.HasPrecept(VariousDefOf.Rape_Disapproved)) return 1.0f;
            else return 3f;
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
                receiver.needs?.mood?.thoughts?.memories?.TryGainMemoryFast(VariousDefOf.AteCum); 
                
            }
        }
    }

    [HarmonyPatch(typeof(xxx), "is_rapist")]
    public static class RJW_Patch_is_rapist
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null)
            {
                __result = __result || ideo.HasMeme(VariousDefOf.Rapist);
            }
        }

    }

    [HarmonyPatch(typeof(xxx), "is_zoophile")]
    public static class RJW_Patch_is_zoophile
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null)
            {
                __result = __result || ideo.HasMeme(VariousDefOf.Zoophile);
            }
        }

    }


    [HarmonyPatch(typeof(SexUtility), "Aftersex", new Type[] {typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(bool), typeof(xxx.rjwSextype) })]
    public static class RJW_Patch_Aftersex
    {

        
        public static void Postfix(Pawn pawn, Pawn partner, bool usedCondom, bool rape, bool isCoreLovin, xxx.rjwSextype sextype)
        {
            //Log.Message("Aftersex " + pawn.Label + ": " + sextype);
            if (xxx.is_human(pawn)) AfterSexHuman(pawn, partner, usedCondom, rape, isCoreLovin, sextype);
            else if (xxx.is_human(partner)) AfterSexHuman(partner, pawn, usedCondom, false, isCoreLovin, sextype, true);

        }

        
        public static void AfterSexHuman(Pawn human, Pawn partner, bool usedCondom, bool rape, bool isCoreLovin, xxx.rjwSextype sextype, bool isHumanReceiving = false)
        {
            string tag = "";
            if (human.IsIncest(partner))
            {
                tag += HETag.Incestous;
            }

            if (partner.IsAnimal())
            {
                if (isHumanReceiving && rape)
                {
                    if (human.IsSlave) RapeEffectSlave(human);
                    if (human.Ideo?.IsVeneratedAnimal(partner) ?? false) Find.HistoryEventsManager.RecordEvent(VariousDefOf.SexWithVeneratedAnimal.TaggedEvent(human, tag + HETag.BeenRaped + HETag.Gender(human), partner));
                    else Find.HistoryEventsManager.RecordEvent(VariousDefOf.SexWithAnimal.TaggedEvent(human, tag + HETag.BeenRaped + HETag.Gender(human), partner));
                }
                else
                {
                    if (human.Ideo?.IsVeneratedAnimal(partner) ?? false) Find.HistoryEventsManager.RecordEvent(VariousDefOf.SexWithVeneratedAnimal.TaggedEvent(human, tag + HETag.Gender(human), partner));
                    else Find.HistoryEventsManager.RecordEvent(VariousDefOf.SexWithAnimal.TaggedEvent(human, tag + HETag.Gender(human), partner));
                }
            }
            else if (xxx.is_human(partner))
            {
                if (rape)
                {
                    if (partner.IsSlave)
                    {
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.RapedSlave.TaggedEvent(human ,tag + HETag.Rape + HETag.Gender(human), partner));
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.WasRapedSlave.TaggedEvent(partner, tag + HETag.BeenRaped + HETag.Gender(partner), human));
                        RapeEffectSlave(partner);
                    }
                    else if (partner.IsPrisoner)
                    {
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.RapedPrisoner.TaggedEvent(human, tag + HETag.Rape + HETag.Gender(human), partner));
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.WasRapedPrisoner.TaggedEvent(partner, tag + HETag.BeenRaped + HETag.Gender(partner), human));
                        partner.guest.will = Math.Max(0, partner.guest.will - 0.2f);
                    }
                    else
                    {
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.Raped.TaggedEvent(human, tag + HETag.Rape + HETag.Gender(human), partner));
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.WasRaped.TaggedEvent(partner, tag + HETag.BeenRaped + HETag.Gender(partner), human));
                    }
                }
                else
                {
                    HistoryEventDef sexevent = GetSexHistoryDef(sextype);
                    if (sexevent != null)
                    {
                        Find.HistoryEventsManager.RecordEvent(sexevent.TaggedEvent(human, tag + HETag.Gender(human), partner));
                        Find.HistoryEventsManager.RecordEvent(sexevent.TaggedEvent(partner, tag + HETag.Gender(partner), human));
                        if (sexevent == VariousDefOf.PromiscuousSex)
                        {
                            human.records.AddTo(VariousDefOf.Lust, 1.0f);
                            partner.records.AddTo(VariousDefOf.Lust, 1.0f);
                        }

                    }
                }
            }
        }

        public static void RapeEffectSlave(Pawn victim)
        {
            Need_Suppression suppression = victim.needs.TryGetNeed<Need_Suppression>();
            if (suppression != null)
            {
                Hediff broken = victim.health.hediffSet.GetFirstHediffOfDef(xxx.feelingBroken);
                if (broken != null) suppression.CurLevel += 0.3f * broken.Severity + 0.05f;
                else suppression.CurLevel += 0.05f;
            }
        }


        /// <summary>
        /// only for non-violate human sex
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="partner"></param>
        /// <param name="sextype"></param>
        /// <returns></returns>
        public static HistoryEventDef GetSexHistoryDef(xxx.rjwSextype sextype)
        {
            switch (sextype)
            {
                case xxx.rjwSextype.None:
                case xxx.rjwSextype.MechImplant:
                default:
                    return null;
                case xxx.rjwSextype.Vaginal:
                    return VariousDefOf.VaginalSex;
                case xxx.rjwSextype.Anal:
                case xxx.rjwSextype.Rimming:
                    return VariousDefOf.AnalSex;
                case xxx.rjwSextype.Oral:
                case xxx.rjwSextype.Fellatio:
                case xxx.rjwSextype.Cunnilingus:
                    return VariousDefOf.OralSex;

                case xxx.rjwSextype.Masturbation:
                case xxx.rjwSextype.Boobjob:
                case xxx.rjwSextype.Handjob:
                case xxx.rjwSextype.Footjob:
                case xxx.rjwSextype.Fingering:
                case xxx.rjwSextype.MutualMasturbation:
                    return VariousDefOf.MiscSex;
                case xxx.rjwSextype.DoublePenetration  :
                case xxx.rjwSextype.Scissoring         :
                case xxx.rjwSextype.Fisting            :
                case xxx.rjwSextype.Sixtynine          :
                    return VariousDefOf.PromiscuousSex;
            }

        }

    }


    /// <summary>
    /// Set prefer sextype using precepts
    /// </summary>
    [HarmonyPatch(typeof(SexUtility), "DetermineSexScores")]
    public static class RJW_Patch_DetermineSexScores
    {
        public static void Postfix(Pawn pawn, Pawn partner, bool rape, bool whoring, Pawn receiving, List<float> __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null) PreceptSextype(ideo, pawn.GetStatValue(xxx.sex_drive_stat), __result, 0);
            
            ideo = partner.Ideo;
            if (!rape && ideo != null) PreceptSextype(ideo, pawn.GetStatValue(xxx.sex_drive_stat), __result, 1);
            
        }

        public static void PreceptSextype(Ideo ideo, float sexdrive,  List<float> result, int offset)
        {
            float mult = 8.0f * Math.Max(0.3f, 1 / Math.Max(0.01f, sexdrive));
            if (ideo.HasPrecept(VariousDefOf.Sex_VaginalOnly))
            {
                result[0 + offset] *= mult;
            }
            else if (ideo.HasPrecept(VariousDefOf.Sex_AnalOnly))
            {
                result[2 + offset] *= mult;
                result[6 + offset] *= mult;
            }
            else if (ideo.HasPrecept(VariousDefOf.Sex_OralOnly))
            {
                result[4 + offset] *= mult;
                result[8 + offset] *= mult;
            }
            else if (ideo.HasPrecept(VariousDefOf.Sex_Promiscuous))
            {
                result[10 + offset] *= mult;
                result[20 + offset] *= mult;
                result[24 + offset] *= mult;
                result[26 + offset] *= mult;
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

    [HarmonyPatch(typeof(SexAppraiser), "would_fuck", new Type[] {typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(bool) })]
    public static class RJW_Patch_would_fuck
    {
        public static void Postfix(Pawn fucker, Pawn fucked, bool invert_opinion, bool ignore_bleeding, bool ignore_gender, ref float __result)
        {
            if (xxx.is_human(fucker))
            {
                Ideo ideo = fucker.Ideo;
                if (ideo != null)
                {
                    if (fucker.IsIncest(fucked))
                    {
                        if (ideo.HasPrecept(VariousDefOf.Incestuos_IncestOnly)) __result *= 2.0f;
                        else if (!fucker.relations?.DirectRelationExists(PawnRelationDefOf.Spouse, fucked) ?? false)
                        {
                            if (ideo.HasPrecept(VariousDefOf.Incestuos_Disapproved)) __result *= 0.5f;
                            else if (ideo.HasPrecept(VariousDefOf.Incestuos_Forbidden)) __result *= 0.1f;
                        }
                    }
                    if (fucked.IsAnimal())
                    {
                        if (ideo.HasPrecept(VariousDefOf.Bestiality_Honorable)) __result *= 2.0f;
                        else if (ideo.HasPrecept(VariousDefOf.Bestiality_OnlyVenerated))
                        {
                            if (ideo.IsVeneratedAnimal(fucked)) __result *= 2.0f;
                            else __result *= 0.05f;
                        }
                        else if (ideo.HasPrecept(VariousDefOf.Bestiality_Acceptable)) __result *= 1.0f;
                        else __result *= 0.5f;
                    }

                }
            }
        }

    }

    [HarmonyPatch(typeof(PawnDesignations_Breedee), "UpdateCanDesignateBreeding")]
    public static class RJW_Patch_UpdateCanDesignateBreeding
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null && ideo.HasMeme(VariousDefOf.Zoophile))
            {
                SaveStorage.DataStore.GetPawnData(pawn).CanDesignateBreeding = true;
                __result = true;
            }
        }
    }


    



}
