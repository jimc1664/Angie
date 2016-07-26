using UnityEngine;
using System.Collections.Generic;

public class Area : MonoBehaviour {


    public float Radius = 10.0f;

    public Station St;

    void Awake() {

        St = GetComponentInChildren<Station>();
    }

    void Start() {

        Simulation.Singleton.Areas.Add(this);
    }

    void OnDrawGizmos() {
        var c = Color.green;
        if(St == null)
            c.a *= 0.5f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }


    public List<Sim.Drone> Drones;
    public List<Sim.Body> Bodies;

    public void proc(ref Simulation.FrameCntx fc) {

        for(int i = Drones.Count; i-- > 0;) {
            var d1 = Drones[i];
            var p1 = d1.fd(fc.FrameInd).Pos + d1.fd(fc.FrameInd).Vel * 0.25f;
            for(int j = i; j-- > 0;) {
                var d2 = Drones[j];
                var p2 = d2.fd(fc.FrameInd).Pos + d2.fd(fc.FrameInd).Vel * 0.25f; ;

                var vec = p1 - p2;
                var mag = (vec).sqrMagnitude;

                var m1 = d1.fd(fc.FrameInd).Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
                var m2 = d2.fd(fc.FrameInd).Vel.sqrMagnitude / (d2.MaxVel * d2.MaxVel);
                var mm = 1.5f - Vector3.Dot(d1.fd(fc.FrameInd).Vel / d1.MaxVel, d2.fd(fc.FrameInd).Vel / d2.MaxVel) * 0.5f;
                float avoidEp = 3.0f * mm;
                avoidEp = avoidEp * (d1.Rad + d2.Rad);
                if(mag < Mathf.Pow(avoidEp, 2) ) {

                    mag = Mathf.Sqrt(mag)  ;

                    //var mod = mag;
                    vec /= mag;
                   // vec.Normalize();
                    vec += Random.onUnitSphere * 0.05f;

                    vec *= (avoidEp - mag) * 0.5f;

                    //vec.Scale(vec);
                    float mt = 1 + m1 + m2;
                    d1.Avoidance += vec * (1 + m1) / mt;
                    d2.Avoidance -= vec * (1 + m2) / mt;
                }
            }

            foreach( var b in Bodies ) {
                var p2 = b.fd(fc.FrameInd).Pos;

                var vec = p1 - p2;
                var mag = (vec).sqrMagnitude;

                var m1 = d1.fd(fc.FrameInd).Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
                var m2 = 0;
                var mm = 1.5f;// - Vector3.Dot(d1.fd(fc.FrameInd).Vel / d1.MaxVel, d2.fd(fc.FrameInd).Vel / d2.MaxVel) * 0.5f;
                float avoidEp = 3.0f * mm;
                avoidEp = avoidEp * (d1.Rad + b.Rad);
                if(mag < Mathf.Pow(avoidEp, 2)) {

                    mag = Mathf.Sqrt(mag);

                    //var mod = mag;
                    vec /= mag;
                    // vec.Normalize();
                    vec += Random.onUnitSphere * 0.05f;

                    vec *= (avoidEp - mag);

                    //vec.Scale(vec);
                    float mt = 1 + m1 + m2;
                    d1.Avoidance += vec * (1 + m1) / mt;
                    //d2.Avoidance -= vec * (1 + m2) / mt;
                }
            }
        }

        foreach(var b in Bodies) {
            Debug.Assert(b._Ar == this, "err");
            b.update(ref fc);
        }
        foreach(var d in Drones) {
            Debug.Assert(d._Ar == this, "err");
            d.Host.foo( ref fc, ( ref Simulation.FrameCntx a ) => {
                d.update(ref a);
            });


                // d.update(ref fc);
            }

    }
}
