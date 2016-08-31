using UnityEngine;
using System.Collections;

namespace Sim {
    [System.Serializable]
    public class Weapon : Cmpnt {
        public Drone FireAt;

        public float DtLim;
        public void pew(ref Simulation.FrameCntx cntx, Drone d,  Drone.FrameDat c, Drone at, Vector3 fwd ) {
            LHits = 0;
            FireAt = null;
            if(at) {

                var td = at.fd(cntx.FrameInd);
                var tp = td.Pos;
                Vector3 vec = tp - c.Pos;
                var mag = vec.magnitude;
                var vn = vec / mag;
                var dt = Vector3.Dot(vn, fwd);

                Vector3 relVel = td.Vel - c.Vel;
                float radialVel = Vector3.Dot(relVel, vn);
                Vector3 transVel = relVel - vn * radialVel;
                Vector3 angVel = transVel / mag;


                if( dt > DtLim ) {
            
                    var tDrn = at as Drone;

                    float tRad = tDrn.Rad;

                    float aimRad = targetingRad(mag);
                    float effVel = transVel.magnitude;// trgt.Vel;

                    var radFromDrift = effVel * Mathf.Pow(mag / ProjSpeed, DriftPow) * DriftFactor;
                    aimRad += radFromDrift;
                    effVel *= cntx.Delta;
                    var trackingRad = (effVel * 0.5f + tRad) * 1.1f;
                    /*
                    if(trackingRad > aimRad) {
                     var mid = trns.position;// (tp2 + TestTarget.position) * 0.5f;
                        DebugExtension.DrawCircle(mid, vn, Color.magenta, aimRad);
                        DebugExtension.DrawCircle(mid, vn, Color.magenta, trackingRad);
                    }*/
                    aimRad = (0.7777777f * aimRad + trackingRad * 0.3333333f);
                    // }
                    // trgt.AHit2 = trgt.Rad < aimRad ? trgt.Rad / aimRad : 1;

                    if(tRad < aimRad) {
                        float a1 = Mathf.Pow(tRad, Grouping), a2 = Mathf.Pow(aimRad, Grouping);
                        LChanceToHit = a1 / a2;
                    } else {
                        LChanceToHit = 1.000000001f;
                    }


                    d.Power -= 0.25f * cntx.Delta;
                    FireAt = at;


                    int slv = Salvo;

                    if(slv >= AmmoCnt) {
                        slv = AmmoCnt;

                        AmmoCnt = 0;

                        d._Behavior = Drone.BehaviorT.Resupply;
                        d._State = Drone.StateT.Idle;
                    } else 
                        AmmoCnt -= slv;
                    for(int i = slv; i-- > 0;)
                        if(Random.value <= LChanceToHit) {
                            tDrn.Dmg += 5 * cntx.Delta;
                            LHits++;
                        }
                }
                //}


                if(d.Host != null) {
                    var hd = (d.Host as global::Drone);
                    if(at.Host) {
                        if(hd.WeaponReport) {
                            hd.FrameReports.Add(new SimObj.ReportStr(
                                    "  Trgt " + at.name
                                    + "\n  Mag: " + mag
                                    + "\n  dt: " + dt
                                    + "\n  relVel: " + relVel.magnitude + "   >> " + relVel
                                    + "\n  radialVel: " + radialVel
                                    + "\n  transVel: " + transVel.magnitude + "   >> " + transVel
                                    + "\n  angVel: " + angVel.magnitude + "   >> " + angVel
                                    + "\n  chanc: " + LChanceToHit *100
                                , at, d.Ar.worldPos(tp)));
                        }
                        //   hFireAt = at.Host.transform;
                    }
                }

            }
            onSim(FireAt);
        }
        public void onSim( Drone d ) {

            if(Host) {
                Transform t = null;
                bool fire;
                if((fire = d != null) && d.Host != null)
                    t = d.Host.transform;

                (Host as global::Weapon).onSim(fire, t);
            }
        }

