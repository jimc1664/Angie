using UnityEngine;
using System.Collections.Generic;

public class BuzzerTest : MonoBehaviour {

    public GameObject BuzzBase, Cap, Weapon, Engine;
    public List<Material> Mats;



    public bool Gen, Sim;
    public int Weps = 2, Thrusters = 3, LastBC = 0, Extra = 0;
    public float ExpMod = 1.5f, ExpScl = 0.1f;

    public Vector3[] OffsetAngs = {
        new Vector3( 120,0,0 ),
        new Vector3( 30,120,0 ),
        new Vector3( 30,-120,0 ),
        new Vector3( 0,0,0 ),
    };


    public Vector3[] Offsets = new Vector3[4];

    void gen() {

        for(int i = 4; i-- > 0;) {
            Offsets[i] = Quaternion.Euler(OffsetAngs[i]) * Vector3.forward;
            Debug.Log(" ang = " + Vector3.Angle(Vector3.forward, Offsets[i]));
        }

        Gen = false;

        foreach(Transform t in transform)
            Destroy(t.gameObject);
        Nodes.Clear();


        Slot.Clear();
       // Slot.Add(Vector3.back*0.1f );

        int bc = Weps + Thrusters;
        bc += Mathf.FloorToInt(Mathf.Pow(bc, ExpMod) * ExpScl) + Extra;
        LastBC = bc;
        for(int i = bc; i-- > 0;)
            section(BType.Base);


        for(int i = Thrusters; i-- > 0;)
            section(BType.Thruster);

        for(int i = Weps; i-- > 0;)
            section(BType.Weapon);

    }

    List<PlaceSlot> Slot = new List<PlaceSlot>();

    class PlaceSlot {
        public Vector3 P;

        public List<Node> Nbrs;
    };
    void section(BType typ) {
        var go = Instantiate(BuzzBase);
        var trn = go.transform;

        int cc = 4;
        var pos = Random.insideUnitSphere * 4;
        Transform sub;
        switch(typ) {
            case BType.Weapon:
                sub = Instantiate(Weapon).transform;
                pos.z = Mathf.Abs(pos.z) *2;
                cc = 3;
                break;
            case BType.Thruster:
                sub = Instantiate(Engine).transform;
                cc = 3;
                break;
            default:
                sub = Instantiate(Cap).transform;
                break;
        }

        sub.parent = trn;

        var mi = Random.Range(0, Mats.Count);
        if(mi != 0) {
            var mat = Mats[mi];

            foreach(var m in go.GetComponentsInChildren<MeshRenderer>()) {
                if(m.sharedMaterial == Mats[0])
                    m.sharedMaterial = mat;
            }
        }


       trn.parent = transform;

        Node.Connection [] conns = new Node.Connection[cc];
        Node nd = new Node() {
            Trn = trn,
            Typ = typ,

            Pos = trn.localPosition,
            Rot = trn.localRotation,
            AVel = Quaternion.identity,
            MaxVel = MaxVel,
            MaxAVel = MaxAVel,
            MaxASteer = MaxASteer,
            MaxSteer = MaxSteer,

            Depth = Nodes.Count,
            Island = Nodes.Count,
            Connections = conns,
            ConCnt = 0,
        };

        cc = 0;
        for(;;) {
            if(Slot.Count == 0) {
                nd.Pos = Vector3.zero;
                nd.Rot = Quaternion.LookRotation(Vector3.forward, Random.onUnitSphere);
                Debug.Assert(Nodes.Count == 0);
                break;
            }
            var plc = Slot[0]; ;// Random.insideUnitSphere * 4;
            Slot.RemoveAt(0);
            var p = plc.P;

            switch(plc.Nbrs.Count) {
                case 1:


                    break;

                default:
                    Debug.LogError("err");
                    break;
            }

            break;
        }


        for(int i = Slot.Count; i-- > 0;) {
            var slt = Slot[i];
            var mag = (slt.P - nd.Pos).sqrMagnitude;

            if(mag > Math_JC.pow2(1.3f))
                continue;
            if(mag < 0.95f * 0.95f || slt.Nbrs.Count >= 4 ) {

                Slot.RemoveAt(i);
            } else {

                slt.Nbrs.Add(nd);
            }
        }

        nd.ConCnt = cc;

        for(; cc-- > 0;)
            conns[cc].N2.ConCnt++;



        trn.localPosition = nd.Pos;
        trn.localRotation = nd.Rot;


        Nodes.Add(nd);

        
        for(int i = 4; i-- > 0;) {
            if(conns[i] != null) continue;
            var p = nd.Pos + nd.Rot * Offsets[i];

            List<Node> sn = new List<Node>();
            foreach(var n in Nodes) {
                var mag = (n.Pos - p).sqrMagnitude;

                if(mag > Math_JC.pow2(1.3f))
                    continue;
                if(mag < 0.95f * 0.95f)
                    goto label_breakContinue;

                if(sn.Count >= 4 || n.ConCnt >= n.Connections.Length)
                    goto label_breakContinue;

            }
            
        
            Slot.Add( new PlaceSlot() {
                P = p,
                Nbrs = sn,
            } );

            label_breakContinue:;
        }
    }

