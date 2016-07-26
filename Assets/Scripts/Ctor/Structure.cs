using UnityEngine;
using System.Collections.Generic;

public enum FaceT {
    Left = 1, Right = 0, Up = 2, Down = 3, Forward = 4, Back = 5,
};

public class Structure : UIEle {
    public static readonly int[,] FaceVi = new int[6, 4] {
        { 2,6,7,3 },
        { 0,1,5,4 },
        { 4,5,7,6 },
        { 0,2,3,1 },
        { 1,3,7,5 },
        { 0,4,6,2 },
   };
    //{ 1, 2, 4},{ 0, 3, 5},{ 3, 0, 6},{ 2, 1, 7},{ 5, 6, 0},{ 4, 7, 1},{ 7, 4, 2},{ 6, 5, 3},
    public static int[,] VertAdj = new int[8, 3] {
        { 1, 4, 2},//
        { 0, 3, 5},//-
        { 3, 0, 6},//-
        { 1, 2, 7},//
        { 5, 6, 0},//-
        { 4, 1, 7},//
        { 7, 2, 4},//
        { 6, 5, 3},//-
   };
    public static readonly FaceT[,] FaceAdj = new FaceT[6, 4] {
        { FaceT.Up, FaceT.Down, FaceT.Forward, FaceT.Back },
        { FaceT.Up, FaceT.Down, FaceT.Forward, FaceT.Back },
        { FaceT.Left, FaceT.Right, FaceT.Forward, FaceT.Back },
        { FaceT.Left, FaceT.Right, FaceT.Forward, FaceT.Back },
        { FaceT.Up, FaceT.Down, FaceT.Left, FaceT.Right },
        { FaceT.Up, FaceT.Down, FaceT.Left, FaceT.Right },
    };
    public static int[,] FaceAdj2;
    public static readonly int[,,] FaceAdjVerts = new int[6, 6, 2] {
        { {0,0},{0,0},{1,2},{3,0},{2,3},{0,1},},
        { {0,0},{0,0},{2,3},{0,1},{1,2},{3,0},},
        { {3,2},{1,0},{0,0},{0,0},{1,2},{3,0},},
        { {2,1},{0,3},{0,0},{0,0},{2,3},{0,1},},
        { {2,1},{0,3},{3,2},{1,0},{0,0},{0,0},},
        { {3,2},{1,0},{2,1},{0,3},{0,0},{0,0},},
    };




    /*
             { 2,6,7,3 },
            { 0,1,5,4 },
            { 4,5,7,6 },
            { 0,2,3,1 },
            { 1,3,7,5 },
            { 0,4,6,2 },
     */

    static public FaceT opp(FaceT fc) {
        switch(fc) {
            case FaceT.Left: return FaceT.Right;
            case FaceT.Right: return FaceT.Left;
            case FaceT.Up: return FaceT.Down;
            case FaceT.Down: return FaceT.Up;
            case FaceT.Forward: return FaceT.Back;
            case FaceT.Back: return FaceT.Forward;
        }
        Debug.LogError("FaceT OOB");
        return (FaceT)(-1);
    }
    static int opp(int fc) { return (int)opp((FaceT)fc); }



    //Vector3[] Verts = new Vector3[0];

    public bool StyleFlag = true;
    bool DrawFlag;
    [SerializeField]
    public List<Voxel> Vox;
    //List<TriRef> Faces;
    // bool Rebuild = false;

    T getAddComponenet<T>() where T : Component {
        var go = gameObject;
        T r = go.GetComponent<T>();
        if(r == null) r = go.AddComponent<T>();
        return r;
    }

    public class TriRef {
        public Voxel Vox;
        public FaceT D;
    };

    public TriRef[] Tris;


    public class Face {

