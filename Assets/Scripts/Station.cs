using UnityEngine;
using System.Collections.Generic;
using Sim;

namespace Sim {

    public struct Metric {
        public Metric(int smpls) {
            Sample = smpls;
            Dat = new float[Sample];
            Total = Avg= 0;
            Iter = 0;
        }
        public void step(float v) {
            Total += v - Dat[Iter];
            Dat[Iter] = v;
            Iter = (Iter + 1) % Sample;
            Avg = Total / Sample;
        }
        float[] Dat;
        int Sample, Iter;
        float Total;
        public float Avg;
    };


    [System.Serializable]
    public class Station : Entity {
        public Metric PowerMet = new Metric(16);


        public enum PowerStatusE {
            Emergency,
            Deficit,
            Negative,
            Balenced,
            Positive,
            Surplus,
            Excess,
        };

        public PowerStatusE PowerStatus = PowerStatusE.Balenced;
        public int MinerCnt, FighterCnt;

        //[System.NonSerialized]
        public List<Drone> Dependants = new List<Drone>();

        public void updateSt(ref Simulation.FrameCntx cntx) {
            float incRate = cntx.Delta;
            var h = Host as global::Station;

            PowerStatus = PowerStatusE.Balenced;
            if(h.PowerProjected > h.MaxPower * 0.9f) {
                PowerStatus = PowerStatusE.Excess;
            } else if(h.PowerProjected < h.MaxPower * 0.2f) {
                PowerStatus = PowerStatusE.Emergency;
            } else {
                if(PowerMet.Avg > h.PowerGain * 0.6f) {
                    PowerStatus = PowerStatusE.Surplus;
                } else if(PowerMet.Avg < -h.PowerGain * 0.2f) {
                    PowerStatus = PowerStatusE.Deficit;
                } else {
                    float prj = h.Power + h.PowerMod * 15.0f;
                    if(prj > h.MaxPower * 0.6f) {
                        PowerStatus = PowerStatusE.Positive;
                    } else if(prj < h.MaxPower * 0.4 ) {
                        PowerStatus = PowerStatusE.Negative;
                    }
                }
            }

            float op = h.Power;
            h.Power += h.PowerGain * incRate;
            h.Minerals += h.MineralsGain * incRate;

            if( PowerStatus >= PowerStatusE.Excess ) {
                float eff = 0.05f, rate = h.PowerGain * incRate * eff;
                float mat = Mathf.Min(h.MaxMinerals - h.Minerals, rate );
                h.Power -= mat / eff;
                h.Minerals += mat;
            }

            if(h.Minerals > 30 && PowerStatus >= PowerStatusE.Positive) {

                for(int i = h.DryDocks.Count; i-- > 0;) {
                    var dd = h.DryDocks[Random.Range(0, h.DryDocks.Count)];
                    if(dd.InConstruction == null) {

                        MiningDrone md;
                        if(MinerCnt < 1 || FighterCnt >= MinerCnt)
                            md = (GameObject.Instantiate(Spawnables.Singleton.MiningShip, dd.transform.position, dd.transform.rotation) as GameObject).GetComponent<MiningDrone>();
                        else
                            md = (GameObject.Instantiate(Spawnables.Singleton.FighterDrn, dd.transform.position, dd.transform.rotation) as GameObject).GetComponent<MiningDrone>();
                        //  md.Drn.Owner = Sm.Owner;
                        dd.InConstruction = md;
                        dd.Eta = 5.0f;
                        h.Minerals -= 30;
                        h.Power -= 10;
                        break;
                    }
                }

            }

            
            foreach(var d in Dependants) {
                float oh = 0.2f, r = 0.9f;
                if(d.Ar != Ar) {
                    oh *= 3;
                    r = 0.7f;
                }
                d.Power -= oh * cntx.Delta;

                float t = Mathf.Min(3 * cntx.Delta, (d.MaxPower - d.Power) / r );
                h.Power -= t;
                d.Power += t * r;
            }

            for(int i = h.DryDocks.Count; i-- > 0;) {
                var dd = h.DryDocks[Random.Range(0, h.DryDocks.Count)];
                if(dd.InConstruction != null) {
                    h.Power -= 5 * cntx.Delta;
                }
            }


            if(Warping == null) {
                if(PowerStatus >= PowerStatusE.Emergency) {
                    for(int i = 0; i < Ws.Count; i++ ) {
                        var ws = Ws[i];
                        if(PowerStatus >= PowerStatusE.Balenced || Ws[i].To == Ar) {

                            if(ws.Dr.WarpRdy < 0.92f) continue;

                           // Debug.Log("start warp " + Ws[i].Dr.Host + "   to " + Ws[i].To + "   from " + Ws[i].Dr.Ar);
                            Warping = Ws[i];
                            Ws.RemoveAt(i);


                            break;
                        }
                    }
                }
            } else {
                var ws = Warping;
                ws.Eta -= cntx.Delta;
                h.Power -= 5 * cntx.Delta;
                if(ws.Eta <= 0) {
                    ws.Dr.WarpFrom = ws.Dr.Ar;
                    ws.Dr.WarpTo = ws.To;
                    var sim = Simulation.Singleton;
                    //sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = sim.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });

                   //  Debug.Log("warp " + ws.Dr.Host+ "   to " + ws.To + "   from " + ws.Dr.Ar);
                    ws.Dr.Ar.remDrone( ws.Dr );
                    ws.Dr._State = Drone.StateT.Warping;
                    Simulation.Singleton.WarpingDrones.Add(ws.Dr);
                    ws.Dr.WarpRdy = 0;
                    ws.Dr.Ws = Warping = null;      
                             
                }
            }

            if(h.Power > h.MaxPower) h.Power = h.MaxPower;
            if(h.Minerals > h.MaxMinerals) h.Minerals = h.MaxMinerals;

            PowerMet.step((h.Power - op) / cntx.Delta );
            h.PowerMod = PowerMet.Avg;
            h.PowerProjected = h.Power + h.PowerMod * 5.0f;

            //h.Dependants = Dependants;
        }