    public enum BType {
        Base,
        Thruster,
        Weapon,
    }   

    public class Node {
        public Transform Trn;
        public BType Typ;


        public Vector3 Pos, OPos, Vel, OVel;
        public Quaternion Rot, ORot, AVel;

        public float MaxVel, MaxSteer, MaxASteer, MaxAVel;

        public Vector3 CollisonPush1, CollisonPush2, Avoidance;

        [System.Serializable]
        public class Connection {
            public Node N1, N2;
            [HideInInspector]
            public int Ci1, Ci2;

            public float Dis, Strength= 0.1f;

            public void remove() {
                N1.Connections[Ci1] = null;
                N2.Connections[Ci2] = null;
            }

            public Node opp(Node t) {
                return t == N1 ? N2 : N1;
            }
        };


        public Connection[] Connections;
        public int ConCnt = 0;
        //public int MaxCon = 4;
        public float Depth;

        public int Island, OIsland;
        public void update(BuzzerTest bt) {

            Pos = Trn.localPosition;
            Rot = Trn.localRotation;

            //  var rot = Rot;
            Matrix4x4 rotM = Matrix4x4.TRS(Vector3.zero, Rot, Vector3.one).transpose;

            var EffPos = Pos + Vel * 0.5f;
            Vector3 DesDir = rotM.GetRow(2);
            Vector3 DesUp = rotM.GetRow(1);
            //Vector3 DesVel = Vector3.zero;
            Quaternion desAvel = Quaternion.identity;

            Vector3 DesVel = Vector3.zero;

            float baseConRotMod = 0.8f;
            switch(Typ) {
                case BType.Thruster:
                    DesDir = Vector3.back;
                    baseConRotMod = 0.2f;
                    break;
                case BType.Weapon:
                    DesDir = (Pos +Vector3.forward*2.0f) .normalized;
                    baseConRotMod = 0.3f;
                    break;
            }

            float avReduct = Mathf.Clamp(Vector3.Dot(-Avoidance, DesVel), 0, MaxVel * 0.5f);
            Avoidance = Avoidance * (MaxVel - avReduct);
            DesVel += Avoidance; Avoidance = Vector3.zero;

            var velM = DesVel.magnitude;
            var invRot = Quaternion.Inverse(Rot);

            
            Vector3 yAx = Vector3.Cross(DesDir, rotM.GetRow(0)), xAx = Vector3.Cross(DesUp, DesDir);
            yAx = Vector3.Cross(DesDir, xAx);

            var desRot = Quaternion.LookRotation(DesDir, yAx);
          
            float conRotMod = 0;

            for(int i = Connections.Length; i-- > 0;) {
                var c = Connections[i];
                if(c == null) continue;
                var o = c.N1 == this ? c.N2 : c.N1;

               // if(o.Depth < Depth + 1)
                //    Depth = o.Depth + 1;
               // if(c.N1 != this) continue;
                var vec = o.OPos - Pos;
                var vn = vec.normalized;
                if(vec.magnitude > 2) {
                    c.remove();
                    continue;

                }

                //var dVec = rotM.MultiplyVector(bt.Offsets[i]);
                var dVec = Rot* bt.Offsets[i] ;
                var dt = Vector3.Dot(dVec, vn);

                float ep = 0.95f;
                float conMod = 0.2f + 0.8f * (dt >= ep ?
                    (ep - dt) / -(1 - ep) :
                    (ep - dt) / ep   );

                conMod *= c.Strength;
                if(dt < 0.99f) {
                    DesDir = vn;
                    Vector3 cYAx = Vector3.Cross(vn, dVec);//, xAx = Vector3.Cross(DesUp, DesDir);
                                                           // yAx = Vector3.Cross(DesDir, xAx)
                    if(bt.DrawFriendLines) {
                        Debug.DrawLine(Trn.position, Trn.position + cYAx, Color.cyan);
                        Debug.DrawLine(Trn.position + dVec, Trn.position + vn, Color.red);
                        Debug.DrawLine(Trn.position + dVec, Trn.position, Color.magenta);
                    }
                    var aAng = Mathf.Acos(dt);
                    var cDesAvel = Quaternion.AngleAxis(-aAng * Mathf.Rad2Deg, cYAx);
                    var cDesRot = cDesAvel * Rot;

                    cDesAvel = invRot * cDesRot;
                    cDesRot = Rot * cDesAvel;

                    float mod = baseConRotMod * conMod * (1- conRotMod);
                    conRotMod += mod;
                    desRot = Quaternion.Slerp(desRot, cDesRot, mod );
                    //conRotMod /= (1 + baseConRotMod);
                }
                vec += (o.ORot * bt.Offsets[c.N1 == this ? c.Ci1 : c.Ci2] - dVec ) * 0.5f;
                var d = vec.magnitude;
                vec /= d;
                DesVel += (vec *(d-0.15f)  + (o.OVel - Vel)*0.3f  )* bt.ConnectionPull * conMod;
            }

            desAvel = invRot * desRot;
            float ang = Quaternion.Angle(desAvel, Quaternion.identity);
            if(ang > MaxAVel)
                desAvel = Quaternion.Slerp(Quaternion.identity, desAvel, MaxAVel / ang);
            //var nRot = rot * desAvel;

            var a2 = Quaternion.Angle(AVel, desAvel);
            if(a2 > MaxASteer)
                AVel = Quaternion.Slerp(AVel, desAvel, MaxASteer / a2);
            else
                AVel = desAvel;

            Rot *= Quaternion.Slerp(Quaternion.identity, AVel, 0.2f);
            Rot = Rot.normalised();

            var nDir = Rot * Vector3.forward;

            if(velM > MaxVel) {
                DesVel *= MaxVel / velM;
                velM = MaxVel;
            }

            var steering = DesVel - Vel;

            var strMag = steering.magnitude;

            if(strMag > Mathf.Epsilon) {
                float effMaxSteer = MaxSteer;        
                effMaxSteer *= 1.0f + Vector3.Dot(-Vel / MaxVel, steering / strMag) * 0.4f;

                if(strMag > effMaxSteer) {
                    steering *= effMaxSteer / strMag;
                    strMag = effMaxSteer;
                }
                var EngPow = Time.fixedDeltaTime * (0.2f + 1.0f * (strMag / MaxSteer));
                // Power -= EngPow;
                Vel += steering * Time.fixedDeltaTime;
            }
            Vel += CollisonPush1;
            Pos += CollisonPush2;
            CollisonPush1 = CollisonPush2 = Vector3.zero;
            Pos += Vel * Time.fixedDeltaTime;


            Trn.localPosition = Pos;
            Trn.localRotation = Rot;
        }
    };

