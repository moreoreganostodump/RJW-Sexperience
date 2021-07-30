using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace RJWSexperience
{
	public static class HistoryEventArgsNamesCustom
	{
		public const string Tag = "TAG";
		public const string Partner = "PARTNER";
	}

	public static class HETag
    {
		public const string Incestous = "[Incestuos]";
		public const string BeenRaped = "[BeenRaped]";
		public const string Rape = "[Rape]";
		public static string Gender(Pawn pawn) => "[" + pawn.gender + "]";

    }

	public class PreceptComp_SelfTookThoughtTagged : PreceptComp_SelfTookMemoryThought
	{
		public string tag;
		public bool exclusive = false;
		public RecordDef recordDef;
		public float? recordoffset;

		public PreceptComp_SelfTookThoughtTagged() { }

		public override void Notify_MemberTookAction(HistoryEvent ev, Precept precept, bool canApplySelfTookThoughts)
		{
			if (tag != null)
            {
				if (ev.args.TryGetArg(HistoryEventArgsNamesCustom.Tag, out string tags))
				{
					if (tags.ContainAll(tag.Replace(" ","").Split(',')) ^ exclusive)
                    {
						TookThought(ev, precept, canApplySelfTookThoughts);
						if (ev.args.TryGetArg(HistoryEventArgsNames.Doer, out Pawn pawn))
                        {
							AdjustRecord(pawn);
                        }
					}
				}
				else if (exclusive)
				{
					TookThought(ev, precept, canApplySelfTookThoughts);
					if (ev.args.TryGetArg(HistoryEventArgsNames.Doer, out Pawn pawn))
					{
						AdjustRecord(pawn);
					}
				}
			}
            else
            {
				TookThought(ev, precept, canApplySelfTookThoughts);
				if (ev.args.TryGetArg(HistoryEventArgsNames.Doer, out Pawn pawn))
				{
					AdjustRecord(pawn);
				}
			}

		}

		protected virtual void TookThought(HistoryEvent ev, Precept precept, bool canApplySelfTookThoughts)
        {
			if (ev.def != this.eventDef || !canApplySelfTookThoughts)
			{
				return;
			}
			Pawn arg = ev.args.GetArg<Pawn>(HistoryEventArgsNames.Doer);
			Pawn partner = ev.args.GetArg<Pawn>(HistoryEventArgsNamesCustom.Partner);
			if (arg.needs != null && arg.needs.mood != null && (!this.onlyForNonSlaves || !arg.IsSlave))
			{
				if (this.thought.minExpectationForNegativeThought != null && ExpectationsUtility.CurrentExpectationFor(arg).order < this.thought.minExpectationForNegativeThought.order)
				{
					return;
				}
				Thought_Memory thought_Memory = ThoughtMaker.MakeThought(this.thought, precept);
				Thought_KilledInnocentAnimal thought_KilledInnocentAnimal;
				Pawn animal;
				if ((thought_KilledInnocentAnimal = (thought_Memory as Thought_KilledInnocentAnimal)) != null && ev.args.TryGetArg<Pawn>(HistoryEventArgsNames.Victim, out animal))
				{
					thought_KilledInnocentAnimal.SetAnimal(animal);
				}
				Thought_MemoryObservation thought_MemoryObservation;
				Corpse target;
				if ((thought_MemoryObservation = (thought_Memory as Thought_MemoryObservation)) != null && ev.args.TryGetArg<Corpse>(HistoryEventArgsNames.Subject, out target))
				{
					thought_MemoryObservation.Target = target;
				}
				arg.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, partner);
			}
		}


		protected void AdjustRecord(Pawn pawn)
        {
			if (recordDef != null)
            {
				pawn.records.AddTo(recordDef, recordoffset ?? 1f);
            }
        }

	}



	public class PreceptComp_KnowsMemoryThoughtTagged : PreceptComp_KnowsMemoryThought
    {
		public string tag;
		public bool exclusive = false;
		public bool applyonpartner = false;

		public PreceptComp_KnowsMemoryThoughtTagged() { }

		public override void Notify_MemberWitnessedAction(HistoryEvent ev, Precept precept, Pawn member)
        {
			if (!applyonpartner)
            {
				if (ev.args.TryGetArg(HistoryEventArgsNamesCustom.Partner, out Pawn pawn))
                {
					if (pawn == member) return;
                }
            }
			if (tag != null)
            {
				if (ev.args.TryGetArg(HistoryEventArgsNamesCustom.Tag, out string tags))
				{
					if (tags.ContainAll(tag.Replace(" ", "").Split(',')) ^ exclusive) base.Notify_MemberWitnessedAction(ev, precept, member);
				}
				else if (exclusive)
				{
					base.Notify_MemberWitnessedAction(ev, precept, member);
				}
			}
			else
            {
				base.Notify_MemberWitnessedAction(ev, precept, member);
            }
        }
    }

}
