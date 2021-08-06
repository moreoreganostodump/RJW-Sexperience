using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;




namespace RJWSexperience
{
    public class Configurations : ModSettings
    {
        public const float MaxInitialLustDefault = 500;
        public const float AvgLustDefault = 0;
        public const float MaxSexCountDeviationDefault = 90;
        public const float LustEffectPowerDefault = 0.5f;
        public const float SexPerYearDefault = 30;
        public const bool SlavesBeenRapedExpDefault = true;
        public const bool EnableStatRandomizerDefault = true;
        public const float LustLimitDefault = 500f/3f;

        public static float MaxLustDeviation = MaxInitialLustDefault;
        public static float AvgLust = AvgLustDefault;
        public static float MaxSexCountDeviation = MaxSexCountDeviationDefault;
        public static float LustEffectPower = LustEffectPowerDefault;
        public static float SexPerYear = SexPerYearDefault;
        public static bool SlavesBeenRapedExp = SlavesBeenRapedExpDefault;
        public static bool EnableRecordRandomizer = EnableStatRandomizerDefault;
        public static float LustLimit = LustLimitDefault;

        public static void ResettoDefault()
        {
            MaxLustDeviation = MaxInitialLustDefault;
            AvgLust = AvgLustDefault;
            MaxSexCountDeviation = MaxSexCountDeviationDefault;
            LustEffectPower = LustEffectPowerDefault;
            SexPerYear = SexPerYearDefault;
            SlavesBeenRapedExp = SlavesBeenRapedExpDefault;
            EnableRecordRandomizer = EnableStatRandomizerDefault;
            LustLimit = LustLimitDefault;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref MaxLustDeviation, "MaxLustDeviation", MaxLustDeviation, true);
            Scribe_Values.Look(ref AvgLust, "AvgLust", AvgLust, true);
            Scribe_Values.Look(ref MaxSexCountDeviation, "MaxSexCountDeviation", MaxSexCountDeviation, true);
            Scribe_Values.Look(ref LustEffectPower, "LustEffectPower", LustEffectPower, true);
            Scribe_Values.Look(ref SexPerYear, "SexPerYear", SexPerYear, true);
            Scribe_Values.Look(ref SlavesBeenRapedExp, "SlavesBeenRapedExp", SlavesBeenRapedExp, true);
            Scribe_Values.Look(ref EnableRecordRandomizer, "EnableRecordRandomizer", EnableRecordRandomizer, true);
            Scribe_Values.Look(ref LustLimit, "LustLimit", LustLimit, true);
            base.ExposeData();
        }
    }

    public class RJWSexperience : Mod
    {
        private readonly Configurations config;
        private static Vector2 scroll;

        public RJWSexperience(ModContentPack content) : base(content)
        {
            config = GetSettings<Configurations>();
        }

        public override string SettingsCategory()
        {
            return Keyed.Mod_Title;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            int Adjuster;
            float fAdjuster;
            Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
            Rect mainRect = new Rect(0f, 0f, inRect.width - 30f, inRect.height + 480f);
            Listing_Standard listmain = new Listing_Standard();
            listmain.maxOneColumn = true;
            Widgets.BeginScrollView(outRect, ref scroll, mainRect);
            listmain.Begin(mainRect);
            listmain.Gap(20f);


            LabelwithTextfield(listmain.GetRect(24f), Keyed.Option_2_Label + " x" + Configurations.LustEffectPower, Keyed.Option_2_Desc, ref Configurations.LustEffectPower, 0f, 100f);
            Adjuster = (int)(Configurations.LustEffectPower * 1000);
            //listmain.Label(Keyed.Option_2_Label + " x" + Configurations.LustEffectPower , -1, Keyed.Option_2_Desc);
            Adjuster = (int)listmain.Slider(Adjuster, 0, 2000);
            Configurations.LustEffectPower = (float)Adjuster / 1000;

            fAdjuster = Configurations.LustLimit * 3;
            LabelwithTextfield(listmain.GetRect(24f), Keyed.Option_8_Label + " " + fAdjuster, Keyed.Option_8_Desc, ref fAdjuster, 0, 10000f);
            fAdjuster = (int)listmain.Slider(fAdjuster, 0, 1000);
            Configurations.LustLimit = fAdjuster / 3;

            listmain.CheckboxLabeled(Keyed.Option_1_Label, ref Configurations.EnableRecordRandomizer, Keyed.Option_1_Desc);
            if (Configurations.EnableRecordRandomizer)
            {
                Listing_Standard section = listmain.BeginSection(24f*9f);


                LabelwithTextfield(section.GetRect(24f), Keyed.Option_3_Label + " " + Configurations.MaxLustDeviation, Keyed.Option_3_Label, ref Configurations.MaxLustDeviation, 0f, 2000f);
                Adjuster = (int)Configurations.MaxLustDeviation;
                //listmain.Label(Keyed.Option_3_Label + " " + Configurations.MaxLustDeviation, -1, Keyed.Option_3_Desc);
                Adjuster = (int)section.Slider(Adjuster, 0, 2000);
                Configurations.MaxLustDeviation = Adjuster;

                LabelwithTextfield(section.GetRect(24f), Keyed.Option_4_Label + " " + Configurations.AvgLust, Keyed.Option_4_Desc, ref Configurations.AvgLust, -1000f, 1000f);
                Adjuster = (int)Configurations.AvgLust;
                //listmain.Label(Keyed.Option_4_Label + " " + Configurations.AvgLust, -1, Keyed.Option_4_Desc);
                Adjuster = (int)section.Slider(Adjuster, -1000, 1000);
                Configurations.AvgLust = Adjuster;


                LabelwithTextfield(section.GetRect(24f), Keyed.Option_5_Label + " " + Configurations.MaxSexCountDeviation, Keyed.Option_5_Desc, ref Configurations.MaxSexCountDeviation, 0f, 2000f);
                Adjuster = (int)Configurations.MaxSexCountDeviation;
                //listmain.Label(Keyed.Option_5_Label + " " + Configurations.MaxSexCountDeviation, -1, Keyed.Option_5_Desc);
                Adjuster = (int)section.Slider(Adjuster, 0, 2000);
                Configurations.MaxSexCountDeviation = Adjuster;

                LabelwithTextfield(section.GetRect(24f), Keyed.Option_6_Label + " " + Configurations.SexPerYear, Keyed.Option_6_Desc, ref Configurations.SexPerYear, 0f, 2000f);
                Adjuster = (int)Configurations.SexPerYear;
                //listmain.Label(Keyed.Option_6_Label + " " + Configurations.SexPerYear, -1, Keyed.Option_6_Desc);
                Adjuster = (int)section.Slider(Adjuster, 0, 2000);
                Configurations.SexPerYear = Adjuster;


                section.CheckboxLabeled(Keyed.Option_7_Label, ref Configurations.SlavesBeenRapedExp, Keyed.Option_7_Desc);

                listmain.EndSection(section);
            }


            if (listmain.ButtonText("reset to default"))
            {
                Configurations.ResettoDefault();
            }
            listmain.End();
            Widgets.EndScrollView();

        }

        public void LabelwithTextfield(Rect rect, string label, string tooltip, ref float value, float min, float max)
        {
            Rect textfieldRect = new Rect(rect.xMax - 100f, rect.y, 100f, rect.height);
            string valuestr = value.ToString();
            Widgets.Label(rect, label);
            Widgets.TextFieldNumeric(textfieldRect,ref value, ref valuestr, min, max);
            Widgets.DrawHighlightIfMouseover(rect);
            TooltipHandler.TipRegion(rect, tooltip);
        }


    }

}
