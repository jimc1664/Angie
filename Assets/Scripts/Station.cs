using UnityEngine;
using System.Collections.Generic;
using Sim;

namespace Sim {



    public class Resource {
        //public string Name;
        enum ResType {
            EnergyStorage,
            Energy,
            Mineral,

            Mining, 
            Defence,
        };


        public float Amnt; 
    };

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

        public float PowerProjected, PowerGain, Minerals, PowerMod, MineralsGain, MaxMinerals;

        public struct DryDock {
            public Drone InConstruction;
            public float Eta;

            public Vector3 Pos;
            public Quaternion Rot;
        };

        public DryDock DD;

        public void updateSt(ref Simulation.FrameCntx cntx) {
            float incRate = cntx.Delta;
            var h = Host as global::Station;

            PowerStatus = PowerStatusE.Balenced;
            if(PowerProjected > MaxPower * 0.9f) {
                PowerStatus = PowerStatusE.Excess;
            } else if(PowerProjected < MaxPower * 0.2f) {
                PowerStatus = PowerStatusE.Emergency;
            } else {
                if(PowerMet.Avg > PowerGain * 0.6f) {
                    PowerStatus = PowerStatusE.Surplus;
                } else if(PowerMet.Avg < -PowerGain * 0.2f) {
                    PowerStatus = PowerStatusE.Deficit;
                } else {
                    float prj = Power + PowerMod * 15.0f;
                    if(prj > MaxPower * 0.6f) {
                        PowerStatus = PowerStatusE.Positive;
                    } else if(prj < MaxPower * 0.4 ) {
                        PowerStatus = PowerStatusE.Negative;
                    }
                }
            }

            float op = Power;
            Power += PowerGain * incRate;
            Minerals += MineralsGain * incRate;

            if( PowerStatus >= PowerStatusE.Excess ) {
                float eff = 0.05f, rate = PowerGain * incRate * eff;
                float mat = Mathf.Min(MaxMinerals - Minerals, rate );
                Power -= mat / eff;
                Minerals += mat;
            }

            if(Minerals > 30 && PowerStatus >= PowerStatusE.Positive) {

                //for(int i = DryDocks.Count; i-- > 0;) {
                    //var dd = dd DryDocks[Random.Range(0, DryDocks.Count)];
                if(DD.InConstruction == null) {
                    global::Drone md;
                    /*if(MinerCnt < 1 || FighterCnt >= MinerCnt)
                        md = (GameObject.Instantiate(Spawnables.Singleton.MiningShip, DD.Pos, DD.Rot ) as GameObject).GetComponent<global::Drone>();
                    else
                        md = (GameObject.Instantiate(Spawnables.Singleton.FighterDrn, DD.Pos, DD.Rot) as GameObject).GetComponent<global::Drone>();*/

                    bool miner = MinerCnt < 1 || FighterCnt >= MinerCnt;

                    miner = false;
                    if(miner)
                        md = Spawnables.Singleton.MiningShip.GetComponent<global::Drone>();
                    else
                        md = Spawnables.Singleton.FighterDrn.GetComponent<global::Drone>();

                    //md.transform.parent = Ar.transform;
                    //  md.Drn.Owner = Sm.Owner;
                    var sim = Simulation.Singleton;
                    DD.InConstruction = md.buildSimDrone(DD.Pos, DD.Rot, this, null );

                    DD.Eta = 5.0f;
                    Minerals -= 30;
                    Power -= 10;
                    // break;
                }
               // }

            }

            
            foreach(var d in Dependants) {
                float oh = 0.2f, r = 0.9f;
                if(d.Ar != Ar) {
                    oh *= 3;
                    r = 0.7f;
                }
                d.Power -= oh * cntx.Delta;

                float t = Mathf.Min(3 * cntx.Delta, (d.MaxPower - d.Power) / r );
                Power -= t;
                d.Power += t * r;
            }

            // for(int i = DryDocks.Count; i-- > 0;) {
            //       var dd = DryDocks[Random.Range(0, DryDocks.Count)];
            if(DD.InConstruction != null) {
                DD.Eta -= cntx.Delta;
                if(DD.Eta < 0) {
                    DD.InConstruction.initState = Drone.StateT.Idle;

                    DD.Eta = 0;
                    DD.InConstruction = null;
                }
                Power -= 5 * cntx.Delta;
            }
            // }


            if(Warping == null) {
                if(PowerStatus >= PowerStatusE.Emergency) {
                    for(int i = 0; i < Ws.Count; i++ ) {
                        var ws = Ws[i];
                        if(PowerStatus >= PowerStatusE.Balenced || Ws[i].To == Ar) {

                            if(ws.Dr.WarpRdy < 0.7f) continue;
                            ws.Wh.State = Wormhole.StateE.Forming;
                            ws.Wh.StateMd = 0;

                            var whh = ws.Wh.Host as global::Wormhole;
                            if(whh != null)
                                whh.Vis.gameObject.SetActive(true);

                            // Debug.Log("start warp " + Ws[i].Dr.Host + "   to " + Ws[i].To + "   from " + Ws[i].Dr.Ar);
                            Warping = Ws[i];
                            Ws.RemoveAt(i);                         
                            break;
                        }
                    }
                }
            } else {
                var ws = Warping;

                if(ws.Dr == null || ws.Dr.Wormhole != ws.Wh ) {
                    ws.Wh.deform();
                    Warping = null;
                } else {
                    switch(ws.Wh.State) {
                        case Wormhole.StateE.Forming:
                            ws.Wh.StateMd += cntx.Delta;
                            Power -= 5 * cntx.Delta;
                            if(ws.Wh.StateMd >= 1) {
                                ws.Wh.State = Wormhole.StateE.Waiting;
                                ws.Wh.StateMd = 0;
                            }
                            break;
                        case Wormhole.StateE.Waiting:
                            Power -= 2 * cntx.Delta;
                            if(ws.Dr.WarpRdy > 0.92f) {
                                ws.Wh.StateMd += cntx.Delta * 2;
                                if(ws.Wh.StateMd >= 1) {
                                    ws.Wh.State = Wormhole.StateE.Warping;
                                    ws.Wh.StateMd = 0;
                                }
                            } else {
                                ws.Wh.StateMd -= cntx.Delta * 5;
                                if(ws.Wh.StateMd < 0) ws.Wh.StateMd = 0;
                            }
                            break;
                        case Wormhole.StateE.Warping:
                            Power -= 2 * cntx.Delta;
                            if(ws.Dr.WarpRdy > 0.99f) {
                                ws.Wh.StateMd += cntx.Delta * 2;
                                if(ws.Wh.StateMd >= 1) {

                                    ws.Wh.State = Wormhole.StateE.Deforming;
                                    ws.Wh.StateMd = 0;
                                    ws.Dr.Wormhole = null;

                                    ws.Dr.WarpFrom = ws.Dr.Ar;
                                    ws.Dr.WarpTo = ws.To;
                                    var sim = Simulation.Singleton;
                                    //sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = sim.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });

                                    //  Debug.Log("warp " + ws.Dr.Host+ "   to " + ws.To + "   from " + ws.Dr.Ar);
                                    ws.Dr.Ar.remDrone(ws.Dr);
                                    ws.Dr._State = Drone.StateT.Warping;
                                    Simulation.Singleton.WarpingDrones.Add(ws.Dr);
                                    ws.Dr.WarpRdy = 0;
                                    ws.Dr.Ws = Warping = null;
                                }
                            }
                            break;
                    }
                }
            }

            if(Power > MaxPower) Power = MaxPower;
            if(Minerals > MaxMinerals) Minerals = MaxMinerals;

            PowerMet.step((Power - op) / cntx.Delta );
            PowerMod = PowerMet.Avg;
            PowerProjected = Power + PowerMod * 5.0f;

            //Dependants = Dependants;
        }