    List<Node> Nodes = new List<Node>();

    public float MaxVel = 0.1f, MaxAVel = 0.1f, MaxSteer = 0.1f, MaxASteer = 0.1f;
    public float AvoidEp = 0.75f, AvoidEp2 = 0.5f, AvoidEp3 = 0.1f, AvoidPow = 6, ColBnc = 0.3f, ColDamp = 0.6f, ColRefDamp = 0.8f, ConeAv = 3.0f, ConnectionPull = 8.0f;
    public float PosFactor = 1, ThrusterAng = 30.0f;
    public float WeaponAng = 80.0f;

    void mergeIsland(Node b1, Node b2) {
        b1.Island = b2.Island = Nodes[Mathf.Min(b1.Island, b2.Island)].Island;
    }

    bool bestConnectionCi( Node b1, Node b2, Vector3 vn, float ds, ref int ci, ref int curI, ref float cd  ) {

       // int ci = -1;
       // float cd = float.MaxValue;
        bool ret = false;
        for(int i = b1.Connections.Length; i-- > 0;) {

            var dir = b1.Trn.localRotation * Offsets[i];
            var dt = Vector3.Dot(dir, vn);

            var d = ds / dt;

           // ds += (b1.Depth + b2.Depth) * 20.0f;

            var c = b1.Connections[i];
            if(c != null) {
                if(c.N1 != b2 && c.N2 != b2) {
                    if(c.Dis < d)
                        continue;
                } else {
                    curI = i;
                }
            }
            if(dt < 0.2f) continue;
            if(d > cd) continue;

            cd = d;
            ci = i;
            ret = true;
        }

        return ret;
    }