        public int LHits;
        public float LChanceToHit;

        public int AmmoCnt, MaxAmmo;
        public int Salvo;
        public float Grouping = 1.5f, ProjSpeed = 50;
        public float DriftPow = 2, DriftFactor = 0.1f;
        public float SpreadA1 = 1, SpreadA2 = 1, SpreadB1 = 1.0f, SpreadB2 = 1;


        public float targetingRad(float d) {
            float d1 = d * SpreadA1, d2 = d * SpreadB1;
            return SpreadA2 * d1 / (1 + d1) + SpreadB2 * d2;
        }


    }
}

public class Weapon : Cmpnt {

    public ChainGun Anim;

    public Sim.Weapon Wep {
        get { return Sc as Sim.Weapon; }
        set { Sc = value; }
    }

    public override void init(Sim.Drone drn, Sim.Cmpnt sc) {
        sc.Host = this;
        Anim.Arc = Arc;


        Sc = sc; ;  
    }

    /*
    struct ProbEntry {
        public int Cum;
    };
    static ProbEntry[][] SalvoTable;

    static void buildSalvoTable() {
        int max =8;
        SalvoTable = new ProbEntry[max][];

        for(int i = max; i-- > 0; ) {
            int c = i + 2;
            Debug.Log("salvo pool  "+c );
            var cur = SalvoTable[i] = new ProbEntry[c+1];


            /*
                  0
                  1

                  0 -  1/2^1
                  1 -  1/2

                  00
                  01
                  10
                  11

                  0 - 1 / 4
                  1 - 2 / 4
                  2 - 1 / 4

                  000
                  001
                  010
                  100
                  011
                  110
                  101
                  111

                  0 - 1 / 8
                  1 - 3 / 8
                  2 - 3 / 8
                  3 - 1 / 8

                  0000
                  0001
                  0010
                  0100
                  1000

                  0011
                  0110
                  0101

                  1001
                  1010
                  1100

                  0111
                  1011
                  1110
                  1101
                  1111

                  0 - 1 / 16
                  1 - 4 / n
                  2 - 3x1 / n
                  3 - 4 / n
                  4 - 1 / n

                  4!
                  /
                  2! 2!

                  4*3    *2
                  /
                  2       *2

    * /

            int tOcc = 0;
            for(int j = c+1; j-- > 0;) {
                int h = j;
                int m = c - h;

                int occ = 1;

                int mx = h, mn = h;
                if(h > m) mn = m; else mx = m;
                
                for(int k = c; k > mx; k-- ) 
                    occ *= k;
                for(int k = mn; k > 1; k-- )
                    occ /= k;

                cur[j].Cum = occ;

                Debug.Log(" chance of " + h + " hits "+ occ +"   / "+ (1 << c) + "     mn  " + mn + "    mx  " + mx);

                tOcc += occ;
            }
            //Debug.Log(" chance of " + 0 + " hits " + 1 + "   / " + (1 << c));
           // tOcc++;
            Debug.Log("   -- tot occ " + tOcc);

            for(int iter = 5; iter-- > 0;) {
                double hit = Random.Range(0.1f, 0.9f), miss = 1- hit;
                double roll = Random.value,r2 = roll;
                double tp = 0;
                for(int j = 0; j <= c; j++) {
                    double cum = cur[j].Cum;
                    double p1 = System.Math.Pow(hit, j) * System.Math.Pow(miss, c - j) * cum;
                    r2 -= p1;
                    Debug.Log("   p1  " + p1 + "   j " + j + "   r2 " + r2 + "   cum " + cum);
                    if(r2 < 0) {
                        Debug.Log("HIT x " + j + "   roll " + roll + "   hit " + hit);
                        r2 = 2;
                    }
                    tp += p1;
                }
                Debug.Log("--- tp " + tp + "   roll " + roll + "   hit " + hit );

                    /*for( int i1 = 0, i2 = c;  ;) {
                        int m = (i1 + i2) >> 1;
                        Debug.Log("   search  " + m + "   i1 " + i1 + "   i2 " + i2 );

                    }* /
                }
        }
    } */

