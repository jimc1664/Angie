using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class OrbitPath : MonoBehaviour {

    public float ThetaScale = 0.01f;
    private int Size;
    private float Theta = 0f;
    LineRenderer LR;

    public float A;
    public void init( Transform p, float a ) {
        A = a;
        transform.parent = p;
        transform.localPosition = Vector3.zero;

        var lr = LR = GetComponent<LineRenderer>();

        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        lr.SetVertexCount(Size);
        for(int i = 0; i < Size; i++) {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = a * Mathf.Cos(Theta);
            float y = a * Mathf.Sin(Theta);
            lr.SetPosition(i, new Vector3(x, 0, y));
        }
       // lr.material = StarGen.Singleton.OrbitLRMat;
    }
    void OnEnable() {
        LR = GetComponent<LineRenderer>();
    }

    void OnWillRenderObject() {
        var cam = Camera.main;
#if UNITY_EDITOR
        if(!Application.isPlaying) {
            if(SceneView.lastActiveSceneView == null) return;
            // if(Camera.current == null) return;
            LR = GetComponent<LineRenderer>();
            // SceneView.lastActiveSceneView.renderMode == DrawCameraMode.

            cam = SceneView.lastActiveSceneView.camera;
            // Debug.Log("w " + w);
        }
#endif
        //var m = Vector3.Dot(cam.transform.rotation * Vector3.forward, -cam.transform.position);

        Vector3 cp = transform.InverseTransformPoint( cam.transform.position ), p1 = cp;
        p1.y = 0;
        p1 = p1.normalized * A;
        var p2 = -p1;
        var m = ((cp - p1).magnitude + (cp - p2).magnitude )*0.5f;
        //m = Mathf.Pow(m*0.05f, 2.0f);
        float w = 0.0015f *m;
        //Camera.current.transform.position.magnitude;
        LR.SetWidth(w, w);
      //  Debug.DrawLine( transform.TransformPoint(p1), cam.transform.position);
      //  Debug.DrawLine(transform.TransformPoint(p2), cam.transform.position, Color.grey );
    }
}
