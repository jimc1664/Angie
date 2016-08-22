using UnityEngine;
using System.Collections.Generic;

//using UnityEngine.EventSystems;

public class Voxel : UIEle { //, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {


    public enum StateE {
        Template = 0,
        Built,
        Pending,
        Invalid,
    };
    public StateE State = StateE.Built;
    public Structure Strct;

    static int EmissivePropId = -1;

    void Start() {
        //init(GetComponentInParent<Structure>());
    }
    public void init( Structure strct ) {

        Debug.Assert(Strct == null);
        Strct = strct;

        transform.parent = strct.transform;
        Pos = transform.localPosition;// = new Vector3(I.X,I.Y,I.Z);
        Rot = transform.localRotation;
        Scale = transform.localScale;

        strct.Vox.Add(this);
        strct.dirty();
        if(EmissivePropId == -1) EmissivePropId = Shader.PropertyToID("_EmissionColor");
    }
/*
    [System.Serializable]
    public struct GridIndex {
        public int X, Y, Z;

        public static GridIndex operator +(GridIndex a, GridIndex b) {
            return new GridIndex() {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z,
            };
        }
        /*
              public static Vector3 operator +(Vector3 a, Vector3 b);
        public static Vector3 operator -(Vector3 a);
        public static Vector3 operator -(Vector3 a, Vector3 b);
        public static Vector3 operator *(float d, Vector3 a);
        public static Vector3 operator *(Vector3 a, float d);
        public static Vector3 operator /(Vector3 a, float d);
        public static bool operator ==(Vector3 lhs, Vector3 rhs);
        public static bool operator !=(Vector3 lhs, Vector3 rhs);
        * /
    };

    //public int X, Y, Z;

    public GridIndex I; */

    bool _Selected = false, Highlighted = false;

    bool Selected {
        get { return _Selected;  }
        set {
            Debug.Assert(_Selected != value);
            _Selected = value;
            if(_Selected) {

            }
        }
    }

    Color Col = Color.white;
    void upCol() {
        var col = Color.white;

        if(Strct.Ui && CtorMain.Singleton.UI.Selected != null && CtorMain.Singleton.UI.Selected.Root == Strct.Root ) {
            bool sel = Selected;
            if(Highlighted) {
                if(sel)
                    col = Color.cyan;
                else 
                    col = Color.green;
            }
            else if(sel)
                col = Color.blue;            
        } else {
            if(Highlighted)
                col = Color.yellow;

        }


        if(col != Col) {
            var c = transform.GetChild(0);
            c.GetComponent<MeshRenderer>().material.SetColor(EmissivePropId, (Col = col)*0.5f );
            c.gameObject.SetActive(col != Color.white);
        }

        /*
        if(Selected)
            GetComponent<MeshRenderer>().material = CtorMain.Singleton.Blue;
        else if( Highlighted )
            GetComponent<MeshRenderer>().material = CtorMain.Singleton.Green;
        else
            GetComponent<MeshRenderer>().material = CtorMain.Singleton.White;

        */


    }

    public override void gotHighlight(UIMain um) {
        Highlighted = true;
        upCol();
        keptHighlight(um);
    }
    public override void lostHighlight(UIMain um) {
        Highlighted = false;
        upCol();
    }
    public override void keptHighlight(UIMain um) {
       
    }
    public override void highlight_Cast(UIMain um, RaycastHit hit) {
        var cm = CtorMain.Singleton;
        cm.Hl_D.FaceI = getFaceHlI(hit);
        cm.Hl_D.Wp = hit.point;
        cm.Hl_D.Nrm = hit.normal;
    }

    public void OnPointerSelect() {
        Selected = true;
        upCol();
    }
    public void OnPointerDeselect() {
        Selected = false;
        upCol();
    }

    public void initCrnrs() {
        for(int i = 8; i-- > 0;) {
            var c = Crnr[i];
            if(c.Hndl == null) {
                c.Hndl = CtorMain.Singleton.cornerHndl(this, c);
            }
        }
    }
    public bool enSelected() {
        
        if(Selected) return false;
        Selected = true;
        upCol();
        return true;
    }

    /*public void OnPointerClick() {
        Debug.Log("clicked");
    }*/

