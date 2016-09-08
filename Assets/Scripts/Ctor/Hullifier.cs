using UnityEngine;
using System.Collections.Generic;

public class Hullifier : MonoBehaviour {


    
    struct HullifyDat {
        // public Voxel V;
        public int Flags;
        public IVec3 Off;
        public float Dis;
        public bool checkSet(int cf, IVec3 off) {
            int chk = 1 << 9;
            int f = Flags;
            Flags |= cf;
            Off += off;
            //  Debug.Log(" before  " + f + "  cf  " + cf + "   after  " + Flags + "     c  " + ((Flags & chk) != 0));
            if((Flags & chk) != 0) return false;
            Flags |= chk;
            Dis = -1;
            return true;
        }
    };

    struct HullifyDat2 {
        // public Voxel V;
        public byte Flags, Vc;
    };

    public bool Hullify = false;
    public float Rad = 0.75f, Displace = 1.2f, BoxEx = 0.25f, CrnrCast = 0.15f, DisplacePow = 1.3f;



    public bool GizCorners = true, GizBox = true;
    //struct HullifyVoxDat
    public void hullify(Structure strct) {
     //   var cc = GetComponentInChildren<CtorComponent>();

        foreach(var v in strct.Vox)
            Destroy(v.gameObject);

        strct.Vox.Clear();

        strct.Selection.Clear();
        CtorMain.Singleton.DirtySelection = true;

        hullifySub(strct);
    }


