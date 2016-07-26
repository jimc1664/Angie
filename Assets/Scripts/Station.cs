using UnityEngine;
using System.Collections.Generic;
using Sim;


public class Station : Body {

    public float Power = 50, PowerGain = 1, MaxPower = 100;
    public float Minerals = 50, MineralsGain = 0, MaxMinerals = 100;

    public MasterAi Controller;
    public StarSystem Sys;

    public bool Inited = false;

   // public Area Ar;

   public override void init() {
        Inited = true;
        Sys = GetComponentInParent<StarSystem>();
       // Ar = GetComponentInParent<Area>();
        foreach(var f in GetComponentsInChildren<Facility>()) {
            switch(f.Type) {
                case Facility_E.Ai:
                    if(Controller != null) Debug.LogError("ai");
                    var go = new GameObject("AnAi");
                    Controller = go.AddComponent<MasterAi>();
                    Controller.init(this);
                    break;
                case Facility_E.DryDock:

                    DryDocks.Add( f.gameObject.AddComponent<DryDock>() );

                    break;

            }
        }

        var ar = GetComponentInParent<Area>();
        Trnsfrm = transform;
        var sim = Simulation.Singleton;
        Bdy = new Sim.Body() {
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

        float incRate = Time.deltaTime *5;

        Power += PowerGain * incRate;
        Minerals += MineralsGain * incRate;

        if(Power > MaxPower * 0.9) {
            float rate = 0.7f, eff = 0.05f;
            float mat = Mathf.Min(MaxMinerals - Minerals, rate * incRate );
            Power -= mat / eff;
            Minerals += mat;
        }

        if(Minerals > 30) {

            for(int i = DryDocks.Count; i-- > 0;) {
                var dd = DryDocks[Random.Range(0, DryDocks.Count)];
                if(dd.InConstruction == null) {

                    var md = (Instantiate(Spawnables.Singleton.MiningShip, dd.transform.position, dd.transform.rotation ) as GameObject).GetComponent<MiningDrone>();
                    dd.InConstruction = md;
                    dd.Eta = 5.0f;
                    Minerals -= 30;
                    
                    break;
                }
            }

        }

        if(Power > MaxPower) Power = MaxPower;
        if(Minerals > MaxMinerals) Minerals = MaxMinerals;

        /*
        if(Power > 30) {
            Power -= 30;
        } */


        if(Ws.Count > 0) {

            var ws = Ws[0];
            ws.Eta -= Time.deltaTime;
            if(ws.Eta < 0) {
                /*Vector3 vec = ws.To.transform.position - ws.Dr.Ar.transform.position;
                ws.Dr.initPos =ws.To.transform.position + -vec.normalized * ws.To.Radius;
                ws.Dr.initVel = vec.normalized *ws.Dr.MaxVel *0.2f;
                ws.Dr.initRot = Quaternion.LookRotation(vec.normalized, ws.Dr.fd(0).Rot * Vector3.up);
                ws.Dr.initAVel = Quaternion.identity;
                ws.Dr.Ar = ws.To;
                ws.Dr.WarpTo = null;
                
                */
                
                ws.Dr.WarpFrom = ws.Dr.Ar;
                ws.Dr.WarpTo = ws.To;
                var sim = Simulation.Singleton;
                //sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = sim.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });

                Debug.Log("warp " + ws.To);
                ws.Dr._Ar.Drones.Remove(ws.Dr);
                ws.Dr._Ar = null;
                ws.Dr._State = Drone.StateT.Warping;
                Simulation.Singleton.WarpingDrones.Add(ws.Dr);
                ws.Dr.WarpRdy = 1;
                Ws.RemoveAt(0);
            }
            Debug.DrawLine(ws.Dr.fd(0).Pos, ws.To.transform.position, Color.green);
        }
    }

    [System.Serializable]
    public class WarpScedhule {
        public Drone Dr;
        public Area To;
        public float Eta; 
    }
    public List<WarpScedhule> Ws;

    public void warp(Drone drone, Area ar) {
        Ws.Add( new WarpScedhule() { Eta = 2.0f, Dr = drone, To = ar } );
    }

    
}