    Sim.Weapon newWep() {
        return new Sim.Weapon() {
            name = name,
            Salvo = Salvo,
            SpreadA1 = SpreadA1,
            SpreadA2 = SpreadA2,
            SpreadB1 = SpreadB1,
            SpreadB2 = SpreadB2,
            Grouping = Grouping,
            ProjSpeed = ProjSpeed,
            DriftPow = DriftPow,
            DriftFactor = DriftFactor,
            AmmoCnt = Ammo,
            MaxAmmo = Ammo,
            DtLim = Mathf.Cos(Arc * Mathf.Deg2Rad),
        };
    }
    public override void build(Sim.Drone drn, int ci) {

        var ths = newWep();

        if(IsPrimary)
            drn.Primary = ths;

        drn.Cmpnts[ci] = ths;

        //if(SalvoTable == null) buildSalvoTable();
    }

    public float Range = 2, Arc = 30;


    public int Salvo = 4, Ammo = 200;
    public float SpreadA1 = 1, SpreadA2 = 1, SpreadB1 = 1.0f, SpreadB2 = 1;
    public float Grouping = 1.5f, ProjSpeed = 50;


    public bool IsPrimary = false;

    //  public Transform[] TestTargets;
    public float DriftPow = 2,  DriftFactor = 0.1f;
    public int Seed = 5; 
    public float SimStep = 0.1f, Step = 0.5f, Tot = 20.0f, DtLim;

    public bool DrawGizmo = false;

    [HideInInspector]
    public int WepI;
    
    public TestTarget LstTrgt;


