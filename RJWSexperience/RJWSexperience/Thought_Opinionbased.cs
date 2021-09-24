using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace RJWSexperience
{
    /// <summary>
    /// ThoughtDef using opinion
    /// </summary>
    public class ThoughtDef_Opinionbased : ThoughtDef
    {
        public List<float> minimumValueforStage = new List<float>();
    }

    /// <summary>
    /// Thought class using record.
    /// </summary>
    public class Thought_Opinionbased : Thought_Memory
    {
        protected ThoughtDef_Opinionbased Def => (ThoughtDef_Opinionbased)def;
        protected List<float> minimumValueforStage => Def.minimumValueforStage;

        public override int CurStageIndex
        {
            get
            {
                float value = 0f;
                if (otherPawn != null) value = pawn.relations?.OpinionOf(otherPawn) ?? 0f;
                for (int i = minimumValueforStage.Count - 1; i > 0; i--)
                {
                    if (minimumValueforStage[i] < value) return i;
                }
                return 0;
            }
        }
    }
}
