using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using rjw;


namespace RJWSexperience.UI
{
    public class SexStatusWindow : Window
    {
        public const float FONTHEIGHT = RJWUIUtility.FONTHEIGHT;
        public const float CARDHEIGHT = RJWUIUtility.CARDHEIGHT;
        public const float LISTPAWNSIZE = RJWUIUtility.LISTPAWNSIZE;
        public const float BASESAT = RJWUIUtility.BASESAT;
        public const float ICONSIZE = RJWUIUtility.ICONSIZE;

        public static readonly int[] Sextype =
        {
            (int)xxx.rjwSextype.Vaginal,
            (int)xxx.rjwSextype.Anal,
            (int)xxx.rjwSextype.Oral,
            (int)xxx.rjwSextype.Fellatio,
            (int)xxx.rjwSextype.Cunnilingus,
            (int)xxx.rjwSextype.DoublePenetration,
            (int)xxx.rjwSextype.Boobjob,
            (int)xxx.rjwSextype.Handjob,
            (int)xxx.rjwSextype.Footjob,
            (int)xxx.rjwSextype.Fingering,
            (int)xxx.rjwSextype.Scissoring,
            (int)xxx.rjwSextype.MutualMasturbation,
            (int)xxx.rjwSextype.Fisting,
            (int)xxx.rjwSextype.Rimming,
            (int)xxx.rjwSextype.Sixtynine
        };


        protected Pawn pawn;
        protected SexHistory selectedPawn;
        protected SexPartnerHistory history;
        protected CompRJW rjwcomp;

        private static GUIStyleState fontstylestate = new GUIStyleState() { textColor = Color.white };
        private static GUIStyleState boxstylestate = GUI.skin.textArea.normal;
        private static GUIStyleState buttonstylestate = GUI.skin.button.normal;
        private static GUIStyle fontstylecenter = new GUIStyle() { alignment = TextAnchor.MiddleCenter, normal = fontstylestate };
        private static GUIStyle fontstyleright = new GUIStyle() { alignment = TextAnchor.MiddleRight, normal = fontstylestate };
        private static GUIStyle fontstyleleft = new GUIStyle() { alignment = TextAnchor.MiddleLeft, normal = fontstylestate };
        private static GUIStyle boxstyle = new GUIStyle(GUI.skin.textArea) { hover = boxstylestate, onHover = boxstylestate, onNormal = boxstylestate };
        private static GUIStyle buttonstyle = new GUIStyle(GUI.skin.button) { hover = buttonstylestate, onHover = buttonstylestate, onNormal = buttonstylestate };

        private static Vector2 pos;
        private static Vector2 scroll;
        private static bool opened;

        public SexStatusWindow(Pawn pawn, SexPartnerHistory history)
        {
            this.pawn = pawn;
            this.history = history;
            this.selectedPawn = null;
            this.rjwcomp = pawn.TryGetComp<CompRJW>();
        }

        protected override void SetInitialSizeAndPosition()
        {
            Vector2 initialSize = InitialSize;
            if (!opened)
            {
                windowRect = new Rect((Verse.UI.screenWidth - initialSize.x) / 2f, ((float)Verse.UI.screenHeight - initialSize.y) / 2f, initialSize.x, initialSize.y);
                opened = true;
            }
            else
            {
                windowRect = new Rect(pos, initialSize);
            }
            windowRect = windowRect.Rounded();
            
        }

        public override Vector2 InitialSize
        {
            get
            {
                float width = 900f;
                float height = 600f;
                soundClose = SoundDefOf.CommsWindow_Close;
                absorbInputAroundWindow = false;
                forcePause = false;
                preventCameraMotion = false;
                draggable = true;
                doCloseX = true;
                return new Vector2(width, height);
            }
        }


        public override void DoWindowContents(Rect inRect)
        {
            pos = windowRect.position;
            if (!Configurations.SelectionLocked)
            {
                List<Pawn> selected = Find.Selector.SelectedPawns;
                if (selected.Count == 1)
                {
                    Pawn p = selected.First();
                    if (p != pawn)
                    {
                        SexPartnerHistory h = p.GetPartnerHistory();
                        if (h != null) ChangePawn(p, h);
                    }
                }
            }


            DrawSexStatus(inRect,history);
        }

