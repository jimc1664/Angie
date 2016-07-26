using UnityEngine;
using System.Collections;

namespace Sim {

    [System.Serializable]
    public class Drone : Body {

        public Body Target;
        public Station St;
        public Beacon Bcn;




        public float MaxVel = 5;
        public float MaxSteer = 0.5f;


        public float MaxAVel = 5;
        public float MaxASteer = 0.5f;

        public float ArriveEp = 0.5f;


        public float Mins = 0;
        float MaxMins = 15;



        public Area WarpTo, WarpFrom;
        public float WarpRdy = 0;


        public enum StateT {
            Idle,
            Arrive,
            PrepWarp,
            Warping,
        };
        public enum BehaviorT {
            Mine,
            Dropoff,
            Hunt,
        };

        public StateT _State = StateT.Idle;
        public StateT State {
            get { return _State; }
            private set {   Debug.LogError("err"); }
        }
        public BehaviorT _Behavior = BehaviorT.Mine;
        public BehaviorT Behavior {
            get { return _Behavior; }
            private set { Debug.LogError("err"); }
        }


        public new void update(ref Simulation.FrameCntx cntx) {
            if((cntx.FrameInd & 1) == 0)
                subUp(ref cntx, Fd1, ref Fd2);
            else
                subUp(ref cntx, Fd2, ref Fd1);
        }

        void warpTo(Area to) {

            WarpTo =  to;
            WarpRdy = -3;
            _State = StateT.PrepWarp;
        }
        void subUp( ref Simulation.FrameCntx cntx,  FrameDat c, ref FrameDat n  ) {


            n = c;

            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, c.Rot, Vector3.one).transpose;


            Vector3 desVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;
            Vector3 desDir = rotM.GetRow(2), oDesDir = desDir;

            float velM = 0;

            var effPos = c.Pos + c.Vel * 0.5f;

