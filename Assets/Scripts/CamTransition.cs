using UnityEngine;
using System.Collections;

public class CamTransition : MonoBehaviour {

    Cam_Beh C;
    Camera[] Cams;
	void Start () {
        C = GetComponent<Cam_Beh>();
        Cams = GetComponentsInChildren <Camera>( true );

        Delta = (UIMain.Singleton.UIMd == UIMain.UIMode.Ctor) ? 1 : 0;
        foreach(var c in Cams)
            c.enabled = UIMain.Singleton.UIMd == UIMain.UIMode.Ctor;


    }

    public float Speed = 4.0f;
    public Vector2 SrcP = new Vector2(-0.1f, 0.5f);
    float Delta;


    void setEnabled( bool e ) {

        if(e)
            UIMain.Singleton.MainCam = Cams[0];
        else if(UIMain.Singleton.MainCam == Cams[0])
            UIMain.Singleton.MainCam = Camera.main;

        if(Cams[0].enabled == e) return;
        foreach(var c in Cams)
            c.enabled = e;


        foreach(var cast in GetComponents<UnityEngine.EventSystems.BaseRaycaster>())
            cast.enabled = e;
    }
    void Update () {
        if(C.enabled) {
            Delta += Time.unscaledDeltaTime * Speed;
            if(Delta > 1) {
                Delta = 1;
            }
            setEnabled(true);
        } else {
            Delta -= Time.unscaledDeltaTime * Speed;
            if(Delta <0 ) {
                Delta = 0;
                setEnabled(false);
            }
          
        }
        if(Cams[0].enabled) {
            float dm = Delta * Delta;


            var r  = new Rect(Vector2.zero, Vector2.one).lerp(new Rect(SrcP, Vector2.one *0.1f),1- dm);
            foreach(var c in Cams)
                c.rect = r;
            //            var pr = Cam.pixelRect;
            float ep = 5;
            //if(Cam.pixelWidth < ep || Cam.pixelHeight < ep || pr.xMin > Screen.width- ep || pr.yMin > Screen.height- ep || pr.xMax < ep || pr.yMax < ep ) {
            //    setEnabled(false);
                // = new Rect( Cam.pixelRect.position, Vector2.
           // }
        }
    }


}