        public static void ToggleWindow(Pawn pawn, SexPartnerHistory history)
        {
            SexStatusWindow window = (SexStatusWindow)Find.WindowStack.Windows.FirstOrDefault(x => x.GetType() == typeof(SexStatusWindow));
            if (window != null)
            {
                if (window.pawn != pawn)
                {
                    SoundDefOf.TabOpen.PlayOneShotOnCamera();
                    window.ChangePawn(pawn, history);
                }
            }
            else
            {
                Find.WindowStack.Add(new SexStatusWindow(pawn, history));
            }
        }

        public void ChangePawn(Pawn pawn, SexPartnerHistory history)
        {
            List<Pawn> selected = Find.Selector.SelectedPawns;
            if (!selected.NullOrEmpty()) foreach(Pawn p in selected)
                {
                    Find.Selector.Deselect(p);
                }
            this.pawn = pawn;
            this.history = history;
            this.selectedPawn = null;
            this.rjwcomp = pawn.TryGetComp<CompRJW>();
            if (!pawn.DestroyedOrNull() && Find.CurrentMap == pawn.Map) Find.Selector.Select(pawn);
        }

        /// <summary>
        /// Main contents
        /// </summary>
        protected void DrawSexStatus(Rect mainrect, SexPartnerHistory history)
        {
            float sectionwidth = mainrect.width / 3;

            Rect leftRect = new Rect(mainrect.x, mainrect.y, sectionwidth, mainrect.height);
            Rect centerRect = new Rect(mainrect.x + sectionwidth, mainrect.y, sectionwidth, mainrect.height);
            Rect rightRect = new Rect(mainrect.x + sectionwidth * 2, mainrect.y, sectionwidth, mainrect.height);

            if (history != null)
            {
                //Left section
                DrawBaseSexInfoLeft(leftRect.ContractedBy(4f));

                //Center section
                DrawBaseSexInfoCenter(centerRect.ContractedBy(4f),history.parent as Pawn);

                //Right section
                DrawBaseSexInfoRight(rightRect.ContractedBy(4f));
            }



        }

        protected void DrawInfoWithPortrait(Rect rect, SexHistory history, string tooltip = "")
        {
            Widgets.DrawMenuSection(rect);
            string str = tooltip;
            Rect portraitRect = new Rect(rect.x, rect.y, rect.height - FONTHEIGHT, rect.height - FONTHEIGHT);
            Rect nameRect = new Rect(rect.x + portraitRect.width, rect.y, rect.width - portraitRect.width, FONTHEIGHT);
            Rect sexinfoRect = new Rect(rect.x + portraitRect.width, rect.y + FONTHEIGHT, rect.width - portraitRect.width, FONTHEIGHT);
            Rect sexinfoRect2 = new Rect(rect.x + portraitRect.width, rect.y + FONTHEIGHT * 2, rect.width - portraitRect.width, FONTHEIGHT);
            Rect bestsexRect = new Rect(rect.x + 2f, rect.y + FONTHEIGHT * 3, rect.width - 4f, FONTHEIGHT - 2f);

            if (history != null)
            {
                if (history.Incest) str += " - " + Keyed.Incest;
                Pawn partner = history.Partner;
                DrawPawn(portraitRect, history);
                Widgets.DrawHighlightIfMouseover(portraitRect);
                if (Widgets.ButtonInvisible(portraitRect))
                {
                    SexPartnerHistory pawnhistory = partner?.GetPartnerHistory();
                    if (pawnhistory != null)
                    {
                        ChangePawn(partner, pawnhistory);
                        SoundDefOf.Click.PlayOneShotOnCamera();
                    }
                    else SoundDefOf.ClickReject.PlayOneShotOnCamera();
                }
                GUI.Label(nameRect, partner?.Name?.ToStringFull ?? history.Label.CapitalizeFirst(), fontstyleleft);
                GUI.Label(sexinfoRect, Keyed.RS_Sex_Count + history.TotalSexCount + " " + history.RapeInfo, fontstyleleft);
                GUI.Label(sexinfoRect2, Keyed.RS_Orgasms + history.OrgasmCount, fontstyleleft);
                GUI.Label(sexinfoRect2, pawn.GetRelationsString(partner) + " ", fontstyleright);
                float p = history.BestSatisfaction / BASESAT;
                FillableBarLabeled(bestsexRect,String.Format(Keyed.RS_Best_Sextype+": {0}", Keyed.Sextype[(int)history.BestSextype]), p / 2, HistoryUtility.SextypeColor[(int)history.BestSextype], Texture2D.blackTexture, null, String.Format("{0:P2}", p));

                if (history.IamFirst) str += "\n" + Keyed.RS_LostVirgin(history.Label, pawn.LabelShort);
                

                TooltipHandler.TipRegion(rect, str);
            }
            else
            {
                Widgets.DrawTextureFitted(portraitRect, HistoryUtility.UnknownPawn, 1.0f);
                Widgets.Label(nameRect, Keyed.Unknown);
                Widgets.Label(sexinfoRect, Keyed.RS_Sex_Count + "?");
                Widgets.Label(sexinfoRect2, Keyed.RS_Orgasms+"?");
                FillableBarLabeled(bestsexRect,String.Format(Keyed .RS_Best_Sextype + ": {0}", Keyed.Sextype[(int)xxx.rjwSextype.None]), 0, Texture2D.linearGrayTexture, Texture2D.blackTexture);
            }
        }