            for( int i = 3; i-- > 0; ) {
                switch(State) {
                    case StateT.Arrive:
                        if(Target == null) { //or is invalid...

                            _State = StateT.Idle;
                            continue;
                        }

                        Vector3 seekEp = Target.fd(cntx.FrameInd).Pos + Target.fd(cntx.FrameInd).Vel * 0.5f;
                        Vector3 seek = seekEp - effPos;


                        //desVel = seek;

                        var arrive = seek;
                        var arriveM = arrive.magnitude;

                        if(arriveM > ArriveEp) {
                            desVel = arrive *= (arriveM - ArriveEp) / arriveM;

                        }
                        if(St != null) {
                            if(arriveM < ArriveEp * 2) {
                                if(Target == St.Bdy) {
                                    float trns = cntx.Delta * 1.0f;
                                    if(Mins > trns) {
                                        Mins -= trns;
                                    } else {
                                        trns = Mins;
                                        Mins = 0;
                                        Target = null;
                                        _State = StateT.Idle;
                                    }
                                    St.Minerals += trns;
                                } else {
                                    Mins += cntx.Delta * 0.15f;
                                    if(Mins > MaxMins) {
                                        Mins = MaxMins;
                                        Target = null;
                                        _State = StateT.Idle;
                                    }
                                }
                            }
                        }
                        break;
                    case StateT.Idle:
                        if(St != null) {
                            if(Mins > MaxMins * 0.5f) {
                                if(St.Bdy.Ar == Ar) {
                                    Target = St.Bdy;
                                    _State = StateT.Arrive;
                                    continue;
                                } else {
                                    // St.warp(this, St.Ar);
                                    warpTo(St.Bdy.Ar);
                                    continue;
                                }
                            } else {
                                Asteroid[] trgs;
                                trgs = Ar.GetComponentsInChildren<Asteroid>();
                                if(trgs.Length == 0) {
                                    trgs = GameObject.FindObjectsOfType<Asteroid>();
                                    // St.warp(this, trgs[Random.Range(0, trgs.Length)].Ar );
                                    warpTo(trgs[Random.Range(0, trgs.Length)].Bdy.Ar );

                                    continue;
                                } else {
                                    Target = trgs[Random.Range(0, trgs.Length)].Bdy;
                                    _State = StateT.Arrive;
                                }
                            }
                        } else {

                            foreach(var d in Ar.Drones) {
                                if(d.St == null) continue;

                                foreach(var md in GameObject.FindObjectsOfType<MiningDrone>()) {
                                    if(md.Drn.Ar == Ar) {
                                        Target = md.Drn;
                                        _State = StateT.Arrive;
                                        break;
                                    }
                                }
                                break;
                            }

                            if(Target == null) {
                                var a = Bcn.getTargetArea();
                                if( a != Ar )
                                    warpTo( a );

                            } else
                                continue;
                        }
                        break;

                    case StateT.PrepWarp:
                        var warp = effPos - (Ar.transform.position + Random.insideUnitSphere * 0.1f );
                        var wm = warp.sqrMagnitude;


                        if(wm > Mathf.Pow(Ar.Radius * 0.95f, 2)) {
                            var v = WarpTo.transform.position - Ar.transform.position;
                            if(v.sqrMagnitude > 0.01f)
                                desDir = v.normalized;
                        }

                        float arr = Ar.Radius * 1.05f;
                        if(wm < Mathf.Pow(arr, 2)) {
                            wm = Mathf.Sqrt(wm);
                            desVel = warp * (arr - wm) / wm;

                            if(WarpRdy > -2)
                                WarpRdy = 0;
                        } else {

                            float wr = Mathf.Max(Vector3.Dot(desDir, oDesDir), 0);
                            if(WarpRdy < -2) {
                                if(wr > 0.9f) {
                                    if(St != null) {
                                        St.warp(this, WarpTo);
                                        WarpRdy = wr;

                                    } else {
                                        //Bcn.warp(this, WarpTo);
                                        WarpFrom = Ar;
                                        cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, B = Simulation.Singleton.WarpingDrones, Ar = null, A = WarpFrom.Drones });
                                        WarpRdy = 0;
                                        _State = StateT.Warping;
                                    }
                                }
                            } else WarpRdy = wr;
                        }
                        break;

                    case StateT.Warping:

                        Vector3 vec = (WarpTo.transform.position - WarpFrom.transform.position);
                        var mag = Mathf.Pow(vec.sqrMagnitude + 1, 0.2f);

                        vec.Normalize();
                        WarpRdy += cntx.Delta / mag;

