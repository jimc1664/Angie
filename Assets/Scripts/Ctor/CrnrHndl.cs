
using UnityEngine;
using System.Collections;

public class CrnrHndl : UIEle {
    public Voxel V;
    public Voxel.Corner C;
    public int Ci;
    void Awake() {
     //   V = GetComponentInParent<Voxel>();
    }

    public override void gotHighlight(UIMain um) {
        //  Highlighted = true;
        //  upCol();
        keptHighlight(um);
    }
    public override void lostHighlight(UIMain um) {
        // Highlighted = false;
        // upCol();
    }
    public override void keptHighlight(UIMain um) {

    }
    public override void highlight_Cast(UIMain um, RaycastHit hit) {
        var cm = CtorMain.Singleton;
        // cm.Hl_D.FaceI = getFaceHlI(hit);
        cm.Hl_D.Wp = hit.point;
        cm.Hl_D.Nrm = hit.normal;
        cm.Hl_D.TriI = -1;
        //        Debug.Log("hit tri " + hit.triangleIndex);
    }
}
