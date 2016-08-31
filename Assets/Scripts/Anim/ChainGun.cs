using UnityEngine;
using System.Collections;


public class ChainGun : MonoBehaviour {


    public bool Fire = false;
    public Transform Barrel, Pivot;

    public Transform Track;
    public float Chnc = 0;

    [System.Serializable]
    public class FlashDat{
        public GameObject Obj;
        public Material Mat;
        public float Alpha;
        public Vector3 InitScl;
    }
    public FlashDat[] Flashes;



    public float MaxSpd = 2.0f, Accel = 0.8f, Fade = 0.8f, RotJitter = 0.01f;
    float Speed = 0, Ang = 0;
    //float FlashA = 0;
    int LastFlash = 0, NextFlashI = 0;

    public float Arc = 179;
    
    void Awake() {
        // Mat = Flash.GetComponent<MeshRenderer>().material;

        foreach(var fd in Flashes) {
            fd.Mat = fd.Obj.GetComponent<MeshRenderer>().material;
            fd.InitScl = fd.Obj.transform.localScale;
        }
    }
    void Update() {

        Quaternion dr = transform.rotation;

        if(Track) {
            Vector3 fwd = transform.forward;
            Vector3 dv = (Track.position - Pivot.position ).normalized;
            var ang = Vector3.Angle(fwd, dv);

            if(ang > Arc) {
                dv = Vector3.Slerp(fwd, dv, Arc / ang);
            }
         //   Pivot.name = "a "+ang +"   m  " + (Arc / ang);
            dr = Quaternion.LookRotation(dv, transform.up);

            
            Debug.DrawLine(transform.position, Track.position, new Color( Chnc, 0, 0.4f, 1 ) );
        }
        Pivot.rotation = Quaternion.Lerp(Pivot.rotation, dr, 5.0f * Time.deltaTime);

        if(Fire) {
            Speed += Accel * Time.deltaTime;
            if(Speed > MaxSpd) Speed = MaxSpd;
        } else {
            Speed -= (Speed * 0.3f + Accel * 0.1f) * Time.deltaTime;
            if(Speed < 0) Speed = 0;
        }
        Ang += Speed *Time.deltaTime;
        if(Ang > 3) Ang -= 3;

        Barrel.localEulerAngles = new Vector3(0, Ang * 120.0f, 0);

        
        if(Fire) {
            int iter = Mathf.RoundToInt(Ang);
            if(iter != LastFlash) {
                LastFlash = iter;
                if(++NextFlashI >= Flashes.Length) NextFlashI = 0;
                var fd = Flashes[NextFlashI];
                //fd.Alpha -= fd.Alpha * Fade * Time.deltaTime;
                fd.Alpha = 1 / (1 - Fade * Time.deltaTime);
                fd.Obj.transform.localRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Random.rotation, RotJitter);
                var scl = fd.InitScl; scl.Scale(new Vector3(0.7f + Random.value * 0.6f, 0.6f + Random.value * 0.8f, 0.7f + Random.value * 0.6f ));
                fd.Obj.transform.localScale = scl;
            }
        }

        foreach(var fd in Flashes) {
            fd.Alpha -= fd.Alpha * Fade * Time.deltaTime;
            if(fd.Alpha < 0.05f) {
                fd.Obj.SetActive(false);
            } else {
                fd.Obj.SetActive(true);
                var c = fd.Mat.color; c.a = fd.Alpha;
                fd.Mat.color = c;
            }
        }
    }

}
