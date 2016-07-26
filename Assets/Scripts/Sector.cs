using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Sector : MonoBehaviour {



    public int EffSeed = -1;
    public float EffDensity = -1;

    //  public int StarCnt = -1;
    /*
    public List<Vector2> So;
    public List<Vector2> Pix;
    public List<Color> C;
    */

    [System.Serializable]
    public class StarSys {
        public Vector3 P;
        [System.Serializable]
        public struct PossHyperLink {
            public int Ni, Si;
        };
        public List<PossHyperLink> Links = new List<PossHyperLink>();
    };
    public List<StarSys> Stars;


    public struct Line {
        public Vector3 P1, P2;
    };
    public List<Line> MissedStars;
    //public List<Line> HyperLanes;
    public List<Line> MissedHyperLanes;

    void Update() {


    }
    public Sector[] Nbrs;

    public enum State { 
        Gen, 
        Done,

    };
    public State Stt = State.Gen;


    public delegate bool SectorCallback(Sector sec);
    public delegate bool SectorCallback2(Sector sec, int ni );

    public void forNearSector(SectorCallback cb) {
        var nSec = this;
        for(int ni = 0; ;) {
            if(!cb(nSec)) return;
            for(;;) {
                if(ni >= 8) return;
                nSec = Nbrs[ni++];
                if(nSec != null) break;
            }
        }
    }
    public void forNearSector(SectorCallback2 cb) {
        var nSec = this;
        for(int ni = 0; ;) {
            if(!cb(nSec, ni-1)) return;
            for(;;) {
                if(ni >= 8) return;
                nSec = Nbrs[ni++];
                if(nSec != null) break;
            }
        }
    }

    void OnDrawGizmos() {


//        Gizmos.color = Color.yellow;

        Vector3 p = transform.position, scl = transform.localScale;
        foreach(var s in Stars) {

            Gizmos.color = Color.yellow;
            var sp = p + s.P;
            Gizmos.DrawWireSphere(sp, 0.03f);

            Gizmos.color = Color.green;
            foreach(var hl in s.Links) {
                var nSec = this;
                if(hl.Ni >= 0) nSec = Nbrs[hl.Ni];
                Gizmos.DrawLine(sp, nSec.Stars[hl.Si].P + nSec.transform.position );
            }
        }

        Gizmos.color = Color.red;

        if(MissedStars != null)
            foreach(var l in MissedStars) {

                Gizmos.DrawLine(l.P1, l.P2);
            }
       /* Gizmos.color = Color.green;

        if(HyperLanes != null)
            foreach(var l in HyperLanes) {

                Gizmos.DrawLine(l.P1, l.P2);
            }
            */

        if(UnityEditor.Selection.Contains(gameObject)) {
            Gizmos.color = Color.blue;

            if(MissedHyperLanes != null)
                foreach(var l in MissedHyperLanes) {

                    Gizmos.DrawLine(l.P1, l.P2);
                }
        }
    }
}



