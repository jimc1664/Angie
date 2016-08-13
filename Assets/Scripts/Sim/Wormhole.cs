using UnityEngine;
using System.Collections;

namespace Sim {

    [System.Serializable]
    public class Wormhole : Body {

        public enum StateE {
            Hypothetical, //lulz
            Forming, 
            Waiting, 
            Warping,
            Deforming,
        };
        public StateE State = StateE.Hypothetical; 
        public float StateMd;
        public Vector3 Dir;
        public void update(ref Simulation.FrameCntx cntx, Vector3 av, Vector3 cp1, Vector3 cp2) {
            if((cntx.FrameInd & 1) == 0)
                subUp(ref cntx, Fd1, ref Fd2, av, cp1, cp2);
            else
                subUp(ref cntx, Fd2, ref Fd1, av, cp1, cp2);
        }
        void subUp(ref Simulation.FrameCntx cntx, FrameDat c, ref FrameDat n, Vector3 av, Vector3 cp1, Vector3 cp2) {

            n = c;
            n.Vel *= 0.9f;
            n.Vel += av + cp1;
            n.Pos += cp2;
            n.Pos += n.Vel * cntx.Delta;

            n.Rot *= Quaternion.Slerp(Quaternion.identity, n.AVel,  0.2f );
            n.Rot = n.Rot.normalised();
        }

        public void initVis() {
            var h = (Instantiate(World.Singleton.WormholeFab, Fd1.Pos, Fd1.Rot) as GameObject).GetComponent<global::Wormhole>();
            Host = h;
            h.transform.parent = Ar.Vis;
            h.Bdy = this;
            h.Rad = Rad;
            h.BaseScl = h.Vis.transform.localScale *= Rad;
            

            if(State > StateE.Forming) {
                h.Vis.gameObject.SetActive(true);
            } else {
                h.Vis.transform.localScale *= 0.001f;
            }
        }

        public void init(Area ar) {
            _setArea( ar );
            ar.WormHs.Add(this);
            if(ar.IsVisible)
                initVis();
            //h.Vis.gameObject.SetActive(false);

        }
    }
}


public class Wormhole : Body {
    public Transform Vis;
    public Vector3 BaseScl;
    new void OnEnable() {
        base.OnEnable();
        Vis = GetComponentInChildren<WormholeEff>(true ).transform;
    }
    public void Update() {

        if(Bdy != null) {
            var fd = Bdy.fdSmooth(Simulation.Singleton);
            Trnsfrm.localPosition = fd.Pos;
            Trnsfrm.localRotation = fd.Rot;
        }
        var wh = Bdy as Sim.Wormhole;

        Vector3 scl = BaseScl;
        switch(wh.State) {
            case Sim.Wormhole.StateE.Hypothetical:
                return;
            case Sim.Wormhole.StateE.Forming:

                scl = BaseScl * wh.StateMd;
                scl.y *= wh.StateMd;
                break;
            case Sim.Wormhole.StateE.Waiting:
                scl.Scale(new Vector3(0.6f, 1.5f, 0.6f) );
                scl = Vector3.LerpUnclamped (BaseScl, scl, wh.StateMd); 
                break;
            case Sim.Wormhole.StateE.Warping:
                scl.Scale(new Vector3(0.6f, 1.9f, 0.6f) );
                scl = Vector3.LerpUnclamped(BaseScl, scl, 1.0f + wh.StateMd);
                break;
            case Sim.Wormhole.StateE.Deforming:
                scl = Vis.localScale * 0.75f;
                break;
        }
        scl *= 1.0f + 0.1f * Mathf.Sin(Time.time * 100.0f);
        Vis.localScale = Vector3.Lerp(Vis.localScale, scl, 10.0f * Time.deltaTime);
       // Debug.Log(" Wormhole:: update   " + scl + "  Vis.localScale  " + Vis.localScale +  "  st " + wh.State + "  StateMd " + wh.StateMd);
    }

}