    void hullifySub( Structure strct, bool gizmos =false ) {

        int lm = 1 << 26;
        RaycastHit hit;
        Vector3 cp = strct.transform.position;

        bool gizC = GizCorners & gizmos;

        DyArray<HullifyDat> arr = new DyArray<HullifyDat>();
        DyArray<HullifyDat2> arr2 = new DyArray<HullifyDat2>();

        List<IVec3> search = new List<IVec3>(), voxL = new List<IVec3>(); ;

        Vector3 ex = Vector3.one * BoxEx;
        // Collider[] col = new Collider[1];


        IVec3[] adj = {
            new IVec3(  1,0,0 ),
            new IVec3(  0,1,0 ),
            new IVec3(  0,0,1 ),
            new IVec3(  -1,0,0 ),
            new IVec3(  0,-1,0 ),
            new IVec3(  0,0,-1 ),

            new IVec3(  1,1,0 ),
            new IVec3(  1,0,1 ),
            new IVec3(  1,-1,0 ),
            new IVec3(  1,0,-1 ),
            new IVec3(  -1,1,0 ),
            new IVec3(  -1,0,1 ),
            new IVec3(  -1,-1,0 ),
            new IVec3(  -1,0,-1 ),
            new IVec3(  0,1,1 ),
            new IVec3(  0,1,-1 ),
            new IVec3(  0,-1,1 ),
            new IVec3(  0,-1,-1 ),


            new IVec3(  1,1,1 ),
            new IVec3(  1,1,-1 ),
            new IVec3(  1,-1,1 ),
            new IVec3(  1,-1,-1 ),
            new IVec3(  -1,1,1 ),
            new IVec3(  -1,1,-1 ),
            new IVec3(  -1,-1,1 ),
            new IVec3(  -1,-1,-1 ),
        };
        float[] adjD = new float[adj.Length];
        for(int i = adj.Length; i-- > 0;)
            adjD[i] = ((Vector3)adj[i]).magnitude;

        //int ii = i * 4 + j * 2 + k;
        // int fi1 = ((int)FaceT.Down - i), fi2 = ((int)FaceT.Left - j);
        //int fi3 = ((int)FaceT.Back - k);

        IVec3[] adj2 =  { ///todo  --  skip this -- generate voxels the kill corners based on adjacency
            new IVec3( 0,0,0),
            new IVec3( 0,0,1),
            new IVec3( 1,0,0),
            new IVec3( 1,0,1),
            new IVec3( 0,1,0),
            new IVec3( 0,1,1),
            new IVec3( 1,1,0),
            new IVec3( 1,1,1),

            new IVec3( 0,0,-1),
            new IVec3( 1,0,-1),
            new IVec3( 0,1,-1),
            new IVec3( 1,1,-1),
            new IVec3( 0,0,2),
            new IVec3( 1,0,2),
            new IVec3( 0,1,2),
            new IVec3( 1,1,2),


            new IVec3( 0,-1,0),
            new IVec3( 0,-1,1),
            new IVec3( 1,-1,0),
            new IVec3( 1,-1,1),
            new IVec3( 0,2,0),
            new IVec3( 0,2,1),
            new IVec3( 1,2,0),
            new IVec3( 1,2,1),

            new IVec3( -1,0,0),
            new IVec3( -1,0,1),
            new IVec3( 2,0,0),
            new IVec3( 2,0,1),
            new IVec3( -1,1,0),
            new IVec3( -1,1,1),
            new IVec3( 2,1,0),
            new IVec3( 2,1,1),

        };

        int[] _invalidVC4 = {
            51,
            15,
            85,
            204,
            240,
            170,

            195,
            90,
            60,
            165,
            102,
            153,
        };
        

        SortedList<int, int> invalidVC4 = new SortedList<int, int>(_invalidVC4.Length);
        foreach(var i in _invalidVC4)
            invalidVC4.Add(i, i);


        //  Debug.Assert(arr.op(new IVec3(0), (ref HullifyDat h) => { return h.checkSet(0); }) == true);
        //  Debug.Assert(arr.op(new IVec3(0), (ref HullifyDat h) => { return h.checkSet(0); }) == false);
        arr.op(new IVec3(0), (ref HullifyDat h) => { h.checkSet(0, new IVec3(0)); });

        Vector3 hv3 = -Vector3.one * 0.5f;
        int vc4 = 0;
        for(IVec3 c = new IVec3(0); ;) {
            var p = cp + (Vector3)c;

            bool vox = false, spread = true;
            bool cb = Physics.CheckBox(p, ex, Quaternion.identity, lm);
            if(!cb) {
                int vc = 0, cf = 255;
                for(int i = 8; i-- > 0;) {
                    //if((arr[c + adj2[i]].Flags & 1) != 0)
                    //   cf &= ~(1 << i);
                    var off = adj2[i];
                    if(arr2.op(c + off, (ref HullifyDat2 d) => {
                        if(d.Flags == 0) {
                            d.Flags = (byte)(Physics.CheckSphere(p + (Vector3)(off) + hv3, CrnrCast, lm) ? 2 : 1);
                            if(gizC) {
                                Gizmos.color = (d.Flags & 3) == 1 ? Color.green : Color.red;
                                Gizmos.DrawWireSphere(p +(Vector3)(off) + hv3, CrnrCast);
                            }
                        }
                        return (d.Flags & 3) == 1;
                    })) {
                        vc++;
                        cf &= ~(1 << i);
                    }
                }


                if(vc == 4) {
                    vc4++;

                    if(invalidVC4.ContainsKey(cf))
                        cb = true;
                    else {
                        Debug.Log("vc4 @ " + c + "  cf " + cf);
                        vox = true;
                    }

                    spread = true;
                } else {
                    spread = vc < 8;
                    vox = vc >= 5;
                }
            }


            if(spread) {
                if(gizmos & GizBox ) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(p, ex * 2);
                }
                foreach(var off in adj) {
                    // var off = new IVec3(adj, ai);
                    var i = c + off;
                    if(arr.op(i, (ref HullifyDat h) => { return h.checkSet(0, off); })) {
                        search.Add(i);
                    }
                }
                foreach(var a in adj2) {
                    arr2.op(c + a, (ref HullifyDat2 d) => {
                        if(d.Flags == 0) {
                            bool cast = Physics.CheckSphere(p +(Vector3)(a) + hv3, CrnrCast, lm);
                            d.Flags = (byte)( cast ? 2 : 1);
                            //Debug.Log("c + a  " + (c + a) + "  cast " + cast + "   flags " + d.Flags);
                            if(gizC) {
                                Gizmos.color = (d.Flags & 3) == 1 ? Color.green : Color.red;
                                Gizmos.DrawWireSphere(p + (Vector3)(a) + hv3, CrnrCast);
                            }
                        }
                        d.Flags |= 4;
                    });
                }


            }
            if(vox) {
                for(int i = 8; i-- > 0;) {
                    //if((arr[c + adj2[i]].Flags & 1) != 0)
                    //   cf &= ~(1 << i);
                    var off = adj2[i];
                    arr2.op(c + off, (ref HullifyDat2 d) => {
                        d.Vc++;
                    });
                }
                // Gizmos.color = Color.green;
                voxL.Add(c);
            }
             


            if(search.Count <= 0) break;

            c = search[0];
            search.RemoveAt(0);

        }