    public int getFaceHlI(RaycastHit hit) {

        var lp = transform.InverseTransformPoint(hit.point);
        var absLp = lp; absLp.Scale(lp);
        if(absLp.x > absLp.y) {
            if(absLp.x > absLp.z)
                return lp.x > 0 ? 1 : 0;
            else
                return lp.z > 0 ? 5 : 4;
        } else {
            if(absLp.y > absLp.z)
                return lp.y > 0 ? 3 : 2;
            else
                return lp.z > 0 ? 5 : 4;
        }
    }
    /*
    public static GridIndex faceOff(int i) {
        switch(i) {
            case 0:
                return new GridIndex() { X = -1 };
            case 1:
                return new GridIndex() { X = 1 };
            case 2:
                return new GridIndex() { Y = -1 };
            case 3:
                return new GridIndex() { Y = 1 };
            case 4:
                return new GridIndex() { Z = -1 };
            case 5:
                return new GridIndex() { Z = 1 };
        }
        Debug.LogError("err");
        return new GridIndex();
    }
    public static GridIndex faceOff(FaceT fi) {
        switch(fi) {
            case FaceT.Left:
                return new GridIndex() { X = -1 };
            case FaceT.Right:
                return new GridIndex() { X = 1 };
            case FaceT.Down:
                return new GridIndex() { Y = -1 };
            case FaceT.Up:
                return new GridIndex() { Y = 1 };
            case FaceT.Back:
                return new GridIndex() { Z = -1 };
            case FaceT.Forward:
                return new GridIndex() { Z = 1 };
        }
        Debug.LogError("err");
        return new GridIndex();
    }
    */
    bool IsDirty = true;
    public void dirty() {
        if(IsDirty) return;

        IsDirty = true;
        Strct.dirty();
    }
    public int Iteration = -1;

    public int TInd = -1;


    public Vector3 Pos = Vector3.zero;
    public Vector3 Scale = Vector3.one;
    public Quaternion Rot = Quaternion.identity;

    public float Weighting = 1;

