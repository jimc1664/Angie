using UnityEngine;
using System.Collections;

//inspiration http://dl.dropboxusercontent.com/u/15234/answers-unity/Gizmo/Gizmo.html


public class Gizmo : MonoBehaviour {

    public GizmoHandle axisX;
    public GizmoHandle axisY;
    public GizmoHandle axisZ;

    public Transform Marker;
    GizmoHandle.GizmoType gizmoType = GizmoHandle.GizmoType.Position;

    //public bool  needUpdate = false;

    void Awake (){
        axisX.axis = GizmoHandle.GizmoAxis.X;
        axisY.axis = GizmoHandle.GizmoAxis.Y;
        axisZ.axis = GizmoHandle.GizmoAxis.Z;
    
        setType(gizmoType);

    }
    void Update (){
        //   needUpdate = (axisX.needUpdate || axisY.needUpdate || axisZ.needUpdate);

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            gizmoType = GizmoHandle.GizmoType.Position;
            setType(gizmoType);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            gizmoType = GizmoHandle.GizmoType.Rotation;
            setType(gizmoType);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            gizmoType = GizmoHandle.GizmoType.Scale;
           setType(gizmoType);
        }
    }

    public void setType ( GizmoHandle.GizmoType type  ){
        axisX.setType(type);
        axisY.setType(type);
        axisZ.setType(type);
    }

    public void setParent ( Transform other  ){

        transform.parent = other;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        axisX.setParent(other);
        axisY.setParent(other);
        axisZ.setParent(other);
    }

    public void setCB(GizmoHandle.TransDel tcb, GizmoHandle.TransDel rcb, GizmoHandle.TransDel scb) {
        axisX.TD = axisY.TD = axisZ.TD = tcb;
        axisX.RD = axisY.RD = axisZ.RD = rcb;
        axisX.SD = axisY.SD = axisZ.SD = scb;

        //axisX.Delta = axisX.Cur = axisY.Delta = axisY.Cur = axisZ.Delta = axisZ.Cur = 0;
    }

}