using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;
using rjw;

namespace RJWSexperience
{
	public class JobGiver_Orgy : ThinkNode_JobGiver
	{
		/// <summary> Checks all of our potential partners to see if anyone's eligible, returning the most attractive and convenient one. </summary>
		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!RJWHookupSettings.HookupsEnabled || !RJWHookupSettings.QuickHookupsEnabled)
				return null;

			if (pawn.Drafted)
				return null;

			if (!SexUtility.ReadyForHookup(pawn))
				return null;

			// We increase the time right away to prevent the fairly expensive check from happening too frequently
			SexUtility.IncreaseTicksToNextHookup(pawn);

			// If the pawn is a whore, or recently had sex, skip the job unless they're really horny
			if (!xxx.is_frustrated(pawn) && (xxx.is_whore(pawn) || !SexUtility.ReadyForLovin(pawn)))
				return null;

			// This check attempts to keep groups leaving the map, like guests or traders, from turning around to hook up
			if (pawn.mindState?.duty?.def == DutyDefOf.TravelOrLeave)
			{
				// TODO: Some guest pawns keep the TravelOrLeave duty the whole time, I think the ones assigned to guard the pack animals.
				// That's probably ok, though it wasn't the intention.
				if (RJWSettings.DebugLogJoinInBed) ModLog.Message($" Quickie.TryGiveJob:({xxx.get_pawnname(pawn)}): has TravelOrLeave, no time for lovin!");
				return null;
			}

			if (pawn.CurJob == null)
			{
				//--Log.Message("   checking pawn and abilities");
				if (CasualSex_Helper.CanHaveSex(pawn))
				{
					//--Log.Message("   finding partner");
					Pawn partner = CasualSex_Helper.find_partner(pawn, pawn.Map, false);

					//--Log.Message("   checking partner");
					if (partner == null)
						return null;

					// Interrupt current job.
					if (pawn.CurJob != null && pawn.jobs.curDriver != null)
						pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);

					//--Log.Message("   returning job");
					return JobMaker.MakeJob(xxx.quick_sex, partner);
				}
			}

			return null;
		}
	}
}
