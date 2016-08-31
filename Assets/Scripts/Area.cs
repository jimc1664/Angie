using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sim {
    public class Area : MonoBehaviour, IQuadTreeObject {


        public Transform Vis;
        

        //public Station St;
        [System.NonSerialized]
        public Sim.Station St;

        //public List<Faction> Factions;

        public SortedDictionary<MasterAi,MasterAi.AreaDat> Occupiers = new SortedDictionary<MasterAi, MasterAi.AreaDat>( new MonoBehaviourComparer<MasterAi>() );

        public int Seed = 0;
        public float GenRoids = 0;
        
        public enum StatusE {
            Owned, 
            Present,
            Contested,
            Unknown, 
        };
        public StatusE Status = StatusE.Unknown, NStatus = StatusE.Unknown;

        //public Vector2 Pos;
        public Vector2 GetPosition() {
            return new Vector2( Pos3.x, Pos3.z);// * 10.0f;
        }
        public float Rad {
            get {
                return _Sol_Radius;
            }
        }
        float _Radius, _Sol_Radius;
        public float Radius {
            get {
                return _Radius;
            }
            set {
                _Radius = value;
                _Sol_Radius = _Radius * StarGen.Sol_2_Area;
            }
        }
        public float Sol_Radius {
            get {
                return _Sol_Radius;
            }
            set {
                _Sol_Radius = value;
                _Radius = _Sol_Radius / StarGen.Sol_2_Area;
            }
        }

        public Vector3 Pos3 {
            get {
                return transform.localPosition;
            }
        }
        public List<Area> Nbrs;

        public int Island = -1;


        static int Seeds = 0;

        public static Transform VisContatiner;

        public RectTransform Bttn;

        void Awake() {
            //St = GetComponentInChildren<Station>();
            Sol_Radius = 0.1f;

            if(VisContatiner == null) {
                var vc = GameObject.Find("__VisContainer");
                if(vc == null)
                    vc = new GameObject("__VisContainer");
                VisContatiner = vc.transform;
            }
            Vis = new GameObject(name +" _vis" ).transform;
            Vis.transform.parent = VisContatiner;
            Vis.gameObject.SetActive(false);
            Vis.transform.localPosition = transform.position *10.0f;
            Vis.transform.localRotation = Quaternion.identity;
            Vis.transform.localScale = Vector3.one;
            
            if(Seed == 0)
                Seed = Seeds++;



        }

        void Start() {
            var ico = Instantiate(UIMain.Singleton.Icon_Area);
            var rt = Bttn = ico.GetComponent<RectTransform>();
            rt.parent = SolMap.Singleton.Areas;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            var bttn = ico.GetComponent<UnityEngine.UI.Button>();

            bttn.onClick.AddListener(() => {
                if(!this ) return;
               
                var cam = FindObjectOfType<OrbitCam>();                
                if(cam.Target == transform) {
                    AreaDisplay.set(this);
                } else {
                    cam.setTarget(transform);
                }
            });

            Simulation.Singleton.Areas.Add(this);
        }
        void LateUpdate() {

            if(UIMain.Singleton.SolCam.Cam.enabled) {                
                Bttn.anchoredPosition = UIMain.Singleton.SolCam.Cam.WorldToScreenPoint(transform.position);
            }
        }

        public List<Asteroid> Roids;
        void gen() {
            if( !Application.isPlaying )
                foreach(var a in GetComponentsInChildren<Asteroid>())
                    DestroyImmediate(a.gameObject);
                
            Random.seed = Seed;
            if(Seed == 0)
                Random.seed = (int)System.DateTime.Now.Ticks;


            if(GenRoids < 0) {
                Random.seed = Seed - 2;
                GenRoids = Random.value * (1 + Random.value);
                GenRoids *= 10.0f;
            }

            float density = 1.3f * (1.0f + Math_JC.pow2(Random.value));
            //  List<Asteroid> roids = new List<Asteroid>();
            Roids.Clear();
            for( float rc = GenRoids; rc > 0; ) {            
                var go = new GameObject ( "an Asteroid" );
                var a = go.AddComponent<Asteroid>();
                var t = go.transform;
                t.parent = transform;
                

                var mesh = Instantiate( StarGen.Singleton.RoidFabs[Random.Range(0, StarGen.Singleton.RoidFabs.Count)] );
                var mt = mesh.transform;
                mt.parent = t;

                Vector3 scl = (new Vector3(Random.value, Random.value, Random.value)+ Vector3.one)  * 0.03f *  (0.2f+ Random.value );
                mt.localScale = scl;

                mt.localPosition = Vector3.zero;
                t.localRotation = Random.rotation;

                mt.GetComponent<MeshRenderer>().material = StarGen.Singleton.RoidMats[Random.Range(0, StarGen.Singleton.RoidMats.Count)];

                var mf = mt.GetComponent<MeshFilter>();

                var vrts = mf.sharedMesh.vertices;

                Vector3 mid = Vector3.zero;
                foreach(var v in vrts) mid += v;
                mid /= vrts.Length;
                mid.Scale(scl);
                mt.localPosition = -mid;

                float r = 0, mr = 0;
                foreach(var v in vrts) {
                    v.Scale(scl);
                    float d = (v - mid).sqrMagnitude;
                    r += d;
                    mr = Mathf.Max(mr, d);
                }
                r = Mathf.Sqrt( r / vrts.Length );

                a.Rad = Mathf.Sqrt(mr) *0.7f + r *0.3f;
                rc -= r;
                
                for(float rm = 2 + 0.25f* Roids.Count; ;) {
                    t.localPosition = Random.insideUnitSphere * a.Rad * Random.value * rm * density;
                    foreach(var o in Roids) {

                        if((o.transform.localPosition - t.localPosition).sqrMagnitude * 0.9f < Math_JC.pow2(a.Rad + o.Rad))
                            goto label_breakContinue;
                    }
                    break;
                    label_breakContinue:;
                    rm += 1 + 0.5f * Roids.Count;
                }


                Roids.Add(a);
                if(Application.isPlaying)
                    a.init();
                //rc = 0;
            }
        }
        public bool Gen = false;
        public bool GenAll = false;
        public bool KillAll = false;
        void OnDrawGizmos() {

            if(KillAll) {
                foreach(var a in FindObjectsOfType<Asteroid>()) DestroyImmediate(a.gameObject);
                foreach(var a in FindObjectsOfType<Area>()) a.Roids.Clear();
               KillAll = false;
            }
            if(GenAll) {
                foreach(var a in FindObjectsOfType<Area>()) a.Gen = true;
                GenAll = false;
            }
            if(Gen) {
                gen();
                Gen = false;
            }

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
            Gizmos.DrawWireSphere(transform.position, Radius * StarGen.Sol_2_Area );

            if(Application.isPlaying) {
                if(Mr != null || (Mr = GetComponentInChildren<MeshRenderer>()) != null)
                    Mr.material.color = c;

                Bttn.GetComponent<UnityEngine.UI.RawImage>().color = c;
            }
        }
        MeshRenderer Mr;

        public bool IsVisible = false;

        void OnGUI() {

            for(int i = Msgs.Count; i-- > 0;) {
                var m = Msgs[i];
                if((m.Tm -= Time.deltaTime) < 0) {
                    Msgs.RemoveAt(i);
                    if(m.UI)
                        Destroy(m.UI.gameObject);
                }  else if(IsVisible && UIMain.Singleton.UIMd == UIMain.UIMode.Ship ) {
                    m.draw();
                }
            }
        }

        void getHost(Drone d) {
            if(d.Host == null) {
                global::Drone gd;
                switch(d.Behavior) {
                    case Drone.BehaviorT.Mine:
                        gd = Instantiate(Spawnables.Singleton.MiningShip).GetComponent<global::Drone>();
                        break;
                    case Drone.BehaviorT.Hunt:
                        gd = Instantiate(Spawnables.Singleton.FighterDrn).GetComponent<global::Drone>();
                        break;
                    case Drone.BehaviorT.Player:
                        gd = Instantiate(Spawnables.Singleton.PlayerShip).GetComponent<global::Drone>();
                        break;
                    default:
                        Debug.LogError("unhandled");
                        return;
                }

                gd.init(d);

            }
            d.Host.transform.parent = Vis;
        }


        public bool VisOverride = false;

        public void addDrone(Drone d) {
            Debug.Assert(d.Ar == null);

            if(d.Host as PlayerShipCtrlr  || VisOverride ) {

                Vis.gameObject.SetActive( IsVisible = true );

                foreach(var od in Drones) getHost(od);

                foreach(var w in WormHs)
                    if(w.Host == null)
                        w.initVis();
            }

            if(IsVisible) {
                getHost(d);
            } else if( d.Host) {
                //destroy host..
                d.Host.transform.parent = Vis;
            }

            if( Drones.Count == 0 )
                if(Roids.Count == 0 && GenRoids != 0) {
                    gen();
                    GenRoids = 0;
                }

            d._setArea(this);

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
            d.AreaD = ad;

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


            if(AreaDisplay.Singleton && AreaDisplay.Singleton.Ar == this)
                AreaDisplay.Singleton.addDrn(d);
        }
        public void remDrone(Drone d) {
            Debug.Assert(d.Ar == this);
            Drones.Remove(d);
            d._setArea(null);
            d.AreaD = null;

            if( d.Host ) 
                d.Host.transform.parent = World.Singleton.WarpingHosts;

            if(d.Host as PlayerShipCtrlr && !VisOverride ) {
                
                Vis.gameObject.SetActive(IsVisible = false);
            }

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

        public void addBody( Body b) {
            Bodies.Add(b);
            Debug.Assert(b.Ar == null);
            b._setArea(this);
            if(b.Host) {
                b.Host.transform.parent = Vis;
                b.Host.transform.localPosition = b.fd(0).Pos;
            }
        }

        public List<Drone> Drones;
        public List<Body> Bodies;
        public List<Wormhole> WormHs;


        //i am relying on this function being inlined....   close as i figure i can get to proper specilisation on c#
        void avoidanceAndCollison(ref Simulation.FrameCntx fc, Body b1, Body b2,
            Vector3 p1, Vector3 p2, Vector3 vel1, Vector3 vel2, Vector3 p1_2, Vector3 p2_2, float mm, float m1, float m2, float ms1, float ms2, float avR,
            ref Vector3 d1Cp1, ref Vector3 d1Cp2, ref Vector3 d2Cp1, ref Vector3 d2Cp2, ref Vector3 d1Av, ref Vector3 d2Av
            ) {

            var vec = p1 - p2;
            var mag = (vec).sqrMagnitude;
            //var mm = 1.0f + Mathf.Abs((d1.fd(fc.FrameInd).Vel / d1.MaxVel).sqrMagnitude - (d2.fd(fc.FrameInd).Vel / d2.MaxVel).sqrMagnitude);

            float ep1 = (b1.Rad + b2.Rad);
            float avoidEp = 1.5f * mm;
            avoidEp = ep1 + (avR * 2.0f + 0.2f) * avoidEp;
            if(mag < Mathf.Pow(avoidEp, 2)) {

                mag = Mathf.Sqrt(mag);
                vec /= mag + Mathf.Epsilon;
                var noise = Random.onUnitSphere * 0.05f;
                vec += noise;

                // Vector3 p1_2 = d1.fd(fc.FrameInd).Pos + d1.fd(fc.FrameInd).Vel * fc.Delta, p2_2 = d2.fd(fc.FrameInd).Pos + d2.fd(fc.FrameInd).Vel * fc.Delta;
                var v2 = p1_2 - p2_2;
                if(v2.sqrMagnitude < Mathf.Pow(ep1, 2)) {
                    float v2m = v2.magnitude;
                    var v2n = v2 / (v2m + Mathf.Epsilon);
                    v2 =  (ep1 - v2m) *v2n;  //force non zero
                    v2 += noise;
                    float bnc = 0.3f;
                    v2 *= 0.75f;
                    //float mass = d1.Rad + d2.Rad, ms1 = d2.Rad / mass, ms2 = d1.Rad / mass;
                    //d1.CollisonPush2 += v2 * (1 - bnc) * ms1;
                    //d2.CollisonPush2 -= v2 * (1 - bnc) * ms2;
                    d1Cp2 += v2 * (1 - bnc) * ms1;
                    d2Cp2 -= v2 * (1 - bnc) * ms2;
                    v2 = v2 * (bnc / fc.Delta);

                    //d1.CollisonPush1 += (v2 - d1.fd(fc.FrameInd).Vel) * 0.8f * ms1;
                    //d2.CollisonPush1 -= (v2 - d2.fd(fc.FrameInd).Vel) * 0.8f * ms2;

                    var velDif = vel1 - vel2;
                   // Vector3.Reflect( vel1 /v1m, v2n) * v1m ;
                    d1Cp1 += (velDif - vel1 ) * 0.8f * ms1;
                    d2Cp1 += (-velDif - vel2 ) * 0.8f * ms2;
                }

                vec *= (avoidEp - mag) * 0.5f;
                vec *= 2.0f;
                //var m1 = d1.fd(fc.FrameInd).Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
                //var m2 = d2.fd(fc.FrameInd).Vel.sqrMagnitude / (d2.MaxVel * d2.MaxVel);
                //float mt = 1 + m1 + m2;
                //m1 = (1 + m1) / mt;
                //m2 = (1 + m2) / mt;
                //d1.Avoidance += vec * m1;
                //d2.Avoidance -= vec * m2;
                d1Av+= vec * m1;
                d2Av -= vec * m2;
            }
        }

        public void proc(ref Simulation.FrameCntx fc) {
            if(St != null) {
                /*foreach( var s in GameObject.FindObjectsOfType<global::Station>() )
                    Debug.Log("   s  " + s + "   h " + s.Sm.Host + "  hc "+ s.Sm.GetHashCode() );
                Debug.Log("St  " + St + "   h " + St.Host + "  hc " + St.GetHashCode());*/
                St.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    St.updateSt(ref a);
                });
            }
            //St.updateSt(ref fc);

            for(int i = Drones.Count; i-- > 0;) {
                var d1 = Drones[i];
                var p1 = d1.fd(fc.FrameInd).Pos + d1.fd(fc.FrameInd).Vel * 0.25f;
                for(int j = i; j-- > 0;) {
                    var d2 = Drones[j];
                    var p2 = d2.fd(fc.FrameInd).Pos + d2.fd(fc.FrameInd).Vel * 0.25f;
                    var mm = 1.0f + Mathf.Abs((d1.fd(fc.FrameInd).Vel / d1.MaxVel).sqrMagnitude - (d2.fd(fc.FrameInd).Vel / d2.MaxVel).sqrMagnitude);
                    Vector3 p1_2 = d1.fd(fc.FrameInd).Pos + d1.fd(fc.FrameInd).Vel * fc.Delta, p2_2 = d2.fd(fc.FrameInd).Pos + d2.fd(fc.FrameInd).Vel * fc.Delta;
                    float mass = d1.Rad + d2.Rad, ms1 = d2.Rad / mass, ms2 = d1.Rad / mass;
                    var m1 = d1.fd(fc.FrameInd).Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
                    var m2 = d2.fd(fc.FrameInd).Vel.sqrMagnitude / (d2.MaxVel * d2.MaxVel);
                    float mt = 1 + m1 + m2;
                    m1 = (1 + m1) / mt  *ms1;
                    m2 = (1 + m2) / mt  *ms2;
                    float avR = Mathf.Max(d1.Rad, d2.Rad);
                    avoidanceAndCollison( ref fc, d1, d2, p1, p2, d1.fd(fc.FrameInd).Vel, d2.fd(fc.FrameInd).Vel, 
                        p1_2, p2_2, mm, m1, m2, ms1, ms2, avR, ref d1.CollisonPush1, ref d1.CollisonPush2, ref d2.CollisonPush1, ref d2.CollisonPush2, ref d1.Avoidance, ref d2.Avoidance);


                    if(d1.Owner != d2.Owner) {

                    }
                }

                foreach(var b in Bodies) {
                    var p2 = b.fd(fc.FrameInd).Pos;

                    var mm = 1.0f + (d1.fd(fc.FrameInd).Vel / d1.MaxVel).sqrMagnitude;
                    Vector3 p1_2 = d1.fd(fc.FrameInd).Pos + d1.fd(fc.FrameInd).Vel * fc.Delta, p2_2 = p2;
                    float ms1 = 1, ms2 = 0;
                    float m1 = d1.fd(fc.FrameInd).Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel), m2 = 0, mt = 1 + m1 + m2;
                    m1 = (1 + m1) / mt;
                    Vector3 dummy = Vector3.zero;
                    avoidanceAndCollison(ref fc, d1, b, p1, p2, d1.fd(fc.FrameInd).Vel, dummy,
                        p1_2, p2_2, mm, m1, m2, ms1, ms2, d1.Rad, ref d1.CollisonPush1, ref d1.CollisonPush2, ref dummy, ref dummy, ref d1.Avoidance, ref dummy);
                }
            }

            for(int i = WormHs.Count; i-- > 0;) {
                var wh = WormHs[i];

                if(wh.State <= Wormhole.StateE.Waiting) {
                    var p1 = wh.fd(fc.FrameInd).Pos + wh.fd(fc.FrameInd).Vel * fc.Delta;
                    Vector3 av = Vector3.zero, cp1 = Vector3.zero, cp2 = Vector3.zero;
                    foreach(var d in Drones) {
                        if(d.Wormhole == wh) continue;

                        var p2 = d.fd(fc.FrameInd).Pos + d.fd(fc.FrameInd).Vel * 0.25f;
                        var mm = 1.0f + (d.fd(fc.FrameInd).Vel / d.MaxVel).sqrMagnitude;
                        Vector3 p1_2 = p1, p2_2 = d.fd(fc.FrameInd).Pos + d.fd(fc.FrameInd).Vel * fc.Delta;
                        float ms1 = 1, ms2 = 0;
                        float m1 = 1, m2 = 0;
                        Vector3 dummy = Vector3.zero;
                        avoidanceAndCollison(ref fc, wh, d, p1, p2, wh.fd(fc.FrameInd).Vel, dummy,
                            p1_2, p2_2, mm, m1, m2, ms1, ms2, wh.Rad, ref cp1, ref cp2, ref dummy, ref dummy, ref av, ref dummy);
                    }
                    foreach(var b in Bodies) {
                        var p2 = b.fd(fc.FrameInd).Pos;
                        var mm = 1.0f;
                        Vector3 p1_2 = p1, p2_2 = p2;
                        float ms1 = 1, ms2 = 0;
                        float m1 = 1, m2 = 0;
                        Vector3 dummy = Vector3.zero;
                        avoidanceAndCollison(ref fc, wh, b, p1, p2, wh.fd(fc.FrameInd).Vel, dummy,
                            p1_2, p2_2, mm, m1, m2, ms1, ms2, wh.Rad, ref cp1, ref cp2, ref dummy, ref dummy, ref av, ref dummy);
                    }
                    foreach(var w2 in WormHs) {  //todo - do this properly...
                        if(ReferenceEquals(wh, w2)) continue;
                        var p2 = w2.fd(fc.FrameInd).Pos;
                        var mm = 1.0f;
                        Vector3 p1_2 = p1, p2_2 = p2;
                        float mass = wh.Rad + w2.Rad, ms1 = w2.Rad / mass, ms2 = wh.Rad / mass;
                        Vector3 dummy = Vector3.zero;
                     //   ms1 *= 0.25f;
                        avoidanceAndCollison(ref fc, wh, w2, p1, p2, wh.fd(fc.FrameInd).Vel, w2.fd(fc.FrameInd).Vel,
                            p1_2, p2_2, mm, ms1, ms2, ms1, ms2, Mathf.Max( wh.Rad, w2.Rad ), ref cp1, ref cp2, ref dummy, ref dummy, ref av, ref dummy);
                    }
                    av *= 0.1f;
                    wh.update(ref fc, av, cp1, cp2);
                } else if( wh.State == Wormhole.StateE.Deforming ) {
                    wh.StateMd += fc.Delta;
                    if(wh.StateMd > 1.0f) {
                        WormHs.RemoveAt(i);
                        if( wh.Host )
                            Destroy(wh.Host.gameObject);
                        Destroy(wh);
                    }
                }

            }

            foreach(var b in Bodies) {
                Debug.Assert(b.Ar == this, "err");
                b.update(ref fc);
                
                b.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    b.update(ref a);
                });
            }
            foreach(var d in Drones) {
                Debug.Assert(d.Ar == this, "err");
                d.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    if(NStatus == StatusE.Contested && NStatus != Status)
                        d.poke(ref a);
                    d.update(ref a);
                });
                // d.update(ref fc);
            }

            for( int i = Drones.Count; i-->0; ) {
                var d = Drones[i];
                d.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    if( !d.updatePost(ref a)) {
                        remDrone(d);
                        if( d.Host )
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
                St.foo(ref fc, (ref Simulation.FrameCntx a) => {
                    St.updateSt(ref a);
                });
            //St.updateSt(ref fc);

            Status = NStatus;
        }

        public Vector3 worldPos(Vector3 tp) {
            if(Vis == null) return tp;
            return Vis.TransformPoint(tp);
        }

        public List<SimObj.ReportMessage> Msgs = new List<SimObj.ReportMessage>();

    }




}