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


namespace RJWSexperience
{
    public class JobGiver_GangbangVictim : ThinkNode_JobGiver
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

			if (dutyDef == DutyDefOf.TravelOrLeave || !xxx.can_rape(pawn))
            {
                return null;
			}

			Pawn target = duty.focusSecond.Pawn;

            if (!pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.None)) return null;

            return JobMaker.MakeJob(VariousDefOf.RapeVictim, target);
        }
    }


	/// <summary>
	/// copied from rjw
	/// </summary>
	public class JobDriver_RapeVictim : JobDriver_Rape
	{
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
			return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
		{
			if (RJWSettings.DebugRape) ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();
			var PartnerJob = xxx.gettin_raped;

			this.FailOnDespawnedNullOrForbidden(iTarget);
			//this.FailOn(() => (!Partner.health.capacities.CanBeAwake) || (!comfort_prisoners.is_designated(Partner)));//this is wrong
			this.FailOn(() => Partner == null);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);

			SexUtility.RapeTargetAlert(pawn, Partner);

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				var dri = Partner.jobs.curDriver as JobDriver_SexBaseRecieverRaped;
				if (dri == null)
				{
					Job gettin_raped = JobMaker.MakeJob(PartnerJob, pawn);
					Building_Bed Bed = null;
					if (Partner.GetPosture() == PawnPosture.LayingInBed)
						Bed = Partner.CurrentBed();

					Partner.jobs.StartJob(gettin_raped, JobCondition.InterruptForced, null, false, true, null);
					if (Bed != null)
						(Partner.jobs.curDriver as JobDriver_SexBaseRecieverRaped)?.Set_bed(Bed);
				}
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.CurJob.def != PartnerJob);
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;
				// Unlike normal rape try use comfort prisoner condom
				CondomUtility.GetCondomFromRoom(Partner);
				usedCondom = CondomUtility.TryUseCondom(Partner);

				if (RJWSettings.DebugRape) ModLog.Message("JobDriver_RapeComfortPawn::MakeNewToils() - reserving prisoner");
				//pawn.Reserve(Partner, xxx.max_rapists_per_prisoner, 0);
				Start();
			};
			SexToil.tickAction = delegate
			{
				//if (pawn.IsHashIntervalTick(ticks_between_hearts))
				//	ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Heart);
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 2);
				if (ticks_left <= 0)
					ReadyForNextToil();
			};
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
					SexUtility.ProcessSex(pawn, Partner, usedCondom: usedCondom, rape: isRape, sextype: sexType);
					Partner.records.Increment(xxx.GetRapedAsComfortPawn);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}
}
