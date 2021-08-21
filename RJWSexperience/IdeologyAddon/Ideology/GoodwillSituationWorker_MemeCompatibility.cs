using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace RJWSexperience.Ideology
{
    public class GoodwillSituationWorker_OneWayReceive : GoodwillSituationWorker_MemeCompatibility
    {
        public override int GetNaturalGoodwillOffset(Faction other)
        {

            if (!Applies(other)) return 0;
            return def.naturalGoodwillOffset;
        }

        protected bool Applies(Faction other)
        {
            Ideo primaryideo = Faction.OfPlayer.ideos?.PrimaryIdeo;
            Ideo primaryideo2 = other.ideos?.PrimaryIdeo;
            if (primaryideo == null || primaryideo2 == null) return false;

            return primaryideo.memes.Contains(def.meme) && !primaryideo2.memes.Contains(def.meme);
        }


    }
}
