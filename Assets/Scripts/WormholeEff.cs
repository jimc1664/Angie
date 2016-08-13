using UnityEngine;
using System.Collections.Generic;

public class WormholeEff : MonoBehaviour {

    struct SubDat {
        public Transform Trns;
        public Vector3 Off;
        public Quaternion Rot;
        public float Depth;
    };

    SubDat[] Subs;
	// Use this for initialization
	void Start () {

        Subs = new SubDat[transform.childCount];

       // Subs = GetComponentsInChildren<Transform>();

        int ci = 0;
        float a  =0.5f /  Subs.Length;
        float dep = 0, dm= 1.0f / Subs.Length;
        foreach( var c in transform ) {
            var s = c as Transform;
            Subs[ci].Trns = s;
            Subs[ci].Off = Random.onUnitSphere * 0.05f;

            Subs[ci].Depth = dep;
            Subs[ci].Rot = Quaternion.Slerp(Quaternion.identity, Random.rotation, 0.05f); ;

            dep += dm;

            s.localRotation = Quaternion.Slerp(Quaternion.identity, Random.rotation, 0.05f);
            //s.localScale.Scale(Vector3.one + Random.onUnitSphere * 0.01f);
            var m = s.GetComponent<MeshRenderer>().material;

            var col = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1), a * (0.9f + 0.2f * Random.value));
          //  if((ci & 1) != 0)
           //     col *= new Vector4(0.2f, 0.2f, 0.2f, 2.0f);

            m.SetColor( "_TintColor",  col  );
            ci++;

            a *= 0.9f;
        }

    }
    float Yr = 0;
    // Update is called once per frame
    void Update () {

       transform.localRotation *= Quaternion.Euler(new Vector3(0,  450.0f * Time.deltaTime, 0));
        var f = Vector3.up;
        float fac = 50.0f;
        float dm = 1.0f / Subs.Length;
        // transform.rotation  *= Quaternion.AngleAxis(fac * Time.deltaTime, f);
        foreach(var s in Subs) {
            s.Trns.localRotation *= Quaternion.AngleAxis(fac * Time.deltaTime, f);


            fac += 5 + fac*0.3f;
            float dep = s.Depth + Mathf.Sin( Time.time *100 )*dm*0.5f + Random.value*0.05f;
            s.Trns.localPosition = s.Off + Random.onUnitSphere * 0.0015f + Vector3.down * dep*0.5f;

            float w = 1 - 0.9f *Mathf.Sin( dep *0.5f *Mathf.PI );
            s.Trns.localScale = new Vector3(w, 0.08f +0.45f *dep, w);
        }
	
	}
}
