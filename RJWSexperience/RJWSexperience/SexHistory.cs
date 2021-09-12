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
        //protected List<SexHistory> histories = new List<SexHistory>();
        protected Dictionary<string,SexHistory> histories = new Dictionary<string,SexHistory>();
        protected string first = "";
        protected bool dirty = true;
        protected string bestaffinity = "";
        protected float bestaffinitysat = 0;
        protected xxx.rjwSextype recentsex = xxx.rjwSextype.None;
        protected string recentpartner = "";

        protected string mostpartnercache = "";
        protected xxx.rjwSextype mostsextypecache = xxx.rjwSextype.None;

        public string FirstSexInfo
        {
            get
            {
                return
                    "Partner: " + histories[first]?.Label ?? "Unknown" +
                    "" +
                    "" +
                    "";
                
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
            base.PostExposeData();
        }

        public void RecordHistory(Pawn partner, SexProps props)
        {
            TryAddHistory(partner);
            SexHistory history = histories[partner.ThingID];
            history?.RecordSex(props);
            dirty = true;
        }

        public void RecordSatisfactionHistory(Pawn partner, SexProps props, float satisfaction)
        {
            TryAddHistory(partner);
            SexHistory history = histories[partner.ThingID];
            history?.RecordSatisfaction(props, satisfaction);

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
            dirty = false;
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
