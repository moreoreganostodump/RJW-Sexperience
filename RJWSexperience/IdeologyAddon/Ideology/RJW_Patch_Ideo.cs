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
using SexperienceDefOf = RJWSexperience.VariousDefOf;

namespace RJWSexperience.Ideology
{
    public static class RJWUtility_Ideo
    {

        public static HistoryEvent TaggedEvent(this HistoryEventDef def, Pawn pawn, string tag, Pawn partner)
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

    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_Bestiality), "MtbHours")]
    public static class RJW_Patch_ChancePerHour_Bestiality
    {
        public static void Postfix(Pawn pawn, ref float __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null) __result *= BestialityByPrecept(ideo); // ideo is null if don't have dlc
        }

        public static float BestialityByPrecept(Ideo ideo)
        {
            if (ideo.HasPrecept(VariousDefOf.Bestiality_Honorable)) return 0.5f;
            else if (ideo.HasPrecept(VariousDefOf.Bestiality_OnlyVenerated)) return 0.65f;
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
            if (ideo.HasPrecept(VariousDefOf.Rape_Honorable)) return 0.5f;
            else if (ideo.HasPrecept(VariousDefOf.Rape_Acceptable)) return 0.75f;
            else if (ideo.HasPrecept(VariousDefOf.Rape_Disapproved)) return 1.0f;
            else return 3f;
        }
    }

    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_Necro), "MtbHours")]
    public static class RJW_Patch_ThinkNode_ChancePerHour_Necro
    {
        public static void Postfix(Pawn pawn, ref float __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null) __result *= NecroByPrecept(ideo); // ideo is null if don't have dlc
        }

        public static float NecroByPrecept(Ideo ideo)
        {
            if (ideo.HasPrecept(VariousDefOf.Necrophilia_Approved)) return 0.5f;
            else if (ideo.HasPrecept(VariousDefOf.Necrophilia_Acceptable)) return 0.75f;
            else if (ideo.HasPrecept(VariousDefOf.Necrophilia_Disapproved)) return 1.0f;
            else return 8f;
        }
    }

    [HarmonyPatch(typeof(xxx), "is_rapist")]
    public static class RJW_Patch_is_rapist
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null && !pawn.IsSubmissive())
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

    [HarmonyPatch(typeof(xxx), "is_necrophiliac")]
    public static class RJW_Patch_is_necrophiliac
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            Ideo ideo = pawn.Ideo;
            if (ideo != null)
            {
                __result = __result || ideo.HasMeme(VariousDefOf.Necrophile);
            }
        }

    }


    [HarmonyPatch(typeof(SexUtility), "Aftersex", new Type[] { typeof(SexProps) })]
    public static class RJW_Patch_Aftersex
    {


        public static void Postfix(SexProps props)
        {
            Pawn pawn = props.pawn;
            Pawn partner = props.partner;
            bool usedCondom = props.usedCondom;
            bool rape = props.isRape;
            bool isCoreLovin = props.isCoreLovin;
            xxx.rjwSextype sextype = props.sexType;

            //Log.Message("Aftersex " + pawn.Label + ": " + sextype);
            if (partner != null)
            {
                if (xxx.is_human(pawn)) AfterSexHuman(pawn, partner, usedCondom, rape, isCoreLovin, sextype);
                else if (xxx.is_human(partner)) AfterSexHuman(partner, pawn, usedCondom, false, isCoreLovin, sextype, true);
            }

        }


        public static void AfterSexHuman(Pawn human, Pawn partner, bool usedCondom, bool rape, bool isCoreLovin, xxx.rjwSextype sextype, bool isHumanReceiving = false)
        {
            string tag = "";
            if (human.IsIncest(partner))
            {
                tag += HETag.Incestous;
            }

            if (partner.Dead)
            {
                Find.HistoryEventsManager.RecordEvent(VariousDefOf.SexWithCorpse.TaggedEvent(human, tag + HETag.Gender(human), partner));
            }
            else if (partner.IsAnimal())
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
                        Find.HistoryEventsManager.RecordEvent(VariousDefOf.RapedSlave.TaggedEvent(human, tag + HETag.Rape + HETag.Gender(human), partner));
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
                        //if (sexevent == VariousDefOf.PromiscuousSex)
                        //{
                        //    human.records.AddTo(SexperienceDefOf.Lust, 1.0f* RJWUtility.LustIncrementFactor(human.records.GetValue(SexperienceDefOf.Lust)));
                        //    partner.records.AddTo(SexperienceDefOf.Lust, 1.0f * RJWUtility.LustIncrementFactor(partner.records.GetValue(SexperienceDefOf.Lust)));
                        //}

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
                case xxx.rjwSextype.DoublePenetration:
                case xxx.rjwSextype.Scissoring:
                case xxx.rjwSextype.Fisting:
                case xxx.rjwSextype.Sixtynine:
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

        public static void PreceptSextype(Ideo ideo, float sexdrive, List<float> result, int offset)
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


    [HarmonyPatch(typeof(SexAppraiser), "would_fuck", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(bool) })]
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

    [HarmonyPatch(typeof(PawnDesignations_Comfort), "UpdateCanDesignateComfort")]
    public static class RJW_PatchUpdateCanDesignateComfort
    {
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.IsSubmissive())
            {
                SaveStorage.DataStore.GetPawnData(pawn).CanDesignateComfort = true;
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Hediff_BasePregnancy), "PostBirth")]
    public static class RJW_Patch_PostBirth
    {
        public static void Postfix(Pawn mother, Pawn father, Pawn baby)
        {
            if (!mother.IsAnimal())
            {
                //baby.SetFactionDirect(baby.GetFactionUsingPrecept());
                baby.SetFaction(baby.GetFactionUsingPrecept(out Ideo ideo));
                baby.ideo?.SetIdeo(ideo);
                if (baby.Faction == Find.FactionManager.OfPlayer && !baby.IsSlave) baby.guest?.SetGuestStatus(null, GuestStatus.Guest);
            }
        }
    }

}
