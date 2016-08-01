using UnityEngine;
using System.Collections;

public class MiningDrone : Body {



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

    public bool Fighter = false;

    void Awake() {
    //    Trnsfrm = transform;
    }

    public override void init() {
        var ar = GetComponentInParent<Sim.Area>();

        var st = ar.GetComponentInChildren<Station>();
        st.init();
        init(st);
    }
    public void init( Station st )  {
       // Debug.Log(" st  " + st + " st.Sm.Host  " + st.Sm.Host + "   st.Owner  " + st.Owner);
        Debug.Assert(ReferenceEquals((st.Sm.Host as Station), st ));
        Drn.St = st.Sm;
        Drn.Owner = st.Owner;
        Drn.St.Dependants.Add(Drn);

        Drn.Ai = st.Ai;
        Drn.Target = null;
        

        if(Fighter) {
            Drn.St.FighterCnt++;
            Drn._Behavior = Sim.Drone.BehaviorT.Hunt;
        }  else
            Drn.St.MinerCnt++;

        st.Bdy.Ar.addDrone(Drn);

    }
    void OnEnable() {

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


            MaxPower = 50, Power = 10,
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

    void OnDrawGizmos() {
        Trnsfrm = transform;
        
        Gizmos.color = Color.green;
       // if(Target != null)
        //    Gizmos.DrawLine(Trnsfrm.position, Target.position);

        Gizmos.color = Color.blue;
       // Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + Vel );
        

        var rot = Trnsfrm.rotation;
        Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one).transpose;


        Vector3 desVel = Vector3.zero;
        Quaternion desAvel = Quaternion.identity;
        Vector3 desDir = rotM.GetRow(2);

        /*
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + desDir);

        
        Vector3 yAx = Vector3.Cross(desDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), desDir);
        yAx = Vector3.Cross(desDir, xAx);


        Gizmos.color = Color.green;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + yAx);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + xAx);
        */

        Gizmos.color = Color.white;
        if( Drn != null && Drn.Owner)  Gizmos.color = Drn.Owner.Col;
        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);
    }


}