        protected void DrawSexInfoCard(Rect rect, SexHistory history, string label, string tooltip)
        {
            Rect labelRect = new Rect(rect.x, rect.y, rect.width, FONTHEIGHT);
            Rect infoRect = new Rect(rect.x, rect.y + FONTHEIGHT, rect.width, rect.height - FONTHEIGHT);
            GUI.Label(labelRect, label, fontstyleleft);
            DrawInfoWithPortrait(infoRect,history, tooltip);
            

        }
        
        /// <summary>
        /// Right section
        /// </summary>
        protected void DrawBaseSexInfoRight(Rect rect)
        {
            Listing_Standard listmain = new Listing_Standard();
            listmain.Begin(rect.ContractedBy(4f));
            DrawSexInfoCard(listmain.GetRect(CARDHEIGHT), history.GetRecentPartnersHistory, Keyed.RS_Recent_Sex_Partner, Keyed.RS_Recent_Sex_Partner_ToolTip);
            DrawSexInfoCard(listmain.GetRect(CARDHEIGHT), history.GetFirstPartnerHistory, Keyed.RS_First_Sex_Partner, Keyed.RS_First_Sex_Partner_ToolTip);
            DrawSexInfoCard(listmain.GetRect(CARDHEIGHT), history.GetMostPartnerHistory, Keyed.RS_Most_Sex_Partner, Keyed.RS_Most_Sex_Partner_ToolTip);
            DrawSexInfoCard(listmain.GetRect(CARDHEIGHT), history.GetBestSexPartnerHistory, Keyed.RS_Best_Sex_Partner, Keyed.RS_Best_Sex_Partner_ToolTip);
            GUI.Label(listmain.GetRect(FONTHEIGHT), Keyed.RS_PreferRace, fontstyleleft);
            DrawPreferRace(listmain.GetRect(66f+15f));
            listmain.GetRect(15f);


            listmain.End();
        }

        protected void DrawPreferRace(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect portraitRect = new Rect(rect.x, rect.y, rect.height-15f, rect.height-15f);
            Rect infoRect1 = new Rect(rect.x + portraitRect.width, rect.y, rect.width - portraitRect.width, FONTHEIGHT);
            Rect infoRect2 = new Rect(rect.x + portraitRect.width, rect.y + FONTHEIGHT, rect.width - portraitRect.width, FONTHEIGHT);
            Rect infoRect3 = new Rect(rect.x + portraitRect.width, rect.y + FONTHEIGHT*2, rect.width - portraitRect.width - 2f, FONTHEIGHT);

            if (history.PreferRace != null)
            {
                Widgets.DrawTextureFitted(portraitRect, history.GetPreferRaceIcon(portraitRect.size), 1.0f);
                GUI.Label(infoRect1, history.PreferRace?.label.CapitalizeFirst() ?? Keyed.None, fontstyleleft);
                GUI.Label(infoRect2, Keyed.RS_Sex_Count + history.PreferRaceSexCount, fontstyleleft);
                if (history.PreferRace != pawn.def)
                {
                    if (history.PreferRace.race.Animal ^ pawn.def.race.Animal) 
                    {
                        GUI.Label(infoRect1, Keyed.RS_Bestiality + " ", fontstyleright);
                        FillableBarLabeled(infoRect3, Keyed.RS_Sex_Info(Keyed.RS_Bestiality, history.BestialityCount.ToString()), history.BestialityCount/100f, Texture2D.linearGrayTexture, Texture2D.blackTexture);
                    }
                    else
                    {
                        GUI.Label(infoRect1, Keyed.RS_Interspecies + " ", fontstyleright);
                        FillableBarLabeled(infoRect3, Keyed.RS_Sex_Info(Keyed.RS_Interspecies, history.InterspeciesCount.ToString()), history.InterspeciesCount / 100f, Texture2D.linearGrayTexture, Texture2D.blackTexture);
                    }
                }
                else
                {
                    //GUI.Label(infoRect1, Keyed.RS_Normal + " ", fontstyleright);
                }
            }
            else
            {
                Widgets.DrawTextureFitted(portraitRect, HistoryUtility.UnknownPawn, 1.0f);
                GUI.Label(infoRect1, Keyed.None, fontstyleleft);
            }
        }