                        var v1 = WarpFrom.transform.position + vec * WarpFrom.Radius;
                        var v2 = WarpTo.transform.position - vec * WarpTo.Radius;
                        Debug.DrawLine(v1, v2, Color.cyan);
                        if(WarpRdy > 1) {
                            //Ar = WarpTo;
                            cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = Simulation.Singleton.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });
                            //Debug.Log("swap? "+cntx.FrameInd );
                            n.Pos = WarpTo.transform.position - vec * WarpTo.Radius + Random.insideUnitSphere * 0.3f;
                            n.Vel = vec * MaxVel * 0.1f;
                            n.AVel = Quaternion.identity;

                           
                            WarpFrom = WarpTo = null;
                            _State = StateT.Idle;
                        } else {
                            n.Pos = Vector3.Lerp(v1, v2, WarpRdy);
                            return;
                        }

                        break;


                }
                break;
            }

       
 
            desVel += Avoidance; Avoidance = Vector3.zero;
            velM = desVel.magnitude;

            if(desDir == oDesDir && velM >Mathf.Epsilon ) {
                desDir = desVel / velM;
            }
            if(desDir != oDesDir) {


                Vector3 yAx = Vector3.Cross(desDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), desDir);
                yAx = Vector3.Cross(desDir, xAx);

                var desRot = Quaternion.LookRotation(desDir, yAx);


                desAvel = Quaternion.Inverse(c.Rot) * desRot;
                float ang = Quaternion.Angle(desAvel, Quaternion.identity);
                if(ang > MaxAVel)
                    desAvel = Quaternion.Slerp(Quaternion.identity, desAvel, MaxAVel / ang);
                //var nRot = rot * desAvel;


            }
            var a2 = Quaternion.Angle(c.AVel, desAvel);
            if(a2 > MaxASteer)
                n.AVel = Quaternion.Slerp(c.AVel, desAvel, MaxASteer / a2);
            else
                n.AVel = desAvel;

            n.Rot *= Quaternion.Slerp(Quaternion.identity, n.AVel, 0.2f);
            n.Rot = n.Rot.normalised();

            if(velM > MaxVel) {
                desVel *= MaxVel / velM;
                velM = MaxVel;
            }


            var steering = desVel - c.Vel;

            var strMag = steering.magnitude;
            if(strMag > Mathf.Epsilon) {
                float effMaxSteer = MaxSteer;
                effMaxSteer *= Mathf.Max(0.5f, Vector3.Dot(rotM.GetRow(2), steering / strMag));
                effMaxSteer *= 1.0f + Vector3.Dot(-desVel / MaxVel, steering / strMag);

                if(strMag > effMaxSteer)
                    steering *= MaxSteer / strMag;

                n.Vel += steering * cntx.Delta;
            }
            n.Pos += n.Vel * cntx.Delta;

            //Debug.Log("up  "+c.Pos +"   ---> " + n.Pos);

        }
        /*
        void subUp2(ref Simulation.FrameCntx cntx, FrameDat c, ref FrameDat n) {

            n = c;

            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, c.Rot, Vector3.one).transpose;


            Vector3 desVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;
            Vector3 desDir = rotM.GetRow(2), oDesDir = desDir;

            float velM = 0;

            var effPos = c.Pos + c.Vel * 0.5f;

            if(WarpTo) {
                //Debug.Log(" warpTo "+WarpTo  +"    ==== " + cntx.FrameInd);
                if(WarpRdy > 2) {

                    Vector3 vec = (WarpTo.transform.position - WarpFrom.transform.position);
                    var mag = Mathf.Pow(vec.sqrMagnitude + 1, 0.2f);

                    vec.Normalize();
                    WarpRdy -= cntx.Delta / mag;

                    var v1 = WarpFrom.transform.position + vec * WarpFrom.Radius;
                    var v2 = WarpTo.transform.position - vec * WarpTo.Radius;
                    Debug.DrawLine(v1, v2, Color.cyan);
                    if(WarpRdy < 2) {
                        //Ar = WarpTo;
                        cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = Simulation.Singleton.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });
                        //Debug.Log("swap? "+cntx.FrameInd );
                        n.Pos = WarpTo.transform.position - vec * WarpTo.Radius + Random.insideUnitSphere * 0.3f;
                        n.Vel = vec * MaxVel * 0.1f;
                        n.AVel = Quaternion.identity;

                        WarpRdy = -5;
                        WarpFrom = WarpTo = null;
                    } else {
                        n.Pos = Vector3.Lerp(v2, v1, WarpRdy - 2);
                    }
                    return;
                } else {
                    var warp = effPos - Ar.transform.position;
                    var wm = warp.sqrMagnitude;


                    if(wm > Mathf.Pow(Ar.Radius * 0.95f, 2)) {
                        var v = WarpTo.transform.position - Ar.transform.position;
                        if(v.sqrMagnitude > 0.01f)
                            desDir = v.normalized;
                    }

                    float arr = Ar.Radius * 1.05f;
                    if(wm < Mathf.Pow(arr, 2)) {
                        if(wm < Mathf.Epsilon) {
                            desVel = Random.onUnitSphere;
                        } else {
                            wm = Mathf.Sqrt(wm);
                            desVel = warp * (arr - wm) / wm;
                        }


                        WarpRdy = -3;
                    } else {

                        float wr = Vector3.Dot(desDir, oDesDir);
                        if(WarpRdy < -2) {
                            if(wr > 0.9f) {
                                WarpRdy = wr;
                                //Bcn.warp(this, WarpTo);
                                WarpFrom = Ar;
                                cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, B = Simulation.Singleton.WarpingDrones, Ar = null, A = WarpFrom.Drones });
                                WarpRdy = 3;
                            }
                        } else WarpRdy = wr;
                    }
                }
            } else if(Target == null) {


                foreach(var d in Ar.Drones) {
                    if(d.St == null ) continue;

                    foreach(var md in GameObject.FindObjectsOfType<MiningDrone>()) {
                        if(md.Drn.Ar == Ar) {
                            Target = md.transform;

                            break;
                        }
                    }
                    

                    break;
                }

                if( Target == null ) {


                    var trgs = GameObject.FindObjectsOfType<MiningDrone>();
                    // St.warp(this, trgs[Random.Range(0, trgs.Length)].Ar );
                    WarpTo = trgs[Random.Range(0, trgs.Length)].Drn.Ar;
                    Debug.Log("warp  to " + WarpTo);
                    WarpRdy = -3;

                }
                

            }


            if(Target) {
                if(Target.GetComponent<MiningDrone>().Drn.Ar) {

                    Vector3 seek = Target.position - effPos;


                    //desVel = seek;

                    var arrive = seek;
                    var arriveM = arrive.magnitude;

                    if(arriveM > ArriveEp) {
                        desVel = arrive *= (arriveM - ArriveEp) / arriveM;

                    }
                } else Target = null;

            }
            desVel += Avoidance; Avoidance = Vector3.zero;
            velM = desVel.magnitude;

            if(desDir == oDesDir && velM > Mathf.Epsilon)
                desDir = desVel / velM;
            if(desDir != oDesDir) {


                Vector3 yAx = Vector3.Cross(desDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), desDir);
                yAx = Vector3.Cross(desDir, xAx);

                var desRot = Quaternion.LookRotation(desDir, yAx);


                desAvel = Quaternion.Inverse(c.Rot) * desRot;
                float ang = Quaternion.Angle(desAvel, Quaternion.identity);
                if(ang > MaxAVel)
                    desAvel = Quaternion.Slerp(Quaternion.identity, desAvel, MaxAVel / ang);
                //var nRot = rot * desAvel;

                var a2 = Quaternion.Angle(c.AVel, desAvel);
                if(a2 > MaxASteer)
                    n.AVel = Quaternion.Slerp(c.AVel, desAvel, MaxASteer / a2);
                else
                    n.AVel = desAvel;
                n.Rot *= Quaternion.Slerp(Quaternion.identity, n.AVel, 0.2f);
                // var pre = n.Rot;
                n.Rot = n.Rot.normalised();
                //Debug.Log("sub up " + c.Rot.sqrMag() +    "   n "+pre.sqrMag() + "   post " + n.Rot.sqrMag() + "   post2 " + post.sqrMag());


                //            float effMaxVel = MaxVel;

                // if(velM > MaxVel * 0.4f) {
                if(velM > MaxVel) {
                    desVel *= MaxVel / velM;
                    velM = MaxVel;
                }

                //}

            }

            var steering = desVel - c.Vel;

            var strMag = steering.magnitude;

            if(strMag > Mathf.Epsilon) {
                float effMaxSteer = MaxSteer;
                effMaxSteer *= Mathf.Max(0.5f, Vector3.Dot(rotM.GetRow(2), steering / strMag));
                effMaxSteer *= 1.0f + Vector3.Dot(-desVel / MaxVel, steering / strMag);

                if(strMag > effMaxSteer)
                    steering *= MaxSteer / strMag;

                n.Vel += steering * cntx.Delta;
            }
            n.Pos += n.Vel * cntx.Delta;


            //  Debug.Log("up  "+c.Pos +"   ---> " + n.Pos);

        }*/



        public class DroneGrp {


        }
    }

}