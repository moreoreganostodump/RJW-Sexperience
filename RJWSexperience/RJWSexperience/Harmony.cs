using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Verse;
using HarmonyLib;


namespace RJWSexperience
{
    [StaticConstructorOnStartup]
    internal static class First
    {
        static First()
        {
            var har = new Harmony("RJW_Sexperience");
            har.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
