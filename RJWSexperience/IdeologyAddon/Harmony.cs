using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using Verse;


namespace RJWSexperience.Ideology
{

    [StaticConstructorOnStartup]
    internal static class First
    {
        static First()
        {
            var har = new Harmony("RJW_Sexperience.Ideology");
            har.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
