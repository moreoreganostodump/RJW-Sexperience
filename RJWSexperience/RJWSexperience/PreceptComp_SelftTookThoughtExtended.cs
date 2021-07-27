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
		public const string Raped = "[Raped]";
		public const string Rape = "[Rape]";
    }

	public class PreceptComp_SelfTookThoughtTagged : PreceptComp_SelfTookMemoryThought
	{
		public string tag;
		public bool exclusive = false;


		public PreceptComp_SelfTookThoughtTagged() { }

		public override void Notify_MemberTookAction(HistoryEvent ev, Precept precept, bool canApplySelfTookThoughts)
		{
			if (tag != null)
            {
				if (ev.args.TryGetArg(HistoryEventArgsNamesCustom.Tag, out string tags))
				{
					if (tags.ToLower().Contains(tag.ToLower()) ^ exclusive) base.Notify_MemberTookAction(ev, precept, canApplySelfTookThoughts);
				}
				else if (exclusive)
				{
					base.Notify_MemberTookAction(ev, precept, canApplySelfTookThoughts);
				}
			}
            else
            {
				base.Notify_MemberTookAction(ev, precept, canApplySelfTookThoughts);
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
					if (tags.ToLower().Contains(tag.ToLower()) ^ exclusive) base.Notify_MemberWitnessedAction(ev, precept, member);
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