        public struct Cp_S {
            public Vector3 A, B, C, D;
            public Vector3 this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                    }
                    Debug.LogError("i OOB");
                    return Vector3.zero;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                        case 2: C = value; return;
                        case 3: D = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = C = D = Vector3.zero;
            }
        };

        public struct CpOff_S {
            public int A, B, C, D;
            public int this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                    }
                    Debug.LogError("i OOB");
                    return -1;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                        case 2: C = value; return;
                        case 3: D = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = C = D = -1;
            }
        };
        public CpOff_S CpOff;
        public Cp_S Cp;
        public Voxel V1 = null, V2 = null;
        public TriRef Tr;
        public Vector3 N;

        public Vector3 vert(Voxel v, int i) {
            if(v == V1) return Cp[i];
            Debug.Assert(v == V2);
            return Cp[CpOff[i]];
        }
        public Vector3 norm(Voxel v) {
            if(v == V1) return N;
            Debug.Assert(v == V2);
            return -N;
        }


        public struct Vertex_S {
            public Vertex A, B, C, D;
            public Vertex this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                    }
                    Debug.LogError("i OOB");
                    return null;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                        case 2: C = value; return;
                        case 3: D = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = C = D = null;
            }
        };
        //public Vertex_S Verts;
        public int Vc;

    };

    public float Inset = 0.1f;



    class NbrLink {
        public Voxel V1, V2;
        public FaceT Fi1, Fi2;
    };
    public class Vertex {
        public Vector4 V;
        public Voxel[] Vox = new Voxel[8];
        public int Vc = 0;
        public long VMask = 0;

        public int Vi = -1;
        public Voxel.Corner Crnr;
    };

    public class Quad {
        public Face F;
        public struct Vi_S {
            public int A, B, C, D;
            public int this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                    }
                    Debug.LogError("i OOB");
                    return -1;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                        case 2: C = value; return;
                        case 3: D = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = C = D = -1;
            }
        };
        public Vi_S Verts;
    };
    public class Tri {
        public Face F;
        public struct Vi_S {
            public int A, B, C;
            public int this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                    }
                    Debug.LogError("i OOB");
                    return -1;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                        case 2: C = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = C = -1;
            }
        };
        public Vi_S Verts;
    };

    public class BuildCntx {
        public BuildCntx(Structure s) {
            // S = s;
            Verts = new List<Vertex>();
            int vc = s.Vox.Count;
            var Crnrs = new Vector3[vc, 8];
            V_Verts = new Vertex[vc, 8];
            Vert = new Vert_Fetch() { Dat = V_Verts };
            Crnr = new Crnr_Fetch() { Dat = Crnrs };
            Flag = new Flag_Fetch() { Dat = new SimpleBitField[vc] };
            FaceVc = new FaceVc_Fetch() { Dat = new char[vc, 6] };
            Vi = new Vi_Fetch() { cntx = this };
        }
        public List<Vertex> Verts;
        // public Vector3[,] Crnrs;
        // public Vector3 crnr(Voxel v, int ci) { return Crnrs[v.TInd, ci]; }
        public Vertex[,] V_Verts;
        // public Vertex vert(Voxel v, int ci) { return V_Verts[v.TInd, ci]; }

        public struct Vert_Fetch {
            public Vertex[,] Dat;
            public Vertex this[Voxel v, int j] {
                get { return Dat[v.TInd, j]; }
                set { Dat[v.TInd, j] = value; }
            }
        };
        public Vert_Fetch Vert;
        public struct Vi_Fetch {
            public BuildCntx cntx;
            public int this[Voxel v, int j] {
                get {
                    var vrt = cntx.Vert[v, j];
                    if(vrt.Vi < 0) {
                        vrt.Vi = cntx.Vp.Count;
                        cntx.Vp.Add(vrt.V);
                    }
                    return vrt.Vi;
                }
            }
        };
        public Vi_Fetch Vi;


        public struct Crnr_Fetch {
            public Vector3[,] Dat;
            public Vector3 this[Voxel v, int j] {
                get { return Dat[v.TInd, j]; }
                set { Dat[v.TInd, j] = value; }
            }
        };
        public Crnr_Fetch Crnr;

        public struct FaceVc_Fetch {
            public char[,] Dat;
            public char this[Voxel v, FaceT j] {
                get { return Dat[v.TInd, (int)j]; }
                set { Dat[v.TInd, (int)j] = value; }
            }
        };
        public FaceVc_Fetch FaceVc;

        public struct Flag_Fetch {
            public SimpleBitField[] Dat;
            public bool this[Voxel v, int j] {
                get { return Dat[v.TInd][j]; }
                set { Dat[v.TInd][j] = value; }
            }
        };
        public Flag_Fetch Flag;
        //Structure S;


        public List<Vector3> Vp = new List<Vector3>();
        public List<Tri> Tris = new List<Tri>();
        public List<Quad> Quads = new List<Quad>();
    };



    void delink(Voxel v, FaceT fi) {
        var n = v.Nbrs[fi];
        if(n == null) return;

        v.Nbrs[fi] = null;

        n.Nbrs[n.opp(v, fi)] = null;
    }
    static void prep() {
        if(FaceAdj2 != null) return;

        FaceAdj2 = new int[6, 5];
        // 

        for(int i = 6; i-- > 0;) {
            for(int j = 4; j-- > 0;) {
                int fi = (int)FaceAdj[i, j];
                if(fi > i) continue;
                FaceAdj2[i, FaceAdj2[i, 4]++] = fi;
            }
        }
        /*  var FaceAdjVerts = new int[6, 6, 2];
          for(int fi1 = 6; fi1-- > 0;)
              for(int fi2i = 4; fi2i-- > 0;) {

                  int fi2 = (int)FaceAdj[fi1, fi2i];
                  if(fi1 < fi2) continue;
                  int vc = 0;
                  for(int i = 4; i-- > 0;) {
                      int vi = FaceVi[fi1, i];
                      for(int j = 4; j-- > 0;) {
                          if(FaceVi[fi2, j] != vi) continue;
                          //vc++;
                          FaceAdjVerts[fi2, fi1, vc] = j;
                          FaceAdjVerts[fi1, fi2, vc++] = i;
                          break;
                      }
                  }
                  //Debug.Log(" fi1 " + fi1 + "   fi2 " + fi2 + "   vc " + vc);
              }

          string s = "";
          for(int fi1 = 0; fi1 <6; fi1 ++ ) {
              s+= "{ ";
              for(int fi2 = 0; fi2 < 6; fi2++) {
                  s += "{" + FaceAdjVerts[fi1, fi2, 0] + "," + FaceAdjVerts[fi1, fi2, 1] + "},";
              }
              // { {2,6},{7,3}... },
              s += "},";
          }
          Debug.Log(s ); */


        /*
        for(int i = 2; i-- > 0;) {
            // Vector3 n2 = Vector3.left;
            for(int j = 2; j-- > 0;) {

                int fi1 = ((int)FaceT.Down - i), fi2 = ((int)FaceT.Left - j);

                // Vector3 n3 = Vector3.forward;
                for(int k = 2; k-- > 0;) {
                    int fi3 = ((int)FaceT.Back - k);
                    
                    int ii = i * 4 + j * 2 + k;
                    VertAdj[ii, 0] = i * 4 + j * 2 + (1 - k);
                    VertAdj[ii, 1] = i * 4 + (1 - j) * 2 + k;
                    VertAdj[ii, 2] = (1-i) * 4 + j * 2 + k;

                }
            }
        }
        string s = "";
        for(int i = 0; i < 8; i++) {
            s += "{ ";
            s += VertAdj[i, 0] + ", " + VertAdj[i, 1] + ", " + VertAdj[i, 2];
            s += "},";
        }
        Debug.Log(s); //*/
    }


    void mergeVertex(BuildCntx cntx, ref Vertex vrtx1, ref Vertex vrtx2, Voxel v1, Voxel v2) {
        if(vrtx1 == null && vrtx2 == null) {
            var v = new Vertex() {
                Vc = 2,
                VMask = v1.VGroup | v2.VGroup,
                V = Vector4.zero,
            };
            cntx.Verts.Add(v);
            vrtx1 = v;
            vrtx2 = v;
            v.Vox[0] = v1;
            v.Vox[1] = v2;
        } else if(vrtx1 != null && vrtx2 != null) {
            if(ReferenceEquals(vrtx1, vrtx2)) return;
            //vrtx2.V += vrtx1.V;
            vrtx2.VMask |= vrtx1.VMask;
            var vrtx1c = vrtx1;
            for(int k = vrtx1.Vc; k-- > 0;) {
                var vox = vrtx1c.Vox[k];
                vrtx2.Vox[vrtx2.Vc++] = vox;
                for(int l = 8; l-- > 0;) {
                    if(cntx.Vert[vox, l] == vrtx1c) {
                        cntx.Vert[vox, l] = vrtx2;
                        break;
                    }
                    Debug.Assert(l != 0);
                }
            }
            Debug.Assert(ReferenceEquals(vrtx1, vrtx2));
            vrtx1c.Vc = 0; //todo recycle
            //vrtx1 = vrtx2;
        } else if(vrtx1 == null) {
            vrtx1 = vrtx2;
            vrtx2.Vox[vrtx2.Vc++] = v1;
            vrtx2.VMask |= v1.VGroup;
        } else {
            vrtx2 = vrtx1;
            vrtx1.Vox[vrtx1.Vc++] = v2;
            vrtx1.VMask |= v2.VGroup;
        }
    }
    bool checkVertex(Vertex vrtx1, Vertex vrtx2) {
        if(vrtx1 == null || vrtx2 == null) return true;
        if(ReferenceEquals(vrtx1, vrtx2)) return true;
        if((vrtx1.VMask & vrtx2.VMask) == 0) return true;

        for(int i = vrtx1.Vc; i-- > 0;) {
            var v1 = vrtx1.Vox[i];
            if((v1.VGroup & vrtx2.VMask) == 0) continue;
            for(int j = vrtx2.Vc; j-- > 0;)
                if(vrtx2.Vox[j] == v1) return false;
        }
        return true;
    }
    bool merge(BuildCntx cntx, Voxel v1, FaceT f1, Voxel v2, FaceT f2) {


        if(DrawFlag) {
            var c = !(v1.Nbrs[f1] || v2.Nbrs[f2]);
            var col = c ? Color.green : Color.red;
            Debug.DrawLine(v1.transform.position, v2.transform.position, col);

        }

        if(v1.Nbrs[f1] || v2.Nbrs[f2]) return false;


        int cj = -1;
        float cd = float.MaxValue;

        for(int j = 4; j-- > 0;) {
            float d = 0;
            for(int i = 4; i-- > 0;) {
                int vi1 = FaceVi[(int)f1, i];
                int vi2 = FaceVi[(int)f2, (4 + j - i) % 4];
                d += (cntx.Crnr[v1, vi1] - cntx.Crnr[v2, vi2]).magnitude;
            }
            if(d < cd) {
                cj = j;
                cd = d;
            }
        }

        for(int i = 4; i-- > 0;) {
            int vi1 = FaceVi[(int)f1, i], vi2 = FaceVi[(int)f2, (4 + cj - i) % 4];

            if(DrawFlag) {
                var c = checkVertex(cntx.Vert[v1, vi1], cntx.Vert[v2, vi2]);
                var col = c ? Color.green : Color.red;
                Debug.DrawLine(transform.TransformPoint(cntx.Crnr[v1, vi1]), transform.TransformPoint(cntx.Crnr[v2, vi2]), col);

                if(i == DebugIter2) {
                    Vertex vrtx1 = cntx.Vert[v1, vi1], vrtx2 = cntx.Vert[v2, vi2];
                    Debug.Log("------------------------");
                    Debug.Log("pass  " + c + "     " + cntx.Crnr[v1, vi1] + "    " + cntx.Crnr[v2, vi2]);
                    Debug.Log(" null   " + vrtx1 + "  " + vrtx2); if(vrtx1 == null || vrtx2 == null) continue;

                    Debug.Log(" ReferenceEquals  " + ReferenceEquals(vrtx1, vrtx2));
                    Debug.Log(" vm   " + (vrtx1.VMask & vrtx2.VMask) + "  " + vrtx1.VMask + "  " + vrtx2.VMask);
                    if((vrtx1.VMask & vrtx2.VMask) != 0) continue;

                    Debug.Log(" vc   " + vrtx1.Vc + "  " + vrtx2.Vc);
                    /*
                    for(int i = vrtx1.Vc; i-- > 0;) {
                        var v1 = vrtx1.Vox[i];
                        if((v1.VGroup & vrtx2.VMask) == 0) continue;
                        for(int j = vrtx2.Vc; j-- > 0;)
                            if(vrtx2.Vox[j] == v1) return false;
                    } */
                }
            } else
                if(!checkVertex(cntx.Vert[v1, vi1], cntx.Vert[v2, vi2])) return false;
        }
        if(DrawFlag) {

            return false;
        }
        for(int i = 4; i-- > 0;) {
            int vi1 = FaceVi[(int)f1, i], vi2 = FaceVi[(int)f2, (4 + cj - i) % 4];
            mergeVertex(cntx, ref cntx.V_Verts[v1.TInd, vi1], ref cntx.V_Verts[v2.TInd, vi2], v1, v2);
            Debug.Assert(ReferenceEquals(cntx.Vert[v1, vi1], cntx.Vert[v2, vi2]));
        }
        v1.Nbrs[f1] = v2;
        v2.Nbrs[f2] = v1;

        return true;
    }
    void doNbrStuff(BuildCntx cntx) {

        SortedList<float, NbrLink> nLinks = new SortedList<float, NbrLink>(new DuplicateKeyComparer<float>());
        foreach(Voxel v in Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;

            List<Voxel> pn = new List<Voxel>();
            foreach(var n in v.PossNbr) {
                bool t = false;
                var vec = v.Pos - n.Pos;
                var vecN = vec.normalized;
                float lim = 0.8f;
                int cFi1 = -1, cFi2 = -1;

                float cD = 3.0f;
                v.forEach((FaceT fi1) => {

                    if(Vector3.Dot(v.MP[(int)fi1, 1], -vecN) < lim) return;
                    //todo -  break forEach
                    // cFi1 = (int)fi1;

                    n.forEach((FaceT fi2) => {
                        if(Vector3.Dot(n.MP[(int)fi2, 1], vecN) < lim) return;
                        if(Vector3.Dot(n.MP[(int)fi2, 1], -v.MP[(int)fi1, 1]) < 0.8f) return;
                        //todo -  break forEach
                        //cFi2 = (int)fi2;
                        t = true;
                        float d = (v.MP[(int)fi1, 0] - n.MP[(int)fi2, 0]).sqrMagnitude;
                        if(d < cD) {

                            cD = d;
                            cFi1 = (int)fi1;
                            cFi2 = (int)fi2;

                            nLinks.Add(d, new NbrLink() {
                                V1 = v,
                                V2 = n,
                                Fi1 = fi1,
                                Fi2 = fi2
                            });
                        }
                    });
                });
                if(t) pn.Add(n);
            }
            v.PossNbr = pn;
        }

        for(int iter = 99999; ;) {

            var en = nLinks.GetEnumerator();
            if(!en.MoveNext()) break;
            var kvp = en.Current;
            nLinks.RemoveAt(0);

            // if(kvp.Value.V1.Nbrs[kvp.Value.Fi1] != null || kvp.Value.V2.Nbrs[kvp.Value.Fi2] != null)
            //    continue;

            DrawFlag = (iter <= 1);
            if(!merge(cntx, kvp.Value.V1, kvp.Value.Fi1, kvp.Value.V2, kvp.Value.Fi2)) {
                //   if(DrawFlag)
                //       break;
                // continue;                
            }
            if(--iter <= 0) break;
        }
    }


    void rebuild() {
        prep();
        IsDirty = false;

        if(MF.sharedMesh) DestroyImmediate(MF.sharedMesh);
        Mesh m = MF.sharedMesh = new Mesh();

        var cntx = new BuildCntx(this);
        int triCnt = 0, vrtCnt = 0, vIndex = 0;

        System.Func<Vector4, Voxel, Vertex> getV = (Vector4 p, Voxel v) => {
            var ret = new Vertex() { V = p, Vc = 1, VMask = v.VGroup };
            ret.Vox[0] = v;
            cntx.Verts.Add(ret);
            return ret;
        };

        //List<int> isolated = 
        for(int l = 0; l < Vox.Count; l++) {
            var v = Vox[l];
            v.TInd = l;
            //v.Vi = null;
            v.PossNbr.Clear();
            v.Nbrs.reset();
            v.NbrsD.reset();
            //            var of = v.Faces
            v.Faces = null;

            v.Pos = v.transform.localPosition;
            v.Rot = v.transform.localRotation;
            v.Scale = v.transform.localScale;

            v.State = Voxel.StateE.Pending;

            for(int j = l; j-- > 0;) {
                var v2 = Vox[j];
                if(v2.State == Voxel.StateE.Invalid) continue;

                var vec = v.Pos - v2.transform.localPosition;
                var sm = vec.sqrMagnitude;
                if(sm > 4.0f)
                    continue;
                if(sm < 0.25f) {
                    v.State = Voxel.StateE.Invalid;
                    goto label_breakContinue;
                }
                v.PossNbr.Add(v2);
            }

            v.MP = v.getMidPoints();
            v.VGroup = 1 << (vIndex++ % 64);

            var mp = v.MP;

            for(int i = 2; i-- > 0;) {
                // Vector3 n2 = Vector3.left;
                for(int j = 2; j-- > 0;) {
                    Vector3 lp, lv;
                    int fi1 = ((int)FaceT.Down - i), fi2 = ((int)FaceT.Left - j);
                    if(!Math_JC.planePlaneIntersection(out lp, out lv, mp[fi1, 1], mp[fi1, 0], mp[fi2, 1], mp[fi2, 0])) Debug.LogError("err");

                    lv.Normalize();
                    // Vector3 n3 = Vector3.forward;
                    for(int k = 2; k-- > 0;) {
                        int fi3 = ((int)FaceT.Back - k);
                        if(!Math_JC.linePlaneIntersection(out lp, lp, lv, mp[fi3, 1], mp[fi3, 0])) Debug.LogError("err");

                        int ii = i * 4 + j * 2 + k;
                        cntx.Crnr[v, ii] = lp;

                        if(v.Crnr[ii] != null && v.Crnr[ii].Flag) {
                            //Debug.Log("clipped");
                            cntx.Flag[v, ii] = true;
                        }
                    }
                }
            }
            int effFc = 6;
            v.Faces = new Face[6];
            v.forEach((FaceT fi) => {
                int vc = 4;
                for(int i = 4; i-- > 0;) {
                    if(cntx.Flag[v, FaceVi[(int)fi, i]]) {
                        vc--;
                        if(vc == 2) effFc--;
                    }
                }

                cntx.FaceVc[v, fi] = (char)vc;
                /*
                v.Faces[(int)fi] = new Face() {
                    V1 = v, N = v.nrm2(fi),
                }; */
            });


            Debug.Log("effFc  " + effFc);
            if(effFc < 3) {
                v.State = Voxel.StateE.Invalid;
                continue;
            }

            SimpleBitField mask = new SimpleBitField();
            int ca_NbrI = -1;
            System.Func<int, int> countAdj = (int c ) => {
                int nc = 0;
                for(int j = 3; j-- > 0;) {
                    var oi = VertAdj[c, j];
                    if(cntx.Flag[v, oi]) {
                        nc++;
                        ca_NbrI = j;
                        //mask[j] = true;
                    }
                }
                return nc;
            };
            
            for(int i = 8; i-- > 0;) {
                var c = i;
                if(!cntx.Flag[v, c] || mask[c] ) continue; //<-- optimisable -todo

                int nc = countAdj(c);
                for(;;) {
                    switch(nc) {
                        case 0: {//a tri
                                Debug.Log("tri " + c);
                                var t = new Tri() { };
                                for(int j = 3; j-- > 0;) {
                                    //Debug.DrawLine(transform.TransformPoint(cntx.Crnr[v, c]), transform.TransformPoint(cntx.Crnr[v, VertAdj[c, j]]));
                                    t.Verts[j] = VertAdj[c, j];
                                }

                                cntx.Tris.Add(t);
                                mask[c] = true;
                                break;
                            }
                        case 1: { //quad or we adj to a  2 or 3
                                var j = ca_NbrI;
                                var n = VertAdj[c, j];
                                nc = countAdj(n);
                                Debug.Assert(!mask[n]);
                                if(nc != 1) {  //recentre on n                             
                                    Debug.Log("  !quad recentre! " + c);
                                    c = n;
                                    Debug.Assert(nc != 0);
                                    continue;
                                }
                                var q = new Quad() { };
                                q.Verts.A = VertAdj[c, (j + 1) % 3];
                                q.Verts.B = VertAdj[c, (j + 2) % 3];
                                q.Verts.C = VertAdj[n, (ca_NbrI + 1) % 3];
                                q.Verts.D = VertAdj[n, (ca_NbrI + 2) % 3];

                                cntx.Quads.Add(q);
                                Debug.Log("quad " + c + " -> " + n);
                                mask[c] = mask[n] = true;
                                break;
                            }
                        case 2: { //double tri -- sharp corner of two slopes
                                
                                int n1 = VertAdj[c, ca_NbrI], n2 = VertAdj[c, (ca_NbrI + 1) % 3], n3 = VertAdj[c, (ca_NbrI + 2) % 3];
                                if(!cntx.Flag[v, n2]) Math_JC.swap(ref n2, ref n3);

                                Debug.Log("d tri " + c + "  n1 " + n1 + "  n2 " + n2  ); 
                                Debug.Assert(!mask[n1]);
                                Debug.Assert(!mask[n2]);
                                System.Action<int> dt = (int a) => {
                                    var t = new Tri() { };
                                    for(int j = 3; j-- > 0;) {   
                                        var k = VertAdj[a, j];
                                        if(k == c) k = n3;
                                        //Debug.DrawLine(transform.TransformPoint(cntx.Crnr[v, c]), transform.TransformPoint(cntx.Crnr[v, k]));
                                        t.Verts[j] = k;
                                    }
                                    cntx.Tris.Add(t);
                                };
                                dt(n1);
                                dt(n2);
                                mask[c] = mask[n1] = mask[n2] = true;
                                break;
                            }
                        case 3: {//single tri but on the adjacent to the opposite of C -- beveled corner of two slopes
                                Debug.Log(" s3  tri  c " + c);
                                mask[c] = true;
                                var t = new Tri() { };
                                cntx.Tris.Add(t);
                                int vc = 2;
                                for(int j = 3; j-- > 0;) {
                                    var n = VertAdj[c, j];
                                    Debug.Assert(!mask[n]);
                                    mask[n] = true;

                                    for(int k = 3; k-- > 0;) {
                                        var n2 = VertAdj[n, k];
                                        if(mask[n2]) continue;
                                        mask[n2] = true;
                                        t.Verts[vc--] = n2;
                                        break;
                                    }
                                    
                                }
                                break;
                            }
                    }
                    break;
                }
            }
            label_breakContinue:;
        }

        doNbrStuff(cntx);

        for(int j = Vox.Count; j-- > 0;) {
            var v = Vox[j];
            if(v.State == Voxel.StateE.Invalid) continue;

            for(int i = 8; i-- > 0;) {
                Vector4 v2 = cntx.Crnr[v, i];
                v2.w = 1;

                if(cntx.Vert[v, i] != null) {
                    var vert = cntx.Vert[v, i];
                    vert.V += v2;
                } else
                    cntx.Vert[v, i] = getV(v2, v);
            }
        }

        for(int i = cntx.Verts.Count; i-- > 0;) {
            var v1 = cntx.Verts[i];
            if(v1.Vc <= 0) continue;
            cntx.Verts[i].V = (Vector3)v1.V / v1.V.w;
        }
        foreach(Voxel v in Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;

            for(int i = 8; i-- > 0;) {
                var vrt = cntx.Vert[v, i];
                if(vrt.Crnr == null)
                    vrt.Crnr = new Voxel.Corner() {
                        V = vrt.V,
                        Flag = cntx.Flag[v, i]
                    };
                v.Crnr[i] = vrt.Crnr;
            }
        }

        foreach(var t in cntx.Tris) {
            var v = Vox[0];
            for(int i = 3; i-- > 0;)
                t.Verts[i] = cntx.Vi[v, t.Verts[i]];


            t.F = new Face() { V1 = v, };

            var tr = new TriRef();
            tr.Vox = v;
            tr.D = 0;
            t.F.Tr = tr;
        }
        foreach(var t in cntx.Quads) {
            var v = Vox[0];
            for(int i = 4; i-- > 0;)
                t.Verts[i] = cntx.Vi[v, t.Verts[i]];


            t.F = new Face() { V1 = v, };

            var tr = new TriRef();
            tr.Vox = v;
            tr.D = 0;
            t.F.Tr = tr;
        }

        foreach(Voxel v in Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;

            v.forEach((FaceT fc) => {
                var nbr = v.Nbrs[fc];
                FaceT oi;
                Face f;
                if(nbr != null && nbr.Faces[(int)(oi = nbr.opp(v, fc))] != null) {
                    f = v.Faces[(int)fc] = nbr.Faces[(int)oi];
                    Debug.Assert(v.Faces[(int)fc].V2 == null);
                    v.Faces[(int)fc].V2 = v;
                } else {
                    float inset = Inset;
                    f = new Face() {
                        V1 = v,
                        Vc = cntx.FaceVc[v, fc]
                    };

                    v.Faces[(int)fc] = f;
                    Vector4 tp = Vector3.zero;
                    /*
                    for(int i = 4; i-- > 0;) {
                        if(!cntx.Flag[v, FaceVi[(int)fc, i]]) {
                            tp += cntx.Vert[v, FaceVi[(int)fc, i]].V;
                            Debug.Log("i  " + i + "    -> " + FaceVi[(int)fc, i]);
                        }
                    }
                   // Debug.Assert(f.Vc == cntx.FaceVc[v, fc]); //*/
                    var tr = new TriRef();
                    tr.Vox = v;
                    tr.D = fc;
                    f.Tr = tr;

                    if(nbr == null) {
                        if(f.Vc == 4) {
                            var s = new Quad() { F = f };
                            for(int i = 4; i-- > 0;) {
                                s.Verts[i] = cntx.Vi[v, FaceVi[(int)fc, i]];
                                //  Debug.Log("i  " + i + "    -> " + FaceVi[(int)fc, i]);
                            }
                            cntx.Quads.Add(s);
                        } else if(f.Vc == 3) {
                            var s = new Tri() { F = f };
                            for(int i = 4, j = 2; i-- > 0;)
                                if(!cntx.Flag[v, FaceVi[(int)fc, i]])
                                    s.Verts[j--] = cntx.Vi[v, FaceVi[(int)fc, i]];
                            cntx.Tris.Add(s);
                        }
                    }
                }

            });
        }

        /* 
        Vector3[] faceV = new Vector3[4];
        Vector3[] faceE = new Vector3[4];
        Vector3[] faceOff = new Vector3[4];
        Vector3[] faceCp = new Vector3[4];

        foreach(Voxel v in Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;

            v.forEach((FaceT fc) => {
                var nbr = v.Nbrs[fc];
                FaceT oi;
                Face f;
                if(nbr != null && nbr.Faces[(int)( oi = nbr.opp(v, fc) ) ] != null ) {
                    f = v.Faces[(int)fc] = nbr.Faces[(int)oi];
                    Debug.Assert(v.Faces[(int)fc].V2 == null);
                    v.Faces[(int)fc].V2 = v;

                    for(int i = 4; i-- > 0;) {
                        for(int j = 4; j-- > 0;) {                           
                            if(ReferenceEquals(cntx.Vert[v,FaceVi[(int)fc, j ]], cntx.Vert[nbr,FaceVi[(int)oi, i]])) {
                                f.CpOff[i] = j;
                                break;
                            }
                        }
                    }

                } else {
                    float inset = Inset;
                    f = new Face() {
                        V1 = v, //Vc = cntx.FaceVc[v, fc]
                    };

                    for(int i = 4; i-- > 0; ) 
                        faceV[i] = cntx.Vert[v,FaceVi[(int)fc, i]].V;

                    for(int i = 4, j = 0; i-- > 0; j = i)
                        faceE[i] = faceV[i] - faceV[j];

                    Vector3 n = Vector3.zero;
                    for(int i = 4, j = 0; i-- > 0; j = i)
                        n += Vector3.Cross(faceE[i], faceE[j]);
                    n.Normalize();
                    f.N = n;

                    for(int i = 4; i-- > 0; )
                        faceOff[i] = Vector3.Cross(faceE[i], n).normalized * inset;
                    /* 
                    for(int adjI = 4; adjI-- > 0;) {
                        var fai = FaceAdj[(int)fc, adjI];
                        int a = FaceAdjVerts[(int)fc, (int)fai, 0];
                        int b = FaceAdjVerts[(int)fc, (int)fai, 1];
                        var i = ( a == (b+1)%4) ? b: a;

                        if(v.Nbrs[fai] == null)
                            faceOff[i] = Vector3.Cross(faceE[i], n).normalized * inset;
                        else
                            faceOff[i] = Vector3.zero;

                        if(DrawCp) {
                            var col = Color.magenta;

                            if(v.Nbrs[fai] == null)
                                col = Color.green;

                            var mid = (faceV[i] + faceV[(i + 5) % 4]) * 0.5f;
                            var off = faceOff[i] + mid;
                            Debug.DrawLine(transform.TransformPoint(mid), transform.TransformPoint(off), col);
                        }
                    } *---/


                    for(int i = 4, j = 0; i -- > 0; j = i ) {
                        Vector3 v1 = faceV[i];
                        Vector3 v2 = faceV[j];
                        var mid = (v1 + v2) * 0.5f;
                        var offN = Vector3.Cross(v1 - v2, n).normalized;

                        var off = faceOff[i] + mid;

                        bool c1 = faceOff[i] == Vector3.zero, c2 = faceOff[j] == Vector3.zero;
                        Vector3 intr;
                        if(c1 == c2) {
                            intr = faceV[j];  //! -> j <- !
                            if(c1) {
                                
                            } else {
                                var o2 = faceOff[j] + faceV[j] - faceE[j] * 0.5f;
                                if(DrawCp) Debug.DrawLine(transform.TransformPoint(o2), transform.TransformPoint(off), Color.blue);

                                Vector3 s1, s2;
                                if(Math_JC.calculateLineLineIntersection(off, off + faceE[i], o2, o2 + faceE[j], out s1, out s2)) {

                                    //s1 && s2 should be very near identical ... check ? todo ?
                                    intr = (s1 + s2) * 0.5f;
 
                                    if(DrawCp) {
                                        Debug.DrawLine(transform.TransformPoint(s1), transform.TransformPoint(s2), Color.cyan);
                                        Debug.DrawLine(transform.TransformPoint(v2), transform.TransformPoint(s2), Color.cyan);
                                        Debug.DrawLine(transform.TransformPoint(s1), transform.TransformPoint(v2), Color.cyan);
                                    }
                                } else Debug.LogError("err..");
                            }
                        } else {
                            if(c1) {
                                intr = faceV[j] + faceE[i].normalized * inset;
                            } else {
                                intr = faceV[j] - faceE[j].normalized * inset;
                            }
                        }
                        f.Cp[j] = faceCp[j] = intr;  //! -> j <- !
                    }
                    if(DrawCp)
                        for(int i = 4, j = 0; i-- > 0; j = i) {
                            Debug.DrawLine(transform.TransformPoint(faceCp[i]), transform.TransformPoint(faceCp[j]), Color.white);
                        }

                    v.Faces[(int)fc] = f;

                    for(int i = 4; i-- > 0;) {
                        if(!cntx.Flag[v, FaceVi[(int)fc, i]])
                            f.Verts[f.Vc++] = cntx.Vert[v, FaceVi[(int)fc, i]];
                    }
                    Debug.Assert(f.Vc == cntx.FaceVc[v, fc]);

                    var tr = new TriRef();
                    tr.Vox = v;
                    tr.D = fc;
                    f.Tr = tr;

                }
                
                for(int adjI = FaceAdj2[(int)fc, 4]; adjI-- > 0;) {
                    var fi = (FaceT)FaceAdj2[(int)fc, adjI];
                    Debug.Assert(v.Faces[(int)fi] != null);
                    //if(v.Nbrs[fi]) continue;
                    var f1 = v.Faces[(int)fi];
                    for(int i = 2; i-- > 0;) {
                        var v1 = f.vert(v, FaceAdjVerts[(int)fc, (int)fi, i]);
                        var v2 = f1.vert(v, FaceAdjVerts[(int)fi, (int)fc, i]);
                        var v3 = f.vert(v, FaceAdjVerts[(int)fc, (int)fi, 1 - i]);
                        if(DrawCp)
                            Debug.DrawLine(transform.TransformPoint(v1), transform.TransformPoint(v2), Color.white);
                        // Debug.DrawLine(transform.TransformPoint(v3), transform.TransformPoint(v2), Color.white);
                    }
                    //triCnt += 2;
                    //vrtCnt += 4;


                    for(int adjI2 = FaceAdj2[(int)fi, 4]; adjI2-- > 0;) {

                        var fi2 = (FaceT)FaceAdj2[(int)fi, adjI2];
                        var f2 = v.Faces[(int)fi2];

                        for(int i = 2; i-- > 0;) {  //todo - optimise  ---->> lookup
                            int i2 = i;
                            if(!ReferenceEquals(cntx.Vert[v,FaceVi[(int)fi, FaceAdjVerts[(int)fi, (int)fc, i]]], cntx.Vert[v,FaceVi[(int)fi, FaceAdjVerts[(int)fi, (int)fi2, i]]])) i2 = 1 - i2;
                            if(!ReferenceEquals(cntx.Vert[v,FaceVi[(int)fi, FaceAdjVerts[(int)fi, (int)fc, i]]], cntx.Vert[v,FaceVi[(int)fi, FaceAdjVerts[(int)fi, (int)fi2, i2]]])) continue;

                            var v1 = f.vert(v, FaceAdjVerts[(int)fc, (int)fi, i]);
                            var v2 = f1.vert(v, FaceAdjVerts[(int)fi, (int)fc, i]);
                            var v3 = f2.vert(v, FaceAdjVerts[(int)fi2, (int)fi, i2]);

                            //triCnt += 1;
                           // vrtCnt += 3;

                            if(DrawCp) {
                                var v4 = f1.vert(v, FaceAdjVerts[(int)fi, (int)fi2, i2]);

                                Debug.DrawLine(transform.TransformPoint(v1), transform.TransformPoint(v2), Color.white);
                                Debug.DrawLine(transform.TransformPoint(v3), transform.TransformPoint(v2), Color.red);
                                Debug.DrawLine(transform.TransformPoint(v3), transform.TransformPoint(v1), Color.blue);

                                Debug.DrawLine(transform.TransformPoint(v2), transform.TransformPoint(v4), Color.black);
                            }
                            break;
                        }

                    }
                }
            });

            v.toMesh(cntx,(TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) => {
                triCnt += 2;
                vrtCnt += 4;
            }, (TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3) => {
                triCnt += 1;
                vrtCnt += 3;
            });
        } */

        triCnt = cntx.Tris.Count + cntx.Quads.Count * 2;
        vrtCnt = cntx.Tris.Count * 3 + cntx.Quads.Count * 4;

        Debug.Log("cntx.Tris.Count " + cntx.Tris.Count + "   cntx.Quads.Count " + cntx.Quads.Count);
        var inds = new int[triCnt * 3];
        var vrts = new Vector3[vrtCnt];
        var nrms = new Vector3[vrtCnt];
        var uv = new Vector2[vrtCnt];

        Tris = new TriRef[triCnt];
        int vi = 0, ti = 0;

        foreach(var s in cntx.Tris) {
            for(int i = 2; i < 3; i++) {
                int ii = ti * 3;
                Tris[ti++] = s.F.Tr;

                inds[ii] = vi;
                inds[ii + 1] = vi + i - 1;
                inds[ii + 2] = vi + i;
            }
            uv[vi] = new Vector2(0, 0);
            uv[vi + 1] = new Vector2(0, 1);
            uv[vi + 2] = new Vector2(1, 1);

            vrts[vi] = cntx.Vp[s.Verts.A];
            vrts[vi + 1] = cntx.Vp[s.Verts.B];
            vrts[vi + 2] = cntx.Vp[s.Verts.C];
            var n = Vector3.Cross(vrts[vi + 1] - vrts[vi], vrts[vi + 2] - vrts[vi]).normalized;
            for(int i = 0; i < 3; i++) {
                //vrts[vi] = vert(f, i);
                nrms[vi++] = n;
            }
        }
        foreach(var s in cntx.Quads) {
            for(int i = 2; i < 4; i++) {
                int ii = ti * 3;
                Tris[ti++] = s.F.Tr;

                inds[ii] = vi;
                inds[ii + 1] = vi + i - 1;
                inds[ii + 2] = vi + i;
            }
            uv[vi] = new Vector2(0, 0);
            uv[vi + 1] = new Vector2(0, 1);
            uv[vi + 2] = new Vector2(1, 1);
            uv[vi + 3] = new Vector2(1, 0);

            vrts[vi] = cntx.Vp[s.Verts.A];
            vrts[vi + 1] = cntx.Vp[s.Verts.B];
            vrts[vi + 2] = cntx.Vp[s.Verts.C];
            vrts[vi + 3] = cntx.Vp[s.Verts.D];

            var n = Vector3.Cross(vrts[vi + 1] - vrts[vi], vrts[vi + 3] - vrts[vi]).normalized;
            for(int i = 0; i < 4; i++) {
                //vrts[vi] = vert(f, i);
                nrms[vi++] = n;
            }
        }
        /*
        foreach(Voxel v in Vox) {
            break;
            if(v.State == Voxel.StateE.Invalid) continue;
            
            v.toMesh(cntx,(TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)=> {

                for(int i = 2; i < 4; i++) {
                    int ii = ti * 3;
                    Tris[ti++] = tr;

                    inds[ii] = vi;
                    inds[ii + 1] = vi + i - 1;
                    inds[ii + 2] = vi + i;
                }
                uv[vi] = new Vector2(0, 0);
                uv[vi + 1] = new Vector2(0, 1);
                uv[vi + 2] = new Vector2(1, 1);
                uv[vi + 3] = new Vector2(1, 0);

                vrts[vi] = v1;
                vrts[vi + 1] = v2;
                vrts[vi + 2] = v3;
                vrts[vi + 3] = v4;
                for(int i = 0; i < 4; i++) {
                    //vrts[vi] = vert(f, i);
                    nrms[vi++] = n;
                }
            }, (TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3) => {
                for(int i = 2; i < 3; i++) {
                    int ii = ti * 3;
                    Tris[ti++] = tr;

                    inds[ii] = vi;
                    inds[ii + 1] = vi + i - 1;
                    inds[ii + 2] = vi + i;
                }
                uv[vi] = new Vector2(0, 0);
                uv[vi + 1] = new Vector2(0, 1);
                uv[vi + 2] = new Vector2(1, 1);

                vrts[vi] = v1;
                vrts[vi + 1] = v2;
                vrts[vi + 2] = v3;
                for(int i = 0; i < 3; i++) {
                    //vrts[vi] = vert(f, i);
                    nrms[vi++] = n;
                }
            });      
        } */


        m.vertices = vrts;
        m.normals = nrms;
        m.SetTriangles(inds, 0);
        m.uv = uv;

        MC.sharedMesh = m;

        CtorMain.Singleton.DirtySelection = true;
    }
    public bool DrawCp = false;

    [HideInInspector]
    [System.NonSerialized]
    public Transform Trnsfrm;
    MeshFilter MF;
    MeshRenderer MR;
    MeshCollider MC;


    void Start() {

        Trnsfrm = transform;
        MF = getAddComponenet<MeshFilter>();
        MR = getAddComponenet<MeshRenderer>();
        MC = getAddComponenet<MeshCollider>();

        Vox = new List<Voxel>();



        var c = GetComponentsInChildren<Voxel>();
        // if(c.Length != 1) Debug.LogError("err");


        //   Grid = new Voxel[1, 1, 1];
        //  Grid[0, 0, 0] = c[0];

        //c[0].init(this, new Voxel.GridIndex() );

        foreach(var v in c) {
            var p = v.transform.localPosition;
            //  Voxel.GridIndex i = new Voxel.GridIndex() { X = (int)p.x, Y = (int)p.y, Z = (int)p.z };
            // if(occupied(i)) {
            //    Destroy(v.gameObject);
            //      continue;
            //  }
            v.init(this);

        }

        // CtorMain.Singleton.UI.add(this);
    }
    /*
    void offset(ref Voxel.GridIndex i) {
        i.X += Ox;
        i.Y += Oy;
        i.Z += Oz;
    }


    public bool occupied(Voxel.GridIndex i) {
        return get(i);
    }
    public Voxel this[Voxel.GridIndex index] {
        get {
            offset(ref index);
            return Grid[index.X, index.Y, index.Z];
        }
        set {
            offset(ref index);
            Grid[index.X, index.Y, index.Z] = value;
        }
    }

    public void safeSet(Voxel v,  Voxel.GridIndex i) {

        int xi = i.X + Ox, yi = i.Y + Oy, zi = i.Z + Oz;

        int nOx = Ox, nOy = Oy, nOz = Oz;
        int nLx = Grid.GetLength(0), nLy = Grid.GetLength(1), nLz = Grid.GetLength(2);
        bool reG = false;
        if(xi < 0) { nOx -= xi; nLx -= xi; xi = 0; reG = true; }
        else if ( xi>= nLx ) { nLx = xi + 1; reG = true; }
        if(yi < 0) { nOy -= yi; nLy -= yi; yi = 0; reG = true; }
        else if ( yi>= nLy ) { nLy = yi + 1; reG = true; }
        if(zi < 0) { nOz -= zi; nLz -= zi; zi = 0; reG = true; }
        else if ( zi>= nLz ) { nLz = zi + 1; reG = true; }


        if(reG) {

            var og = Grid;

            Grid = new Voxel[nLx, nLy, nLz];

            for(int x = og.GetLength(0); x-- > 0;)
                for(int y = og.GetLength(1); y-- > 0;)
                    for(int z = og.GetLength(2); z-- > 0;) {
                        Grid[x - Ox + nOx, y - Oy + nOy, z - Oz + nOz] = og[x, y, z];
                    }

            Ox = nOx; Oy = nOy; Oz = nOz;
        }
        if(Grid[xi, yi, zi] != null) Debug.LogError("err");
        Grid[xi, yi, zi] = v;
    }

    public Voxel get(Voxel.GridIndex i) {
        offset(ref i);
        if(i.X < 0 || i.Y < 0 || i.Z < 0
            || i.X >= Grid.GetLength(0) || i.Y >= Grid.GetLength(1) || i.Z >= Grid.GetLength(2))
            return null;

        return Grid[i.X, i.Y, i.Z];
    }

   // public int Ox, Oy, Oz;
  //  public Voxel[,,] Grid;
  */
    public int Iteration = -1;

    //todo -- move
    public List<Voxel> Selection = new List<Voxel>();
    public TreeView_Ele Ui;

    public int DebugIter = 1;
    public int DebugIter2 = 0;
    public bool Inc = false, Dec = false, Constant = false;

    public void selectAll() {
        foreach(var v in Vox) {
            // for(int x = Grid.GetLength(0); x-- > 0;)
            // for(int y = Grid.GetLength(1); y-- > 0;)
            //for(int z = Grid.GetLength(2); z-- > 0;) {
            //  var v = Grid[x, y, z];
            //if(v == null) continue;
            if(v.enSelected())
                Selection.Add(v);
        }
    }

    int LastDI = -1;
    void Update() {

        if(Inc) {
            DebugIter++;
            Inc = false;
        }
        if(Dec) {
            DebugIter--;
            Dec = false;
        }
        if(DebugIter != LastDI) IsDirty = true;

        if(IsDirty || Constant) {
            rebuild();
            LastDI = DebugIter;
        }

    }

    public bool IsDirty = true;
    public void dirty() {
        if(IsDirty) return;

        IsDirty = true;

    }


    public override void gotHighlight(UIMain um) {
        //  Highlighted = true;
        //  upCol();
        keptHighlight(um);
    }
    public override void lostHighlight(UIMain um) {
        // Highlighted = false;
        // upCol();
    }
    public override void keptHighlight(UIMain um) {

    }
    public override void highlight_Cast(UIMain um, RaycastHit hit) {
        var cm = CtorMain.Singleton;
        // cm.Hl_D.FaceI = getFaceHlI(hit);
        cm.Hl_D.Wp = hit.point;
        cm.Hl_D.Nrm = hit.normal;
        cm.Hl_D.TriI = hit.triangleIndex;
        //        Debug.Log("hit tri " + hit.triangleIndex);
    }

}
