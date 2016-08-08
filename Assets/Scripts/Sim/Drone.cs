using UnityEngine;
using System.Collections;

namespace Sim {

    [System.Serializable]
    public class Drone : Entity {
        public Body Target;
        [System.NonSerialized]
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
            EnterArea,
            Idle,
            Mine,
            Attack,
            PrepWarp,
            Warping,
            PilotCtrl,
        };
        public enum BehaviorT {
            Mine,
            //Dropoff,
            Hunt,
            Player,
        };

        public StateT _State = StateT.EnterArea;
        public StateT State {
            get { return _State; }
            private set {   Debug.LogError("err"); }
        }
        public BehaviorT _Behavior = BehaviorT.Mine;
        public BehaviorT Behavior {
            get { return _Behavior; }
            private set { Debug.LogError("err"); }
        }

        public void poke(ref Simulation.FrameCntx cntx) {
            if(State == StateT.PrepWarp && Behavior == BehaviorT.Hunt) {
                _State = StateT.Idle;
                WarpTo = null;
                Target = null;
            }
        }
        public new void update(ref Simulation.FrameCntx cntx) {
            if((cntx.FrameInd & 1) == 0)
                subUp(ref cntx, Fd1, ref Fd2);
            else
                subUp(ref cntx, Fd2, ref Fd1);
        }
        public bool updatePost(ref Simulation.FrameCntx cntx) {
            if(Dmg > 0) {
                if(Dmg > Hp) {
                    return false;
                }
                Hp -= Dmg;
                Dmg = 0;

                if(State == StateT.Mine && Ar != St.Ar ) {
                    setWarpTarget(St.Ar);
                }
            }
            return true;
        }
        void setWarpTarget(Area to) {
            Target = null;
            WarpTo =  to;
            WarpRdy = 0;
            _State = StateT.PrepWarp;
        }
        public void enterWarp(ref Simulation.FrameCntx cntx ) {
            WarpFrom = Ar;
            cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, B = Simulation.Singleton.WarpingDrones, Ar = null, A = WarpFrom.Drones });
            WarpRdy = 0;
            _State = StateT.Warping;
        }

        public float Hp = 50, MaxHp = 50, Dmg = 0;
        public float PowerMod, EngPow;
        public float EffArriveEp = 0, AimDot = 0;

        public struct DroneCntx {

            //Simulation.FrameCntx  --manual inheritance...  i hate c#
            public readonly float Delta;
            public readonly int FrameInd;
            public Simulation Sim;
            public DroneCntx( ref Simulation.FrameCntx fc, FrameDat c ) {
                Sim = fc.Sim;
                Delta = fc.Delta;
                FrameInd = fc.FrameInd;

                DesVel = EffPos = DesDir = ODesDir = DesUp = Vector3.zero;
                MinVel = 0;

                Pos = c.Pos;
            }

            public Vector3 EffPos, DesVel, DesDir, ODesDir, Pos, DesUp;
            public float MinVel;
        };
        bool act_Idle(ref DroneCntx cntx ) {
            Target = null;
            switch(Behavior) {
                case BehaviorT.Mine:
                    if(Mins > MaxMins * 0.5f || (St.Ar == Ar && Mins > 0 ) ) {
                        if(St.Ar == Ar) {
                            Target = St;
                            _State = StateT.Mine;
                            return true;
                        } else {
                            // St.warp(this, St.Ar);
                            setWarpTarget(St.Ar);
                            return true;
                        }
                    } else {
                        Asteroid[] trgs;
                        trgs = Ar.GetComponentsInChildren<Asteroid>();
                        if(trgs.Length == 0) {
                            setWarpTarget(Ai.getTargetArea()  );

                            return true;
                        } else {
                            Target = trgs[Random.Range(0, trgs.Length)].Bdy;
                            _State = StateT.Mine;
                        }
                    }
                    break;
                case BehaviorT.Hunt:
                   
                    // Debug.Log("hunt check " + Ar.Occupiers.Count);
                    if(Ar.Occupiers.Count > 1) {
                        foreach(var d in Ar.Drones) {
                           // Debug.Log("  target ?? " + d );

                            if(d.Ai != Ai) {
                                _State = StateT.Attack;
                                Target = d;
                               // Debug.Log("AYE  target " + d);
                                break;
                            }
                        }
                    }
                    /*
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
                    }*/

                    if(Target == null) {
                        var a = Ai.getTargetArea();
                        if(a != Ar)
                            setWarpTarget(a);
                    } else
                        return true;
                    break;
                case BehaviorT.Player:
                    _State = StateT.PilotCtrl;
                    break;
            }
            return false;
           /* if(St != null ) {
                
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
                    if(a != Ar)
                        setWarpTarget(a);

                } else
                    return true;
            }

            return false; */
        }

        float act_Sub_Arrive(ref DroneCntx cntx, float ep ) {

            Vector3 seekEp = Target.fd(cntx.FrameInd).Pos + Target.fd(cntx.FrameInd).Vel * 0.5f;
            Vector3 seek = seekEp - cntx.EffPos;


            var arrive = seek;
            var arriveM = arrive.magnitude;

            if(arriveM > ep) {
                cntx.DesVel = arrive *= (arriveM - ep) / arriveM;
            }
            
            return arriveM;
        }
        bool act_Mine(ref DroneCntx cntx) {
            if(Target == null) { //or is invalid...
                _State = StateT.Idle;
                return true;
            }

            var arriveM = act_Sub_Arrive(ref cntx, ArriveEp);

            if(arriveM < ArriveEp * 2) {
                if(Target == St) {
                    float trns = cntx.Delta * 1.0f;
                    if(Mins > trns) {
                        Mins -= trns;
                    } else {
                        trns = Mins;
                        Mins = 0;
                        Target = null;
                        _State = StateT.Idle;
                    }
                    (St.Host as global::Station).Minerals += trns;
                } else {
                    Mins += cntx.Delta * 0.15f;
                    Power -= cntx.Delta * 1;
                    if(Mins > MaxMins) {
                        Mins = MaxMins;
                        Target = null;
                        _State = StateT.Idle;
                    }
                }
            }

            return false;
        }

        bool act_Attack(ref DroneCntx cntx) {
            if(Target == null || Target.Ar != Ar ) { //or is invalid...
                
                _State = StateT.Idle;
                return true;
            }

            Vector3 seekEp = Target.fd(cntx.FrameInd).Pos + Target.fd(cntx.FrameInd).Vel * 0.25f;
            Vector3 seek = seekEp - cntx.EffPos;


            var arrive = seek;
            var arriveM = arrive.magnitude;

            if(arriveM > Mathf.Epsilon) {
                cntx.DesDir = arrive / arriveM;

                float arrE = ArriveEp;
                arrE *= 1.0f + (1 - Vector3.Dot(cntx.DesDir, cntx.ODesDir)) * 1.5f;
                EffArriveEp = arrE;
                if(arriveM < arrE + MaxVel*0.5f ) {
                    Vector3 dv;
                    if(Vector3.Dot(cntx.DesDir, cntx.ODesDir) > 0.97f) {
                        dv = Vector3.Cross(cntx.DesDir, Random.onUnitSphere) * MaxVel;
                    } else {
                        dv = Vector3.Cross(cntx.DesDir, Vector3.Cross(cntx.ODesDir, cntx.DesDir ) ) * MaxVel;
                    }
                    Debug.DrawLine(cntx.Pos + Vector3.Cross(cntx.ODesDir, cntx.DesDir)*MaxVel, cntx.Pos, Color.cyan);
                    Debug.DrawLine(cntx.Pos + dv, cntx.Pos, Color.yellow);
                    if(arriveM > arrE) {
                        dv = Vector3.Lerp(dv, cntx.DesDir * MaxVel, (arriveM - arrE) / (MaxVel *0.5f) );
                    }
                    cntx.DesVel += dv;
                    Debug.DrawLine( cntx.Pos+dv, cntx.Pos, Color.green );
                } else
                    cntx.DesVel = cntx.DesDir * (arriveM - arrE) *2;

            }

            //cntx.MinVel = 0.8f;

            return false;
        }

        bool act_PilotCtrl(ref DroneCntx cntx) {
            PlayerShipCtrlr ctrlr = Host as PlayerShipCtrlr;

            cntx.DesDir = ctrlr.CamCntrl.transform.forward;
            cntx.DesUp = ctrlr.CamCntrl.transform.up;

            cntx.DesVel += ctrlr.CamCntrl.FlyInput * MaxVel;

            return false;
        }

        void subUp( ref Simulation.FrameCntx fc,  FrameDat c, ref FrameDat n  ) {

            var cntx = new DroneCntx(ref fc, c );
            n = c;

            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, c.Rot, Vector3.one).transpose;

            cntx.EffPos = c.Pos + c.Vel * 0.5f;
            cntx.ODesDir = cntx.DesDir = rotM.GetRow(2);
            cntx.DesUp = rotM.GetRow(1);
            //Vector3 cntx.DesVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;


            float velM = 0;


            float op = Power;

            if(Hp < MaxHp  ) {
                float p = Mathf.Min( 10* cntx.Delta,  Power - MaxPower * 0.1f );
                if(p > 0) {
                    Power -= p;
                    Hp += p * 0.5f;
                    if(Hp > MaxHp)
                        Hp = MaxHp;
                }
            }

            for( int i = 3; i-- > 0; ) {
                switch(State) {
                    case StateT.Mine:
                        if(act_Mine(ref cntx)) continue;
                        break;
                    case StateT.Attack:
                        if(act_Attack(ref cntx)) continue;
                        break;
                    case StateT.Idle:
                        if(act_Idle( ref cntx)) continue;
                        break;
                    case StateT.PilotCtrl:
                        if(act_PilotCtrl( ref cntx)) continue;
                        break;
                    case StateT.EnterArea:
                        //Debug.Log(Host.name + "entered area  " + Ar.name);
                        _State = StateT.Idle;
                        continue;                     
                    case StateT.PrepWarp:
                        var warp = cntx.EffPos - (Ar.transform.position + Random.insideUnitSphere * 0.1f );
                        var wm = warp.sqrMagnitude;


                        if(wm > Mathf.Pow(Ar.Radius * 0.95f, 2)) {
                            var v = WarpTo.transform.position - Ar.transform.position;
                            if(v.sqrMagnitude > 0.01f)
                                cntx.DesDir = v.normalized;
                        }

                        float arr = Ar.Radius * 1.05f;
                        if(wm < Mathf.Pow(arr, 2)) {
                            wm = Mathf.Sqrt(wm);
                            cntx.DesVel = warp * (arr - wm) / wm;

                            if(WarpRdy > -2)
                                WarpRdy = 0;
                        } else {

                            WarpRdy = Mathf.Max(Vector3.Dot(cntx.DesDir, cntx.ODesDir), 0);

                            if(WarpRdy > 0.5f) {
                                if( St != null) {
                                    if(Ws == null )
                                        Ws = St.requestWarp(this, WarpTo);                                        
                                } else if(WarpRdy>0.9f ) {
                                    //Bcn.warp(this, WarpTo);
                                    enterWarp(ref fc );
                                }
                            }                   
                        }
                        break;

                    case StateT.Warping:

                        Vector3 vec = (WarpTo.transform.position - WarpFrom.transform.position);
                        var mag = Mathf.Pow(vec.sqrMagnitude + 1, 0.2f);

                        vec.Normalize();
                        WarpRdy += cntx.Delta / mag;
                        Power -= cntx.Delta * 2;
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

            cntx.DesVel += Avoidance; Avoidance = Vector3.zero;
            velM = cntx.DesVel.magnitude;

            if(cntx.DesDir == cntx.ODesDir && velM > MaxVel *0.1f ) {
                cntx.DesDir = cntx.DesVel / velM;
            }
            if(cntx.DesDir != cntx.ODesDir) {


                Vector3 yAx = Vector3.Cross(cntx.DesDir, rotM.GetRow(0)), xAx = Vector3.Cross(cntx.DesUp, cntx.DesDir);
                yAx = Vector3.Cross(cntx.DesDir, xAx);

                var desRot = Quaternion.LookRotation(cntx.DesDir, yAx);


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

            var nDir = n.Rot * Vector3.forward;

            if(State == StateT.Attack && Power > MaxPower *0.25f ) {
                AimDot = Vector3.Dot(nDir, cntx.DesDir);
                if(Vector3.Dot(nDir, cntx.DesDir) > 0.7f) {
                    Debug.DrawLine(Target.fd(cntx.FrameInd).Pos, cntx.Pos, Color.red);
                    var td = Target as Drone;
                    td.Dmg += 5 * cntx.Delta;
                    Power -= 0.5f * cntx.Delta;
                }
            }

            if(velM > MaxVel) {
                cntx.DesVel *= MaxVel / velM;
                velM = MaxVel;
            } else if(velM < cntx.MinVel * MaxVel) {
                if(velM > Mathf.Epsilon) {
                    cntx.DesVel *= cntx.MinVel * MaxVel / velM;
                    velM = cntx.MinVel * MaxVel;
                } else {
                    cntx.DesVel = cntx.DesDir * (velM = cntx.MinVel * MaxVel);
                }
            }


            var steering = cntx.DesVel - c.Vel;

            var strMag = steering.magnitude;

            if(strMag > Mathf.Epsilon) {
                float effMaxSteer = MaxSteer;
                effMaxSteer *= 0.7f + 0.3f * Mathf.Max(0.2f, Vector3.Dot(nDir, steering / strMag));
                effMaxSteer *= 1.0f + Vector3.Dot(-c.Vel / MaxVel, steering / strMag) * 0.4f;

                if(strMag > effMaxSteer) {
                    steering *= effMaxSteer / strMag;
                    strMag = effMaxSteer;
                }
                EngPow = cntx.Delta * (0.2f + 1.0f * (strMag / MaxSteer));
                Power -= EngPow;
                n.Vel += steering * cntx.Delta;
            } 
            n.Pos += n.Vel * cntx.Delta;

            //Debug.Log("up  "+c.Pos +"   ---> " + n.Pos);

            PowerMod = Power - op;
        }
        /*
        void subUp2(ref Simulation.FrameCntx cntx, FrameDat c, ref FrameDat n) {

            n = c;

            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, c.Rot, Vector3.one).transpose;


            Vector3 cntx.DesVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;
            Vector3 cntx.DesDir = rotM.GetRow(2), cntx.ODesDir = cntx.DesDir;

            float velM = 0;

            var cntx.EffPos = c.Pos + c.Vel * 0.5f;

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
                    var warp = cntx.EffPos - Ar.transform.position;
                    var wm = warp.sqrMagnitude;


                    if(wm > Mathf.Pow(Ar.Radius * 0.95f, 2)) {
                        var v = WarpTo.transform.position - Ar.transform.position;
                        if(v.sqrMagnitude > 0.01f)
                            cntx.DesDir = v.normalized;
                    }

                    float arr = Ar.Radius * 1.05f;
                    if(wm < Mathf.Pow(arr, 2)) {
                        if(wm < Mathf.Epsilon) {
                            cntx.DesVel = Random.onUnitSphere;
                        } else {
                            wm = Mathf.Sqrt(wm);
                            cntx.DesVel = warp * (arr - wm) / wm;
                        }


                        WarpRdy = -3;
                    } else {

                        float wr = Vector3.Dot(cntx.DesDir, cntx.ODesDir);
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

                    Vector3 seek = Target.position - cntx.EffPos;


                    //cntx.DesVel = seek;

                    var arrive = seek;
                    var arriveM = arrive.magnitude;

                    if(arriveM > ArriveEp) {
                        cntx.DesVel = arrive *= (arriveM - ArriveEp) / arriveM;

                    }
                } else Target = null;

            }
            cntx.DesVel += Avoidance; Avoidance = Vector3.zero;
            velM = cntx.DesVel.magnitude;

            if(cntx.DesDir == cntx.ODesDir && velM > Mathf.Epsilon)
                cntx.DesDir = cntx.DesVel / velM;
            if(cntx.DesDir != cntx.ODesDir) {


                Vector3 yAx = Vector3.Cross(cntx.DesDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), cntx.DesDir);
                yAx = Vector3.Cross(cntx.DesDir, xAx);

                var desRot = Quaternion.LookRotation(cntx.DesDir, yAx);


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
                    cntx.DesVel *= MaxVel / velM;
                    velM = MaxVel;
                }

                //}

            }

            var steering = cntx.DesVel - c.Vel;

            var strMag = steering.magnitude;

            if(strMag > Mathf.Epsilon) {
                float effMaxSteer = MaxSteer;
                effMaxSteer *= Mathf.Max(0.5f, Vector3.Dot(rotM.GetRow(2), steering / strMag));
                effMaxSteer *= 1.0f + Vector3.Dot(-cntx.DesVel / MaxVel, steering / strMag);

                if(strMag > effMaxSteer)
                    steering *= MaxSteer / strMag;

                n.Vel += steering * cntx.Delta;
            }
            n.Pos += n.Vel * cntx.Delta;


            //  Debug.Log("up  "+c.Pos +"   ---> " + n.Pos);

        }*/

        [System.NonSerialized]
        public Station.WarpScedhule Ws =null;
        public class DroneGrp {


        }
    }

}