        /// <summary>
        /// Center section
        /// </summary>
        protected void DrawBaseSexInfoCenter(Rect rect, Pawn pawn)
        {
            Rect portraitRect = new Rect(rect.x + rect.width / 4, rect.y, rect.width / 2, rect.width / 1.5f);
            Rect nameRect = new Rect(portraitRect.x, portraitRect.yMax - FONTHEIGHT * 2, portraitRect.width, FONTHEIGHT * 2);
            Rect infoRect = new Rect(rect.x, rect.y + portraitRect.height, rect.width, rect.height - portraitRect.height);
            Rect lockRect = new Rect(portraitRect.xMax - ICONSIZE, portraitRect.y, ICONSIZE, ICONSIZE);
            Rect tmp;

            if (Mouse.IsOver(portraitRect))
            {
                Texture lockicon = Configurations.SelectionLocked ? HistoryUtility.Locked : HistoryUtility.Unlocked;
                Widgets.DrawTextureFitted(lockRect, lockicon, 1.0f);
                if (Widgets.ButtonInvisible(lockRect))
                {
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    Configurations.SelectionLocked = !Configurations.SelectionLocked;
                }
            }


            GUI.Box(portraitRect, "", boxstyle);
            Widgets.DrawTextureFitted(portraitRect, PortraitsCache.Get(pawn, portraitRect.size, Rot4.South, default, 1, true, true, false, false), 1.0f);
            Widgets.DrawHighlightIfMouseover(portraitRect);
            if (Widgets.ButtonInvisible(portraitRect))
            {
                SoundDefOf.Click.PlayOneShotOnCamera();
                selectedPawn = null;
            }

            GUI.Box(nameRect, "", boxstyle);
            GUI.Label(nameRect.TopHalf(), pawn.Name?.ToStringFull ?? pawn.Label, fontstylecenter);
            if (pawn.story != null) GUI.Label(nameRect.BottomHalf(), pawn.ageTracker.AgeBiologicalYears + ", " + pawn.story.Title, fontstylecenter);
            else GUI.Label(nameRect.BottomHalf(), pawn.ageTracker.AgeBiologicalYears + ", " + pawn.def.label, fontstylecenter);

            Listing_Standard listmain = new Listing_Standard();
            listmain.Begin(infoRect);
            listmain.Gap(20f);
            float p;

            if (pawn.IsVirgin())
            {
                tmp = listmain.GetRect(FONTHEIGHT);
                GUI.color = Color.red;
                GUI.Box(tmp, "", boxstyle);
                GUI.color = Color.white;
                GUI.Label(tmp, Keyed.Virgin, fontstylecenter);
            }
            else
            {
                p = history.TotalSexHad;
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_TotalSexHad + ": {0:0} ({1:0})", p, pawn.records.GetValue(xxx.CountOfSex)), p / 100, HistoryUtility.TotalSex, Texture2D.blackTexture, null, Keyed.RS_SAT_AVG(String.Format("{0:P2}", history.AVGSat)));
            }
            listmain.Gap(1f);