        [System.Serializable]
        public class WarpScedhule {
            
            public Drone Dr;
            public Area To;
            public float Eta;
        }
        public List<WarpScedhule> Ws = new List<WarpScedhule>();
        WarpScedhule Warping;
        public WarpScedhule requestWarp( Drone drone, Area ar) {
            var ws = new WarpScedhule() { Eta = 2.0f, Dr = drone, To = ar };
            Ws.Add( ws );
            return ws;
        }

    }
}
public class Station : Body {
   // public List<Sim.Drone> Dependants = new List<Drone>();
    public Faction Owner;

    public float Power = 50, PowerGain = 1, MaxPower = 100, PowerMod, PowerProjected;
    public float Minerals = 50, MineralsGain = 0, MaxMinerals = 100;

    public float WarpRange = 10;
    public MasterAi Ai;
    public StarSystem Sys;

    [System.NonSerialized]
    public bool Inited = false;

   // [System.NonSerialized]
    public Sim.Station Sm;
    // public Area Ar;

    public override void init() {
        if(Inited) return;
        Debug.Log("init Station " + name);
        Inited = true;
        Sys = GetComponentInParent<StarSystem>();
       // Ar = GetComponentInParent<Area>();
        

        var ar = GetComponentInParent<Area>();
        Trnsfrm = transform;
        var sim = Simulation.Singleton;
        Bdy = Sm = ar.St = new Sim.Station() {
            Owner = Owner,

            /*ArriveEp = ArriveEp,
            MaxVel = MaxVel * sim.Glob_KineticScale,
            MaxSteer = MaxSteer * sim.Glob_KineticScale * sim.Glob_AccelScale,
            MaxAVel = MaxAVel * sim.Glob_KineticScale * sim.Glob_RotScale,
            MaxASteer = MaxASteer * sim.Glob_KineticScale * sim.Glob_AccelScale * sim.Glob_RotScale,
            */
            Rad = Rad,

            initPos = Trnsfrm.position,
            initVel = Vector3.zero,
            initRot = Trnsfrm.rotation,
            initAVel = Quaternion.identity,
            _Ar = ar,
            Host = this,
        };
        ar.Bodies.Add(Bdy);

        foreach(var f in GetComponentsInChildren<Facility>()) {
            switch(f.Type) {
                case Facility_E.Ai:
                    if(Ai != null) Debug.LogError("ai");
                    var go = new GameObject("AnAi");
                    Ai = go.AddComponent<MasterAi>();
                    Ai.init(this);
                    break;
                case Facility_E.DryDock:

                    DryDocks.Add(f.gameObject.AddComponent<DryDock>());

                    break;

            }
        }

        Debug.Log("init Station " + name +"  h "+ ar.St.Host);
    }

    void Start() {
     //   if(!Inited) init();      
        
        //float highThresh = 
        //float highPowerAt = (MaxPower -Power) 
    }

    public enum Facility_E {
        PowerCore,
        Ai,
        Conveyor,
        Materialiser,
        Workshop, 
        Storage,
        SystemScanner,
        Docking,
        DryDock,
    };
    public List<Facility_E> Facilities;

    public List<DryDock> DryDocks;

    void Update() {


        if(Sm != null && Sm.Ws.Count > 0) {

            var ws = Sm.Ws[0];
            Debug.DrawLine(ws.Dr.fd(0).Pos, ws.To.transform.position, Color.green);
        }
    }

    void OnDrawGizmos() {

        

                    Trnsfrm = transform;

        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Trnsfrm.position, WarpRange);

    }

}