    /*
    // [System.Serializable]
    public struct Nbrs_S {
        public Voxel L, R, U, D, F, B;
        public Voxel this[FaceT fc] {
            get {
                switch(fc) {
                    case FaceT.Left: return L;
                    case FaceT.Right: return R;
                    case FaceT.Up: return U;
                    case FaceT.Down: return D;
                    case FaceT.Forward: return F;
                    case FaceT.Back: return B;
                }
                Debug.LogError("FaceT OOB");
                return null;
            }
            set {
                switch(fc) {
                    case FaceT.Left: L = value; return;
                    case FaceT.Right: R = value; return;
                    case FaceT.Up: U = value; return;
                    case FaceT.Down: D = value; return;
                    case FaceT.Forward: F = value; return;
                    case FaceT.Back: B = value; return;
                }
                Debug.LogError("FaceT OOB");
            }
        }
        public void reset() {
            L = R = U = D = F = B = null;
        }
    };
    public Nbrs_S Nbrs_Old;

    public struct NbrsD_S {
        public float L, R, U, D, F, B;
        public float this[FaceT fc] {
            get {
                switch(fc) {
                    case FaceT.Left: return L;
                    case FaceT.Right: return R;
                    case FaceT.Up: return U;
                    case FaceT.Down: return D;
                    case FaceT.Forward: return F;
                    case FaceT.Back: return B;
                }
                Debug.LogError("FaceT OOB");
                return 0;
            }
            set {
                switch(fc) {
                    case FaceT.Left: L = value; return;
                    case FaceT.Right: R = value; return;
                    case FaceT.Up: U = value; return;
                    case FaceT.Down: D = value; return;
                    case FaceT.Forward: F = value; return;
                    case FaceT.Back: B = value; return;
                }
                Debug.LogError("FaceT OOB");
            }
        }
        public void reset(float v = float.MaxValue ) {
            L= R= U= D= F= B = v;
        }
    };
    public NbrsD_S NbrsD_Old;
    */
    public Vector3 midOff(FaceT fc) {
        Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, Rot, Scale);
        return mat.MultiplyVector(nrm2(fc)*0.5f);
    }

    public Vector3 nrm(FaceT fc) {

        return midOff(fc).normalized;
    }
    public Vector3 nrm2(FaceT fc) {
        //* 
        switch(fc) {
            case FaceT.Left: return Vector3.left;
            case FaceT.Right: return Vector3.right;
            case FaceT.Up: return Vector3.up;
            case FaceT.Down: return Vector3.down;
            case FaceT.Forward: return Vector3.forward;
            case FaceT.Back: return Vector3.back;
        }
        return Vector3.zero;
        //*/

        //  return (Nbrs[fc].P - P).normalized;
    }

    /* public struct Faces_S {
         public Structure.Face L, R, U, D, F, B;
         public Structure.Face this[FaceT fc] {
             get {
                 switch(fc) {
                     case FaceT.Left: return L;
                     case FaceT.Right: return R;
                     case FaceT.Up: return U;
                     case FaceT.Down: return D;
                     case FaceT.Forward: return F;
                     case FaceT.Back: return B;
                 }
                 Debug.LogError("FaceT OOB");
                 return null;
             }
             set {
                 switch(fc) {
                     case FaceT.Left: L = value; return;
                     case FaceT.Right: R = value; return;
                     case FaceT.Up: U = value; return;
                     case FaceT.Down: D = value; return;
                     case FaceT.Forward: F = value; return;
                     case FaceT.Back: B = value; return;
                 }
                 Debug.LogError("FaceT OOB");
             }
         }
         public void reset() {
             L = R = U = D = F = B = null;
         }
     };
     public Faces_S Faces; */

  //  public Structure.Face_Old[] Faces_Old;
    public Structure.Face[] Faces;

    //[System.NonSerialized]
    //public int[] Vi = new int[8];
    public long VGroup;

    public List<Voxel> PossNbr;
    public Vector3[,] MP;

    public class Corner {
        public Vector3 V;
        public bool Flag = false;
        public Transform Hndl;
    };
    public struct Crnr_S {
        public Corner A, B, C, D, E, F, G, H;
        public Corner this[int i] {
            get {
                switch(i) {
                    case 0: return A;
                    case 1: return B;
                    case 2: return C;
                    case 3: return D;
                    case 4: return E;
                    case 5: return F;
                    case 6: return G;
                    case 7: return H;
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
                    case 4: E = value; return;
                    case 5: F = value; return;
                    case 6: G = value; return;
                    case 7: H = value; return;
                }
                Debug.LogError("i OOB");
            }
        }
        public void reset() {
            A = B = C = D = E= F= G= H= null;
        }
    };

    public Crnr_S Crnr;
  //  public Structure.Vertex [] Verts = new Structure.Vertex[8];

    public void forEach(System.Action<FaceT> sub) {
        sub(FaceT.Right);
        sub(FaceT.Left);
        sub(FaceT.Up);
        sub(FaceT.Down);
        sub(FaceT.Forward);
        sub(FaceT.Back);
    }

    public void forEach_Hf(System.Action<FaceT> sub) {
        sub(FaceT.Left);
        sub(FaceT.Down);
        sub(FaceT.Back);
    }

    public delegate void DecQuad(Structure.TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4);
    public delegate void DecTri(Structure.TriRef tr, Vector3 n, Vector3 v1, Vector3 v2, Vector3 v3);
    /*public void toMesh(Structure.BuildCntx cntx, DecQuad quad, DecTri tri) {
        bool flip = Strct.StyleFlag;
        flip = false;
        forEach((FaceT fc) => {

            var f = Faces_Old[(int)fc];
            var nbr = Nbrs_Old[fc];

            for(int adjI = Structure.FaceAdj2[(int)fc, 4]; adjI-- > 0;) {
                break;
                var fi = (FaceT)Structure.FaceAdj2[(int)fc, adjI];

                var f1 = Faces_Old[(int)fi];

                if(!flip)
                    quad(f.Tr, -(f.norm(this) + f1.norm(this)).normalized,    //todo  - normal is wrong...
                        f.vert(this, Structure.FaceAdjVerts[(int)fc, (int)fi, 1]),
                        f.vert(this, Structure.FaceAdjVerts[(int)fc, (int)fi, 0]),
                        f1.vert(this, Structure.FaceAdjVerts[(int)fi, (int)fc, 0]),
                        f1.vert(this, Structure.FaceAdjVerts[(int)fi, (int)fc, 1]));
                else {
                    //if(nbr != null || Nbrs[fi] != null ) continue;
                    quad(f.Tr, (f.norm(this) + f1.norm(this)).normalized,    //todo  - normal is wrong...
                        f.vert(this, Structure.FaceAdjVerts[(int)fc, (int)fi, 0]),
                        f.vert(this, Structure.FaceAdjVerts[(int)fc, (int)fi, 1]),
                        f1.vert(this, Structure.FaceAdjVerts[(int)fi, (int)fc, 1]),
                        f1.vert(this, Structure.FaceAdjVerts[(int)fi, (int)fc, 0]));
                }

                for(int adjI2 = Structure.FaceAdj2[(int)fi, 4]; adjI2-- > 0;) {
                    var fi2 = (FaceT)Structure.FaceAdj2[(int)fi, adjI2];
                    var f2 = Faces_Old[(int)fi2];
                    //if(flip && Nbrs[fi2] != null) continue;
                    for(int i = 2; i-- > 0;) {  //todo - optimise  ---->> lookup
                        int i2 = i;

                        if(!ReferenceEquals(cntx.Vert[this, Structure.FaceVi_Old[(int)fi, Structure.FaceAdjVerts[(int)fi, (int)fc, i]]], cntx.Vert[this, Structure.FaceVi_Old[(int)fi, Structure.FaceAdjVerts[(int)fi, (int)fi2, i]]])) i2 = 1 - i2;
                        if(!ReferenceEquals(cntx.Vert[this, Structure.FaceVi_Old[(int)fi, Structure.FaceAdjVerts[(int)fi, (int)fc, i]]], cntx.Vert[this, Structure.FaceVi_Old[(int)fi, Structure.FaceAdjVerts[(int)fi, (int)fi2, i2]]])) continue;

                        var v1 = f.vert(this, Structure.FaceAdjVerts[(int)fc, (int)fi, i]);
                        var v2 = f1.vert(this, Structure.FaceAdjVerts[(int)fi, (int)fc, i]);
                        var v3 = f2.vert(this, Structure.FaceAdjVerts[(int)fi2, (int)fi, i2]);

                        var n2 = -(f.norm(this) + f1.norm(this) + f2.norm(this)).normalized; //todo  - wrong...

                        if(flip)
                            n2 = -n2;
                        var tn = Vector3.Cross(v3 - v2, v1 - v2);

                        if(Vector3.Dot(n2, tn) < 0)
                            Math_JC.swap(ref v2, ref v3);

                        tri(f.Tr, n2, v1, v2, v3);

                        break;
                    }

                }
            }


            if(nbr != null) return;

            if(flip) {
                quad(f.Tr, f.norm(this),
                        f.Cp[0],
                        f.Cp[1],
                        f.Cp[2],
                        f.Cp[3]);
            } else {
                for(int j = 4; j-- > 0;) {
                    int j1 = (j + 1) % 4;
                    quad(f.Tr, f.norm(this),
                        cntx.Vert[this, Structure.FaceVi_Old[(int)fc, j]].V,
                        cntx.Vert[this, Structure.FaceVi_Old[(int)fc, j1]].V,
                        f.Cp[j1],
                        f.Cp[j]);
                }
            }
        });
    }
    */
    public Vector3[,] getMidPoints() {

        var ret = new Vector3[6, 2];

        Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, Rot, Scale);
        for(int i = 6; i-- > 0;) {
            ret[i, 0] = mat.MultiplyVector(nrm2((FaceT)i)*0.5f);
            ret[i, 1] = ret[i, 0].normalized;
            ret[i, 0] += Pos;
        }
        return ret;
    }

    /*
    public FaceT opp(Voxel n, FaceT fi) {
        var oi = Structure.opp(fi);
        
        if(Nbrs_Old[oi] == n ) {
            
        } else {
          //  Debug.Log("opp-long  " + n  +"    " + fi + " -> " + oi);
            bool fail = true;
            forEach((FaceT oi2) => {
             //   Debug.Log("       " + oi2 + "    " + fi + " -> " + oi);
                if(Nbrs_Old[oi2] == n) {
                    oi = oi2;
                    fail = false;
                }
            });
            if(fail) Debug.LogError("err");
        }
        return oi;
    }*/


    void OnDrawGizmos() {

        Gizmos.color = Color.blue;
        if( State == StateE.Invalid) Gizmos.color = Color.red;


        Gizmos.DrawWireSphere(transform.position, 0.1f);

        if(Strct == null) return;

        Gizmos.color = Color.green;

        foreach(var n in PossNbr) {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
        /*
       Gizmos.color = Color.red;
       /*
      forEach_Hf((FaceT fi) => {
          var n = Nbrs_Old[fi];
          if(n) {
              Gizmos.DrawLine(transform.position, n.transform.position);

          }
      });
      Gizmos.color = Color.blue;

      forEach((FaceT fi) => {
          var n = Nbrs_Old[fi];
          if(n) {
              Gizmos.DrawLine(transform.position, transform.position+ nrm(fi) );

          }
      });


      for(int i = 6; i-- > 0;) {
          var n = Strct.get(I + faceOff(i));
          if(n && n.GetInstanceID() > GetInstanceID()) {
              Gizmos.DrawLine(transform.position, n.transform.position);

          }
      }*/
    }


}
