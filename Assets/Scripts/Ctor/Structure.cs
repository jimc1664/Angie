using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using System.IO;
using Unity.IO.Compression;

public enum FaceT {
    Left = 1, Right = 0, Up = 2, Down = 3, Forward = 4, Back = 5,
};

public class Structure : UIEle {
    /* public static readonly int[,] FaceVi_Old = new int[6, 4] {
         { 2,6,7,3 },
         { 0,1,5,4 },
         { 4,5,7,6 },
         { 0,2,3,1 },
         { 1,3,7,5 },
         { 0,4,6,2 },
    }; */
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

    public static readonly int[][] FaceVi = new int[6][] {
        new int[4] { 2,6,7,3 },
        new int[4] { 0,1,5,4 },
        new int[4] { 4,5,7,6 },
        new int[4] { 0,2,3,1 },
        new int[4] { 1,3,7,5 },
        new int[4] { 0,4,6,2 },
   };


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

    public enum StyleE {
        Solid,
        Frame,
        Pipe,
    };
    public StyleE Style;

    bool DrawFlag;
    

    public List<Voxel> Vox = new List<Voxel>();
    public List<Structure> Subs = new List<Structure>();
    public List<CtorComponent> Cmps = new List<CtorComponent>();

    T getAddComponenet<T>() where T : Component {
        var go = gameObject;
        T r = go.GetComponent<T>();
        if(r == null) r = go.AddComponent<T>();
        return r;
    }

    public class TriRef {
        public Voxel Vox;
        public Face F;
    };

    public TriRef[] Tris;

    public class Face {
        public int[] Vi;
        //        public Voxel Nbr;

        public SharedFace Sf;

        public int CpOff = 0;

        public Voxel V;

        public Voxel getNbr() {
            Voxel ret = null;
            if(Sf.F2 != null)
                ret = Sf.opp(this).V;
            return ret;
        }

        public struct Edges_S {
            public EdgeLink A, B, C, D;
            public EdgeLink this[int i] {
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

            public int getI(EdgeLink el) {
                for(int i = 0; i < 4; i++)
                    if(this[i] == el)
                        return i;
                Debug.LogError("not found");
                return -1;
            }
        };
        public Edges_S Edges;

        public void cp(int[] a) {
            if(Sf.F1 == this) {
                for(int i = Vi.Length; i-- > 0;)
                    a[i] = Sf.Cp[i];
            } else {
                for(int i = Vi.Length; i-- > 0;)
                    a[i] = Sf.Cp[(Vi.Length - i + CpOff) % Vi.Length];

            }
        }

        public int setCp(int i, int a) {
            if(Sf.F1 == this) {
                Sf.Cp[i] = a;
            } else {
                Sf.Cp[(Vi.Length - i + CpOff) % Vi.Length] = a;
            }
            return a;
        }
    };

    public class SharedFace {
        public SharedFace() {
            // Cp.reset();
        }
        public TriRef Tr;
        public Vector3 Nrm, Mid;

        public Face F1, F2;
        public Face opp(Face th) {
            return th == F1 ? F2 : F1;
        }
        public struct Cp_S {
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
        public Cp_S Cp;

    };

    public float Inset = 0.1f;

    public class NbrLink {
        public Voxel V1, V2;
        public int Fi1, Fi2;
    };
    public class Vertex {
        public Vector4 V;
        public List<Voxel> Vox = new List<Voxel>(8);
       // public int Vc = 0;
        public long VMask = 0;

        public int Vi = -1;
        public Voxel.Corner Crnr;
    };
    public class Edge {
        public struct Vi_S {
            public int A, B;
            public int this[int i] {
                get {
                    switch(i) {
                        case 0: return A;
                        case 1: return B;
                    }
                    Debug.LogError("i OOB");
                    return -1;
                }
                set {
                    switch(i) {
                        case 0: A = value; return;
                        case 1: B = value; return;
                    }
                    Debug.LogError("i OOB");
                }
            }
            public void reset() {
                A = B = -1;
            }
        };
        public Vi_S InVi;

        public Vertex V1;
        public int Ref = 0;
    };
    public class EdgeLink {
        public int EdgeI;
        public Face F1, F2;

        public Face opp(Face f) { return f == F1 ? F2 : F1; }
    }
    public class Quad {
        //        public Face_Old F;
        public Face F;
        public Voxel V;
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
        //        public Face_Old F;
        public Face F;
        public Voxel V;
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

            int sc = 0;
            Vox = new List<Voxel>();
            s.foreach_Structure((Structure sub) => {
                Vox.AddRange(sub.Vox);
                sub.TInd = sc++;
            });
            NLinks = new SortedList<float, NbrLink>[sc];
            for( int i = sc; i-- >0; )
                NLinks[i] = new SortedList<float, NbrLink>(new DuplicateKeyComparer<float>());

            int vc = Vox.Count;
            
            var Crnrs = new Vector3[vc, 8];
            V_Verts = new Vertex[vc, 8];
            Vert = new Vert_Fetch() { Dat = V_Verts };
            Crnr = new Crnr_Fetch() { Dat = Crnrs };
            Flag = new Flag_Fetch() { Dat = new SimpleBitField[vc] };
            FaceVc = new FaceVc_Fetch() { Dat = new char[vc, 6] };
            Vi = new Vi_Fetch() { cntx = this };
        }

        public SortedList<float, NbrLink>[] NLinks;

        public List<Voxel> Vox;

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