    public void testTarget(TestTarget trgt) {

        var wep = newWep();
        Wep = null;


        float dtLim = Mathf.Cos(Arc * Mathf.Deg2Rad);
        DtLim = dtLim;

        LstTrgt = trgt;
        var trns = trgt.transform;


        var fwd = transform.forward;
        var vec = trns.position - transform.position;


        trgt.Dis = vec.magnitude;

        trgt.A1 = Math_JC.pow2(trgt.Rad) * Mathf.PI;
        trgt.A2 = Math_JC.pow2(wep.targetingRad(trgt.Dis)) * Mathf.PI; 
        trgt.AHit2 = trgt.AHit = 1;
        if(trgt.A1 < trgt.A2) {
            trgt.AHit = trgt.A1 / trgt.A2;
            trgt.AHit2 = trgt.Rad / wep.targetingRad(trgt.Dis);
        }


        trgt.Dot = Vector3.Dot(fwd, vec) / trgt.Dis;
        trgt.Ang = Mathf.Acos(trgt.Dot) * Mathf.Rad2Deg;

        Gizmos.color = Color.red;
        if(trgt.Dot < DtLim) Gizmos.color = Color.black;
        Gizmos.DrawLine(trns.position, transform.position);

        var vn = vec / trgt.Dis;
        var velDir = Vector3.Cross(vn, Vector3.up).normalized;
        var tp2 = trns.position + velDir * trgt.Vel * SimStep;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(trns.position, tp2);

        Gizmos.color = Color.blue;
        var tRight = Vector3.Cross(vn, Vector3.up).normalized;
        var tUp = Vector3.Cross(vn, tRight).normalized;
        DebugExtension.DrawCircle(tp2, vn, Gizmos.color, trgt.Rad);
        Gizmos.DrawLine(trns.position + tUp * trgt.Rad, tp2 + tUp * trgt.Rad);
        Gizmos.DrawLine(trns.position - tUp * trgt.Rad, tp2 - tUp * trgt.Rad);

        Gizmos.color = Color.red;
        var mid = trns.position;// (tp2 + TestTarget.position) * 0.5f;
        float aimRad = wep.targetingRad(trgt.Dis);
        float effVel = trgt.Vel;
        
        trgt.RadFromDrift = effVel * Mathf.Pow( trgt.Dis/ ProjSpeed , DriftPow ) * DriftFactor;
        aimRad += trgt.RadFromDrift;
        //effVel *= SimStep;
       /* var trackingRad = (effVel /trgt.Dis + trgt.Rad)* 1.1f;

        if(trackingRad > aimRad) {
            DebugExtension.DrawCircle(mid, vn, Color.magenta, aimRad);
            DebugExtension.DrawCircle(mid, vn, Color.magenta, trackingRad);


            aimRad = (aimRad + trackingRad ) *0.5f;
        }
          //  aimRad = (0.7777777f * aimRad + trackingRad * 0.3333333f);
       // } */
       
        trgt.AHit2 = trgt.Rad < aimRad ? trgt.Rad / aimRad : 1;

        if(trgt.Rad < aimRad) {
            float a1 = Mathf.Pow(trgt.Rad, Grouping), a2 = Mathf.Pow(aimRad, Grouping);
                 trgt.ChanceToHit = a1 / a2;
        } else {
            trgt.ChanceToHit = 1;
        }
        //trgt.name = "Chnc:  " + chanceToHit * 100.0f;
        trgt.AimRad = aimRad;


        DebugExtension.DrawCircle(mid, vn, Gizmos.color, aimRad);
        Random.seed = Seed;

        trgt.BruteHit = 0;
        for(int maxi = 300, i = maxi; i-- > 0;) {

            var tp = mid;
            var div = Random.insideUnitCircle;

            div *= (div.magnitude * 0.5f + 0.5f);
            //div *= 0.5f;
            div *= aimRad;
            tp += tUp * div.y + tRight * div.x;
            Gizmos.DrawLine(tp - tRight * aimRad * 0.05f, tp + tRight * aimRad * 0.05f);
            Gizmos.DrawLine(tp - tUp * aimRad * 0.05f, tp + tUp * aimRad * 0.05f);

            if((tp - trns.position).sqrMagnitude < trgt.Rad * trgt.Rad)
                 trgt.BruteHit += 1.0f / maxi;
        }
    }

    void OnDrawGizmos() {


        var wep = newWep();

        WepI = 0;

        if(!DrawGizmo) return;


        {
            var fwd = transform.forward;
            // fwd = vn;
            Vector3 up = Vector3.Slerp(fwd, -fwd, 0.5f);
            Vector3 right = Vector3.Cross(fwd, up).normalized;


            Vector3 lp = transform.position;
            float d = Step;
            float td = 0;

            Vector3[] rp = new Vector3[4] { lp, lp, lp, lp };
            for(int iter = Mathf.CeilToInt(Tot/Step); iter-- > 0;) {

                float proj = d * 2.0f;
                float effD = td + proj;

                float rad = wep.targetingRad(effD);
                float arc = Mathf.Atan( rad/ proj) * Mathf.Rad2Deg;

                lp = transform.position + fwd * td;
                //DebugExtension.DrawCone(lp, fwd * proj, Color.green, arc);

                Gizmos.color = Color.green;

                Vector3 tp = lp + fwd * proj;

                Vector3[] rp2 = new Vector3[4] { tp + up*rad, tp -up*rad, tp-right*rad, tp+right*rad };
                for(int i = 4; i-- > 0;) 
                    Gizmos.DrawLine(rp[i],rp2[i] );
                rp = rp2;
               

                DebugExtension.DrawCircle( tp, fwd, Gizmos.color, rad );

                td += d;
                
            }
        }

        DebugExtension.DrawCone(transform.position, transform.forward * Range, Color.yellow, Arc);
    }

    public void onSim(bool fire, Transform hFireAt) {

        Anim.Fire = fire;
        Anim.Track = hFireAt;

        Anim.Chnc = Wep.LChanceToHit;
    }
}