    bool isExtendedNeighbour(Node b1, Node b2,  float mm, ref float ep) {

        foreach(var c1 in b1.Connections) {
            if(c1 == null) continue;
            var n = c1.opp(b1);
            TConList.Add(n);
        }
        if(TConList.Count >= 0) {
            foreach(var c2 in b2.Connections) {
                if(c2 == null) continue;
                if(TConList.Contains(c2.opp(b2))) {
                    ep = ep1 + AvoidEp2 * mm;
                    TConList.Clear();
                    return true;
                }
            }
            TConList.Clear();
        }
        return false;
    }


    List<int> TC = new List<int>();
    bool connectionProc(Node b1, Node b2, Vector3 vn, float ds, float mm, ref float ep ) {
        int ci1 = 0, ci2 = 0 , curI1 = -1, curI2 = -1;

        if(b1.Typ != BType.Base && b2.Typ != BType.Base)
            return isExtendedNeighbour( b1,b2, mm, ref ep );
        float cd1 = float.MaxValue, cd2 = float.MaxValue;
        if(!bestConnectionCi(b1, b2, -vn, ds, ref ci1, ref curI1, ref cd1) ||
            !bestConnectionCi(b2, b1, vn, ds, ref ci2, ref curI2, ref cd2 )) return isExtendedNeighbour(b1, b2, mm, ref ep);

        bool  cc1 = curI1 != -1, cc2 = curI2  != -1 ;


        float d = Mathf.Max(cd1, cd2); ;
        if(cc1) {
            if(curI1 == ci1 && curI2 == ci2) {
                var c = b1.Connections[ci1];
                c.Dis = Mathf.Max(cd1, cd2);



                //c.Strength = Mathf.Lerp(c.Strength, 1.0f - Mathf.Min( Mathf.Pow(c.Dis - 0.9f, 1.5f), 0.9f), 0.05f );
                if(c.Dis > ConThresh) {
                    c.Strength = Mathf.Lerp(c.Strength, 0.0f, ConSpeed );
                    if(c.Strength < 0.1f) {
                        c.remove();
                    } else {
                        ep = ep1 + AvoidEp3 * mm;
                        return true;
                    }
                } else {
                    c.Strength = Mathf.Lerp(c.Strength, 1.0f, ConSpeed);
                    ep = ep1 + AvoidEp3 * mm;

                    mergeIsland(b1, b2);
                    return true;
                }
            }
            if(cc2)
                b2.Connections[curI2] = null;
            b1.Connections[curI1] = null; //lazy - could fix..        
        } else if(cc2)
            b2.Connections[curI2] = null;
        else if(b1.OIsland == b2.OIsland && d > ConThresh ) {
            return false;
        }
       
       // TC.Clear();
        float nDis = float.MaxValue ;
        foreach(var c1 in b1.Connections) {
            if(c1 == null) continue;
            var n = c1.opp(b1);

            if(c1.Dis < nDis)
                nDis = c1.Dis;

            TConList.Add(n);
        }
        if(TConList.Count >= 0) {
            bool extended = false;

            foreach(var c2 in b2.Connections) {
                if(c2 == null) continue;
                if(c2.Dis < nDis)
                    nDis = c2.Dis;
                if(TConList.Contains(c2.opp(b2))) {
                    if(nDis < d) {
                        ep = ep1 + AvoidEp2 * mm;
                        TConList.Clear();
                        return true;
                    }
                    extended = true;
                }
            }

            if(extended) {
                Debug.Log("fixing....");

                var l2 = new List<Node>();
                foreach(var c2 in b2.Connections) {
                    if(c2 == null) continue;
                    if(TConList.Contains(c2.opp(b2))) {
                        l2.Add(c2.opp(b2));
                        c2.remove();
                    }
                }
                foreach(var c1 in b1.Connections) {
                    if(c1 == null) continue;
                    if(l2.Contains(c1.opp(b1)))
                        c1.remove();
                }
            }
            TConList.Clear();
       }



     
        if(cd2 > cd1) {
            d = cd2;
            var c = b1.Connections[ci1];
            if(c != null) {
                if(c.Dis < d) return false;
                c.remove();
                if(b2.Connections[ci2] != null)
                    b2.Connections[ci2].remove();
            }
        } else {
            var c = b2.Connections[ci2];
            if(c != null) {
                if(c.Dis < d) return false;
                c.remove();
                if(b1.Connections[ci1] != null)
                    b1.Connections[ci1].remove();
            }
        }

        b1.Connections[ci1] = b2.Connections[ci2] = new Node.Connection() {
            N1 = b1, N2 = b2, Ci1 = ci1, Ci2 = ci2, Dis = d,
            Strength = 0.5f,
        };
        ep = ep1 + AvoidEp3 * mm;
        return true;
    }

