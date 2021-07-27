using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;


namespace RJWSexperience
{
    public static class Keyed
    {
        public static string LustStatFactor(float value) => "LustStatFactor".Translate(value);
        public static string SlaveStatFactor(float value) => "SlaveStatFactor".Translate(value);
        public static string MemeStatFactor(float value) => "MemeStatFactor".Translate(value);

        public static readonly string SlaveStatFactorDefault = "SlaveStatFactorDefault".Translate();

    }
}
