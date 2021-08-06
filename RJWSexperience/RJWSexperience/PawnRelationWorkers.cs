using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;


namespace RJWSexperience
{
    public class PawnRelationWorker_Bastard : PawnRelationWorker_Child
    {
        public override bool InRelation(Pawn me, Pawn other)
        {
            Pawn mother = other.GetMother();
            Pawn father = other.GetFather();

            if (me != other && (mother == me || father == me))
            {
                //if (!mother?.relations?.DirectRelationExists(PawnRelationDefOf.Spouse, father) ?? true) return true;
                //if (!mother?.relations?.DirectRelationExists(PawnRelationDefOf.ExSpouse, father) ?? true) return true;
                return (!mother?.relations?.DirectRelationExists(PawnRelationDefOf.Spouse, father) ?? true) || (!mother?.relations?.DirectRelationExists(PawnRelationDefOf.ExSpouse, father) ?? true);
            }
            
            return false;
        }


    }
}
