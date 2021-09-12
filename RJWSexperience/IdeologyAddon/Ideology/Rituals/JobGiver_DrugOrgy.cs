using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rjw;
using Verse;
using Verse.AI;
using RimWorld;


namespace RJWSexperience.Ideology
{
    public class JobGiver_DrugOrgy : ThinkNode_JobGiver
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
            
            Pawn target = FindPartner(pawn, duty);

            if (target == null || !pawn.CanReserveAndReach(target, PathEndMode.ClosestTouch, Danger.None,1)) return JobMaker.MakeJob(VariousDefOf.DrugMasturbate);

            return JobMaker.MakeJob(VariousDefOf.DrugSex, target);
        }

        protected Pawn FindPartner(Pawn pawn, PawnDuty duty)
        {
            if (duty != null)
            {
                List<Pawn> pawns = pawn.Map.mapPawns.AllPawnsSpawned.FindAll(x => x.mindState?.duty?.def == duty.def);
                return pawns.RandomElementByWeightWithDefault(x => SexAppraiser.would_fuck(pawn,x), 0.1f);
            }
            
            

            return null;
        }

    }

	/// <summary>
	/// copied from rjw
	/// </summary>
	public class JobDriver_SexDrugOrgy : JobDriver_SexBaseInitiator
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			//ModLog.Message("" + this.GetType().ToString() + "::MakeNewToils() called");
			setup_ticks();
			var PartnerJob = VariousDefOf.GettinDrugSex;

			this.FailOnDespawnedNullOrForbidden(iTarget);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner == null);
			yield return Toils_Goto.GotoThing(iTarget, PathEndMode.OnCell);


			//Toil findQuickieSpot = new Toil();
			//findQuickieSpot.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			//findQuickieSpot.initAction = delegate
			//{
			//	var spot = Partner.Position.RandomAdjacentCell8Way();
			//	pawn.pather.StartPath(spot, PathEndMode.OnCell);
			//	Partner.jobs.StopAll();         //sometimes errors with stuff like vomiting
			//	Job job = JobMaker.MakeJob(JobDefOf.GotoMindControlled, spot);
			//	Partner.jobs.StartJob(job, JobCondition.InterruptForced);
			//};
			//yield return findQuickieSpot;

			Toil WaitForPartner = new Toil();
			WaitForPartner.defaultCompleteMode = ToilCompleteMode.Delay;
			WaitForPartner.initAction = delegate
			{
				ticksLeftThisToil = 5000;
			};
			WaitForPartner.tickAction = delegate
			{
				pawn.GainComfortFromCellIfPossible();
				if (pawn.Position.DistanceTo(Partner.Position) <= 1f)
				{
					ReadyForNextToil();
				}
			};
			yield return WaitForPartner;

			Toil StartPartnerJob = new Toil();
			StartPartnerJob.defaultCompleteMode = ToilCompleteMode.Instant;
			StartPartnerJob.socialMode = RandomSocialMode.Off;
			StartPartnerJob.initAction = delegate
			{
				var dri = Partner.jobs.curDriver as JobDriver_DrugSexReceiver;
				if (dri == null)
                {
					Job gettingQuickie = JobMaker.MakeJob(PartnerJob, pawn, Partner);
					Partner.jobs.StartJob(gettingQuickie, JobCondition.InterruptForced);
				}
			};
			yield return StartPartnerJob;

			Toil SexToil = new Toil();
			SexToil.defaultCompleteMode = ToilCompleteMode.Never;
			SexToil.socialMode = RandomSocialMode.Off;
			SexToil.defaultDuration = duration;
			SexToil.handlingFacing = true;
			SexToil.FailOn(() => Partner.CurJob.def != PartnerJob);
			SexToil.initAction = delegate
			{
				Partner.pather.StopDead();
				Partner.jobs.curDriver.asleep = false;
				
				Start();
				Sexprops.usedCondom = CondomUtility.TryUseCondom(pawn) || CondomUtility.TryUseCondom(Partner);
			};
			SexToil.AddPreTickAction(delegate
			{
				SexTick(pawn, Partner);
				SexUtility.reduce_rest(Partner, 1);
				SexUtility.reduce_rest(pawn, 1);
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
					//// Trying to add some interactions and social logs
					SexUtility.ProcessSex(Sexprops);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}

	/// <summary>
	/// copied from rjw
	/// </summary>
	public class JobDriver_DrugSexReceiver : JobDriver_SexBaseRecieverLoved
    {

		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();
			parteners.Add(Partner);// add job starter, so this wont fail, before Initiator starts his job
								   //--ModLog.Message("JobDriver_GettinLoved::MakeNewToils is called");
								   //ModLog.Message("" + Partner.CurJob.def);

			// More/less hearts based on opinion.
			if (pawn.relations.OpinionOf(Partner) < 0)
				ticks_between_hearts += 50;
			else if (pawn.relations.OpinionOf(Partner) > 60)
				ticks_between_hearts -= 25;

			this.FailOnDespawnedOrNull(iTarget);
			this.FailOn(() => !Partner.health.capacities.CanBeAwake);
			this.FailOn(() => pawn.Drafted);
			this.FailOn(() => Partner.Drafted);
			this.FailOn(() => Partner == null);

			yield return Toils_Reserve.Reserve(iTarget, 1, 0);

				var get_loved = MakeSexToil();
				get_loved.handlingFacing = false;
				yield return get_loved;
		}

		protected Toil MakeSexToil()
		{
			Toil get_loved = new Toil();
			get_loved.defaultCompleteMode = ToilCompleteMode.Never;
			get_loved.socialMode = RandomSocialMode.Off;
			get_loved.handlingFacing = true;
			//get_loved.initAction = delegate
			//{
			//};
			get_loved.tickAction = delegate
			{
			};
			get_loved.AddEndCondition(new Func<JobCondition>(() =>
			{
				if (parteners.Count <= 0)
				{
					return JobCondition.Succeeded;
				}
				return JobCondition.Ongoing;
			}));
			get_loved.AddFinishAction(delegate
			{
				if (xxx.is_human(pawn))
					pawn.Drawer.renderer.graphics.ResolveApparelGraphics();
			});
			get_loved.socialMode = RandomSocialMode.Off;
			return get_loved;
		}
	}

	/// <summary>
	/// copied from rjw
	/// </summary>
	public class JobDriver_DrugMasturabate : JobDriver_Masturbate
    {

		protected override IEnumerable<Toil> MakeNewToils()
		{
			setup_ticks();

			//this.FailOn(() => PawnUtility.PlayerForcedJobNowOrSoon(pawn));
			this.FailOn(() => pawn.health.Downed);
			this.FailOn(() => pawn.IsBurning());
			this.FailOn(() => pawn.IsFighting());
			this.FailOn(() => pawn.Drafted);


			//ModLog.Message(" Making new toil for QuickFap.");

			Toil SexToil = Toils_General.Wait(duration);
			SexToil.handlingFacing = true;
			SexToil.initAction = delegate
			{
				Start();
			};
			SexToil.tickAction = delegate
			{
				SexTick(pawn, null, true);
				SexUtility.reduce_rest(pawn, 1);
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
					SexUtility.Aftersex(Sexprops);
					if (!SexUtility.ConsiderCleaning(pawn)) return;

					LocalTargetInfo own_cum = pawn.PositionHeld.GetFirstThing<Filth>(pawn.Map);

					Job clean = JobMaker.MakeJob(JobDefOf.Clean);
					clean.AddQueuedTarget(TargetIndex.A, own_cum);

					pawn.jobs.jobQueue.EnqueueFirst(clean);
				},
				defaultCompleteMode = ToilCompleteMode.Instant
			};
		}
	}


}