            tmp = listmain.GetRect(FONTHEIGHT);
            p = pawn.records.GetValue(VariousDefOf.Lust);
            FillableBarLabeled(tmp, String.Format(Keyed.Lust +": {0:0.00}", p), Mathf.Clamp01(p.Normalization(-Configurations.LustLimit*3, Configurations.LustLimit*3)), HistoryUtility.Slaanesh, Texture2D.blackTexture, null, String.Format(xxx.sex_drive_stat.LabelCap.CapitalizeFirst() + ": {0:P2}", pawn.GetStatValue(xxx.sex_drive_stat)));
            listmain.Gap(1f);
            if (Mouse.IsOver(tmp))
            {
                TooltipHandler.TipRegion(tmp, RJWUIUtility.GetStatExplanation(pawn, xxx.sex_drive_stat, pawn.GetStatValue(xxx.sex_drive_stat)));
            }

            p = history.GetBestSextype(out xxx.rjwSextype sextype) / BASESAT;
            FillableBarLabeled(listmain.GetRect(FONTHEIGHT),String.Format(Keyed.RS_Best_Sextype+": {0}", Keyed.Sextype[(int)sextype]), p / 2, HistoryUtility.SextypeColor[(int)sextype], Texture2D.blackTexture, null, Keyed.RS_SAT_AVG(String.Format("{0:P2}", p)));
            listmain.Gap(1f);

            p = history.GetRecentSextype(out sextype) / BASESAT;
            FillableBarLabeled(listmain.GetRect(FONTHEIGHT),String.Format(Keyed.RS_Recent_Sextype+": {0}", Keyed.Sextype[(int)sextype]), p / 2, HistoryUtility.SextypeColor[(int)sextype], Texture2D.blackTexture, null, String.Format("{0:P2}", p));
            listmain.Gap(1f);

