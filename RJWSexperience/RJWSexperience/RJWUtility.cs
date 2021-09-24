using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using RimWorld;
using Verse;
using Verse.AI;
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
            if (pawn.records.GetValue(VariousDefOf.VaginalSexCount) == 0) return true;
            return false;
        }
        public static bool HasHymen(this Pawn pawn)
        {
            Trait virgin = pawn.story?.traits?.GetTrait(VariousDefOf.Virgin);
            if (virgin != null)
            {
                if (virgin.Degree > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// If pawn is virgin, lose his/her virginity.
        /// </summary>
        public static void PoptheCherry(this Pawn pawn, Pawn partner, SexProps props)
        {
            if (props != null && props.sexType == xxx.rjwSextype.Vaginal)
            {
                if (pawn.IsVirgin())
                {
                    SexPartnerHistory history = pawn.GetPartnerHistory();
                    if (history != null)
                    {
                        history.RecordFirst(partner, props);
                    }
                    if (RemoveVirginTrait(pawn, partner, props))
                    {
                        Messages.Message(Keyed.RS_LostVirgin(pawn.LabelShort, partner.LabelShort), MessageTypeDefOf.NeutralEvent, true);
                    }
                }
                else
                {
                    RemoveVirginTrait(pawn, partner, props);
                }
            }
        }

        public static bool RemoveVirginTrait(Pawn pawn, Pawn partner, SexProps props)
        {
            int degree;
            Trait virgin = pawn.story?.traits?.GetTrait(VariousDefOf.Virgin);
            if (virgin != null)
            {
                degree = virgin.Degree;
                if (pawn.gender == Gender.Female && degree > 0)
                {
                    FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Blood, pawn.LabelShort, 1, FilthSourceFlags.Pawn);
                }
                ThrowVirginHIstoryEvent(pawn, partner, props, degree);
                pawn.story.traits.RemoveTrait(virgin);
                return true;
            }
            return false;
        }


        /// <summary>
        /// For ideo patch
        /// </summary>
        public static void ThrowVirginHIstoryEvent(Pawn pawn, Pawn partner, SexProps props, int degree)
        {
            //for non-ideo
            if (partner.Ideo == null)
            {
                partner.needs?.mood?.thoughts?.memories.TryGainMemory(xxx.took_virginity, pawn);
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
                            IncreaseRecords(giver, receiver, VariousDefOf.GenitalCaressCount, VariousDefOf.HandjobCount);
                        }
                        else
                        {
                            IncreaseRecords(giver, receiver, VariousDefOf.HandjobCount, VariousDefOf.GenitalCaressCount);
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

        public static void UpdatePartnerHistory(Pawn pawn, Pawn partner, SexProps props)
        {
            if (partner != null)
            {
                SexPartnerHistory pawnshistory = pawn.TryGetComp<SexPartnerHistory>();
                pawnshistory?.RecordHistory(partner, props);
            }
        }

        public static void UpdateSatisfactionHIstory(Pawn pawn, Pawn partner, SexProps props, float satisfaction)
        {
            if (partner != null)
            {
                SexPartnerHistory pawnshistory = pawn.TryGetComp<SexPartnerHistory>();
                pawnshistory?.RecordSatisfactionHistory(partner, props, satisfaction);
            }
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

        public static void GenerateSextypeRecords(Pawn pawn, int totalsex)
        {
            float totalweight =
                RJWPreferenceSettings.vaginal +
                RJWPreferenceSettings.anal +
                RJWPreferenceSettings.fellatio +
                RJWPreferenceSettings.cunnilingus +
                RJWPreferenceSettings.rimming +
                RJWPreferenceSettings.double_penetration +
                RJWPreferenceSettings.breastjob +
                RJWPreferenceSettings.handjob +
                RJWPreferenceSettings.mutual_masturbation +
                RJWPreferenceSettings.fingering +
                RJWPreferenceSettings.footjob +
                RJWPreferenceSettings.scissoring +
                RJWPreferenceSettings.fisting +
                RJWPreferenceSettings.sixtynine;
            Gender prefer = pawn.PreferGender();
            int sex = (int)(totalsex * RJWPreferenceSettings.vaginal / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.VaginalSexCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.anal / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.AnalSexCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.fellatio / totalweight);
            totalsex -= sex;
            if (prefer == Gender.Male) pawn.records.AddTo(VariousDefOf.BlowjobCount, sex);
            else pawn.records.AddTo(VariousDefOf.OralSexCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.cunnilingus / totalweight);
            totalsex -= sex;
            if (prefer == Gender.Male) pawn.records.AddTo(VariousDefOf.OralSexCount, sex);
            else pawn.records.AddTo(VariousDefOf.CunnilingusCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.rimming / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.MiscSexualBehaviorCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.double_penetration / totalweight) / 2;
            totalsex -= sex;
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.VaginalSexCount, sex);
            pawn.records.AddTo(VariousDefOf.AnalSexCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.breastjob / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.MiscSexualBehaviorCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.handjob / totalweight);
            totalsex -= sex;
            if (prefer == Gender.Male) pawn.records.AddTo(VariousDefOf.HandjobCount, sex);
            else pawn.records.AddTo(VariousDefOf.GenitalCaressCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.fingering / totalweight);
            totalsex -= sex;
            if (prefer == Gender.Female) pawn.records.AddTo(VariousDefOf.FingeringCount, sex);
            else pawn.records.AddTo(VariousDefOf.GenitalCaressCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.mutual_masturbation / totalweight);
            totalsex -= sex;
            if (prefer == Gender.Male) pawn.records.AddTo(VariousDefOf.HandjobCount, sex);
            else pawn.records.AddTo(VariousDefOf.FingeringCount, sex);
            pawn.records.AddTo(VariousDefOf.GenitalCaressCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.footjob / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.FootjobCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.scissoring / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.MiscSexualBehaviorCount, sex);

            sex = (int)(totalsex * RJWPreferenceSettings.fisting / totalweight);
            totalsex -= sex;
            pawn.records.AddTo(VariousDefOf.MiscSexualBehaviorCount, sex);

            pawn.records.AddTo(VariousDefOf.OralSexCount, totalsex);
            if (prefer == Gender.Male) pawn.records.AddTo(VariousDefOf.BlowjobCount, totalsex);
            else pawn.records.AddTo(VariousDefOf.CunnilingusCount, totalsex);

        }

        public static Gender PreferGender(this Pawn pawn)
        {
            if (pawn.gender == Gender.Male)
            {
                if (xxx.is_homosexual(pawn)) return Gender.Male;
                else return Gender.Female;
            }
            else
            {
                if (xxx.is_homosexual(pawn)) return Gender.Female;
                else return Gender.Male;
            }
        }

        public static bool GetRapist(this SexProps props, out Pawn rapist)
        {
            if (!props.isRape)
            {
                rapist = null;
                return false;
            }

            rapist = props.pawn;
            return true;
        }

        public static bool IsBestiality(this SexProps props)
        {
            if (props.partner != null)
            {
                return props.pawn.IsAnimal() ^ props.partner.IsAnimal();
            }
            return false;
        }

        public static Building_CumBucket FindClosestBucket(this Pawn pawn)
        {
            List<Building> buckets = pawn.Map.listerBuildings.allBuildingsColonist.FindAll(x => x is Building_CumBucket);
            Dictionary<Building, float> targets = new Dictionary<Building, float>();
            if (!buckets.NullOrEmpty()) for (int i = 0; i < buckets.Count; i++)
                {
                    if (pawn.CanReach(buckets[i], PathEndMode.ClosestTouch, Danger.None))
                    {
                        targets.Add(buckets[i], pawn.Position.DistanceTo(buckets[i].Position));
                    }
                }
            if (!targets.NullOrEmpty())
            {
                return (Building_CumBucket)targets.MinBy(x => x.Value).Key;
            }
            return null;
        }

        public static void AteCum(this Pawn pawn, float amount, bool doDrugEffect = false)
        {
            pawn.records.AddTo(VariousDefOf.NumofEatenCum, 1);
            pawn.records.AddTo(VariousDefOf.AmountofEatenCum, amount);
            if (doDrugEffect) pawn.CumDrugEffect();
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

        public static void AddVirginTrait(this Pawn pawn)
        {
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


    }
}
