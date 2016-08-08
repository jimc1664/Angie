using UnityEngine;
using System.Collections;

public class PlayerShipCtrlr : Body {

    public CameraControl CamCntrl;

    public Transform Target;
    public Station St;

    public Vector3 Vel;
    public Quaternion AVel = Quaternion.identity;


    public float MaxVel = 5;
    public float MaxSteer = 0.5f;


    public float MaxAVel = 5;
    public float MaxASteer = 0.5f;

    public float ArriveEp = 0.5f;

    // public float Rad = 0.2f;

    public Sim.Drone Drn;

    void Awake() {
        //    Trnsfrm = transform;
    }

    public override void init() {
        var ar = GetComponentInParent<Sim.Area>();

        var st = ar.GetComponentInChildren<Station>();
        st.init();
        init(st);
    }
    public void init(Station st) {
        // Debug.Log(" st  " + st + " st.Sm.Host  " + st.Sm.Host + "   st.Owner  " + st.Owner);
        Debug.Assert(ReferenceEquals((st.Sm.Host as Station), st));
        Drn.St = st.Sm;
        Drn.Owner = st.Owner;
        Drn.St.Dependants.Add(Drn);

        Drn.Ai = st.Ai;
        Drn.Target = null;

        Drn._Behavior = Sim.Drone.BehaviorT.Player;

        st.Bdy.Ar.addDrone(Drn);
    }
    void OnEnable() {

        CamCntrl = FindObjectOfType<CameraControl>();
        Trnsfrm = transform;
        var sim = Simulation.Singleton;
        Drn = new Sim.Drone() {
            ArriveEp = ArriveEp,
            MaxVel = MaxVel * sim.Glob_KineticScale,
            MaxSteer = MaxSteer * sim.Glob_KineticScale * sim.Glob_AccelScale,
            MaxAVel = MaxAVel * sim.Glob_KineticScale * sim.Glob_RotScale,
            MaxASteer = MaxASteer * sim.Glob_KineticScale * sim.Glob_AccelScale * sim.Glob_RotScale,

            Rad = Rad,

            initPos = Trnsfrm.position,
            initVel = Vel,
            initRot = Trnsfrm.rotation,
            initAVel = AVel,
            Host = this,


            MaxPower = 50,
            Power = 10,
        };

    }

    void Update() {

        if(Drn != null) {
            var fd = Drn.fdSmooth(Simulation.Singleton);

            Trnsfrm.position = fd.Pos;
            Trnsfrm.rotation = fd.Rot;
            Vel = fd.Vel;
            AVel = fd.AVel;


        }
    }

  

}
