using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using RimWorld;
using Verse;


namespace RJWSexperience
{
    public class StatPart_Lust : StatPart
    {
        public float factor;

        public override string ExplanationPart(StatRequest req)
        {
            Pawn pawn = req.Thing as Pawn;
            return Keyed.LustStatFactor(LustFactor(pawn) * factor * 100);

        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn != null) val *= LustFactor(pawn) * factor;
        }

        protected float LustFactor(Pawn pawn)
        {
            return 1 + pawn.records.GetValue(VariousDefOf.Lust) / 100;
        }

    }

    public class StatPart_GenderPrimacy : StatPart
    {
        public float modifier;

        public override string ExplanationPart(StatRequest req)
        {
            Pawn pawn = req.Thing as Pawn;
            Ideo ideo = null;
            if (pawn != null) ideo = pawn.Ideo;
            float fact = 1f;
            if (ideo != null && !ideo.memes.NullOrEmpty()) for (int i = 0; i < ideo.memes.Count; i++)
                {
                    if (ideo.memes[i] == MemeDefOf.MaleSupremacy)
                    {
                        if (pawn.gender == Gender.Male) fact = modifier;
                        else if (pawn.gender == Gender.Female) fact = 1/modifier;
                        break;
                    }
                    else if (ideo.memes[i] == MemeDefOf.FemaleSupremacy)
                    {
                        if (pawn.gender == Gender.Male) fact = 1/modifier;
                        else if (pawn.gender == Gender.Female) fact = modifier;
                        break;
                    }
                }
            return Keyed.MemeStatFactor(fact * 100);
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            Pawn pawn = req.Thing as Pawn;
            Ideo ideo = null;
            if (pawn != null) ideo = pawn.Ideo;
            if (ideo != null && !ideo.memes.NullOrEmpty()) for(int i=0; i< ideo.memes.Count; i++)
                {
                    if (ideo.memes[i] == MemeDefOf.MaleSupremacy)
                    {
                        if (pawn.gender == Gender.Male) val *= modifier;
                        else if (pawn.gender == Gender.Female) val /= modifier;
                        break;
                    }
                    else if(ideo.memes[i] == MemeDefOf.FemaleSupremacy)
                    {
                        if (pawn.gender == Gender.Male) val /= modifier;
                        else if (pawn.gender == Gender.Female) val *= modifier;
                        break;
                    }
                }
        }
    }

    public class StatPart_Slave : StatPart
    {
        public float factor;
        public override string ExplanationPart(StatRequest req)
        {
            float fact = factor * 100;
            Pawn pawn = req.Thing as Pawn;
            if (pawn != null)
            {
                if (pawn.IsSlave)
                {
                    return Keyed.SlaveStatFactor(fact);
                }
            }
            return Keyed.SlaveStatFactorDefault;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            Pawn pawn = req.Thing as Pawn;
            if (pawn != null)
            {
                if (pawn.IsSlave)
                {
                    val *= factor;
                }
            }

        }
    }


}
