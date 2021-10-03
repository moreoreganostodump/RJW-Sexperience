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
    public class SexPartnerHistory : ThingComp
    {
        public SexPartnerHistory() { }
        public const int ARRLEN = 20;

        //protected List<SexHistory> histories = new List<SexHistory>();
        protected Dictionary<string, SexHistory> histories = new Dictionary<string, SexHistory>();
        protected string first = "";
        protected bool dirty = true;
        protected xxx.rjwSextype recentsex = xxx.rjwSextype.None;
        protected float recentsat = 0;
        protected string recentpartner = "";
        protected int[] sextypecount = new int[ARRLEN];
        protected float[] sextypesat = new float[ARRLEN];
        protected int[] sextyperecenttickabs = new int[ARRLEN];
        protected int virginstaken = 0;
        protected int incestuous = 0;
        protected int bestiality = 0;
        protected int corpsefuck = 0;
        protected int interspecies = 0;
        protected int firstsextickabs = 0;

        protected string mostpartnercache = "";
        protected xxx.rjwSextype mostsextypecache = xxx.rjwSextype.None;
        protected xxx.rjwSextype mostsatsextypecache = xxx.rjwSextype.None;
        protected xxx.rjwSextype bestsextypecache = xxx.rjwSextype.None;
        protected float bestsextypesatcache = 0;
        protected string bestpartnercache = "";
        protected int totalsexcache = 0;
        protected int totalrapedcache = 0;
        protected int totalbeenrapedcache = 0;
        protected ThingDef preferracecache = null;
        protected int preferracesexcountcache = 0;
        protected Pawn preferracepawncache = null;
        protected float avtsatcache = 0;
        protected int recentsextickabscache = 0;
        protected int mostsextickabscache = 0;
        protected int bestsextickabscache = 0;


        private List<SexHistory> partnerlistcache;
        private List<int> sextypecountsave;
        private List<float> sextypesatsave;
        private List<int> sextyperecenttickabssave;


        public SexHistory GetFirstPartnerHistory
        {
            get
            {
                Update();
                return histories.TryGetValue(first);
            }
        }
        public SexHistory GetMostPartnerHistory
        {
            get
            {
                Update();
                return histories.TryGetValue(mostpartnercache);
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
        public SexHistory GetRecentPartnersHistory
        {
            get
            {
                return histories.TryGetValue(recentpartner);
            }
        }
        public SexHistory GetBestSexPartnerHistory
        {
            get
            {
                Update();
                SexHistory history = histories.TryGetValue(bestpartnercache);
                return history;
            }
        }
        public float TotalSexHad
        {
            get
            {
                Update();
                return totalsexcache;
            }
        }
        public int VirginsTaken
        {
            get
            {
                return virginstaken;
            }
        }
        public List<SexHistory> PartnerList
        {
            get
            {
                Update();
                return partnerlistcache;
            }
        }
        public int PartnerCount
        {
            get
            {
                if (histories == null) histories = new Dictionary<string, SexHistory>();
                return histories.Count;
            }
        }
        public int IncestuousCount
        {
            get
            {
                return incestuous;
            }
        }
        public int RapedCount
        {
            get
            {
                Update();
                return totalrapedcache;
            }
        }
        public int BeenRapedCount
        {
            get
            {
                Update();
                return totalbeenrapedcache;
            }
        }
        public ThingDef PreferRace
        {
            get
            {
                Update();
                return preferracecache;
            }
        }
        public int PreferRaceSexCount
        {
            get
            {
                Update();
                return preferracesexcountcache;
            }
        }
        public int BestialityCount
        {
            get
            {
                return bestiality;
            }
        }
        public int CorpseFuckCount
        {
            get
            {
                return corpsefuck;
            }
        }
        public int InterspeciesCount
        {
            get
            {
                return interspecies;
            }
        }
        public float AVGSat
        {
            get
            {
                Update();
                if (totalsexcache == 0) return 0;
                return sextypesat.Sum() / totalsexcache;
            }
        }
        public int RecentSexElapsedTicks
        {
            get
            {
                return GenTicks.TicksAbs - recentsextickabscache;
            }
        }
        public string RecentSexDays
        {
            get
            {
                if (recentsextickabscache != 0) return GenDate.ToStringTicksToDays(RecentSexElapsedTicks) + " " + Keyed.RS_Ago;
                return "";
            }
        }
        public int FirstSexElapsedTicks
        {
            get
            {
                return GenTicks.TicksAbs - firstsextickabs;
            }
        }
        public string FirstSexDays
        {
            get
            {
                if (firstsextickabs != 0) return GenDate.ToStringTicksToDays(FirstSexElapsedTicks) + " " + Keyed.RS_Ago;
                return "";
            }
        }
        public int MostSexElapsedTicks
        {
            get
            {
                return GenTicks.TicksAbs - mostsextickabscache;
            }
        }
        public string MostSexDays
        {
            get
            {
                if (mostsextickabscache != 0) return GenDate.ToStringTicksToDays(MostSexElapsedTicks) + " " + Keyed.RS_Ago;
                return "";
            }
        }
        public int BestSexElapsedTicks
        {
            get
            {
                return GenTicks.TicksAbs - bestsextickabscache;
            }
        }
        public string BestSexDays
        {
            get
            {
                if (bestsextickabscache != 0) return GenDate.ToStringTicksToDays(BestSexElapsedTicks) + " " + Keyed.RS_Ago;
                return "";
            }
        }


        public Texture GetPreferRaceIcon(Vector2 size)
        {
            Update();
            if (preferracepawncache != null) return PortraitsCache.Get(preferracepawncache, size, Rot4.South, default, 1, true, true, false, false);
            else return HistoryUtility.UnknownPawn;

        }

        public float GetBestSextype(out xxx.rjwSextype sextype)
        {
            if (dirty) Update();
            sextype = bestsextypecache;
            return bestsextypesatcache;
        }

        public float GetRecentSextype(out xxx.rjwSextype sextype)
        {
            if (dirty) Update();
            sextype = recentsex;
            return recentsat;
        }

        public string SextypeRecentDays(int sextype)
        {
            int index = (int)sextype;
            if (sextyperecenttickabs[index] != 0) return GenDate.ToStringTicksToDays(GenTicks.TicksAbs - sextyperecenttickabs[index]) + " " + Keyed.RS_Ago;
            return Keyed.Unknown;
        }

        public SexHistory this[Pawn pawn]
        {
            get
            {
                return histories.TryGetValue(pawn.ThingID);
            }
        } 

        public float GetAVGSat(xxx.rjwSextype sextype)
        {
            int index = (int)sextype;
            return GetAVGSat(index);
        }

        public float GetAVGSat(int index)
        {
            float res = sextypesat[index] / sextypecount[index];
            return float.IsNaN(res) ? 0f : res;
        }

        public int GetSexCount(int index)
        {
            return sextypecount[index];
        }



        public override void PostExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                sextypecountsave = sextypecount.ToList();
                sextypesatsave = sextypesat.ToList();
                sextyperecenttickabssave = sextyperecenttickabs.ToList();
            }

            Scribe_Collections.Look(ref histories, "histories", LookMode.Value, LookMode.Deep);
            Scribe_Values.Look(ref first, "first", "", true);
            Scribe_Values.Look(ref recentsex, "recentsex", recentsex, true);
            Scribe_Values.Look(ref recentsat, "recentsat", recentsat, true);
            Scribe_Values.Look(ref recentpartner, "recentpartner", recentpartner, true);
            Scribe_Values.Look(ref virginstaken, "virginstaken", virginstaken, true);
            Scribe_Values.Look(ref incestuous, "incestous", incestuous, true);
            Scribe_Values.Look(ref bestiality, "bestiality", bestiality, true);
            Scribe_Values.Look(ref corpsefuck, "corpsefuck", corpsefuck, true);
            Scribe_Values.Look(ref interspecies, "interspecies", interspecies, true);
            Scribe_Values.Look(ref firstsextickabs, "firstsextickabs", firstsextickabs, true);
            Scribe_Collections.Look(ref sextypecountsave, "sextypecountsave", LookMode.Value);
            Scribe_Collections.Look(ref sextypesatsave, "sextypesatsave", LookMode.Value);
            Scribe_Collections.Look(ref sextyperecenttickabssave, "sextyperecenttickabssave", LookMode.Value);
            //Scribe_Values.Look(ref sextypecount, "sextypecount", new int[ARRLEN], true); // not work
            //Scribe_Values.Look(ref sextypesat, "sextypesat", new float[ARRLEN], true);
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                sextypecount = sextypecountsave?.ToArray() ?? new int[ARRLEN];
                sextypesat = sextypesatsave?.ToArray() ?? new float[ARRLEN];
                sextyperecenttickabs = sextyperecenttickabssave?.ToArray() ?? new int[ARRLEN];
            }

            if (histories == null) histories = new Dictionary<string, SexHistory>();

            base.PostExposeData();
        }

        public void RecordHistory(Pawn partner, SexProps props)
        {
            Pawn pawn = parent as Pawn;
            TryAddHistory(partner);
            RecordFirst(partner, props);
            recentpartner = partner.ThingID;
            SexHistory history = histories[partner.ThingID];
            history?.RecordSex(props);
            recentsex = props.sexType;
            sextypecount[(int)props.sexType]++;
            sextyperecenttickabs[(int)props.sexType] = GenTicks.TicksAbs;
            if (partner.IsIncest(pawn)) incestuous++;
            if (partner.Dead) corpsefuck++;
            if (props.IsBestiality()) bestiality++;
            else if (pawn.def != partner.def) interspecies++;
            dirty = true;
        }

        public void RecordSatisfactionHistory(Pawn partner, SexProps props, float satisfaction)
        {
            TryAddHistory(partner);
            RecordFirst(partner, props);
            SexHistory history = histories[partner.ThingID];
            history?.RecordSatisfaction(props, satisfaction);
            recentsat = satisfaction;
            sextypesat[(int)props.sexType] += satisfaction;
            dirty = true;
        }

        protected bool TryAddHistory(Pawn partner)
        {
            if (!histories.ContainsKey(partner.ThingID))
            {
                SexHistory newhistory = new SexHistory(partner,partner.IsIncest(parent as Pawn));
                histories.Add(partner.ThingID, newhistory);
                Pawn pawn = parent as Pawn;
                if (pawn != null)
                {
                    pawn.records.AddTo(VariousDefOf.SexPartnerCount, 1);
                }
                return true;
            }
            return false;
        }

        public void RecordFirst(Pawn partner, SexProps props)
        {
            if (VirginCheck() && props.sexType == xxx.rjwSextype.Vaginal)
            {
                TryAddHistory(partner);
                first = partner.ThingID;
                SexPartnerHistory history = partner.GetPartnerHistory();
                firstsextickabs = GenTicks.TicksAbs;
                if (history != null)
                {
                    history.TakeSomeonesVirgin(parent as Pawn);
                }

            }
        }

        public void TakeSomeonesVirgin(Pawn partner)
        {
            TryAddHistory(partner);
            SexHistory history = histories[partner.ThingID];
            if (history != null) history.TookVirgin();
            virginstaken++;
        }

        protected void Update()
        {
            if (dirty)
            {
                UpdateStatistics();
                UpdateBestSex();
                UpdatePartnerList();
                dirty = false;
            }
        }

        protected void UpdateStatistics()
        {
            int max = 0;
            float maxsat = 0;
            float maxf = 0;
            int maxindex = 0;
            string mostID = Keyed.Unknown;
            string bestID = Keyed.Unknown;
            
            totalsexcache = 0;
            totalrapedcache = 0;
            totalbeenrapedcache = 0;
            Dictionary<ThingDef, int> racetotalsat = new Dictionary<ThingDef, int>();
            List<Pawn> allpartners = new List<Pawn>();

            foreach (KeyValuePair<string,SexHistory> element in histories)
            {
                SexHistory h = element.Value;

                //find most sex partner
                if (max < h.TotalSexCount)
                {
                    max = h.TotalSexCount;
                    mostID = element.Key;
                }
                if (maxsat < h.BestSatisfaction)
                {
                    maxsat = h.BestSatisfaction;
                    bestID = element.Key;
                }

                if (h.Partner != null)
                {
                    Pawn partner = h.Partner;
                    allpartners.Add(partner);
                    if (racetotalsat.ContainsKey(partner.def))
                    {
                        racetotalsat[partner.def] += h.TotalSexCount - h.RapedMe;
                    }
                    else
                    {
                        racetotalsat.Add(partner.def, h.TotalSexCount - h.RapedMe);
                    }
                }

                totalsexcache += h.TotalSexCount;
                totalrapedcache += h.Raped;
                totalbeenrapedcache += h.RapedMe;
            }

            if (!racetotalsat.NullOrEmpty())
            {
                KeyValuePair<ThingDef, int> prefer = racetotalsat.MaxBy(x => x.Value);
                preferracecache = prefer.Key;
                preferracesexcountcache = prefer.Value;
                preferracepawncache = allpartners.FirstOrDefault(x => x.def == preferracecache);
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
            bestpartnercache = bestID;

            recentsextickabscache = histories.TryGetValue(recentpartner)?.RecentSexTickAbs ?? 0;
            mostsextickabscache = histories.TryGetValue(mostpartnercache)?.RecentSexTickAbs ?? 0;
            bestsextickabscache = histories.TryGetValue(bestpartnercache)?.BestSexTickAbs ?? 0;

            racetotalsat.Clear();
            allpartners.Clear();
        }

        protected void UpdateBestSex()
        {
            int bestindex = 0;
            float bestsat = 0;
            float avgsat;
            for(int i=0; i< sextypecount.Length; i++)
            {
                avgsat = sextypesat[i] / sextypecount[i];
                if (bestsat < avgsat)
                {
                    bestindex = i;
                    bestsat = avgsat;
                }
            }
            bestsextypecache = (xxx.rjwSextype)bestindex;
            bestsextypesatcache = bestsat;
        }

        protected void UpdatePartnerList()
        {
            if (partnerlistcache == null) partnerlistcache = new List<SexHistory>();
            partnerlistcache.Clear();
            if (!histories.NullOrEmpty()) foreach (SexHistory history in histories.Values)
                {
                    if (history != null) partnerlistcache.Add(history);
                }
        }

        protected bool VirginCheck()
        {
            if (histories.TryGetValue(first) != null) return false;

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
        protected Pawn partner = null;
        protected string namecache;
        protected int totalsexhad = 0;
        protected int raped = 0;
        protected int rapedme = 0;
        protected int orgasms = 0;
        protected xxx.rjwSextype bestsextype = xxx.rjwSextype.None;
        protected float bestsatisfaction = 0;
        protected bool itookvirgin = false;
        protected bool incest = false;
        protected int recentsextickabs = 0;
        protected int bestsextickabs = 0;

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
        public Pawn Partner
        {
            get
            {
                return partner;
            }
        }
        public string RapeInfo
        {
            get
            {
                string res = "";
                if (raped > 0) res += Keyed.RS_Raped + raped + " ";
                if (rapedme > 0) res += Keyed.RS_RapedMe + rapedme + " ";
                return res;
            }
        }
        public int OrgasmCount
        {
            get
            {
                return orgasms;
            }
        }
        public bool IamFirst
        {
            get
            {
                return itookvirgin;
            }
        }
        public bool Incest
        {
            get
            {
                return incest;
            }
        }
        public int Raped
        {
            get
            {
                return raped;
            }
        }
        public int RapedMe
        {
            get
            {
                return rapedme;
            }
        }
        public int RecentSexTickAbs
        {
            get
            {
                return recentsextickabs;
            }
        }
        public int BestSexTickAbs
        {
            get
            {
                return bestsextickabs;
            }
        }
        public int BestSexElapsedTicks
        {
            get
            {
                return GenTicks.TicksAbs - bestsextickabs;
            }
        }
        public string BestSexDays
        {
            get
            {
                if (bestsextickabs != 0) return Keyed.RS_HadBestSexDaysAgo(GenDate.ToStringTicksToDays(BestSexElapsedTicks) + " " + Keyed.RS_Ago);
                return "";
            }
        }
        public SexHistory() { }

        public SexHistory(Pawn pawn, bool incest = false)
        {
            this.partner = pawn; 
            this.namecache = pawn.Label;
            this.incest = incest;
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
            Scribe_Values.Look(ref itookvirgin, "itookvirgin", itookvirgin, true);
            Scribe_Values.Look(ref incest, "incest", incest, true);
            Scribe_Values.Look(ref recentsextickabs, "recentsextickabs", recentsextickabs, true);
            Scribe_Values.Look(ref bestsextickabs, "bestsextickabs", bestsextickabs, true);
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
            recentsextickabs = GenTicks.TicksAbs;
            
        }

        public void RecordSatisfaction(SexProps props, float satisfaction)
        {
            if (satisfaction > bestsatisfaction)
            {
                orgasms++;
                bestsextype = props.sexType;
                bestsatisfaction = satisfaction;
                bestsextickabs = GenTicks.TicksAbs;
            }
        }

        public void TookVirgin()
        {
            itookvirgin = true;
        }
    }

}