     List<Node> TConList = new List<Node>();

    const float ep1 = 0.9f;

    void avoidanceAndCollison(Node b1, Node b2,
        Vector3 p1, Vector3 p2, Vector3 vel1, Vector3 vel2, Vector3 p1_2, Vector3 p2_2, float mm, float m1, float m2, float ms1, float ms2, float avR,
        ref Vector3 d1Cp1, ref Vector3 d1Cp2, ref Vector3 d2Cp1, ref Vector3 d2Cp2, ref Vector3 d1Av, ref Vector3 d2Av
        ) {

        var vec = p1 - p2;
        var mag = (vec).sqrMagnitude;
        //var mm = 1.0f + Mathf.Abs((d1.Vel / d1.MaxVel).sqrMagnitude - (d2.Vel / d2.MaxVel).sqrMagnitude);    
        // (b1.Rad + b2.Rad);

        float avoidEp = AvoidEp;
   
        avoidEp = (avR *1.0f + 1.0f) * avoidEp; ///max

        avoidEp *= mm;
        avoidEp += ep1;
        if(mag < Mathf.Pow(avoidEp, 2)) {
            mag = Mathf.Sqrt(mag);
            vec /= mag + Mathf.Epsilon;

            if(connectionProc(b1, b2, vec, Mathf.Max( mag, ep1 ), mm, ref avoidEp )) {
                if(mag > avoidEp )
                    return;                
            }

            var noise = Random.onUnitSphere * 0.05f;
            vec += noise;

            // Vector3 p1_2 = d1.Pos + d1.Vel * fc.Delta, p2_2 = d2.Pos + d2.Vel * fc.Delta;
            var v2 = p1_2 - p2_2;
            if(v2.sqrMagnitude < Mathf.Pow(ep1, 2)) {
                float v2m = v2.magnitude;
                var v2n = v2 / (v2m + Mathf.Epsilon);
                v2 = (ep1 - v2m) * v2n;  //force non zero
                v2 += noise*0.1f;
                float bnc = ColBnc;
                v2 *= ColDamp;
                //float mass = d1.Rad + d2.Rad, ms1 = d2.Rad / mass, ms2 = d1.Rad / mass;
                //d1.CollisonPush2 += v2 * (1 - bnc) * ms1;
                //d2.CollisonPush2 -= v2 * (1 - bnc) * ms2;
                d1Cp2 += v2 * (1 - bnc) * ms1 ;
                d2Cp2 -= v2 * (1 - bnc) * ms2;
                v2 = v2 * (bnc / 0.1f);

                //d1.CollisonPush1 += (v2 - d1.Vel) * 0.8f * ms1;
                //d2.CollisonPush1 -= (v2 - d2.Vel) * 0.8f * ms2;

                var velDif = (vel1 - vel2)* ColRefDamp;
                // Vector3.Reflect( vel1 /v1m, v2n) * v1m ;
                d1Cp1 += (velDif - vel1)  *0.1f* ms1;
                d2Cp1 += (-velDif - vel2)  *0.1f* ms2;
            }

            vec *= (avoidEp - mag) * 0.5f;
            vec *= AvoidPow;
            //var m1 = d1.Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
            //var m2 = d2.Vel.sqrMagnitude / (d2.MaxVel * d2.MaxVel);
            //float mt = 1 + m1 + m2;
            //m1 = (1 + m1) / mt;
            //m2 = (1 + m2) / mt;
            //d1.Avoidance += vec * m1;
            //d2.Avoidance -= vec * m2;
            d1Av += vec * m1;
            d2Av -= vec * m2;
        }
    }

