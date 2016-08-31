using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SimObj : MonoBehaviour {

    
    public void foo(ref Simulation.FrameCntx fc,  Sim.Body.FooDlg act){
        act(ref fc);
    }

   public virtual void init() {

        Debug.LogError("err");
    }



    public virtual void onFrame() {
        FrameReports.Clear();
    }


    public class ReportStr {
        public ReportStr( string s, Sim.Body b, Vector3 op ) {
            S = s;
            Trns = b.Host.transform;
            Rad = b.Rad *1.5f;
            WPos = OrigPos = op;
        }
        public string S;
        public Transform Trns;
        public Vector3 WPos, OrigPos;
        public float Rad = 1.5f;
        public void draw() {
            if(Trns)
                WPos = Trns.position;

            Debug.DrawLine(OrigPos, WPos, Color.grey);
            var lp = WPos + Vector3.up * Rad;
            Debug.DrawLine(lp, WPos, Color.black );
            Handles.Label(lp, S);
        }
    };

    public class ReportMessage : ReportStr  {
        public float Tm;
        public ReportMessage(string s, Sim.Body b, Vector3 op, float tm ) : base( s,b,op ){
            Tm = tm;
        }
        public RectTransform UI; 
    };
    public List<ReportStr> FrameReports = new List<ReportStr>();



    void drawReports() {

        foreach(var r in FrameReports)
            r.draw();

        
    }

    void OnDrawGizmosSelected() {
        drawReports();
    }

}
