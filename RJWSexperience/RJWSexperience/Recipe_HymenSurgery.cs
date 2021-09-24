using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;

namespace RJWSexperience
{
    public class Recipe_HymenSurgery : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {

            if (pawn.gender != Gender.Female)
            {
                yield break;
            }

            BodyPartRecord part = Genital_Helper.get_genitalsBPR(pawn);
            if (part != null)
            {
                List<Hediff> hediffs = Genital_Helper.get_PartsHediffList(pawn, part);
                if (Genital_Helper.has_vagina(pawn, hediffs) && !pawn.HasHymen())
                {
                    yield return part;
                }

            }
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                SurgeryResult(pawn);
            }
        }

        protected void SurgeryResult(Pawn pawn)
        {
            int degree = 1;
            if (pawn.IsVirgin()) degree = 2;
            Trait virgin = new Trait(VariousDefOf.Virgin, degree, true);
            pawn.story.traits.GainTrait(virgin);
        }
    }
}
