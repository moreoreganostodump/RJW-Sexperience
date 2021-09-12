using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;


namespace RJWSexperience.Ideology
{
    public class RitualOutcomeComp_HediffBased : RitualOutcomeComp_QualitySingleOffset
    {
		HediffDef hediffDef = null;
		float minSeverity = 0;
		string roleId = "";

        protected override string LabelForDesc => label;
        public override bool DataRequired => false;
		public override bool Applies(LordJob_Ritual ritual)
		{
			Pawn victim = null;
			foreach(RitualRole ritualRole in ritual.assignments.AllRolesForReading)
            {
				if (ritualRole != null && ritualRole.id.Contains(roleId))
				{
					victim = ritual.assignments.FirstAssignedPawn(ritualRole);
				}
			}
			if (victim != null && hediffDef != null)
            {
				Hediff hediff = victim.health.hediffSet.GetFirstHediffOfDef(hediffDef);
				if (hediff?.Severity >= minSeverity)
                {
					return true;
                }
            }
			return false;
		}
		

<<<<<<< HEAD

		public override ExpectedOutcomeDesc GetExpectedOutcomeDesc(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
=======
        public override ExpectedOutcomeDesc GetExpectedOutcomeDesc(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
>>>>>>> 2c61e8b5425da0190af8f9c6a499a3da7cd49dcf
		{
			return new ExpectedOutcomeDesc
			{
				label = LabelForDesc.CapitalizeFirst(),
				present = false,
				uncertainOutcome = true,
				effect = ExpectedOffsetDesc(true, -1f),
				quality = qualityOffset,
				positive = true
			};
		}

	}

	public class RitualOutcomeComp_NeedBased : RitualOutcomeComp_QualitySingleOffset
	{
		NeedDef needDef = null;
		float minAvgNeed = 0;

		protected override string LabelForDesc => label;
		public override bool DataRequired => false;
		public override bool Applies(LordJob_Ritual ritual)
		{
			float avgNeed = 0;
			foreach (Pawn pawn in ritual.assignments.AllPawns)
			{
				avgNeed += pawn.needs?.TryGetNeed(needDef)?.CurLevel ?? 0f;
			}
			avgNeed /= ritual.assignments.AllPawns.Count;
			if (avgNeed >= minAvgNeed) return true;

			return false;
		}

		public override ExpectedOutcomeDesc GetExpectedOutcomeDesc(Precept_Ritual ritual, TargetInfo ritualTarget, RitualObligation obligation, RitualRoleAssignments assignments, RitualOutcomeComp_Data data)
		{
			return new ExpectedOutcomeDesc
			{
				label = LabelForDesc.CapitalizeFirst(),
				present = false,
				uncertainOutcome = true,
				effect = ExpectedOffsetDesc(true, -1f),
				quality = qualityOffset,
				positive = true
			};
		}
	}



}