            if (history.IncestuousCount < history.CorpseFuckCount)
            {
                p = history.CorpseFuckCount;
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_Necrophile + ": {0}", p), p / 50, HistoryUtility.Nurgle, Texture2D.blackTexture);
                listmain.Gap(1f);
            }
            else
            {
                p = history.IncestuousCount;
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.Incest + ": {0}", p), p / 50, HistoryUtility.Nurgle, Texture2D.blackTexture);
                listmain.Gap(1f);
            }

            p = pawn.records.GetValue(VariousDefOf.AmountofEatenCum);
            FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_Cum_Swallowed + ": {0:0.00} mL, {1} " + Keyed.RS_NumofTimes, p, pawn.records.GetValue(VariousDefOf.NumofEatenCum)), p / 1000, Texture2D.linearGrayTexture, Texture2D.blackTexture);
            listmain.Gap(1f);

            Hediff addiction = pawn.health.hediffSet.GetFirstHediffOfDef(VariousDefOf.CumAddiction);
            if (addiction != null)
            {
                p = addiction.Severity;
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_CumAddiction + ": {0:P2}", p), p, Texture2D.linearGrayTexture, Texture2D.blackTexture, Keyed.RS_CumAddiction_Tooltip);
                listmain.Gap(1f);
            }
            else if ((addiction = pawn.health.hediffSet.GetFirstHediffOfDef(VariousDefOf.CumTolerance)) != null)
            {
                p = addiction.Severity;
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_CumAddictiveness + ": {0:P}", p), p, Texture2D.linearGrayTexture, Texture2D.blackTexture, Keyed.RS_CumAddictiveness_Tooltip);
                listmain.Gap(1f);
            }
            else
            {
                listmain.GetRect(FONTHEIGHT);
                listmain.Gap(1f);
            }


            //listmain.GetRect(FONTHEIGHT);
            //listmain.Gap(1f);

            p = history.RapedCount;
            tmp = listmain.GetRect(FONTHEIGHT);
            if (p < history.BeenRapedCount)
            {
                p = history.BeenRapedCount;
                FillableBarLabeled(tmp, String.Format(Keyed.RS_BeenRaped + ": {0}", p), p / 50, Texture2D.grayTexture, Texture2D.blackTexture, null, String.Format(xxx.vulnerability_stat.LabelCap.CapitalizeFirst() + ": {0:P2}", pawn.GetStatValue(xxx.vulnerability_stat)));
                listmain.Gap(1f);
            }
            else
            {
                FillableBarLabeled(tmp, String.Format(Keyed.RS_RapedSomeone + ": {0}", p), p / 50, HistoryUtility.Khorne, Texture2D.blackTexture, null, String.Format(xxx.vulnerability_stat.LabelCap.CapitalizeFirst() + ": {0:P2}", pawn.GetStatValue(xxx.vulnerability_stat)));
                listmain.Gap(1f);
            }
            if (Mouse.IsOver(tmp))
            {
                TooltipHandler.TipRegion(tmp, RJWUIUtility.GetStatExplanation(pawn, xxx.vulnerability_stat, pawn.GetStatValue(xxx.vulnerability_stat)));
            }


            p = pawn.GetStatValue(xxx.sex_satisfaction);
            tmp = listmain.GetRect(FONTHEIGHT);
            FillableBarLabeled(tmp, String.Format(xxx.sex_satisfaction.LabelCap.CapitalizeFirst() + ": {0:P2}", p), p/2, HistoryUtility.Satisfaction, Texture2D.blackTexture);
            listmain.Gap(1f);
            if (Mouse.IsOver(tmp))
            {
                TooltipHandler.TipRegion(tmp, RJWUIUtility.GetStatExplanation(pawn, xxx.sex_satisfaction, pawn.GetStatValue(xxx.sex_satisfaction)));
            }

            //p = pawn.GetStatValue(xxx.vulnerability_stat);
            //FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(xxx.vulnerability_stat.LabelCap.CapitalizeFirst() + ": {0:P2}", p), p / 2, HistoryUtility.Khorne, Texture2D.blackTexture, xxx.vulnerability_stat.description);
            //listmain.Gap(1f);

            SkillRecord skill = pawn.skills?.GetSkill(VariousDefOf.SexSkill);
            p = skill?.Level ?? 0;
            tmp = listmain.GetRect(FONTHEIGHT);
            FillableBarLabeled(tmp, String.Format(Keyed.RS_SexSkill + ": {0}, {1:P2}", p, skill?.xpSinceLastLevel/ skill?.XpRequiredForLevelUp), p / 20, HistoryUtility.Tzeentch, Texture2D.blackTexture, null, String.Format(xxx.sex_stat.LabelCap.CapitalizeFirst() + ": {0:P2}", pawn.GetStatValue(xxx.sex_stat)), HistoryUtility.PassionBG[(int)(skill?.passion ?? 0)]);
            if (Mouse.IsOver(tmp))
            {
                TooltipHandler.TipRegion(tmp, RJWUIUtility.GetStatExplanation(pawn, xxx.sex_stat, pawn.GetStatValue(xxx.sex_stat)));
            }


            listmain.Gap(1f);

            if (selectedPawn != null) DrawSexInfoCard(listmain.GetRect(CARDHEIGHT), selectedPawn, Keyed.RS_Selected_Partner, Keyed.RS_Selected_Partner);
            else DrawExtraInfo(listmain.GetRect(CARDHEIGHT));
            
            listmain.End();
        }

        protected void DrawExtraInfo(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inRect = rect.ContractedBy(4f);
            Listing_Standard listmain = new Listing_Standard();
            listmain.Begin(inRect);
            listmain.Gap(4f);
            listmain.GetRect(FONTHEIGHT).DrawSexuality(rjwcomp);
            listmain.Gap(1f);
            listmain.GetRect(FONTHEIGHT).DrawQuirk(pawn);
            listmain.Gap(1f);



            listmain.End();


        }

        

        /// <summary>
        /// Left section
        /// </summary>
        protected void DrawBaseSexInfoLeft(Rect rect)
        {

            Listing_Standard listmain = new Listing_Standard();
            listmain.Begin(rect);
            float p;

            //Sex statistics
            GUI.Label(listmain.GetRect(FONTHEIGHT), " " + Keyed.RS_Statistics, fontstyleleft);
            listmain.Gap(1f);
            for (int i = 0; i < Sextype.Length; i++)
            {
                int sexindex = Sextype[i];
                p = history.GetAVGSat(sexindex) / BASESAT;
                string label = Keyed.RS_Sex_Info(Keyed.Sextype[sexindex], history.GetSexCount(sexindex).ToString());
                FillableBarLabeled(listmain.GetRect(FONTHEIGHT),label, p / 2, HistoryUtility.SextypeColor[sexindex], Texture2D.blackTexture, null, Keyed.RS_SAT_AVG(String.Format("{0:P2}", p)));
                listmain.Gap(1f);
            }

            p = history.PartnerCount;
            FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_Sex_Partners + ": {0} ({1})", p, pawn.records.GetValue(VariousDefOf.SexPartnerCount)), p / 50, HistoryUtility.Partners, Texture2D.blackTexture);
            listmain.Gap(1f);

            p = history.VirginsTaken;
            FillableBarLabeled(listmain.GetRect(FONTHEIGHT), String.Format(Keyed.RS_VirginsTaken + ": {0:0}", p), p / 100, HistoryUtility.Partners, Texture2D.blackTexture);
            listmain.Gap(1f);
            
            //Partner list
            GUI.Label(listmain.GetRect(FONTHEIGHT)," "+Keyed.RS_PartnerList, fontstyleleft);
            listmain.Gap(1f);

            Rect scrollRect = listmain.GetRect(CARDHEIGHT+1f);
            GUI.Box(scrollRect,"", buttonstyle);
            List<SexHistory> partnerList = history.PartnerList;
            Rect listRect = new Rect(scrollRect.x, scrollRect.y, LISTPAWNSIZE * partnerList.Count, scrollRect.height - 30f);
            Widgets.BeginScrollView(scrollRect, ref scroll, listRect);
            Widgets.ScrollHorizontal(scrollRect, ref scroll, listRect);
            DrawPartnerList(listRect, partnerList);
            Widgets.EndScrollView();

            listmain.End();
        }

        protected void DrawPartnerList(Rect rect, List<SexHistory> partnerList)
        {
            Rect pawnRect = new Rect(rect.x, rect.y, LISTPAWNSIZE, LISTPAWNSIZE);
            for (int i = 0; i < partnerList.Count; i++)
            {
                Rect labelRect = new Rect(pawnRect.x,pawnRect.yMax - FONTHEIGHT,pawnRect.width,FONTHEIGHT);

                DrawPawn(pawnRect, partnerList[i]);
                Widgets.DrawHighlightIfMouseover(pawnRect);
                GUI.Label(labelRect, partnerList[i].Label, fontstylecenter);
                if (Widgets.ButtonInvisible(pawnRect))
                {
                    selectedPawn = partnerList[i];
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                if (partnerList[i] == selectedPawn)
                {
                    Widgets.DrawHighlightSelected(pawnRect);
                }

                pawnRect.x += LISTPAWNSIZE;
            }
        }

        protected void DrawPawn(Rect rect, SexHistory history)
        {
            if (history != null)
            {
                bool drawheart = false;
                Rect iconRect = new Rect(rect.x + rect.width * 3 / 4, rect.y, rect.width / 4, rect.height / 4);
                Texture img = HistoryUtility.UnknownPawn;
                if (history.Partner != null)
                {
                    img = PortraitsCache.Get(history.Partner, rect.size, Rot4.South, default, 1, true, true, false, false);
                    if (history.IamFirst)
                    {
                        GUI.color = HistoryUtility.HistoryColor;
                        Widgets.DrawTextureFitted(rect, HistoryUtility.FirstOverlay, 1.0f);
                        GUI.color = Color.white;
                    }
                    
                    drawheart = LovePartnerRelationUtility.LovePartnerRelationExists(pawn, history.Partner);

                }

                if (history.Incest)
                {
                    Widgets.DrawTextureFitted(iconRect, HistoryUtility.Incest, 1.0f);
                    iconRect.x -= iconRect.width;
                }
                Widgets.DrawTextureFitted(rect, img, 1.0f);
                if (drawheart)
                {
                    Widgets.DrawTextureFitted(iconRect, HistoryUtility.Heart,1.0f);
                    iconRect.x -= iconRect.width;
                }
            }
        }

       


        public static void FillableBarLabeled(Rect rect, string label, float fillPercent, Texture2D filltexture, Texture2D bgtexture, string tooltip = null, string rightlabel = "", Texture2D border = null)
        {
            Widgets.FillableBar(rect, Math.Min(fillPercent, 1.0f), filltexture, bgtexture, true);
            GUI.Label(rect, "  " + label.CapitalizeFirst(), fontstyleleft);
            GUI.Label(rect, rightlabel.CapitalizeFirst() + "  ", fontstyleright);
            Widgets.DrawHighlightIfMouseover(rect);
            if (tooltip != null) TooltipHandler.TipRegion(rect, tooltip);
            if (border != null)
            {
                rect.DrawBorder(border,2f);
            }
            
        }

    }


}
