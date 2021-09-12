using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using rjw;


namespace RJWSexperience
{
    public class SexPartnerHistory : ThingComp
    {
        public SexPartnerHistory() { }

        
        //protected List<SexHistory> histories = new List<SexHistory>();
        protected Dictionary<string,SexHistory> histories = new Dictionary<string,SexHistory>();
        protected string first = "";
        protected bool dirty = true;
        protected string bestaffinity = "";
        protected float bestaffinitysat = 0;
        protected xxx.rjwSextype recentsex = xxx.rjwSextype.None;
        protected string recentpartner = "";
        protected int[] sextypecount = new int[20];
        protected float[] sextypesat = new float[20];

        protected string mostpartnercache = "";
        protected xxx.rjwSextype mostsextypecache = xxx.rjwSextype.None;
        protected xxx.rjwSextype mostsatsextypecache = xxx.rjwSextype.None;


        public string FirstSexInfo
        {
            get
            {
                Update();
                return
                    "Partner: " + histories.TryGetValue(first)?.Label ?? "Unknown" +
                    "";
            }
        }
        public string MostSexPartner
        {
            get
            {
                Update();
                return histories.TryGetValue(mostpartnercache)?.Label ?? "Unknown";
            }
        }
        public xxx.rjwSextype MostSextype
        {
            get
            {
                Update();
                return mostsextypecache;
            }
        }
        public xxx.rjwSextype MostSatisfiedSex
        {
            get
            {
                Update();
                return mostsatsextypecache;
            }
        }



        public override void PostExposeData()
        {
            Scribe_Collections.Look(ref histories, "histories", LookMode.Deep);
            Scribe_Values.Look(ref first, "first", "", true);
            Scribe_Values.Look(ref bestaffinity, "bestaffinity", "", true);
            Scribe_Values.Look(ref bestaffinitysat, "bestaffinitysat", bestaffinitysat, true);
            Scribe_Values.Look(ref recentsex, "recentsex", recentsex, true);
            Scribe_Values.Look(ref recentpartner, "recentpartner", recentpartner, true);
            Scribe_Values.Look(ref sextypecount, "sextypecount", sextypecount, true);
            Scribe_Values.Look(ref sextypesat, "sextypesat", sextypesat, true);
            base.PostExposeData();
        }

        public void RecordHistory(Pawn partner, SexProps props)
        {
            TryAddHistory(partner);
            recentpartner = partner.ThingID;
            SexHistory history = histories[partner.ThingID];
            history?.RecordSex(props);
            recentsex = props.sexType;
            sextypecount[(int)props.sexType]++;

            dirty = true;
        }

        public void RecordSatisfactionHistory(Pawn partner, SexProps props, float satisfaction)
        {
            TryAddHistory(partner);
            RecordFirst(partner, props);
            SexHistory history = histories[partner.ThingID];
            history?.RecordSatisfaction(props, satisfaction);
            sextypesat[(int)props.sexType] += satisfaction;
            dirty = true;
        }

        protected bool TryAddHistory(Pawn partner)
        {
            if (!histories.ContainsKey(partner.ThingID))
            {
                histories.Add(partner.ThingID, new SexHistory(partner));
                Pawn pawn = parent as Pawn;
                if (pawn != null)
                {
                    pawn.records.AddTo(VariousDefOf.SexPartnerCount, 1);
                }
                return true;
            }
            return false;
        }

        protected void RecordFirst(Pawn partner, SexProps props)
        {
            if (VirginCheck() && props.sexType == xxx.rjwSextype.Vaginal)
            {
                first = partner.ThingID;
            }
        }

        protected void Update()
        {
            if (dirty)
            {
                UpdateStatistics();
                dirty = false;
            }
        }

        protected void UpdateStatistics()
        {
            int max = 0;
            float maxf = 0;
            int maxindex = 0;
            string mostID = "Unknown";

            foreach (KeyValuePair<string,SexHistory> element in histories)
            {
                SexHistory h = element.Value;

                //find most sex partner
                if (max < h.TotalSexCount)
                {
                    max = h.TotalSexCount;
                    mostID = element.Key;
                }
            }

            max = 0;
            for (int i=0; i < sextypecount.Length; i++)
            {
                float avgsat = sextypesat[i] / sextypecount[i];
                if (maxf < avgsat)
                {
                    maxf = avgsat;
                    maxindex = i;
                }
            }

            mostsatsextypecache = (xxx.rjwSextype)maxindex;
            mostsextypecache = (xxx.rjwSextype)sextypecount.FirstIndexOf(x => x == sextypecount.Max());
            mostpartnercache = mostID;
        }

        protected bool VirginCheck()
        {
            Pawn pawn = parent as Pawn;
            if (pawn != null)
            {
                if (pawn.IsVirgin()) return true;
            }
            return false;
        }

    }


    public class SexHistory : IExposable
    {
        protected Pawn partner;
        protected string namecache;
        protected int totalsexhad = 0;
        protected int raped = 0;
        protected int rapedme = 0;
        protected int orgasms = 0;
        protected xxx.rjwSextype bestsextype = xxx.rjwSextype.None;
        protected float bestsatisfaction = 0; 
       


        public string Label
        {
            get
            {
                if (partner != null)
                {
                    namecache = partner.Label;
                    return namecache;
                }
                else return namecache;
            }
        }
        public xxx.rjwSextype BestSextype
        {
            get
            {
                return bestsextype;
            }
        }
        public float BestSatisfaction
        {
            get
            {
                return bestsatisfaction;
            }
        }
        public int TotalSexCount
        {
            get
            {
                return totalsexhad;
            }
        }


        public SexHistory() { }

        public SexHistory(Pawn pawn)
        {
            partner = pawn; 
            namecache = pawn.Label;
        }


        public void ExposeData()
        {
            Scribe_References.Look(ref partner, "partner", true);
            Scribe_Values.Look(ref namecache, "namecache", namecache, true);
            Scribe_Values.Look(ref totalsexhad, "totalsexhad", totalsexhad, true);
            Scribe_Values.Look(ref raped, "raped", raped, true);
            Scribe_Values.Look(ref rapedme, "rapedme", rapedme, true);
            Scribe_Values.Look(ref orgasms, "orgasms", orgasms, true);
            Scribe_Values.Look(ref bestsextype, "bestsextype", bestsextype, true);
            Scribe_Values.Look(ref bestsatisfaction, "bestsatisfaction", bestsatisfaction, true);
        }

        public void RecordSex(SexProps props)
        {
            totalsexhad++;
            if (props.isRape)
            {
                if (partner == props.giver)
                {
                    rapedme++;
                }
                else if (partner == props.reciever)
                {
                    raped++;
                }
            }
        }

        public void RecordSatisfaction(SexProps props, float satisfaction)
        {
            if (satisfaction > bestsatisfaction)
            {
                orgasms++;
                bestsextype = props.sexType;
                bestsatisfaction = satisfaction;
            }
        }


    }

}
