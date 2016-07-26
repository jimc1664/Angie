using UnityEngine;
using System.Collections;

//inspiration http://dl.dropboxusercontent.com/u/15234/answers-unity/Gizmo/Gizmo.html

public class Gizmonizer : MonoBehaviour {
    public GameObject gizmoAxis;
    public float gizmoSize = 1.0f;

    private GameObject gizmoObj;
    private Gizmo gizmo;
    private GizmoHandle.GizmoType gizmoType = GizmoHandle.GizmoType.Position;

    void Update (){
        if (Input.GetKeyDown(KeyCode.Escape)) {
            removeGizmo();
        }
    
        if (gizmo) {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                gizmoType = GizmoHandle.GizmoType.Position;
                gizmo.setType(gizmoType);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                gizmoType = GizmoHandle.GizmoType.Rotation;
                gizmo.setType(gizmoType);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                gizmoType = GizmoHandle.GizmoType.Scale;
                gizmo.setType(gizmoType);
            }        
           //// if (gizmo.needUpdate) {
           //     resetGizmo();
           // }
        }
    }
    /*void OnMouseDown (){
        if (!gizmoObj) {
            resetGizmo();
        }
    } */

    void removeGizmo (){
        if (gizmoObj) {
            gameObject.layer = 0;
            foreach(Transform child in transform) {
                child.gameObject.layer = 0;
            }        
            Destroy(gizmoObj);    
            Destroy(gizmo);    
        }
    }

    void resetGizmo (){
        removeGizmo();
        gameObject.layer = 2;
        foreach(Transform child in transform) {
            child.gameObject.layer = 2;
        }        
        gizmoObj = (GameObject)Instantiate(gizmoAxis, transform.position, transform.rotation);
        gizmoObj.transform.localScale *= gizmoSize;
        gizmo = gizmoObj.GetComponent<Gizmo>();
        gizmo.setParent(transform);
        gizmo.setType(gizmoType);
    }

}