      //  if( vc4 != 0 )
       //     Debug.Log("vc4  " + vc4  );
        foreach(var c in voxL) {
            var dat = arr[c];
            dat.Dis = 0;
            if(dat.Off != new IVec3(0)) {
                Vector3 dir = -((Vector3)dat.Off).normalized;

                var r = new Ray(cp + (Vector3)c, dir);
                float hDis = Displace / dir.maxAbsD();
                if(Physics.BoxCast(r.origin, ex, r.direction, out hit, Quaternion.identity, hDis, lm)) {
                    hDis = hit.distance;
                }
                dat.Dis = hDis * dir.maxAbsD();
            }
            // Debug.Log("c1  " + c + "  dat.Off  " + dat.Off + "  dat.Dis  " + dat.Dis );
            arr[c] = dat;
        }
        foreach(var c in voxL) {

            Vector3 p = (Vector3)c;
            var dat = arr[c];
            // Debug.Log("c  " + c + "  dat.Off  " + dat.Off + "  dat.Dis  " + dat.Dis + "  f " + arr2[c].Flags + "  vc " + arr2[c].Vc);

            if(dat.Off != new IVec3(0)) {
                Vector3 dir = -((Vector3)dat.Off).normalized;

                float mod = 1.0f;
                float d = dat.Dis * mod;

                for(int i = adj.Length; i-- > 0;) {
                    var nd = arr[c + adj[i]].Dis;
                    if(nd >= 0) {
                        d += nd;
                        mod += adjD[i];
                    }
                }
                p += dir * (d / (mod * ( Mathf.Pow( dir.maxAbsD(), DisplacePow ) )));
            }


            if(gizmos) {
                if(GizBox) {
                    Gizmos.color = Color.green;
                    p = transform.TransformPoint(p);
                    Gizmos.DrawWireCube(p, ex * 2);
                }
            } else {
                //int cf = (~dat.Flags) & 255;

                var go = Instantiate(CtorMain.Singleton.VoxelFab);
                var v = go.GetComponent<Voxel>();
                v.init(strct);
                v.transform.resetTransformation();
              
                v.transform.localPosition = p;
                v.transform.localScale *= Rad;

                int cf = 255;

                for(int i = 8; i-- > 0;) {
                    //if((arr[c + adj2[i]].Flags & 1) != 0)
                    //   cf &= ~(1 << i);
                    var off = adj2[i];
                    if(arr2.op(c + off, (ref HullifyDat2 d) => {
                        return d.Flags == 5 && d.Vc >= 4;
                    })) {
                        cf &= ~(1 << i);
                    }
                }
                // break;
                v.CrnrFlgs.Data = cf;
            }
        }


    }

    public bool Gizmo = false;
    void OnDrawGizmos() {
        if(!Gizmo) return;
        hullifySub(GetComponent<Structure>(), true);
    }
}
