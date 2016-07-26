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

    public bool Enemy = false;

    void Awake() {
    //    Trnsfrm = transform;
    }

    public override void init() {
        Drn._Ar = GetComponentInParent<Area>();

        var st = Drn.Ar.GetComponentInChildren<Station>();
        st.Bdy._Ar = Drn._Ar;
        init(st);
    }
    public void init( Station st )  {

        Drn.St = st;
        Drn._Ar = st.Bdy.Ar;
        Drn._Ar.Drones.Add(Drn);
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

        };

    }

    void Update() {

        if(Drn != null) {
            var fd = Drn.fdSmooth(Simulation.Singleton);

            Trnsfrm.position = fd.Pos;
            Trnsfrm.rotation = fd.Rot;
            Vel = fd.Vel;
            AVel = fd.AVel;

            if(Enemy) {
                

            }
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

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + desDir);

        
        Vector3 yAx = Vector3.Cross(desDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), desDir);
        yAx = Vector3.Cross(desDir, xAx);


        Gizmos.color = Color.green;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + yAx);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + xAx);


        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);
    }


}