    void avoidProc( Node n1, Node n2,
        Vector3 p1, Vector3 p2, Vector3 vel1, Vector3 vel2, Vector3 p1_2, Vector3 p2_2, float mm, float m1, float m2, float ms1, float ms2, float avR,
        ref Vector3 d1Cp1, ref Vector3 d1Cp2, ref Vector3 d2Cp1, ref Vector3 d2Cp2, ref Vector3 d1Av, ref Vector3 d2Av
        ) {

        /*
       
        var sm = vec.sqrMagnitude;
        if(  n1.Friend == n2 ) {
         //   n1.FriendDis = sm * FriendEp;
        } else if( sm< n1.FriendDis&& n2.Friend != n1  && ( n2.Typ == BType.Base ) ) {
            n1.Friend = n2;
            n1.FriendDis = sm * FriendEp;
        } */

            float ang;
        Vector3 fwd;
        switch(n1.Typ) {
            case BType.Thruster:
                ang = ThrusterAng;
                fwd = Vector3.back;
                break;
            case BType.Weapon:
                ang = WeaponAng;
                fwd = n1.Trn.forward;
                break;
            default:
                return;
        }
        var vec = p2 - p1 +fwd *0.3f;

        var vn = vec.normalized;


        var dtLim = Mathf.Cos(ang * Mathf.Deg2Rad);
        var dt = Vector3.Dot(vec, fwd);
        if( dt > dtLim) {
            float md = (1.0f + dt );
            var tan = ( Vector3.Cross(Vector3.Cross(fwd, vn),vn) * md * md - fwd*0.3f ).normalized;

            md *= ConeAv*PosFactor;
            if(n1.Typ == n2.Typ) {
                d2Av += tan * m2 * md;
            } else {
                d1Av += fwd * m1 * md;
                d2Av += tan * m2 * md * 0.2f;
            }
        }
    }

    static float FriendEp = 0.95f;

