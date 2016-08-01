using UnityEngine;
using System.Collections.Generic;


namespace Sim {
    public class Area : MonoBehaviour {

        public float Radius = 10.0f;

        //public Station St;
        [System.NonSerialized]
        public Sim.Station St;

        //public List<Faction> Factions;

        public SortedDictionary<MasterAi,MasterAi.AreaDat> Occupiers = new SortedDictionary<MasterAi, MasterAi.AreaDat>( new MonoBehaviourComparer<MasterAi>() );


        
        public enum StatusE {
            Owned, 
            Present,
            Contested,
            Unknown, 
        };
        public StatusE Status = StatusE.Unknown, NStatus = StatusE.Unknown; 

        void Awake() {

            //St = GetComponentInChildren<Station>();
        }

        void Start() {

            Simulation.Singleton.Areas.Add(this);
        }

        void OnDrawGizmos() {
            var c = Color.gray;

            c.a *= 0.5f;
            if(St != null && St.Owner != null)
                c = St.Owner.Col;
            else if(Occupiers.Count > 0) {
                var iter = Occupiers.GetEnumerator();
                iter.MoveNext();
                if(Occupiers.Count == 1) {
                    c = iter.Current.Key.Owner.Col;
                    c.a *= 0.5f;
                } else {
                    float tm = Time.time * 2;
                    int ind = Mathf.FloorToInt (tm);
                    MasterAi a = iter.Current.Key, b=a;
                    for(int i = 1 + ind % (Occupiers.Count-1); i-- > 0;) {
                        b = a;
                        iter.MoveNext();
                        a = iter.Current.Key;
                    }
                    c = Color.Lerp(a.Owner.Col, b.Owner.Col, Mathf.Sin( (tm - ind) *Mathf.PI *0.5f));
                }
                
            }
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }


        public void addDrone(Drone d) {
            Debug.Assert(d._Ar == null);
            d._Ar = this;


            MasterAi.AreaDat ad = null;
            bool enemy = false;

            foreach(var kvp in Occupiers) {
                if(kvp.Key == d.Ai) {
                    ad = kvp.Value;
                    Debug.Assert(ad.Count > 0 );
                    ad.Count++;                 
                } else {
                    kvp.Key.spotted(d, kvp.Value);
                    //kvp.Value.Status = MasterAi.AreaDat.StatusE.Contested;
                    enemy = true;
                }
            }

            if(ad == null) {
                if(d.Ai.Areas.TryGetValue(this, out ad)) {
                    Debug.Assert(ad.Count == 0 );
                    ad.Count = 1;
                    //ad.Status = MasterAi.AreaDat.StatusE.Present;
                } else {
                    ad = new MasterAi.AreaDat() {
                        Ar = this,
                        Count = 1,
                      //  Status = MasterAi.AreaDat.StatusE.Present
                    };
                    d.Ai.Areas.Add(this, ad);
                }
                Occupiers.Add(d.Ai, ad);
            }

            if(enemy) {
              //  ad.Status = MasterAi.AreaDat.StatusE.Contested;
                foreach(var o in Drones) {
                    if( o.Ai != d.Ai )
                        d.Ai.spotted(o, ad);

                    break;
                }
            }            
            Drones.Add(d);

            if(Occupiers.Count > 1) {
                NStatus = StatusE.Contested;
            } else if( St == null )
                NStatus = StatusE.Present;
            else
                NStatus = StatusE.Owned;
        }
        public void remDrone(Drone d) {
            Debug.Assert(d._Ar == this);
            Drones.Remove(d);
            d._Ar = null;

            MasterAi.AreaDat ad;
            if(Occupiers.TryGetValue(d.Ai, out ad)) {
                if(--ad.Count <= 0) {
                    Debug.Assert(ad.Count == 0);
                    Occupiers.Remove(d.Ai);
                }
            } else {
                Debug.LogError("err");
            }

            if(Occupiers.Count > 1) {
                NStatus = StatusE.Contested;
            } else if(Occupiers.Count == 0 ) {
                NStatus = StatusE.Unknown;
            } else if(St == null)
                NStatus = StatusE.Present;
            else
                NStatus = StatusE.Owned;
        }
        public List<Drone> Drones;
        public List<Body> Bodies;

        public void proc(ref Simulation.FrameCntx fc) {


            if(St != null) {
                /*foreach( var s in GameObject.FindObjectsOfType<global::Station>() )
                    Debug.Log("   s  " + s + "   h " + s.Sm.Host + "  hc "+ s.Sm.GetHashCode() );
                Debug.Log("St  " + St + "   h " + St.Host + "  hc " + St.GetHashCode());*/
                St.Host.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    St.updateSt(ref a);
                });
            }
            //St.updateSt(ref fc);

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
                    if(mag < Mathf.Pow(avoidEp, 2)) {

                        mag = Mathf.Sqrt(mag);

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

                foreach(var b in Bodies) {
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
                b.Host.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    b.update(ref a);
                });
            }
            foreach(var d in Drones) {
                Debug.Assert(d._Ar == this, "err");
                d.Host.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    if(NStatus == StatusE.Contested && NStatus != Status)
                        d.poke(ref a);
                    d.update(ref a);
                });
                // d.update(ref fc);
            }

            for( int i = Drones.Count; i-->0; ) {
                var d = Drones[i];
                d.Host.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    if( !d.updatePost(ref a)) {
                        remDrone(d);
                        Destroy(d.Host.gameObject);
                        d.St.Dependants.Remove(d);
                        if(d.Behavior == Drone.BehaviorT.Hunt)
                            d.St.FighterCnt--;
                        else
                            d.St.MinerCnt--;
                        Destroy(d);
                    }
                });
            }
            if(St != null)
                St.Host.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    St.updateSt(ref a);
                });
            //St.updateSt(ref fc);

            Status = NStatus;
        }
    }

}