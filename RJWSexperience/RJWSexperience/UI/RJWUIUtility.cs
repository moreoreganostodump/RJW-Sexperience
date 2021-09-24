using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using rjw;


namespace RJWSexperience.UI
{
    public static class RJWUIUtility
	{
		public const float FONTHEIGHT = 22f;
		public const float CARDHEIGHT = 110f;
		public const float LISTPAWNSIZE = 100f;
		public const float BASESAT = 0.40f;
		public const float ICONSIZE = 30f;
		

		public static void DrawQuirk(this Rect rect, Pawn pawn)
        {
            List<Quirk> quirks = Quirk.All.FindAll(x => pawn.Has(x));
            string quirkstr = quirks.Select(x => x.Key).ToCommaList();
			string tooltip = "";

			Widgets.Label(rect, "Quirks".Translate() + quirkstr);

			if (Mouse.IsOver(rect))
            {
				if (quirks.NullOrEmpty())
				{
					tooltip = "NoQuirks".Translate();
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (var q in quirks)
					{
						stringBuilder.AppendLine(q.Key.Colorize(Color.yellow));
						stringBuilder.AppendLine(q.LocaliztionKey.Translate(pawn.Named("pawn")).AdjustedFor(pawn).Resolve());
						stringBuilder.AppendLine("");
					}
					tooltip = stringBuilder.ToString().TrimEndNewlines();
				}
				Widgets.DrawHighlight(rect);
			}
			
			TooltipHandler.TipRegion(rect, tooltip);
        }

		public static void DrawSexuality(this Rect rect, CompRJW comp)
        {
			if (comp != null)
            {
				string sexuality = Keyed.Sexuality[(int)comp.orientation];
				Widgets.Label(rect, Keyed.RS_Sexuality + ": " + sexuality);
				Widgets.DrawHighlightIfMouseover(rect);
			}
        }

		public static string GetRelationsString(this Pawn pawn, Pawn otherpawn)
        {
			if (otherpawn != null)
            {
				IEnumerable<PawnRelationDef> relations = pawn.GetRelations(otherpawn);
				if (!relations.EnumerableNullOrEmpty()) return relations.Select(x => x.GetGenderSpecificLabel(otherpawn)).ToCommaList().CapitalizeFirst();
            }
			return "";
        }
		
		public static void DrawBorder(this Rect rect, Texture border, float thickness = 1f)
        {
			GUI.DrawTexture(new Rect(rect.x,rect.y,rect.width, thickness), border);
			GUI.DrawTexture(new Rect(rect.x+rect.width-thickness,rect.y, thickness, rect.height), border);
			GUI.DrawTexture(new Rect(rect.x,rect.y+rect.height - thickness,rect.width, thickness), border);
			GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), border);
		}

		public static string GetStatExplanation(Pawn pawn, StatDef stat, float val)
        {
			return stat.description + "\n" +
				stat.Worker.GetExplanationFull(StatRequest.For(pawn), ToStringNumberSense.Undefined, val);
        }

    }
}