        [System.Serializable]
        public class WarpScedhule {
            public Wormhole Wh;
            public Drone Dr;
            public Area To;
           // public float Eta;
        }
        public List<WarpScedhule> Ws = new List<WarpScedhule>();
        WarpScedhule Warping;

        public WarpScedhule requestWarp( Drone drone, Area ar) {
            var ws = new WarpScedhule() { Wh= drone.Wormhole, Dr = drone, To = ar };
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
  //  public MasterAi Ai;
    public StarSystem Sys;

    [System.NonSerialized]
    public bool Inited = false;

   // [System.NonSerialized]
    public Sim.Station Sm;
    // public Area Ar;

    public override void init() {
        if(Inited) return;
       // Debug.Log("init Station " + name);
        Inited = true;
        Sys = GetComponentInParent<StarSystem>();
       // Ar = GetComponentInParent<Area>();
        

        var ar = GetComponentInParent<Area>();
        Trnsfrm = transform;
        var sim = Simulation.Singleton;
        Bdy = Sm = ar.St = new Sim.Station() {
            Owner = Owner,
            name =name,
            /*ArriveEp = ArriveEp,
            MaxVel = MaxVel * sim.Glob_KineticScale,
            MaxSteer = MaxSteer * sim.Glob_KineticScale * sim.Glob_AccelScale,
            MaxAVel = MaxAVel * sim.Glob_KineticScale * sim.Glob_RotScale,
            MaxASteer = MaxASteer * sim.Glob_KineticScale * sim.Glob_AccelScale * sim.Glob_RotScale,
            */
            Rad = Rad,

            Power= Power,
            MaxPower = MaxPower,
            PowerProjected = PowerProjected,
            PowerGain = PowerGain,
            Minerals = Minerals,
            PowerMod = PowerMod,
            MineralsGain = MineralsGain,
            MaxMinerals = MaxMinerals,

            initPos = Trnsfrm.localPosition,
            initVel = Vector3.zero,
            initRot = Trnsfrm.localRotation,
            initAVel = Quaternion.identity,
            //_Ar = ar,
            Host = this,
        };
        ar.addBody(Bdy);




        foreach(var f in GetComponentsInChildren<Facility>()) {
            switch(f.Type) {
                case Facility_E.Ai:
                    if(Sm.Ai != null) Debug.LogError("ai");
                    var go = new GameObject("AnAi");
                    Sm.Ai = go.AddComponent<MasterAi>();
                    Sm.Ai.init(this);
                    break;
                case Facility_E.DryDock:

                    //DryDocks.Add(f.gameObject.AddComponent<DryDock>());
                    //var dd = GetComponentInChildren<DryDock>();
                    //Sm.DD.Pos = f.transform.localPosition;
                    Sm.DD.Pos = transform.InverseTransformPoint(f.transform.position);
                    Sm.DD.Rot = f.transform.rotation * Quaternion.Inverse(  transform.rotation );
                    break;

            }
        }

       // Debug.Log("init Station " + name +"  h "+ ar.St.Host);
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


    void OnDrawGizmos() {

        

                    Trnsfrm = transform;

        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Trnsfrm.position, WarpRange);

    }

}
