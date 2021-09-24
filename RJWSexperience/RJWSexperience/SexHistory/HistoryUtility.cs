using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;
using UnityEngine;

namespace RJWSexperience
{
    [StaticConstructorOnStartup]
    public static class HistoryUtility
    {
        public static readonly Texture2D HistoryIcon = ContentFinder<Texture2D>.Get("UI/buttons/OpenStatsReport");
        public static readonly Texture2D UnknownPawn = ContentFinder<Texture2D>.Get("UI/Icon/UnknownPawn");
        public static readonly Texture2D FirstOverlay = ContentFinder<Texture2D>.Get("UI/Icon/FirstBG");
        public static readonly Texture2D Heart = ContentFinder<Texture2D>.Get("Things/Mote/Heart");
        public static readonly Texture2D Incest = ContentFinder<Texture2D>.Get("UI/Icon/Incest");
        public static readonly Texture2D Locked = ContentFinder<Texture2D>.Get("UI/Icon/RSLocked");
        public static readonly Texture2D Unlocked = ContentFinder<Texture2D>.Get("UI/Icon/RSUnlocked");
        public static Texture2D Slaanesh = SolidColorMaterials.NewSolidColorTexture(0.686f, 0.062f, 0.698f, 1.0f);
        public static Texture2D Khorne = SolidColorMaterials.NewSolidColorTexture(0.415f, 0.0f, 0.003f, 1.0f);
        public static Texture2D Tzeentch = SolidColorMaterials.NewSolidColorTexture(0.082f, 0.453f, 0.6f, 1.0f);
        public static Texture2D Nurgle = SolidColorMaterials.NewSolidColorTexture(0.6f, 0.83f, 0.35f, 1.0f);
        public static Texture2D Partners = SolidColorMaterials.NewSolidColorTexture(0.843f, 0.474f, 0.6f, 1.0f);
        public static Texture2D TotalSex = SolidColorMaterials.NewSolidColorTexture(0.878f, 0.674f, 0.411f, 1.0f);
        public static Texture2D Satisfaction = SolidColorMaterials.NewSolidColorTexture(0.325f, 0.815f, 0.729f,1.0f);
        public static readonly Color HistoryColor = new Color(0.9f,0.5f,0.5f);

        public static readonly Texture2D[] SextypeColor = new Texture2D[]
        {
            Texture2D.linearGrayTexture,    //None = 0,
            SolidColorMaterials.NewSolidColorTexture(0.900f, 0.500f, 0.500f, 1.0f),    //Vaginal = 1,
            SolidColorMaterials.NewSolidColorTexture(0.529f, 0.313f, 0.113f, 1.0f),    //Anal = 2,
            SolidColorMaterials.NewSolidColorTexture(0.529f, 0.113f, 0.305f, 1.0f),    //Oral = 3,
            SolidColorMaterials.NewSolidColorTexture(0.000f, 0.819f, 0.219f, 1.0f),    //Masturbation = 4,
            SolidColorMaterials.NewSolidColorTexture(0.000f, 0.560f, 0.090f, 1.0f),    //DoublePenetration 
            SolidColorMaterials.NewSolidColorTexture(0.839f, 0.850f, 0.505f, 1.0f),    //Boobjob = 6,
            SolidColorMaterials.NewSolidColorTexture(0.858f, 0.886f, 0.113f, 1.0f),    //Handjob = 7,
            SolidColorMaterials.NewSolidColorTexture(0.752f, 0.780f, 0.000f, 1.0f),    //Footjob = 8,
            SolidColorMaterials.NewSolidColorTexture(0.484f, 0.500f, 0.241f, 1.0f),    //Fingering = 9,
            SolidColorMaterials.NewSolidColorTexture(0.913f, 0.909f, 0.909f, 1.0f),    //Scissoring = 10,
            SolidColorMaterials.NewSolidColorTexture(0.588f, 0.576f, 0.431f, 1.0f),    //MutualMasturbation
            SolidColorMaterials.NewSolidColorTexture(0.741f, 0.000f, 0.682f, 1.0f),    //Fisting = 12,
            SolidColorMaterials.NewSolidColorTexture(0.121f, 0.929f, 1.000f, 1.0f),    //MechImplant = 13,
            SolidColorMaterials.NewSolidColorTexture(0.478f, 0.274f, 0.160f, 1.0f),    //Rimming = 14,
            SolidColorMaterials.NewSolidColorTexture(0.819f, 0.301f, 0.552f, 1.0f),    //Fellatio = 15,
            SolidColorMaterials.NewSolidColorTexture(0.819f, 0.301f, 0.552f, 1.0f),    //Cunnilingus = 16,
            SolidColorMaterials.NewSolidColorTexture(0.529f, 0.113f, 0.305f, 1.0f),    //Sixtynine = 17
            Texture2D.linearGrayTexture,    //? = 18
            Texture2D.linearGrayTexture,    //? = 19
            Texture2D.linearGrayTexture    //? = 20
        };

        public static readonly Texture2D[] PassionBG = new Texture2D[]
        {
            
            Texture2D.blackTexture,    //None = 0,
            SolidColorMaterials.NewSolidColorTexture(0.800f, 0.800f, 0.800f, 1.0f),    //Minor = 1,
            SolidColorMaterials.NewSolidColorTexture(1.000f, 0.875f, 0.000f, 1.0f)     //Major = 2,
        };

        public static SexPartnerHistory GetPartnerHistory(this Pawn pawn)
        {
            return pawn.TryGetComp<SexPartnerHistory>();
        }



    }
}
