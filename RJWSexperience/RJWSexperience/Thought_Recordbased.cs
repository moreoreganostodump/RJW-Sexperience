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
    /// ThoughtDef using record
    /// </summary>
    public class ThoughtDef_Recordbased : ThoughtDef
    {
        public RecordDef recordDef;
        public List<float> minimumValueforStage = new List<float>();
        public float increment;
    } 

    /// <summary>
    /// Thought class using record.
    /// </summary>
    public class Thought_Recordbased : Thought_Memory
    {
        protected ThoughtDef_Recordbased Def => (ThoughtDef_Recordbased)def;
        protected RecordDef recordDef => Def.recordDef;
        protected List<float> minimumValueforStage => Def.minimumValueforStage;
        protected float increment => Def.increment;

        public override int CurStageIndex
        {            
            get
            {                
                float value = pawn?.records?.GetValue(recordDef) ?? 0f;
                for (int i = minimumValueforStage.Count - 1; i > 0; i--)
                {
                    if (minimumValueforStage[i] < value) return i + 1;
                }
                return 0;
            }
        }
    }

    public class Thought_AteCum : Thought_Recordbased
    {

        protected int recordIncrement = 1;

        public override int CurStageIndex
        {
            get
            {
                if (pawn?.health?.hediffSet?.HasHediff(VariousDefOf.CumAddiction) ?? false) return minimumValueforStage.Count;
                return base.CurStageIndex;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref recordIncrement, "recordIncrement", recordIncrement, true);
        }



        //There is no direct way to modify custom records via ingestion. So i increase it from thought.
        public override void ThoughtInterval()
        {
            base.ThoughtInterval();
            if (recordIncrement >= 1)
            {
                recordIncrement--;
                pawn.records.AddTo(VariousDefOf.NumofEatenCum, 1);
            }
            
        }

        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            ThoughtHandler thoughts = pawn.needs.mood.thoughts;
            if (thoughts.memories.NumMemoriesInGroup(this) >= def.stackLimit)
            {
                Thought_AteCum thought_Memory = (Thought_AteCum)thoughts.memories.OldestMemoryInGroup(this);
                if (thought_Memory != null)
                {
                    showBubble = (thought_Memory.age > thought_Memory.def.DurationTicks / 2);
                    thought_Memory.Merged();
                    return true;
                }
            }
            showBubble = true;
            return false;
        }

        public override void Init()
        {
            base.Init();
            recordIncrement = 1;
        }

        protected virtual void Merged()
        {
            age = 0;
            recordIncrement += 1;
        }
    }

    public class Thought_IncreaseRecord : Thought_Recordbased
    {
        protected float recordIncrement;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref recordIncrement, "recordIncrement", recordIncrement, true);
        }

        public override void ThoughtInterval()
        {
            base.ThoughtInterval();
            if (recordIncrement != 0)
            {
                pawn.records.AddTo(recordDef, recordIncrement);
                recordIncrement = 0;
            }

        }

        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            ThoughtHandler thoughts = pawn.needs.mood.thoughts;
            if (thoughts.memories.NumMemoriesInGroup(this) >= def.stackLimit)
            {
                Thought_IncreaseRecord thought_Memory = (Thought_IncreaseRecord)thoughts.memories.OldestMemoryInGroup(this);
                if (thought_Memory != null)
                {
                    showBubble = (thought_Memory.age > thought_Memory.def.DurationTicks / 2);
                    thought_Memory.Merged();
                    return true;
                }
            }
            showBubble = true;
            return false;
        }

        public override void Init()
        {
            base.Init();
            recordIncrement = increment;
        }
        protected virtual void Merged()
        {
            age = 0;
            recordIncrement += increment;
        }
    }


}
