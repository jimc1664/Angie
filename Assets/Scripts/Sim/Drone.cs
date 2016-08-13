using UnityEngine;
using System.Collections;

namespace Sim {

    [System.Serializable]
    public class Drone : Entity {
        public Body Target;
        [System.NonSerialized]
        public Station St;
        public Beacon Bcn;

        public MasterAi.AreaDat AreaD;

        public float MaxVel = 5;
        public float MaxSteer = 0.5f;


        public float MaxAVel = 5;
        public float MaxASteer = 0.5f;

        public float ArriveEp = 0.5f;


        public float Mins = 0;
        float MaxMins = 15;



        public Area WarpTo, WarpFrom;
        public float WarpRdy = 0;

        public Vector3 Avoidance, CollisonPush1, CollisonPush2;


        public Wormhole Wormhole;  //todo remove  - use warpschedule only

        public enum StateT {
            EnterArea,
            Idle,
            Mine,
            Attack,
            PrepWarp,
            Warping,
            PilotCtrl,

            UnderCtor,
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
            private set { Debug.LogError("err"); }
        }
        public StateT initState {
            get { return _State; }
            set { _State = value; }
        }
        public BehaviorT _Behavior = BehaviorT.Mine;
        public BehaviorT Behavior {
            get { return _Behavior; }
            private set { Debug.LogError("err"); }
        }
        public BehaviorT initBehavior {
            get { return _Behavior; }
            set { _Behavior = value; }
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

                if(State == StateT.Mine && Ar != St.Ar) {
                    setWarpTarget(St.Ar);
                }
            }
            return true;
        }
        public void setWarpTarget(Area to) {

            
            Debug.Assert(to != Ar);
            Target = null;
            WarpTo = to;
            WarpRdy = 0;
            _State = StateT.PrepWarp;

            Debug.Log(  name+ " of "+Owner.name + "  warping too " + WarpTo );
        }
        public void enterWarp(ref Simulation.FrameCntx cntx) {
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
            public DroneCntx(ref Simulation.FrameCntx fc, FrameDat c) {
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
        bool act_Idle(ref DroneCntx cntx) {
            Target = null;
            switch(Behavior) {
                case BehaviorT.Mine:
                    if(Mins > MaxMins * 0.9f || (St.Ar == Ar && Mins > 0)) {
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
                        trgs = Ar.GetComponentsInChildren<Asteroid>( true );
                        if(trgs.Length == 0) {
                            if(Ar == St.Ar)
                                setWarpTarget(Ai.getTargetArea());
                            else
                                setWarpTarget( St.Ar );

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
                        var a = St.Ar;
                        if(a == Ar)
                            a= Ai.getTargetArea();

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
        float act_Sub_Arrive(ref DroneCntx cntx, float ep) {

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
                    St.Minerals += trns;
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
            if(Target == null || Target.Ar != Ar) { //or is invalid...

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
                if(arriveM < arrE + MaxVel * 0.5f) {
                    Vector3 dv;
                    if(Vector3.Dot(cntx.DesDir, cntx.ODesDir) > 0.97f) {
                        dv = Vector3.Cross(cntx.DesDir, Random.onUnitSphere) * MaxVel;
                    } else {
                        dv = Vector3.Cross(cntx.DesDir, Vector3.Cross(cntx.ODesDir, cntx.DesDir)) * MaxVel;
                    }
                    Debug.DrawLine(cntx.Pos + Vector3.Cross(cntx.ODesDir, cntx.DesDir) * MaxVel, cntx.Pos, Color.cyan);
                    Debug.DrawLine(cntx.Pos + dv, cntx.Pos, Color.yellow);
                    if(arriveM > arrE) {
                        dv = Vector3.Lerp(dv, cntx.DesDir * MaxVel, (arriveM - arrE) / (MaxVel * 0.5f));
                    }
                    cntx.DesVel += dv;
                    Debug.DrawLine(cntx.Pos + dv, cntx.Pos, Color.green);
                } else
                    cntx.DesVel = cntx.DesDir * (arriveM - arrE) * 2;

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
        bool act_PrepWarp(ref Simulation.FrameCntx fc, ref DroneCntx cntx) {


            var warp = cntx.EffPos - (Random.insideUnitSphere * 0.1f);
            var wm = warp.sqrMagnitude;


            if(Wormhole == null) {
                var v = warp.normalized;

                Vector3 wVec = (WarpTo.transform.position - Ar.transform.position).normalized;

                Wormhole = new Wormhole() {
                    initPos = v * Ar.Radius,
                    initRot = Quaternion.LookRotation(wVec, cntx.DesUp),
                    Rad = 0.25f + Rad * 2.0f,
                    Dir = wVec,
                };
                Wormhole.init(Ar);
            }

            if(Wormhole != null) {
                var v = Wormhole.fd(cntx.FrameInd).Pos - cntx.EffPos;
                if(Wormhole.State <= Wormhole.StateE.Forming)
                    v -= Wormhole.Dir * Rad;
                else
                    v += Wormhole.Dir * Rad;

                cntx.DesVel = v * 2;
                var vm = v.magnitude;
                if(vm < Rad * 4) {
                    if(vm < Rad * 2) {
                        cntx.DesDir = Wormhole.Dir;
                        WarpRdy = 1;
                    } else
                        WarpRdy = 1 - vm / Rad * 2;

                    if(Wormhole.State >= Wormhole.StateE.Forming)
                        if(Wormhole.State == Wormhole.StateE.Warping) {

                        } else
                            WarpRdy = Vector3.Dot(cntx.DesDir, cntx.ODesDir);

                    if(Ws == null)
                        Ws = St.requestWarp(this, WarpTo);
                } else
                    WarpRdy = 0;

            } else {
                Debug.LogError("old...");
                if(wm > Mathf.Pow(Ar.Radius * 0.95f, 2)) {
                    var v = WarpTo.transform.position - Ar.transform.position;
                    if(v.sqrMagnitude > 0.01f)
                        cntx.DesDir = v.normalized;
                }

                float arr = Ar.Radius * 1.05f;
                if(wm < Mathf.Pow(arr, 2)) {
                    wm = Mathf.Sqrt(wm);
                    cntx.DesVel += warp * (arr - wm) / wm;
                    WarpRdy = 0;
                } else {

                    WarpRdy = Mathf.Max(Vector3.Dot(cntx.DesDir, cntx.ODesDir), 0);
                    WarpRdy = 0.5f;
                    if(WarpRdy > 0.1f) {
                        if(St != null) {
                            if(Ws == null)
                                Ws = St.requestWarp(this, WarpTo);
                        } else if(WarpRdy > 0.9f) {      ///beacon stuff begone...
                            //Bcn.warp(this, WarpTo);
                            enterWarp(ref fc);
                        }
                    }
                }
            }
            return false;
        }
        bool act_Warping(ref DroneCntx cntx, ref FrameDat n) {

            Vector3 vec = (WarpTo.transform.position - WarpFrom.transform.position);
            var mag = Mathf.Pow(vec.sqrMagnitude + 1, 0.3333f);

            vec.Normalize();

            if(Wormhole == null) {
                Wormhole = new Wormhole() {
                    initPos = -(vec + Random.insideUnitSphere * 0.3f) * WarpTo.Radius,
                    initRot = Quaternion.LookRotation(-vec, cntx.DesUp),
                    Rad = 0.25f + Rad * 2.0f,
                    Dir = -vec,
                };
                Wormhole.init(WarpTo);
            }

            Power -= cntx.Delta * 2;
            var v1 = WarpFrom.transform.position + vec * WarpFrom.Radius;
            var v2 = WarpTo.transform.position - vec * WarpTo.Radius;
            Debug.DrawLine(v1, v2, Color.cyan);

            switch(Wormhole.State) {
                case Wormhole.StateE.Hypothetical:
                    WarpRdy += cntx.Delta / mag;
                    if(WarpRdy >= 1) {
                        Wormhole.State = Wormhole.StateE.Forming;
                        Wormhole.StateMd = 0;
                        WarpRdy = 1;

                        var whh = Wormhole.Host as global::Wormhole;
                        if(whh != null)
                            whh.Vis.gameObject.SetActive(true);
                    }
                    n.Pos = Vector3.Lerp(v1, v2, 0.1f + WarpRdy * 0.8f);
                    n.Rot = Quaternion.LookRotation(vec, cntx.DesUp);
                    return true;
                case Wormhole.StateE.Forming:
                    Wormhole.StateMd += cntx.Delta * 2;
                    if(Wormhole.StateMd > 1) {
                        cntx.Sim.DSwap.Add(new Simulation.DroneSwap() { Dr = this, A = Simulation.Singleton.WarpingDrones, Ar = WarpTo, B = WarpTo.Drones });
                        //Debug.Log("swap? "+cntx.FrameInd );
                        n.Pos = Wormhole.fd(cntx.FrameInd).Pos;
                        n.Vel = vec * MaxVel * 0.1f;
                        n.AVel = Quaternion.identity;

                        WarpFrom = WarpTo = null;
                        _State = StateT.Idle;

                        Wormhole.State = Wormhole.StateE.Deforming;
                        Wormhole.StateMd = 0;
                        Wormhole = null;
                        WarpRdy = 0;
                        return true;
                    }
                    break;
            }
            return false;
        }

        void updateStateStuff( ref Simulation.FrameCntx fc, ref DroneCntx cntx, FrameDat c, ref FrameDat n) {
            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, c.Rot, Vector3.one).transpose;

            cntx.EffPos = c.Pos + c.Vel * 0.5f;
            cntx.ODesDir = cntx.DesDir = rotM.GetRow(2);
            cntx.DesUp = rotM.GetRow(1);
            //Vector3 cntx.DesVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;

            float velM = 0;
            for(int i = 3; i-- > 0;) {
                switch(State) {
                    case StateT.Mine:
                        if(act_Mine(ref cntx)) continue;
                        break;
                    case StateT.Attack:
                        if(act_Attack(ref cntx)) continue;
                        break;
                    case StateT.Idle:
                        if(act_Idle(ref cntx)) continue;
                        break;
                    case StateT.PilotCtrl:
                        if(act_PilotCtrl(ref cntx)) continue;
                        break;
                    case StateT.EnterArea:
                        //Debug.Log(Host.name + "entered area  " + Ar.name);
                        _State = StateT.Idle;
                        continue;
                    case StateT.PrepWarp:
                        if(act_PrepWarp( ref fc, ref cntx)) continue;
                        break;
                    case StateT.Warping:
                        if(act_Warping(ref cntx, ref n)) return;
                        break;
                    case StateT.UnderCtor:
                        return;
                }
                break;
            }

            float avReduct = Mathf.Clamp(Vector3.Dot(-Avoidance, cntx.DesVel), 0, MaxVel * 0.5f);
            Avoidance = Avoidance * (MaxVel - avReduct);
            cntx.DesVel += Avoidance; Avoidance = Vector3.zero;

            velM = cntx.DesVel.magnitude;

            if(cntx.DesDir == cntx.ODesDir && velM > MaxVel * 0.1f) {
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

            if(State == StateT.Attack && Power > MaxPower * 0.25f) {
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
            n.Vel += CollisonPush1;
            n.Pos += CollisonPush2;
            CollisonPush1 = CollisonPush2 = Vector3.zero;
            n.Pos += n.Vel * cntx.Delta;
        }
        void subUp(ref Simulation.FrameCntx fc, FrameDat c, ref FrameDat n) {


            var cntx = new DroneCntx(ref fc, c);

            n = c;

            float op = Power;

            if(Hp < MaxHp) {
                float p = Mathf.Min(10 * cntx.Delta, Power - MaxPower * 0.1f);
                if(p > 0) {
                    Power -= p;
                    Hp += p * 0.5f;
                    if(Hp > MaxHp)
                        Hp = MaxHp;
                }
            }


            updateStateStuff(ref fc, ref cntx, c, ref n);
         

            //Debug.Log("up  "+c.Pos +"   ---> " + n.Pos);

            PowerMod = Power - op;
        }

        [System.NonSerialized]
        public Station.WarpScedhule Ws = null;
        public class DroneGrp {


        }

        public void init( Station st) {

            St = st;
            Owner = st.Owner;
            St.Dependants.Add(this);

            Ai = st.Ai;
            //  Target = null;


            if(_Behavior != Sim.Drone.BehaviorT.Mine ) {
                St.FighterCnt++;
                
            } else
                St.MinerCnt++;

            st.Ar.addDrone(this);
        }
    }

}
public class Drone : Body {

   // public Transform Target;
   // public Station St;

    public Vector3 Vel;
    public Quaternion AVel = Quaternion.identity;


    public float MaxVel = 5;
    public float MaxSteer = 0.5f;


    public float MaxAVel = 5;
    public float MaxASteer = 0.5f;

    public float ArriveEp = 0.5f;

   // public float Rad = 0.2f;

    public Sim.Drone Drn;

    public bool Fighter = false;

    void Awake() {
    //    Trnsfrm = transform;
    }
    public Sim.Drone buildSimDrone( Vector3 pos, Quaternion rot, Sim.Station st, Drone host ) {
        var sim = Simulation.Singleton;
        var ret = new Sim.Drone() {
            name = name,
            initPos = pos,
            initRot = rot,

            ArriveEp = ArriveEp,
            MaxVel = MaxVel * sim.Glob_KineticScale,
            MaxSteer = MaxSteer * sim.Glob_KineticScale * sim.Glob_AccelScale,
            MaxAVel = MaxAVel * sim.Glob_KineticScale * sim.Glob_RotScale,
            MaxASteer = MaxASteer * sim.Glob_KineticScale * sim.Glob_AccelScale * sim.Glob_RotScale,

            Rad = Rad,

            MaxPower = 50,
            Power = 10,
            //Hp = 10,

            initBehavior = !Fighter ? Sim.Drone.BehaviorT.Mine : Sim.Drone.BehaviorT.Hunt,
            initState = Sim.Drone.StateT.UnderCtor,

            Host = host,
        };
        if(host) host.Drn = ret;

        ret.init(st);
        return ret;
    }
    public override void init() {
        var ar = GetComponentInParent<Sim.Area>();

        var st = ar.GetComponentInChildren<Station>( true );
        st.init();

        var sim = Simulation.Singleton;
        Drn = buildSimDrone(transform.localPosition, transform.rotation, st.Sm, this );
        Drn.initState = Sim.Drone.StateT.Idle;
    }

    public void init(Sim.Drone d) {
        d.Host = this;
        Drn = d;
    }

    protected new void OnEnable() {

        base.OnEnable();
        //Trnsfrm = transform;


    }

    void Update() {

        if(Drn != null) {
            var fd = Drn.fdSmooth(Simulation.Singleton);

            Trnsfrm.localPosition = fd.Pos;
            Trnsfrm.localRotation = fd.Rot;
            Vel = fd.Vel;
            AVel = fd.AVel;
        }
    }

    void OnDrawGizmos() {
        Trnsfrm = transform;
        
        Gizmos.color = Color.green;
       // if(Target != null)
        //    Gizmos.DrawLine(Trnsfrm.position, Target.position);

        Gizmos.color = Color.blue;
       // Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + Vel );
        

        var rot = Trnsfrm.rotation;
        Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, rot, Vector3.one).transpose;


        Vector3 desVel = Vector3.zero;
        Quaternion desAvel = Quaternion.identity;
        Vector3 desDir = rotM.GetRow(2);

        /*
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + desDir);

        
        Vector3 yAx = Vector3.Cross(desDir, rotM.GetRow(0)), xAx = Vector3.Cross(rotM.GetRow(1), desDir);
        yAx = Vector3.Cross(desDir, xAx);


        Gizmos.color = Color.green;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + yAx);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Trnsfrm.position, Trnsfrm.position + xAx);
        */

        Gizmos.color = Color.white;
        if( Drn != null && Drn.Owner)  Gizmos.color = Drn.Owner.Col;
        Gizmos.DrawWireSphere(Trnsfrm.position, Rad);
    }


}
