using UnityEngine;
using System.Collections;


namespace Sim {

    /*
    public class FrameDat<T> {

        public T this[int i] {
            get { return  (i&1)== 0 ? D1 : D2; }
            set { if((i & 1) == 0) D1 = value; else D2 = value; }
        }

        Vector3 lerp(Vector3 v1, Vector3 v2, float lrp) {

        }

        public  T lerp(Simulation s) {
            var lrp = 1.0f - s.Timer / s.Step;
            if( (s.FrameInd & 1) == 0 )
                return lerp(D1, D2, lrp);
            else
                return lerp(D2, D1, lrp);


        }
        T D1, D2;
    }
    */



    [System.Serializable]
    public class Body : ScriptableObject {
        [System.NonSerialized]
        public SimObj Host;


        public float Rad = 0.2f;

        private Area _Ar;
        public void _setArea(Area ar) {
            var o = _Ar;
            _Ar = ar;
            var plyr = Host as PlayerShipCtrlr;
            if(plyr != null) plyr.onAreaChange( o );  ///ewww remove todo            
        }

        public Area Ar {
            get { return _Ar; }
            private set {
                Debug.LogError("err");
            }
        }

        [System.Serializable]
        public struct FrameDat {
            public Vector3 Vel, Pos;
            public Quaternion Rot, AVel;
        };
        [SerializeField]
        protected FrameDat Fd1, Fd2;

        public Vector3 initVel { set { Fd1.Vel = Fd2.Vel = value; } }
        public Vector3 initPos { set { Fd1.Pos = Fd2.Pos = value; } }
        public Quaternion initRot {
            set {
                Fd1.Rot = Fd2.Rot = value;
            }
        }
        public Quaternion initAVel { set { Fd1.AVel = Fd2.AVel = value; } }

        public FrameDat fd(int fi) {
            if((fi & 1) != 0)
                return Fd1;
            else
                return Fd2;
        }
        public FrameDat fdSmooth(Simulation s) {
            if((s.FrameInd & 1) == 0)
                return fdSub(s, Fd1, ref Fd2);
            else
                return fdSub(s, Fd2, ref Fd1);
        }
        public void fd_Sync(int fi) {
            if((fi & 1) != 0)
                Fd2 = Fd1;
            else
                Fd1 = Fd2;

        }
        FrameDat fdSub(Simulation s, FrameDat a, ref FrameDat b) {
            var lrp = 1.0f - s.Timer / s.Step;
            a.Pos = Vector3.LerpUnclamped(a.Pos, b.Pos, lrp);
            a.Vel = Vector3.LerpUnclamped(a.Vel, b.Vel, lrp);
            a.Rot = Quaternion.Slerp(a.Rot, b.Rot, lrp);
            a.AVel = Quaternion.Slerp(a.AVel, b.AVel, lrp);
            return a;
        }



        public void update(ref Simulation.FrameCntx cntx) {
            if((cntx.FrameInd & 1) == 0)
                subUp(ref cntx, Fd1, ref Fd2);
            else
                subUp(ref cntx, Fd2, ref Fd1);
        }
        void subUp(ref Simulation.FrameCntx cntx, FrameDat c, ref FrameDat n) {

            n = c;

            n.Rot *= Quaternion.Slerp(Quaternion.identity, n.AVel, 0.2f);
            n.Rot = n.Rot.normalised();
        }

        public delegate void FooDlg(ref Simulation.FrameCntx fc);
        public void foo(ref Simulation.FrameCntx fc, FooDlg act) {
            if(Host)
                Host.foo(ref fc, act);
            else 
                act(ref fc);
        }
    }


    public class Entity : Body {
        public Faction Owner;
        public MasterAi Ai;
        public float Power, MaxPower;
    }

}


public class Body : SimObj {

    [System.NonSerialized]
    public Sim.Body Bdy;
    public float Rad = 0.5f;



    protected void OnEnable() {
        Trnsfrm = transform;
    }
    protected Transform Trnsfrm;

    void Update() {

        if(Bdy != null) {
            var fd = Bdy.fdSmooth(Simulation.Singleton);

            Trnsfrm.localPosition = fd.Pos;
            Trnsfrm.localRotation = fd.Rot;


        }
    }

    void OnDrawGizmos() {
        Trnsfrm = transform;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);
    }

}


