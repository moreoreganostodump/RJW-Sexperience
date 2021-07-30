using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;



namespace RJWSexperience
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
}
