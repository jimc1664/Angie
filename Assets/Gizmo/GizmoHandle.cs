using UnityEngine;
using System.Collections;

//inspiration http://dl.dropboxusercontent.com/u/15234/answers-unity/Gizmo/Gizmo.html

public class GizmoHandle : UIEle.Draggable {

    public GameObject positionEnd;
    public GameObject rotationEnd;
    public GameObject scaleEnd;
    float moveSensitivity = 1;
    public float rotationSensitivity = 64;
    //public bool  needUpdate = false;

    public Material Highlighted, Unhighlighted; 

    enum GizmoControl {Horizontal, Vertical, Both}
    public enum GizmoType {Position, Rotation, Scale} 
    public enum GizmoAxis {X, Y, Z}

    GizmoType type = GizmoType.Position;
   // GizmoControl control = GizmoControl.Both;
    public GizmoAxis axis = GizmoAxis.X;

    //private bool  mouseDown = false;
    private Transform otherTrans;
    Transform Trnsfrm; 
    void Awake (){
        Trnsfrm = transform;
        otherTrans = Trnsfrm.parent;
    }

    public void setParent ( Transform other  ){
        otherTrans = other;    
    }

    public void setType ( GizmoType type  ){
        this.type = type;
        positionEnd.SetActive( type == GizmoType.Position );
        rotationEnd.SetActive( type == GizmoType.Rotation );
        scaleEnd.SetActive(  type == GizmoType.Scale );
    }


    public delegate void TransDel( Vector3 a );
    public TransDel TD, RD, SD;
    void translate(Vector3 a) {
        if(TD != null) TD(a);
        else otherTrans.Translate(a);
    }
    void rotate(Vector3 a) {
        if(RD != null) RD(a);
        else otherTrans.Rotate(a);
    }
    void scale(Vector3 a) {
        if(SD != null) SD(a);
        else otherTrans.localScale += a;
    }

    float Delta = 0, Cur = 0;

    float MDelta;
    float mouseDelta(UIMain um ) {

        //Vector3 mp3 = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        var mRay = um.MRay;
        Plane p;
        var d1 = Trnsfrm.forward;
        var d2 = Trnsfrm.right;

        if( Mathf.Abs( Vector3.Dot( d1, mRay.direction) )  > Mathf.Abs( Vector3.Dot( d2, mRay.direction) ) ) 
            p = new Plane( d1, Trnsfrm.position);
        else 
            p = new Plane( d2, Trnsfrm.position);

        float dis, d;
        if(p.Raycast(mRay, out dis)) {

            var pos = mRay.GetPoint(dis);
            d = Vector3.Dot(Trnsfrm.up, pos);

            Trnsfrm.parent.gameObject.GetComponent<Gizmo>().Marker.position = pos;
        } else {
            d = Vector3.Dot(Trnsfrm.up, Trnsfrm.position);
        }
        return d;
    }

    public override void gotHighlight( UIMain um ) {
       // MDelta = mouseDelta( um );
      //  base.gotHighlight(im);
        GetComponent<MeshRenderer>().material = Highlighted;
    }
    public override void lostHighlight(UIMain um) {
      //  base.lostHighlight(im);
        GetComponent<MeshRenderer>().material = Unhighlighted;
    }

    /* void inLClick(InputMan im) {
         mouseDown = false;
         needUpdate = true;
         Delta = Cur = 0;
     } */
    public override void startDrag(UIMain um) {
        Delta = Cur = 0;
        MDelta = mouseDelta(um);
    }
    public override void drag(UIMain um) {

        // if(!Input.GetMouseButton(0)) { //todo this is wrong
        //     Delta = Cur = 0;
        //   needUpdate = true;
        // }
        Vector3 mMove = um.MMove; mMove.z = 0;// new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        //Vector3 mMove =  new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        //if( mMove.sqrMagnitude <0.001f ) return;
        /*
         var ax = Trnsfrm.up;
         var dir = (Camera.main.WorldToScreenPoint(ax + Trnsfrm.position) - Camera.main.WorldToScreenPoint(Trnsfrm.position));
         var len = Mathf.Max( dir.magnitude, 0.0001f );
         dir *= len / (len*len);
         float addDelta = Vector3.Dot( mMove, dir) *0.01f;*/

        // float d = addDelta;

        float d = mouseDelta( um );

        float desCur, effDelta,step;

        float addDelta = d-MDelta; MDelta = d;
        switch (type) {
            case GizmoType.Position:

                Delta += addDelta *= moveSensitivity;
                    desCur = Delta;
                if(Input.GetKey(KeyCode.LeftControl)) {
                    step = 0.25f;
                    desCur = Mathf.Round(desCur/step)*step;
                    if(Cur == desCur) return;
                }
                effDelta = desCur - Cur;  Cur = desCur;

                switch (axis) {
                    case GizmoAxis.X:
                        translate(Vector3.right * effDelta);
                        break;
                    case GizmoAxis.Y:
                        translate(Vector3.up * effDelta );
                        break;
                    case GizmoAxis.Z:
                        translate(Vector3.forward * effDelta );
                        break;
                }
                break;

            case GizmoType.Scale:
                Delta += addDelta *= moveSensitivity;
                desCur = Delta;
                if(Input.GetKeyDown(KeyCode.LeftControl)) {
                    desCur = (int)desCur;
                    if(Cur == desCur) return;
                }
                effDelta = desCur - Cur;  Cur = desCur;

                switch (axis) {
                    case GizmoAxis.X:
                        scale( Vector3.right * effDelta);
                        break;
                    case GizmoAxis.Y:
                        scale(Vector3.up * effDelta);
                        break;
                    case GizmoAxis.Z:
                        scale(Vector3.forward * effDelta);
                        break;
                }
                break;
            
            case GizmoType.Rotation:
                Delta += addDelta *= rotationSensitivity;

                desCur = Delta;
                if(Input.GetKeyDown(KeyCode.LeftControl)) {
                    desCur = (int)desCur;
                    if(Cur == desCur) return;
                }
                effDelta = desCur - Cur;  Cur = desCur;

                switch (axis) {
                    case GizmoAxis.X:
                        rotate(Vector3.right * effDelta);
                        break;
                    case GizmoAxis.Y:
                        rotate(Vector3.up * effDelta);
                        break;
                    case GizmoAxis.Z:
                        rotate(Vector3.forward * effDelta);
                        break;
                    }
                    break;
            }
    }
    //Delta = 0;
    

}