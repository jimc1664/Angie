using UnityEngine;
using System.Collections;
using UnityEditor;
using NodeEditorFramework.Utilities;

public class TestTarget : MonoBehaviour {

    public Weapon Wep;

    public float Rad = 1, Vel =0, A1, A2, AHit, AHit2, BruteHit, Dis, Dot, Ang, ChanceToHit, AimRad, RadFromDrift;

    static GUIStyle HndlStyle;
    void OnDrawGizmos() {
        if(!Wep || !Wep.DrawGizmo) return;
        transform.localScale = Vector3.one * Rad*2.0f;
        Wep.testTarget(this);

        if(HndlStyle == null) {
            GUIStyle style = HndlStyle = new GUIStyle();
            style.normal.textColor = Color.black;
          //  style.normal.background = ResourceManager.LoadTexture("Textures/NE_Box.png");
        }
       
        Handles.Label(transform.position + (Vector3.up+Vector3.right) * (AimRad+0.5f), " Dis: "+ Dis
            + "\n RadFromDrift:  " + RadFromDrift
            + "\n AHit2:  " + AHit2 * 100.0f
            + "\n Chnc:  " + ChanceToHit * 100.0f
            , HndlStyle);

    }

}
