using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RJWSexperience.Ideology
{
    public class RitualBehaviorWorker_Gangbang : RitualBehaviorWorker
    {
		public RitualBehaviorWorker_Gangbang() { }

		public RitualBehaviorWorker_Gangbang(RitualBehaviorDef def) : base(def) { }
		
		public override void PostCleanup(LordJob_Ritual ritual)
		{
			Pawn warden = ritual.PawnWithRole("initiator");
			Pawn pawn = ritual.PawnWithRole("victim");
			if (pawn.IsPrisonerOfColony)
			{
				WorkGiver_Warden_TakeToBed.TryTakePrisonerToBed(pawn, warden);
				pawn.guest.WaitInsteadOfEscapingFor(1250);
			}
		}

        protected override LordJob CreateLordJob(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
        {
            return new LordJob_Ritual_Gangbang("victim", target, ritual, obligation, def.stages, assignments, organizer);
		}
    }

	public class RitualBehaviorWorker_Gangbang_Consensual : RitualBehaviorWorker
	{
		public RitualBehaviorWorker_Gangbang_Consensual() { }

		public RitualBehaviorWorker_Gangbang_Consensual(RitualBehaviorDef def) : base(def) { }

		protected override LordJob CreateLordJob(TargetInfo target, Pawn organizer, Precept_Ritual ritual, RitualObligation obligation, RitualRoleAssignments assignments)
		{
			return new LordJob_Ritual_Gangbang("initiator", target, ritual, obligation, def.stages, assignments, organizer);
		}
	}

	public class RitualStage_InteractWithVictim : RitualStage
    {
		public override TargetInfo GetSecondFocus(LordJob_Ritual ritual)
		{
			return ritual.assignments.AssignedPawns("victim").FirstOrDefault(p => RitualRole_RapeVictim.CanBeVictim(p));
		}
	}

	public class RitualStage_InteractWithVictim_All : RitualStage
	{
		public override TargetInfo GetSecondFocus(LordJob_Ritual ritual)
		{
			return ritual.assignments.AssignedPawns("victim").FirstOrDefault();
		}
	}
	public class RitualStage_InteractWithInitiator : RitualStage
	{
		public override TargetInfo GetSecondFocus(LordJob_Ritual ritual)
		{
			return ritual.assignments.AssignedPawns("initiator").FirstOrDefault();
		}
	}
}
