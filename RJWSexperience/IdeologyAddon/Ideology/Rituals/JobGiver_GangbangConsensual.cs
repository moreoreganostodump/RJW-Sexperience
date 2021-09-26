using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using rjw;

namespace RJWSexperience.Ideology
{
    public class JobGiver_GangbangConsensual : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Drafted) return null;
            DutyDef dutyDef = null;
            PawnDuty duty = null;
            if (pawn.mindState != null)
            {
                duty = pawn.mindState.duty;
                dutyDef = duty.def;
            }
            else return null;

            if (dutyDef == DutyDefOf.TravelOrLeave || !xxx.can_do_loving(pawn))
            {
                return null;
            }

            Pawn target = duty.focusSecond.Pawn;

            if (!pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.None)) return null;

            return JobMaker.MakeJob(VariousDefOf.Gangbang, target);
        }
    }


	public class JobDriver_Gangbang : JobDriver_SexBaseInitiator
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => Partner == null);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{

				var dri = Partner.jobs.curDriver as JobDriver_SexBaseRecieverRaped;
				if (dri == null)
                {
					Job gettin_loved = JobMaker.MakeJob(VariousDefOf.GettinGangbang, pawn, Bed);
					Partner.jobs.StartJob(gettin_loved, JobCondition.InterruptForced);
				}
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.CurJob.def != VariousDefOf.GettinGangbang);
			SexToil.initAction = delegate
			{
				Start();
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);
			};
			SexToil.AddPreTickAction(delegate
			{
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			});
			SexToil.AddFinishAction(delegate
			{
				End();
			});
			yield return SexToil;

			yield return new Toil
			{
				initAction = delegate
				{
					// Trying to add some interactions and social logs
					SexUtility.ProcessSex(Sexprops);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}

	public class JobDriver_GangbangReceiver : JobDriver_SexBaseRecieverLoved
    {
		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();
			parteners.Add(Partner);// add job starter, so this wont fail, before Initiator starts his job

			Toil get_banged = new Toil();
			get_banged.defaultCompleteMode = ToilCompleteMode.Never;
			get_banged.handlingFacing = true;
			get_banged.initAction = delegate
			{
				pawn.pather.StopDead();
				pawn.jobs.curDriver.asleep = false;
			};
			get_banged.tickAction = delegate
			{
				if ((parteners.Count > 0) && (pawn.IsHashIntervalTick(ticks_between_hearts / parteners.Count)))
					if (pawn.IsHashIntervalTick(ticks_between_hearts))
							ThrowMetaIconF(pawn.Position, pawn.Map, FleckDefOf.Heart);
			};
			get_banged.AddEndCondition(new Func<JobCondition>(() =>
			{
				if (parteners.Count <= 0)
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			}));
			get_banged.AddFinishAction(delegate
			{
				if (xxx.is_human(pawn))
					pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
				GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);

				if (Bed != null && pawn.Downed)
				{
					Job tobed = JobMaker.MakeJob(JobDefOf.Rescue, pawn, Bed);
					tobed.count = 1;
					Partner.jobs.jobQueue.EnqueueFirst(tobed);
					//Log.Message(xxx.get_pawnname(Initiator) + ": job tobed:" + tobed);
				}
				else if (pawn.HostileTo(Partner))
					pawn.health.AddHediff(xxx.submitting);
			});
			get_banged.socialMode = RandomSocialMode.Off;
			yield return get_banged;

		}
	}

}
