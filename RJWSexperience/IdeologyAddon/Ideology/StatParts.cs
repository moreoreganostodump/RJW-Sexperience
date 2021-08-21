using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using RimWorld;
using Verse;


namespace RJWSexperience.Ideology
{
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
            return Keyed.MemeStatFactor(String.Format("{0:0.##}", fact * 100));
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


}