        public int addVp(Vector3 v) {
            var ret = Vp.Count;
            Vp.Add(v);
            return ret;
        }
        public List<Vector3> Vp = new List<Vector3>();
        public List<Tri> Tris = new List<Tri>();
        public List<Quad> Quads = new List<Quad>();
        //public List<Edge> Edges = new List<Edge>();
        public Edge[] Edges;
    };
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
                //Vc = 2,
                VMask = v1.VGroup | v2.VGroup,
                V = Vector4.zero,
            };
            cntx.Verts.Add(v);
            vrtx1 = v;
            vrtx2 = v;
            v.Vox.Add( v1 );
            v.Vox.Add(v2 );
        } else if(vrtx1 != null && vrtx2 != null) {
            if(ReferenceEquals(vrtx1, vrtx2)) return;
            //vrtx2.V += vrtx1.V;
            vrtx2.VMask |= vrtx1.VMask;
            var vrtx1c = vrtx1;
            for(int k = vrtx1.Vox.Count; k-- > 0;) {
                var vox = vrtx1c.Vox[k];
                vrtx2.Vox.Add( vox );
                for(int l = 8; l-- > 0;) {
                    if(cntx.Vert[vox, l] == vrtx1c) {
                        cntx.Vert[vox, l] = vrtx2;
                        break;
                    }
                    Debug.Assert(l != 0);
                }
            }
            Debug.Assert(ReferenceEquals(vrtx1, vrtx2));
          //  vrtx1c.Vc = 0; //todo recycle
            vrtx1c.Vox.Clear();//todo recycle
            //vrtx1 = vrtx2;
        } else if(vrtx1 == null) {
            vrtx1 = vrtx2;
            vrtx2.Vox.Add(v1);
            vrtx2.VMask |= v1.VGroup;
        } else {
            vrtx2 = vrtx1;
            vrtx1.Vox.Add(v2);
            vrtx1.VMask |= v2.VGroup;
        }
    }
    bool checkVertex(Vertex vrtx1, Vertex vrtx2) {
        if(vrtx1 == null || vrtx2 == null) return true;
        if(ReferenceEquals(vrtx1, vrtx2)) return true;
        if((vrtx1.VMask & vrtx2.VMask) == 0) return true;

        for(int i = vrtx1.Vox.Count; i-- > 0;) {
            var v1 = vrtx1.Vox[i];
            if((v1.VGroup & vrtx2.VMask) == 0) continue;
            for(int j = vrtx2.Vox.Count; j-- > 0;)
                if(vrtx2.Vox[j] == v1) return false;
        }
        return true;
    }
    bool merge(BuildCntx cntx, Voxel v1, int fi1, Voxel v2, int fi2) {

        if(v1.State == Voxel.StateE.Invalid || v1.State == Voxel.StateE.Invalid)
            return false;
        Face f1 = v1.Faces[fi1], f2 = v2.Faces[fi2];

        if(DrawFlag) {
            var c = !(f1.Sf.F2 != null || f2.Sf.F2 != null);
            var col = c ? Color.green : Color.red;
            Debug.DrawLine(v1.transform.position, v2.transform.position, col);
        }

        if(f1.Sf.F2 != null || f2.Sf.F2 != null) return false;

        foreach(var f in v2.Faces)
            if(f.getNbr() == v1) return false;

        int cj = -1;
        float cd = float.MaxValue;

        for(int j = f1.Vi.Length; j-- > 0;) {
            float d = 0;

            for(int i = f1.Vi.Length; i-- > 0;) {
                int vi1 = f1.Vi[i];
                int vi2 = f2.Vi[(f1.Vi.Length + j - i) % f1.Vi.Length];
                //if(cntx.Flag[v1, vi1] || cntx.Flag[v1, vi1]) continue;
                d += (cntx.Crnr[v1, vi1] - cntx.Crnr[v2, vi2]).magnitude;
            }
            if(d < cd) {
                cj = j;
                cd = d;
            }
        }

        for(int i = f1.Vi.Length; i-- > 0;) {
            int vi1 = f1.Vi[i], vi2 = f2.Vi[(f1.Vi.Length + cj - i) % f1.Vi.Length];

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

                    Debug.Log(" vc   " + vrtx1.Vox.Count + "  " + vrtx2.Vox.Count);
                }
            } else
                if(!checkVertex(cntx.Vert[v1, vi1], cntx.Vert[v2, vi2])) return false;
        }
        if(DrawFlag) {

            return false;
        }
        for(int i = f1.Vi.Length; i-- > 0;) {
            int vi1 = f1.Vi[i], vi2 = f2.Vi[(f1.Vi.Length + cj - i) % f1.Vi.Length];
            /*bool flg1 = cntx.Flag[v1, vi1], flg2 = cntx.Flag[v2, vi2];
            if( flg1 != flg2 ) {
                if(flg1) {
                    cj--;
                    continue;
                }
                cj++;
                vi2 = FaceVi_Old[(int)f2, (8 + cj - i) % 4];
            } */
            mergeVertex(cntx, ref cntx.V_Verts[v1.TInd, vi1], ref cntx.V_Verts[v2.TInd, vi2], v1, v2);
            Debug.Assert(ReferenceEquals(cntx.Vert[v1, vi1], cntx.Vert[v2, vi2]));
        }
        f1.Sf = f2.Sf;
        f1.Sf.F2 = f1;

        return true;
    }
    void doNbrStuff(BuildCntx cntx) {

        SortedList<float, NbrLink> nLinks = new SortedList<float, NbrLink>(new DuplicateKeyComparer<float>());
        foreach(Voxel v in Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            // Debug.Log("nbr  " + v +"  pn "+ v.PossNbr.Count );

          //  List<Voxel> pn = new List<Voxel>();
            foreach(var n in v.PossNbr) {
                //bool t = false;
                var vec = v.Pos - n.Pos;
                var vecN = vec.normalized;
                float lim = 0.6f;

                float cD = 3.0f;

                for(int i = v.Faces.Length; i-- > 0;) {
                    var f1 = v.Faces[i];

                    // Debug.Log("d1 " + Vector3.Dot(f1.Sf.Nrm, -vecN));
                    if(Vector3.Dot(f1.Sf.Nrm, -vecN) < lim) continue;
                    for(int j = n.Faces.Length; j-- > 0;) {
                        var f2 = n.Faces[j];
                        if(f1.Vi.Length != f2.Vi.Length) continue;
                        //Debug.Log("d2 " + Vector3.Dot(f2.Sf.Nrm, vecN));
                        //Debug.Log("d3 " + Vector3.Dot(f2.Sf.Nrm, -f1.Sf.Nrm));

                        if(Vector3.Dot(f2.Sf.Nrm, vecN) < lim) continue;
                        if(Vector3.Dot(f2.Sf.Nrm, -f1.Sf.Nrm) < lim) continue;
                        //todo -  break forEach
                        //cFi2 = (int)fi2;
                        //t = true;
                        float d = (f1.Sf.Mid - f2.Sf.Mid).sqrMagnitude;
                        if(d < cD) {
                            cD = d;
                            nLinks.Add(d, new NbrLink() {
                                V1 = v,
                                V2 = n,
                                Fi1 = i,
                                Fi2 = j
                            });
                        }
                    }
                }
               // if(t) pn.Add(n);
            }
           // v.PossNbr = pn;
        }

        for(int iter = 9999; ;) {
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

    void doClipping(BuildCntx cntx, Voxel v, List<Face> tFaceL) {
        SimpleBitField mask = new SimpleBitField();
        int ca_NbrI = -1;
        System.Func<int, int> countAdj = (int c) => {
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
            if(!cntx.Flag[v, c] || mask[c]) continue; //<-- optimisable -todo

            int nc = countAdj(c);
            for(;;) {
                switch(nc) {
                    case 0: {//a tri
                         //   Debug.Log("tri " + c);
                            var f = new Face() {
                                Vi = new int[3],
                                V = v,
                            };
                            //var t = new Tri() { V = v };
                            for(int j = 3; j-- > 0;) {
                                //Debug.DrawLine(transform.TransformPoint(cntx.Crnr[v, c]), transform.TransformPoint(cntx.Crnr[v, VertAdj[c, j]]));
                                //t.Verts[j] = VertAdj[c, j];
                                f.Vi[j] = VertAdj[c, j];
                            }
                            tFaceL.Add(f);
                            //cntx.Tris.Add(t);
                            mask[c] = true;
                            break;
                        }
                    case 1: { //quad or we adj to a  2 or 3
                            var j = ca_NbrI;
                            var n = VertAdj[c, j];
                            nc = countAdj(n);
                            Debug.Assert(!mask[n]);
                            if(nc != 1) {  //recentre on n                             
                           //     Debug.Log("  !quad recentre! " + c);
                                c = n;
                                Debug.Assert(nc != 0);
                                continue;
                            }
                            /*
                            var q = new Quad() { V = v };
                            q.Verts.A = VertAdj[c, (j + 1) % 3];
                            q.Verts.B = VertAdj[c, (j + 2) % 3];
                            q.Verts.C = VertAdj[n, (ca_NbrI + 1) % 3];
                            q.Verts.D = VertAdj[n, (ca_NbrI + 2) % 3]; 
                             cntx.Quads.Add(q); */


                            tFaceL.Add(new Face() {
                                V = v,
                                Vi = new int[4] {
                                            VertAdj[c, (j + 1) % 3],
                                            VertAdj[c, (j + 2) % 3],
                                            VertAdj[n, (ca_NbrI + 1) % 3],
                                            VertAdj[n, (ca_NbrI + 2) % 3],
                                        },
                            });

                          //  Debug.Log("quad " + c + " -> " + n);
                            mask[c] = mask[n] = true;

                            //special case -- can form neighbour links
                            //continue here

                            break;
                        }
                    case 2: { //double tri -- sharp corner of two slopes

                            int n1 = VertAdj[c, ca_NbrI], n2 = VertAdj[c, (ca_NbrI + 1) % 3], n3 = VertAdj[c, (ca_NbrI + 2) % 3];
                            if(!cntx.Flag[v, n2]) Math_JC.swap(ref n2, ref n3);

                           // Debug.Log("d tri " + c + "  n1 " + n1 + "  n2 " + n2);
                            Debug.Assert(!mask[n1]);
                            Debug.Assert(!mask[n2]);
                            System.Action<int> dt = (int a) => {
                                var f = new Face() { V = v, Vi = new int[3], };
                                tFaceL.Add(f);
                                //var t = new Tri() { V = v };
                                for(int j = 3; j-- > 0;) {
                                    var k = VertAdj[a, j];
                                    if(k == c) k = n3;
                                    //Debug.DrawLine(transform.TransformPoint(cntx.Crnr[v, c]), transform.TransformPoint(cntx.Crnr[v, k]));
                                    //t.Verts[j] = k;
                                    f.Vi[j] = k;
                                }
                                //cntx.Tris.Add(t);
                            };
                            dt(n1);
                            dt(n2);
                            mask[c] = mask[n1] = mask[n2] = true;
                            break;
                        }
                    case 3: {//single tri but on the adjacent to the opposite of C -- beveled corner of two slopes
                          //  Debug.Log(" s3  tri  c " + c);
                            mask[c] = true;
                            //var t = new Tri() { V = v };
                            //cntx.Tris.Add(t);
                            var f = new Face() { V = v, Vi = new int[3], };
                            tFaceL.Add(f);
                            int vc = 2;
                            for(int j = 3; j-- > 0;) {
                                var n = VertAdj[c, j];
                                Debug.Assert(!mask[n]);
                                mask[n] = true;

                                for(int k = 3; k-- > 0;) {
                                    var n2 = VertAdj[n, k];
                                    if(mask[n2]) continue;
                                    mask[n2] = true;
                                    // t.Verts[vc--] = n2;
                                    f.Vi[vc--] = n2;
                                    break;
                                }

                            }
                            break;
                        }
                }
                break;
            }
        }
    }

    void mesh_cornerCap(BuildCntx cntx, Voxel v, int[,] crnrCap, bool flip) {
        for(int ci = 8; ci-- > 0;) {
            var vc = crnrCap[ci, 0];
            // Debug.Log("crnr " + ci + " vc " + vc);
            if(vc == 3) {
                var s = new Tri() { F = v.Faces[0], };
                Vector3 v1 = cntx.Vp[s.Verts.A = crnrCap[ci, 1]], v2 = cntx.Vp[s.Verts.B = crnrCap[ci, 2]], v3 = cntx.Vp[s.Verts.C = crnrCap[ci, 3]];
                var vec = (v1 + v2 + v3) / 3 - v.transform.localPosition;
                var n = Vector3.Cross(v2 - v1, v3 - v1);
                if((Vector3.Dot(n, vec) > 0) == flip)
                    Math_JC.swap(ref s.Verts.B, ref s.Verts.C);
                cntx.Tris.Add(s);
            } else if(vc == 4) {
                var s = new Quad() { F = v.Faces[0], };
                Vector3 v1 = cntx.Vp[s.Verts.A = crnrCap[ci, 1]], v2 = cntx.Vp[s.Verts.B = crnrCap[ci, 2]], v3 = cntx.Vp[s.Verts.C = crnrCap[ci, 3]], v4 = cntx.Vp[s.Verts.D = crnrCap[ci, 4]];
                var vec = (v1 + v2 + v3 + v4) / 4 - v.transform.localPosition;
                for(int i = 4, j = 0, k = 1; i-- > 0; k = j, j = i) {
                    var n = -Vector3.Cross(cntx.Vp[s.Verts[j]] - cntx.Vp[s.Verts[k]], cntx.Vp[s.Verts[i]] - cntx.Vp[s.Verts[k]]);
                    if((Vector3.Dot(n, vec) > 0) == flip) {
                        var t = s.Verts[j];
                        s.Verts[j] = s.Verts[i];
                        s.Verts[i] = t;
                        n = -Vector3.Cross(cntx.Vp[s.Verts[j]] - cntx.Vp[s.Verts[k]], cntx.Vp[s.Verts[i]] - cntx.Vp[s.Verts[k]]);
                    }
                }
                cntx.Quads.Add(s);
            } //else Debug.Assert(vc == 0);
        }
    }
    void genEdges(BuildCntx cntx) {

        List<int> edgeReDir = new List<int>();
        List<EdgeLink> edgeLs = new List<EdgeLink>();

        int ec = 0;

        foreach(Voxel v in cntx.Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            for(int fi1 = v.Faces.Length; fi1-- > 0;) {
                var f = v.Faces[fi1];
                Voxel nbr = f.getNbr();
                for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                    //if(f.Sf.Edges[i] != null) continue;

                    for(int fi2 = fi1; fi2-- > 0;) {
                        var f2 = v.Faces[fi2];
                        for(int i2 = f2.Vi.Length, j2 = 0; i2-- > 0; j2 = i2) {
                            if(f.Vi[i] != f2.Vi[j2] || f.Vi[j] != f2.Vi[i2]) continue;

                            Debug.Assert(f.Edges[i] == null && f2.Edges[i2] == null);
                            var el = new EdgeLink() {
                                EdgeI = edgeReDir.Count,
                                F1 = f,
                                F2 = f2,
                            };
                            edgeReDir.Add(el.EdgeI);
                            f.Edges[i] = f2.Edges[i2] = el;
                            edgeLs.Add(el);
                            ec++;
                        }
                    }


                }
            }
        }


        foreach(Voxel v in cntx.Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            for(int fi1 = v.Faces.Length; fi1-- > 0;) {
                var f = v.Faces[fi1];

                Voxel nbr = f.getNbr();
                if(nbr != null && v.TInd < nbr.TInd) {
                    var f2 = f.Sf.opp(f);
                    for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                        //if(f.Sf.Edges[i] != null) continue;

                        // Debug.Log(" i  " + cntx.Vert[v, f.Vi[i]].Vi);
                        bool found = false;
                        for(int i2 = f2.Vi.Length, j2 = 0; i2-- > 0; j2 = i2) {

                            if(cntx.Vert[v, f.Vi[i]] == cntx.Vert[nbr, f2.Vi[i2]] && cntx.Vert[v, f.Vi[j]] == cntx.Vert[nbr, f2.Vi[j2]]) {
                                Debug.Log("HMMM");
                            } else
                            if(cntx.Vert[v, f.Vi[i]] != cntx.Vert[nbr, f2.Vi[j2]] || cntx.Vert[v, f.Vi[j]] != cntx.Vert[nbr, f2.Vi[i2]]) continue;
                            //if(f.Vi[i] != f2.Vi[j2] || f.Vi[j] != f2.Vi[i2]) continue;

                            EdgeLink e1 = f.Edges[i], e2 = f2.Edges[i2];

                            while(edgeReDir[e1.EdgeI] != e1.EdgeI) e1.EdgeI = edgeReDir[e1.EdgeI];
                            while(edgeReDir[e2.EdgeI] != e2.EdgeI) e2.EdgeI = edgeReDir[e2.EdgeI];
                            //int ei = Mathf.Min(e1.EdgeI, e2.EdgeI);

                            if(e1.EdgeI != e2.EdgeI) {
                                ec--;
                                if(e1.EdgeI > e2.EdgeI)
                                    e1.EdgeI = edgeReDir[e1.EdgeI] = e2.EdgeI;
                                else
                                    e2.EdgeI = edgeReDir[e2.EdgeI] = e1.EdgeI;
                            }

                            found = true;
                            break;
                        }
                        Debug.Assert(found);
                    }
                }


            }
        }

        cntx.Edges = new Edge[edgeReDir.Count];

        foreach(Voxel v in cntx.Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            for(int fi1 = v.Faces.Length; fi1-- > 0;) {
                var f = v.Faces[fi1];

                for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                    EdgeLink e1 = f.Edges[i];
                    while(edgeReDir[e1.EdgeI] != e1.EdgeI) e1.EdgeI = edgeReDir[e1.EdgeI];
                    Edge e;

                    Color col;
                    if(cntx.Edges[e1.EdgeI] == null) {
                        Debug.Assert(e1.F1 == f);
                        e = cntx.Edges[e1.EdgeI] = new Edge() {
                            V1 = cntx.Vert[v, f.Vi[i]],
                        };

                        Vector3 v1 = e.V1.V, v2 = cntx.Vert[v, f.Vi[j]].V, m = (v1 + v2) * 0.5f;

                        e.InVi.A = cntx.addVp((v1 + m) * 0.5f);
                        e.InVi.B = cntx.addVp((v2 + m) * 0.5f);
                        col = Color.red;
                    } else {
                        e = cntx.Edges[e1.EdgeI];
                        col = Color.black;
                    }

                    /*if(DrawCp) {
                        Debug.DrawLine(transform.TransformPoint(f.Sf.Mid), transform.TransformPoint(cntx.Vp[e.InVi.A]), col);
                        Debug.DrawLine(transform.TransformPoint(f.Sf.Mid), transform.TransformPoint(cntx.Vp[e.InVi.B]), col);
                    }*/

                    e.Ref++;

                }

            }
        }
        /*

                for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                                    int ii = f.Vi[i];
                                    crnrCap[ii, ++crnrCap[ii, 0]] = cp1[i];

                                    if(f.Sf.F2 == null) {
                                        var s = new Quad() { F = f };

                                        s.Verts[0] = cntx.Vi[v, f.Vi[i]];
                                        s.Verts[1] = cntx.Vi[v, f.Vi[j]];
                                        s.Verts[2] = cp1[j];
                                        s.Verts[3] = cp1[i];
                                        cntx.Quads.Add(s);
                                    }
                                    var el = f.Edges[i];
                                    if(el.F1 != f) continue;
                                    {
                                        var s = new Quad() { F = f };
                                        var of = el.F2;
                                        of.cp(cp2);
                                        int oi = of.Edges.getI(el), oj = (oi + of.Vi.Length + 1) % of.Vi.Length;
                                        s.Verts[0] = cp1[i];
                                        s.Verts[1] = cp1[j];
                                        s.Verts[2] = cp2[oi];
                                        s.Verts[3] = cp2[oj];
                                        cntx.Quads.Add(s);
                                    }
                                }*/
    }
    void meshify(BuildCntx cntx) {
        Vector3[] faceV = new Vector3[4];
        Vector3[] faceE = new Vector3[4];
        Vector3[] faceOff = new Vector3[4];
        Vector3[] faceCp = new Vector3[4];
        int[] cp1 = new int[4], cp2 = new int[4];

        foreach(Voxel v in cntx.Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            int[,] crnrCap = new int[8, 5];

            foreach(var f in v.Faces) {
                var nbrF = f.Sf.opp(f);
                //if(Style != StyleE.Solid) {
                if(f.Sf.F2 == null || v.TInd < nbrF.V.TInd) {
                    float inset = Inset;

                    for(int i = f.Vi.Length; i-- > 0;)
                        faceV[i] = cntx.Vert[v, f.Vi[i]].V;

                    for(int i = f.Vi.Length, j = 0; i-- > 0; j = i)
                        faceE[i] = faceV[i] - faceV[j];

                    Vector3 n = Vector3.zero;
                    for(int i = f.Vi.Length, j = 0; i-- > 0; j = i)
                        n += Vector3.Cross(faceE[i], faceE[j]);
                    n.Normalize();
                    f.Sf.Nrm = n;

                    for(int i = f.Vi.Length; i-- > 0;)
                        faceOff[i] = Vector3.Cross(faceE[i], n).normalized * inset;

                    for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                        Vector3 v1 = faceV[i];
                        Vector3 v2 = faceV[j];
                        var mid = (v1 + v2) * 0.5f;
                        var offN = Vector3.Cross(v1 - v2, n).normalized;

                        var off = faceOff[i] + mid;

                        bool c1 = faceOff[i] == Vector3.zero, c2 = faceOff[j] == Vector3.zero;
                        Vector3 intr = faceV[j];


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

                        f.Sf.Cp[j] = cntx.addVp(faceCp[j] = intr);  //! -> j <- !
                    }
                    if(DrawCp)
                        for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                            Debug.DrawLine(transform.TransformPoint(faceCp[i]), transform.TransformPoint(faceCp[j]), Color.white);
                        }

                    var tr = new TriRef();
                    tr.Vox = v;
                    tr.F = f;
                    f.Sf.Tr = tr;

                } else {

                    for(int desOff = 0; ;) {
                        var ov = cntx.Vert[nbrF.V, nbrF.Vi[0]];
                        var tv = cntx.Vert[v, f.Vi[(f.Vi.Length - (0) + desOff) % f.Vi.Length]];

                        if(ov == tv) {
                            f.CpOff = desOff;
                            break;
                        }
                        if(++desOff >= f.Vi.Length) {
                            Debug.LogError("notfound right off");
                            break;
                        }
                    }
                }


                switch(Style) {
                    case StyleE.Solid:
                        if(f.Sf.F2 != null) continue;

                        if(f.Vi.Length == 4) {
                            var s = new Quad() { F = f };
                            for(int i = f.Vi.Length; i-- > 0;)
                                s.Verts[i] = cntx.Vi[v, f.Vi[i]];
                            cntx.Quads.Add(s);
                        } else if(f.Vi.Length == 3) {
                            var s = new Tri() { F = f };
                            for(int i = f.Vi.Length; i-- > 0;)
                                s.Verts[i] = cntx.Vi[v, f.Vi[i]];
                            cntx.Tris.Add(s);
                        }
                        break;
                    case StyleE.Frame:
                        f.cp(cp1);
                        for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                            int ii = f.Vi[i];
                            crnrCap[ii, ++crnrCap[ii, 0]] = cp1[i];

                            if(f.Sf.F2 == null) {
                                var s = new Quad() { F = f };

                                s.Verts[0] = cntx.Vi[v, f.Vi[i]];
                                s.Verts[1] = cntx.Vi[v, f.Vi[j]];
                                s.Verts[2] = cp1[j];
                                s.Verts[3] = cp1[i];
                                cntx.Quads.Add(s);
                            }
                            var el = f.Edges[i];
                            if(el.F1 != f) continue;
                            {
                                var s = new Quad() { F = f };
                                var of = el.F2;
                                of.cp(cp2);
                                int oi = of.Edges.getI(el), oj = (oi + of.Vi.Length + 1) % of.Vi.Length;
                                s.Verts[0] = cp1[i];
                                s.Verts[1] = cp1[j];
                                s.Verts[2] = cp2[oi];
                                s.Verts[3] = cp2[oj];
                                cntx.Quads.Add(s);
                            }
                        }
                        break;
                    case StyleE.Pipe:
                        f.cp(cp1);

                        if(f.Sf.F2 == null) {
                            if(f.Vi.Length == 4) {
                                var s = new Quad() { F = f };
                                for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {
                                    var vrt = cntx.Vert[v, f.Vi[j]];


                                    EdgeLink el1 = f.Edges[i], el2 = f.Edges[j];
                                    Edge e1 = cntx.Edges[el1.EdgeI], e2 = cntx.Edges[el2.EdgeI];
                                    int epI1 = (e1.V1 == vrt) ? e1.InVi.A : e1.InVi.B, epI2 = (e2.V1 == vrt) ? e2.InVi.A : e2.InVi.B;


                                    if(DrawCp) {
                                        Debug.DrawLine(transform.TransformPoint(cntx.Vp[cp1[j]]), transform.TransformPoint(cntx.Vp[epI1]), Color.red);
                                        Debug.DrawLine(transform.TransformPoint(cntx.Vp[cp1[j]]), transform.TransformPoint(cntx.Vp[epI2]), Color.green);
                                    }

                                    bool c1 = el1.opp(f).Sf.F2 != null, c2 = el2.opp(f).Sf.F2 != null;


                                    if(c1 != c2) {
                                        if(c1)
                                            cp1[j] = f.setCp(j, epI1);
                                        else
                                            cp1[j] = f.setCp(j, epI2);
                                    } else if(c1) {
                                        cp1[j] = f.setCp(j, epI1);
                                    }
                                    s.Verts[j] = cp1[j];
                                }

                                cntx.Quads.Add(s);
                            } else if(f.Vi.Length == 3) {
                                var s = new Tri() { F = f };
                                for(int i = f.Vi.Length; i-- > 0;)
                                    s.Verts[i] = cp1[i];
                                cntx.Tris.Add(s);
                            }
                        }

                        for(int i = f.Vi.Length, j = 0; i-- > 0; j = i) {

                            int ii = f.Vi[i];
                            if(cntx.Vert[v, ii].Vox.Count == 1)
                                crnrCap[ii, ++crnrCap[ii, 0]] = cp1[i];

                            var el = f.Edges[i];

                            if(el.F1 != f) continue;
                            var e = cntx.Edges[el.EdgeI];
                            if(e.Ref > 2) continue;

                            var s = new Quad() { F = f };
                            var of = el.F2;
                            of.cp(cp2);
                            int oi = of.Edges.getI(el), oj = (oi + of.Vi.Length + 1) % of.Vi.Length;
                            s.Verts[0] = cp1[i];
                            s.Verts[3] = cp1[j];
                            s.Verts[2] = cp2[oi];
                            s.Verts[1] = cp2[oj];
                            cntx.Quads.Add(s);

                            /*
                            if(cntx.Edges[el.EdgeI].Ref > 2) {

                                if(f.Sf.F2 != null) {
                                    if(cntx.Vert[v, f.Vi[i]] == e.V1) {
                                        s.Verts[0] = e.InVi.A;
                                        s.Verts[3] = e.InVi.B;
                                    } else {
                                        s.Verts[3] = e.InVi.A;
                                        s.Verts[0] = e.InVi.B;
                                    }
                                } else if(of.Sf.F2 != null) {
                                    if(cntx.Vert[of.V, of.Vi[oi]] == e.V1) {
                                        s.Verts[2] = e.InVi.A;
                                        s.Verts[1] = e.InVi.B;
                                    } else {
                                        s.Verts[2] = e.InVi.A;
                                        s.Verts[1] = e.InVi.B;
                                    }
                                }

                            } //*/

                        }
                        break;
                }
            }

            switch(Style) {
                case StyleE.Frame:
                    mesh_cornerCap(cntx, v, crnrCap, true);
                    break;
                case StyleE.Pipe:
                    mesh_cornerCap(cntx, v, crnrCap, false);
                    break;
            }
        }
    }

    bool possNbr(Voxel v, Voxel v2) {
        if(v2.State == Voxel.StateE.Invalid) return false;

        var vec = v.Pos - v2.Pos;
        var sm = vec.sqrMagnitude;
        if(sm > 4.0f)
            return false;
        if(sm < 0.25f) {
            v.State = Voxel.StateE.Invalid;
            return true;
        }
        v.PossNbr.Add(v2);
        return false;
    }
    void possNLink( SortedList<float, NbrLink> nLinks,  Voxel v, Voxel n) {
        var vec = v.Pos - n.Pos;
        var vecN = vec.normalized;
        float lim = 0.6f;

        float cD = 3.0f;

        for(int i = v.Faces.Length; i-- > 0;) {
            var f1 = v.Faces[i];

            // Debug.Log("d1 " + Vector3.Dot(f1.Sf.Nrm, -vecN));
            if(Vector3.Dot(f1.Sf.Nrm, -vecN) < lim) continue;
            for(int j = n.Faces.Length; j-- > 0;) {
                var f2 = n.Faces[j];
                if(f1.Vi.Length != f2.Vi.Length) continue;
                //Debug.Log("d2 " + Vector3.Dot(f2.Sf.Nrm, vecN));
                //Debug.Log("d3 " + Vector3.Dot(f2.Sf.Nrm, -f1.Sf.Nrm));

                if(Vector3.Dot(f2.Sf.Nrm, vecN) < lim) continue;
                if(Vector3.Dot(f2.Sf.Nrm, -f1.Sf.Nrm) < lim) continue;
                //todo -  break forEach
                //cFi2 = (int)fi2;
                //t = true;
                float d = (f1.Sf.Mid - f2.Sf.Mid).sqrMagnitude;
                if(d < cD) {
                    cD = d;
                    nLinks.Add(d, new NbrLink() {
                        V1 = v,
                        V2 = n,
                        Fi1 = i,
                        Fi2 = j
                    });
                }
            }
        }
    }
    void prepVoxels(BuildCntx cntx) {
        int vIndex = 0, vIndex2 = 0;

        List<Face> tFaceL = new List<Face>();
 
        // int vFloor; 
        System.Action<Structure, Matrix4x4, Quaternion > prepVoxelSub = null;
        prepVoxelSub = (Structure s, Matrix4x4 m, Quaternion r) => {
            s.Trnsfrm.localScale = Vector3.one;

            m *= Matrix4x4.TRS(s.Trnsfrm.localPosition, s.Trnsfrm.localRotation, Vector3.one);
            r *= s.Trnsfrm.localRotation;

            int myFloor = vIndex;
            foreach(var sub in s.Subs) {
                prepVoxelSub(sub, m, r);
            }
            var nLinks = cntx.NLinks[s.TInd];

            for(int i = vIndex; --i > myFloor;) {
                var v1 = cntx.Vox[i];
                if(v1.State == Voxel.StateE.Invalid) continue;
                int on = v1.PossNbr.Count;
                for(int j = i; j-- > myFloor;) {
                    if(possNbr(v1, cntx.Vox[j]))
                        break;
                }
                for(int j = on; j < v1.PossNbr.Count; j++)
                    possNLink(nLinks, v1, v1.PossNbr[j]);
            }

            foreach(var v in s.Vox) {
                v.TInd = vIndex ++;

                //v.Vi = null;
                v.PossNbr.Clear();
                v.Crnr.reset();
                //            var of = v.Faces
                //v.Faces_Old = null;

                v.Pos =  m.MultiplyPoint( v.transform.localPosition );
                v.Rot = v.transform.localRotation * r;
                v.Scale = v.transform.localScale;

                v.State = Voxel.StateE.Pending;

                bool clipping = false;
                int effFc = 6;
                //v.Faces_Old = new Face_Old[6];

                tFaceL.Clear();
                //todo -- check for whole cube -- shortcut
                v.forEach((FaceT fi) => {
                    int vc = 4;
                    var viA = FaceVi[(int)fi];
                    for(int i = 4; i-- > 0;) {
                        int ii = viA[i];
                        if(v.CrnrFlgs[ii] ) {
                            //Debug.Log("clipped");
                            clipping = cntx.Flag[v, ii] = true;

                            vc--;
                            if(vc == 2) effFc--;
                        }
                    }
                    cntx.FaceVc[v, fi] = (char)vc;

                    if(vc < 3) return;
                    var f = new Face() {
                        V = v,
                    };

                    if(vc == 4) {
                        f.Vi = viA;
                    } else {
                        f.Vi = new int[3];
                        for(int i = 4; i-- > 0;) {
                            int ii = viA[i];
                            if(!cntx.Flag[v, ii]) {
                                f.Vi[--vc] = ii;
                            }
                        }
                    }
                    tFaceL.Add(f);
                });

                // Debug.Log("effFc  " + effFc);
                if(effFc < 3) {
                    v.State = Voxel.StateE.Invalid;
                    continue;
                }

                for(int j = v.TInd; j-- > myFloor;) {
                    if(possNbr(v, cntx.Vox[j]))
                        goto label_breakContinue;
                }

                v.MP = v.getMidPoints();
                v.VGroup = 1 << (vIndex2++ % 64);

                var mp = v.MP;

                for(int i = 8; i-- > 0;)
                    cntx.Crnr[v, i] = v.baseCrnrP(i);
                /*
                for(int i = 2; i-- > 0;) {   //todo get rid of this
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

                            Debug.Log("ii " + ii + "   " + lp + "   " + cntx.Crnr[v, ii]);

                            cntx.Crnr[v, ii] = lp;

                            
                        }
                    }
                } */



                if(clipping)
                    s.doClipping(cntx, v, tFaceL);


                foreach(var f in tFaceL) {
                    f.Sf = new SharedFace() {
                        F1 = f,
                    };
                    f.Sf.Nrm = Vector3.Cross(cntx.Crnr[v, f.Vi[1]] - cntx.Crnr[v, f.Vi[0]], cntx.Crnr[v, f.Vi[2]] - cntx.Crnr[v, f.Vi[0]]).normalized;
                    f.Sf.Mid = Vector3.zero;
                    foreach(int ii in f.Vi)
                        f.Sf.Mid += cntx.Crnr[v, ii];

                    f.Sf.Mid /= f.Vi.Length;
                }

                v.Faces = tFaceL.ToArray();
                tFaceL.Clear();


                foreach(var n in v.PossNbr)
                    possNLink(nLinks, v, n);

                label_breakContinue:;
            }

            
        };

        prepVoxelSub(this, Matrix4x4.TRS(Trnsfrm.localPosition, Trnsfrm.localRotation, Vector3.one).inverse, Quaternion.Inverse( Trnsfrm.localRotation ) );
        //List<int> isolated = 

        int nbrIter = 9999;
        foreach_Structure((Structure s) => {
            var nLinks = cntx.NLinks[s.TInd];
            for(int iter = nbrIter; ;) {
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
        });
        
    }
    void rebuild() {

        if(Hullify) hullify();

        var cntx = new BuildCntx(this);

        prepVoxels(cntx);   
        //doNbrStuff(cntx);

        System.Func<Vector4, Voxel, Vertex> getV = (Vector4 p, Voxel v) => {
            var ret = new Vertex() { V = p, VMask = v.VGroup };
            ret.Vox.Add(v);
            cntx.Verts.Add(ret);
            return ret;
        };
        foreach( var v in cntx.Vox ) {
            if(v.State == Voxel.StateE.Invalid) continue;

            for(int i = 8; i-- > 0;) {
                Vector4 v2 = cntx.Crnr[v, i];
                v2.w = 1;
                v2 *= v.Weighting;
                if(cntx.Vert[v, i] != null) {
                    var vert = cntx.Vert[v, i];
                    vert.V += v2;
                } else
                    cntx.Vert[v, i] = getV(v2, v);
            }
        }

        for(int i = cntx.Verts.Count; i-- > 0;) {
            var v1 = cntx.Verts[i];
            if(v1.Vox.Count <= 0) continue;
            cntx.Verts[i].V = (Vector3)v1.V / v1.V.w;
        }



        foreach(var v in cntx.Vox) {
            if(v.State == Voxel.StateE.Invalid) continue;
            for(int i = 8; i-- > 0;) {
                var vrt = cntx.Vert[v, i];
                if(vrt.Crnr == null) {
                    vrt.Crnr = new Voxel.Corner() {
                        V = vrt.V,
                        // Ci = i,
                        //Flag = cntx.Flag[v, i]
                        Vox = vrt.Vox,
                    };
                }
                v.CrnrFlgs[i] = cntx.Flag[v, i];
                v.Crnr[i] = vrt.Crnr;
            }
        }

        genEdges(cntx);
        meshify(cntx);

        if(MF.sharedMesh) DestroyImmediate(MF.sharedMesh);
        Mesh m = MF.sharedMesh = new Mesh();

        int triCnt = cntx.Tris.Count + cntx.Quads.Count * 2;
        int vrtCnt = cntx.Tris.Count * 3 + cntx.Quads.Count * 4;

        //Debug.Log("cntx.Tris.Count " + cntx.Tris.Count + "   cntx.Quads.Count " + cntx.Quads.Count);
        var inds = new int[triCnt * 3];
        var vrts = new Vector3[vrtCnt];
        var nrms = new Vector3[vrtCnt];
        var uv = new Vector2[vrtCnt];

        Tris = new TriRef[triCnt];
        int vi = 0, ti = 0;

        foreach(var s in cntx.Tris) {
            for(int i = 2; i < 3; i++) {
                int ii = ti * 3;
                Tris[ti++] = s.F.Sf.Tr;

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
                Tris[ti++] = s.F.Sf.Tr;

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

        m.vertices = vrts;
        m.normals = nrms;
        m.SetTriangles(inds, 0);
        m.uv = uv;

        MC.sharedMesh = m;

        CtorMain.Singleton.DirtySelection = true;


        foreach_Sub((Structure s) => {
            s.IsDirty = false;
        });
        
        Hullify &= Constant;
            
        IsDirty = false;

        
    }

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
    public float HRad = 0.7f, HDisplace = 0.1f, HBoxEx = 0.3f, HCrnrCast = 0.4f;

    //struct HullifyVoxDat

    void hullify() {

        var cc = GetComponentInChildren<CtorComponent>();

        foreach(var v in Vox) 
            Destroy(v.gameObject);

        Vox.Clear();

        Selection.Clear();
        CtorMain.Singleton.DirtySelection = true;

        int lm = 1 << 26;
        RaycastHit hit;
        Vector3 cp = transform.position;

        DyArray<HullifyDat> arr = new DyArray<HullifyDat>();
        DyArray<HullifyDat2> arr2 = new DyArray<HullifyDat2>();

        List<IVec3> search = new List<IVec3>(),  voxL = new List<IVec3>(); ;

        Vector3 ex = Vector3.one *HBoxEx;
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
        for( int i = adj.Length; i-- >0; )
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
      //  Debug.Assert(arr.op(new IVec3(0), (ref HullifyDat h) => { return h.checkSet(0); }) == true);
      //  Debug.Assert(arr.op(new IVec3(0), (ref HullifyDat h) => { return h.checkSet(0); }) == false);
        arr.op(new IVec3(0), (ref HullifyDat h) => { h.checkSet(0, new IVec3(0)); });

        Vector3 hv3 = Vector3.one * 0.5f;
        int vc4 = 0;
        for(IVec3 c = new IVec3(0); ;) {
            var p = cp + (Vector3)c;


            bool cb = Physics.CheckBox(p, ex, Quaternion.identity, lm);
            if(!cb) {
                int vc = 0, cf = 255;
                for(int i = 8; i-- > 0;) {
                    //if((arr[c + adj2[i]].Flags & 1) != 0)
                    //   cf &= ~(1 << i);
                    var off = adj2[i];
                    if(arr2.op(c + off, (ref HullifyDat2 d) => {
                        if( d.Flags  == 0) {
                            d.Flags = (byte) (Physics.CheckSphere((Vector3)(c + off) + hv3, HCrnrCast, lm) ? 2 : 1);
                        }
                        return (d.Flags & 3) == 1;
                    })) {
                        vc++;
                        cf &= ~(1 << i);
                    }
                }


                if(vc == 4)
                    vc4++;

                
                cb = ! (vc >= 5);
            }
            if( cb ) {


                // Gizmos.color = Color.red;
                foreach(var off in adj) {
                   // var off = new IVec3(adj, ai);
                    var i = c + off; 
                    if( arr.op(i, (ref HullifyDat h) => { return h.checkSet(0, off ); }) ) {
                        search.Add(i);
                    }
                }
                foreach(var a in adj2) {
                    arr2.op(c + a, (ref HullifyDat2 d) => {
                        if(d.Flags == 0) {
                            d.Flags = (byte)(Physics.CheckSphere((Vector3)(c + a) + hv3, HCrnrCast, lm) ? 2 : 1);
                        }
                        d.Flags |= 4;
                    });
                }
            } else {
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
            //  Gizmos.DrawWireSphere(p, 0.5f);


            if(search.Count <= 0) break;

            c = search[0];
            search.RemoveAt(0);

        }

        Debug.Log("vc4  " + vc4);
        foreach(var c in voxL) {           
            var dat = arr[c];
            dat.Dis = 0;
            if(dat.Off != new IVec3(0)) {
                Vector3 dir = -((Vector3)dat.Off).normalized;
                              
                var r = new Ray(cp + (Vector3)c, dir);
                float hDis = HDisplace / dir.maxAbsD();
                if(Physics.BoxCast(r.origin, ex, r.direction, out hit, Quaternion.identity, hDis, lm)) {
                    hDis = hit.distance;
                }
                dat.Dis = hDis * dir.maxAbsD();
            }
           // Debug.Log("c1  " + c + "  dat.Off  " + dat.Off + "  dat.Dis  " + dat.Dis );
            arr[c] = dat;
        }
        foreach(var c in voxL) {

            var go = Instantiate(CtorMain.Singleton.VoxelFab);
            var v = go.GetComponent<Voxel>();
            v.init(this);
            v.transform.resetTransformation();
            v.transform.localPosition = (Vector3)c;

            var dat = arr[c];
            Debug.Log("c  " + c + "  dat.Off  " + dat.Off + "  dat.Dis  " + dat.Dis + "  f " + arr2[c].Flags + "  vc " + arr2[c].Vc);

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
                v.transform.position += dir* (d / (mod*dir.maxAbsD()) );
            }

            v.transform.localScale *= HRad; 

            //int cf = (~dat.Flags) & 255;

            int cf = 255;

            for(int i = 8; i-- > 0;) {
                //if((arr[c + adj2[i]].Flags & 1) != 0)
                //   cf &= ~(1 << i);
                var off = adj2[i];
                if(arr2.op(c + off, (ref HullifyDat2 d) => {
                    return d.Flags == 5 && d.Vc >=4;
                })) {
                    cf &= ~(1 << i);
                }
            }
           // break;
            v.CrnrFlgs.Data = cf;
        }


    }
    public bool DrawCp = false;

    [HideInInspector]
    [System.NonSerialized]
    public Transform Trnsfrm;
    MeshFilter MF;
    MeshRenderer MR;
    MeshCollider MC;

    public int TInd;
    public Structure Parent, Root;

    public bool SelectInEditor = true;
    void OnEnable() {
        Trnsfrm = transform;
        MF = getAddComponenet<MeshFilter>();
        MR = getAddComponenet<MeshRenderer>();
        MC = getAddComponenet<MeshCollider>();
    }

    void Awake() {
        Vox = new List<Voxel>();
        Subs = new List<Structure>();
        Parent = null;
    }

    void startSub() {
        CtorMain.Singleton.UI.add(this);

        for(int i = 0; i < Trnsfrm.childCount; i++) {
            var t = Trnsfrm.GetChild(i);

            Structure sub; CtorComponent cmp;
            var v = t.GetComponent<Voxel>();
            if(v) {
                v.init(this);
            } else if(sub = t.GetComponent<Structure>()) {
                Subs.Add(sub);
                sub.Parent = this;
                sub.Root = Root;
                sub.startSub();
            } else if(cmp = t.GetComponent<CtorComponent>()) {

                Cmps.Add(cmp);
            }
        }
    }
    void Start() {

        if(Parent || this.getComponentInParentAct<Structure>())
            return;

        Root = this;
        startSub();

        prep();
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


    public List<Voxel> _Selection = new List<Voxel>();
    public List<Voxel> Selection {
        get { return Root._Selection; }
    }

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
            if(v.enSelected()) {
                Selection.Add(v);
                CtorMain.Singleton.DirtySelection = true;
            }
        }
    }

    int LastDI = -1;

    public bool Save, Load;

    public string SubDir;

    public class BWriter : BinaryWriter {
        public BWriter(Stream s) : base( s)  { }
        public void Write(Vector3 v) {
            Write(v.x);
            Write(v.y);
            Write(v.z);
        }
        public void Write(Quaternion v) {
            Write(v.x);
            Write(v.y);
            Write(v.z);
            Write(v.w);
        }
    }
    public class BReader : BinaryReader {
        public BReader(Stream s) : base( s)  { }

        public Vector3 ReadV3() {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }
        public Quaternion ReadQuat() {
            return new Quaternion(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }

    }

    public enum Version {
        Initial,
        Latest,
    };
    public struct FileHdr {
        public static uint _MagicNo = 0xbedabb1e;
        public uint MagicNo;

        public ushort StructCnt;

        public void write(BWriter io, Version version) {
            io.Write(_MagicNo);
            io.Write((int)version);
            io.Write(StructCnt); //not needed
        }
        public FileHdr read(BReader io, ref Version version) {
            MagicNo = io.ReadUInt32();
            Debug.Assert(MagicNo == _MagicNo);
            version = (Version)io.ReadInt32();
            StructCnt = io.ReadUInt16(); //not needed
            return this;
        }
    };
    public struct Structure_Parser {
        public Structure_Parser(Structure s) { St = s; }
        Structure St;
       /* public string Name;
        public ushort VoxelCnt, SubCnt;
        public uint Flags;
        public float Inset;
        public int Style; */

        public void write(BWriter io, Version version) {
            io.Write( St.name);
            if(St.Parent != null) {
                io.Write(St.Trnsfrm.localPosition);
                io.Write(St.Trnsfrm.localRotation);
            }
            io.Write( (ushort) St.Vox.Count);
            io.Write( (uint) 0 );
            io.Write( (float) St.Inset);
            io.Write( (int)St.Style);
            io.Write((ushort)St.Subs.Count);
            io.Write((ushort)St.Cmps.Count);

            foreach(var v in St.Vox)
                new Voxel_Parser(v).write(io, version);
            foreach(var c in St.Cmps)
                new Component_Parser(c).write(io, version);
            foreach(var s in St.Subs)
                new Structure_Parser(s).write(io, version);
        }
        public void read(BReader io, Version version) {

            St.name = io.ReadString();
            if(St.Parent != null) {
                St.transform.localPosition = io.ReadV3();
                St.transform.localRotation = io.ReadQuat();
            }
            int vc = io.ReadUInt16();
            uint flags = io.ReadUInt32();
            St.Inset = io.ReadSingle();
            
            //if(version >= Version.Add_Style) {
            St.Style = (StyleE)io.ReadInt32();
            int subCnt = io.ReadUInt16();
            int cmpCnt = io.ReadUInt16();
           
            foreach(var v in St.Vox)
                Destroy(v.gameObject);
            St.Vox.Clear();
            if( St.Vox.Capacity < vc )
                St.Vox.Capacity = vc;

            for(int i = vc; i-- > 0;) {
              //  var vFd = new Voxel_FileDat().read(io, ver);
                var go = Instantiate(CtorMain.Singleton.VoxelFab);
                var v = go.GetComponent<Voxel>();
                v.init(St);
                new Voxel_Parser(v).read(io, version);
            }

            foreach(var c in St.Cmps) {
                Destroy(c.gameObject);
            }
            St.Cmps.Clear();

            if(St.Cmps.Capacity < cmpCnt)
                St.Cmps.Capacity = cmpCnt;

            for(int i = cmpCnt; i-- > 0;) {
                new Component_Parser() { P = St } .read(io,version );
            }

            foreach(var s in St.Subs) {
                Destroy(s.gameObject);
            }
            St.Subs.Clear();

            if(St.Subs.Capacity < subCnt)
                St.Subs.Capacity = subCnt;

            for(int i = subCnt; i-- > 0;) {
                var go = new GameObject();
                go.transform.parent = St.transform; 
                var s = go.AddComponent<Structure>();
                s.Parent = St;
                s.Root = St.Root;
                new Structure_Parser(s).read(io, version);
                CtorMain.Singleton.UI.add(s);
                St.Subs.Add(s);
            }
        }
    };
    public struct Voxel_Parser {
        /*
        public Vector3 Pos, Scl;
        public Quaternion Rot;
        public byte CornerFlgs;
        public float Weighting;
        */
        Voxel V;
        public Voxel_Parser(Voxel v) { V = v; }
        public void write(BWriter io, Version version) {
            byte cf = (byte)V.CrnrFlgs.Data;

            io.Write(V.transform.localPosition);
            io.Write(V.transform.localScale);
            io.Write(V.transform.localRotation);
            io.Write(cf);
            io.Write(V.Weighting);
        }
        public void read(BReader io, Version version) {
            V.transform.localPosition = io.ReadV3();
            V.transform.localScale =  io.ReadV3();
            V.transform.localRotation = io.ReadQuat();
            var cf = io.ReadByte();

            //if(version >= Version.Add_Subs_Weighting) {
                V.Weighting = io.ReadSingle();


            V.CrnrFlgs.Data = cf;
        }
    };

    public struct Component_Parser {
        /*
        public Vector3 Pos, Scl;
        public Quaternion Rot;
        public byte CornerFlgs;
        public float Weighting;
        */
        CtorComponent C;
        public Structure P;
        public Component_Parser(CtorComponent c) { C = c; P = null; }
        public void write(BWriter io, Version version) {
            io.Write(C.name);
            io.Write(C.transform.localPosition);
            io.Write(C.transform.localScale);
            io.Write(C.transform.localRotation);
        }
        public void read(BReader io, Version version) {
            var n = io.ReadString();
            var fab = CtorMain.Singleton.ComponentFabs.Find((GameObject g) => { return g.name.Equals( n, System.StringComparison.CurrentCulture ); });
            if(fab == null) Debug.LogError("err");
            var go = Instantiate(fab);
            go.name = n;
            go.transform.parent = P.Trnsfrm;
            go.transform.localPosition = io.ReadV3();
            go.transform.localScale = io.ReadV3();
            go.transform.localRotation = io.ReadQuat();
        }
    };

    void fromStream(Stream strm) {
        using(var io = new BReader(strm)) {
            Version ver = 0;
            var fh = new FileHdr().read(io, ref ver);
            Selection.Clear();
            CtorMain.Singleton.DirtySelection = true;
            new Structure_Parser( this ).read(io, ver);
        }
    }
    void toStream(Stream strm) {
        // new System.IO.Compression.DeflateStream(strm, System.IO.Compression.CompressionMode.Compress);
        // 
        using(var io = new BWriter(strm)) {
            Version ver = Version.Latest -1;
            new FileHdr() { StructCnt = 1, }.write(io, ver);
            new Structure_Parser(this).write(io, ver);         
        }
    }

    void save() {
        // try {
        // BinaryFormatter bf = new BinaryFormatter();

        string tempFn = Application.persistentDataPath + "/muhTemp.tmp";

        using(FileStream file = File.Open(tempFn, FileMode.Create)) {

            using(DeflateStream cmp = new DeflateStream(file, CompressionMode.Compress)) {
                toStream(cmp);
            }
            file.Close();

            string dir = Application.persistentDataPath + "/Ctor/"+ SubDir;
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string fn = dir + name.asValidFileName() + ".dkPic";

            if(File.Exists(fn))
                File.Delete(fn);
            File.Move(tempFn, fn);

            Debug.Log("saved  " + fn);
        }
        /* } catch( System.Exception e) {

             Debug.LogError("someException.." + e.Message);
         } */
    }
    void load() {
        // try {
        // BinaryFormatter bf = new BinaryFormatter();

        string dir = Application.persistentDataPath + "/Ctor/"+ SubDir;
        string fn = dir + name.asValidFileName() + ".dkPic";

        using(FileStream file = File.Open(fn, FileMode.Open )) {
            using(DeflateStream cmp = new DeflateStream(file, CompressionMode.Decompress)) {
                fromStream(cmp);
            }
            file.Close();
            Debug.Log("loaded  " + fn);
        }
        /* } catch( System.Exception e) {

             Debug.LogError("someException.." + e.Message);
         } */
    }

    void foreach_Structure(System.Action<Structure> a) {
        foreach(var s in Subs) {
            s.foreach_Structure(a);
        }
        a(this);
    }

    void foreach_StructureFirst(System.Action<Structure> a) {
        a(this);
        foreach(var s in Subs) {
            s.foreach_StructureFirst(a);
        }
    }


    void foreach_Sub(System.Action<Structure> a) {
        foreach(var s in Subs) {
            s.foreach_Sub(a);
            a(s);
        }
    }
    void Update() {

        if(Parent != null) return;

        foreach_Sub((Structure s) => {
            if(s.IsDirty)
                IsDirty = true;
        });

        if(Save) {
            save();
            Save = false;
        }
        if(Load) {
            load();
            Load = false;
            IsDirty = true;
        }

        if(Inc) {
            DebugIter++;
            Inc = false;
        }
        if(Dec) {
            DebugIter--;
            Dec = false;
        }
        if(DebugIter != LastDI) IsDirty = true;

        bool re = IsDirty || Hullify;
        if(Skip > 0) {
            --Skip;
          //  re = false;
        }


        if(re || Constant) {
            Skip = 5;

            rebuild();

            LastDI = DebugIter;
        }
    }

   
    public int Skip = 0;

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


    /*
        public class Face_Old {

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
*/

   

}