    public Vector3 Shape;
    void FixedUpdate() {
        if(Gen) gen();

        if(!Sim) return;


        //St.updateSt(ref fc);


        Vector4 wepAvg = Vector3.zero, thrusterAvg = Vector4.zero, baseAvg = Vector4.zero;
        WepAvg= BaseAvg =ThrusterAvg =0;
        float BaseMax = 0;
        Shape = Vector3.zero;
        for(int i = Nodes.Count; i-- > 0;) {
            var d1 = Nodes[i];
            var p1 = d1.Pos + d1.Vel * 0.25f;

            d1.OPos = d1.Pos;
            d1.OVel = d1.Vel;
            d1.ORot = d1.Rot;
            d1.OIsland = d1.Island;
            d1.Island = i;
                //   if(d1.Depth > 1.5f)
                Shape += p1.abs();
            Vector4 p4 = p1; p4.w = 1;
            switch(d1.Typ) {
                case BType.Base:
                    baseAvg += p4;
                    BaseAvg += p1.sqrMagnitude;
                    BaseMax = Mathf.Max(p1.sqrMagnitude, BaseMax);
                    break;
                case BType.Weapon:
                    wepAvg += p4;
                    WepAvg += p1.sqrMagnitude;
                    break;
                case BType.Thruster:
                    thrusterAvg += p4;
                    ThrusterAvg += p1.sqrMagnitude;
                    break;
            }

            for(int j = i; j-- > 0;) {
                var d2 = Nodes[j];
                var p2 = d2.Pos + d2.Vel * 0.25f;
                var mm = 1.0f + Mathf.Abs((d1.Vel / d1.MaxVel).sqrMagnitude - (d2.Vel / d2.MaxVel).sqrMagnitude);
                Vector3 p1_2 = d1.Pos + d1.Vel * Time.fixedDeltaTime, p2_2 = d2.Pos + d2.Vel * Time.fixedDeltaTime;
                // float mass = d1.Rad + d2.Rad, ms1 = d2.Rad / mass, ms2 = d1.Rad / mass;
                float ms1 = 1, ms2 = 1;
                var m1 = d1.Vel.sqrMagnitude / (d1.MaxVel * d1.MaxVel);
                var m2 = d2.Vel.sqrMagnitude / (d2.MaxVel * d2.MaxVel);
                float mt = 1 + m1 + m2;
                m1 = (1 + m1) / mt * ms1;
                m2 = (1 + m2) / mt * ms2;
                //float avR = Mathf.Max(d1.Rad, d2.Rad);
                float avR = 0.5f;
                //mm = m1 = m2 = 1;
                avoidProc(d1, d2, p1, p2, d1.Vel, d2.Vel,
                    p1_2, p2_2, mm, m1, m2, ms1, ms2, avR, ref d1.CollisonPush1, ref d1.CollisonPush2, ref d2.CollisonPush1, ref d2.CollisonPush2, ref d1.Avoidance, ref d2.Avoidance);

                avoidProc(d2, d1, p2, p1, d2.Vel, d1.Vel,
                    p2_2, p1_2, mm, m2, m1, ms2, ms1, avR, ref d2.CollisonPush1, ref d2.CollisonPush2, ref d1.CollisonPush1, ref d1.CollisonPush2, ref d2.Avoidance, ref d1.Avoidance);

                avoidanceAndCollison(d1, d2, p1, p2, d1.Vel, d2.Vel,
                    p1_2, p2_2, mm, m1, m2, ms1, ms2, avR, ref d1.CollisonPush1, ref d1.CollisonPush2, ref d2.CollisonPush1, ref d2.CollisonPush2, ref d1.Avoidance, ref d2.Avoidance);

            }

        }
        Shape /= Nodes.Count;
        Shape += new Vector3(2, 2, 0);
        float shapeMax = Shape.maxAbsD();

        WepAvg /= wepAvg.w;
        BaseAvg /= baseAvg.w;
        ThrusterAvg /= thrusterAvg.w;

        wepAvg *= 2.0f * PosFactor / wepAvg.w;
        thrusterAvg *= 2.0f * PosFactor / thrusterAvg.w;
        baseAvg *= 2.0f * PosFactor / baseAvg.w;
        wepAvg.w = ((Vector3)wepAvg).magnitude;
        thrusterAvg.w = ((Vector3)thrusterAvg).magnitude;
        baseAvg.w = ((Vector3)baseAvg).magnitude;

        wepAvg.x *= 3;
        wepAvg.z *= 0.1f;
        thrusterAvg.z *= 0.1f;
        baseAvg.z *= 0.8f;
        baseAvg.y *= 3.0f;

        for(int i = Nodes.Count; i-- > 0;) {
            var d1 = Nodes[i];


            while(d1.Island != Nodes[d1.Island].Island)
                d1.Island = Nodes[d1.Island].Island;

            var avp = -d1.Pos * PosFactor;


            switch(d1.Typ) {
                case BType.Base:
                    d1.Avoidance -= (Vector3)baseAvg;
                    break;
                case BType.Weapon:
                    d1.Avoidance -= (Vector3)wepAvg;
                    if(WepAvg < BaseAvg)
                        d1.Avoidance += Math_JC.SetVectorLength(d1.Pos, BaseAvg - WepAvg) * 1.0f * PosFactor;

                    if( d1.Pos.magnitude < WepAvg *0.75f )
                        d1.Avoidance += Math_JC.SetVectorLength(d1.Pos, d1.Pos.magnitude - WepAvg * 0.75f) * 1.0f * PosFactor;
                    avp.y *= 2.0f;
                    break;
                case BType.Thruster:
                    d1.Avoidance -= (Vector3)thrusterAvg;
                    if(ThrusterAvg < BaseAvg)
                        d1.Avoidance += Math_JC.SetVectorLength(d1.Pos, BaseAvg - ThrusterAvg) * 1.0f * PosFactor;
                    break;
            }

            //avp.z *= 0.3f;
            //avp.y *= 0.8f;
            d1.Avoidance += avp;
            avp = -avp.normalized *PosFactor *0.5f;
            avp.x *= Shape.x / shapeMax;
            avp.y *= Shape.y / shapeMax;
            avp.z *= Shape.z / shapeMax;
         //   d1.Avoidance += avp;

           // d1.Avoidance += (d1.Pos.normalized * d1.Depth*0.2f - d1.Pos) * PosFactor*2;

            if(i != 0) {
               // d1.Depth = Mathf.Lerp(d1.Depth, Mathf.Sqrt(BaseMax)*0.9f +5.0f, 0.1f);
            } else
                d1.Depth = 0;

            d1.update(this);

            d1.MaxVel = MaxVel;
            d1.MaxAVel = MaxAVel;
            d1.MaxASteer = MaxASteer;
            d1.MaxSteer = MaxSteer;

        }

        foreach( var d1 in Nodes ) {
            // break;var d1 = Nodes[Random.Range(1, Nodes.Count)]
  
            if(d1.Typ != BType.Base) continue;
            if(d1.Vel.magnitude > d1.MaxVel * 0.2f)
                continue;
            int cc = 0;
            var roll = Random.value;
            if( roll > RemChance ) continue;

            foreach(var c in d1.Connections) {
                if(c == null || c.Dis < ConThresh) continue;
                cc++;
             //   if(c.opp(d1).Typ != BType.Base)  return;
            }
            if(cc  != 1 ) continue;
          //  if( cc == 
            //if(cc > 2 && Shape.z +2 > shapeMax * 0.8f)
            //    break;

            foreach(var c in d1.Connections) {
                if(c == null) continue;
                c.remove();
            }
            d1.Pos = Random.onUnitSphere * 5.0f;
           // d1.Pos.z *= 0.5f;
            d1.Trn.localPosition = d1.Pos;
           // d1.Depth = 0;
        }

    }
    public float ConThresh = 1.2f, ConSpeed = 0.05f;
    public float RemChance = 0.05f;
    public float WepAvg, ThrusterAvg, BaseAvg;

    public bool DrawCones = true, DrawFriendLines = true;

    public List<Node.Connection> DisCon;
    void OnDrawGizmos() {
      
        for(int i = Nodes.Count; i-- > 0;) {
            var d1 = Nodes[i];
            if(UnityEditor.Selection.Contains(d1.Trn.gameObject)) {
                DisCon = new List<Node.Connection>(  d1.Connections );
            }
            if(DrawFriendLines ) {
                foreach(var c in d1.Connections) {
                    if(c == null || c.N1 != d1 ) continue;
                   // Debug.Log("con dis " + c.Dis + "con str " + c.Strength);
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.TransformPoint(c.N1.Pos), transform.TransformPoint(c.N2.Pos));
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.TransformPoint(c.N1.Pos), transform.TransformPoint(c.N1.Pos + c.N1.Trn.localRotation* Offsets[c.Ci1]*0.6f ));
                    Gizmos.DrawLine(transform.TransformPoint(c.N2.Pos), transform.TransformPoint(c.N2.Pos + c.N2.Trn.localRotation * Offsets[c.Ci2] * 0.6f));

                }
            }
            if(DrawCones)
                switch(d1.Typ) {
                    case BType.Thruster:
                        DebugExtension.DrawCone(transform.TransformPoint(d1.Pos), d1.Trn.forward, ThrusterAng);
                        break;
                    case BType.Weapon:
                        DebugExtension.DrawCone(transform.TransformPoint(d1.Pos), d1.Trn.forward, WeaponAng);
                        break;
                }
        }
    }
}