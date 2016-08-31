using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CtorMain : MonoBehaviour {


    public GameObject VoxelFab = null, CrnrFab;
    public GameObject GizmoFab;

    public List<GameObject> ComponentFabs;


    public CtorUI UI;

    public Gizmo Giz;

    public Material White, Green, Blue, GizRed, GizGreen;

    static CtorMain _Singleton = null;
    public static CtorMain Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<CtorMain>();
            return _Singleton;
        }
        private set { }
    }
    
    [System.Serializable]
    public struct HLDat {
      //  public Voxel V;

        public int TriI, FaceI;

        public Vector3 Wp, Nrm;
    };

    public HLDat Hl_D;
        

    public struct CtorUpCntx {

        public CtorUpCntx(CtorMain cm, UIMain um, CtorUI cu ) {
            Cu = cu;
            Cm = cm;
            Um = um;
            SHl = um.Highlight as Structure;
            VHl = null;
            FHl = null;
            Tr = null;
            if(SHl) {
                Tr = SHl.Tris[cm.Hl_D.TriI];
                //FHl = Tr.Vox.Faces_Old[(int)Tr.D];
                FHl = Tr.F;
                VHl = Tr.Vox;
                SHl = VHl.Strct;
            } else {
                var h = um.Highlight as Handle;
                if(h == null) return;
                
                
                var v = VHl = h.V;
                SHl = VHl.Strct;
                float cd = float.MinValue;

                var n = SHl.Root.transform.InverseTransformDirection(cm.Hl_D.Nrm);
                //var f = FHl;
                FHl = null;
                if( VHl.State != Voxel.StateE.Invalid )
                foreach(var f in VHl.Faces) {
                    var d = Vector3.Dot(f.Sf.Nrm, n);
                    if(d > cd) {
                        cd = d;
                        FHl = f;
                    }
                }
                              
                if( FHl != null )
                    Tr = FHl.Sf.Tr; 
            }
        }
        readonly public UIMain Um;
        readonly public CtorMain Cm;
        readonly public CtorUI Cu;

        readonly public Structure SHl;
        readonly public Structure.Face FHl;
        readonly public Structure.TriRef Tr;
        readonly public Voxel VHl;
    };

    [System.Serializable]
    public class CtorTool {
        public string Name = "<-Invalid->";
        public virtual void mouseUpdate(ref CtorUpCntx cntx, int bi) {
            
        }
    };

    public List<CrnrHndl> Crnrs;

    public Transform cornerHndl(Voxel v, Voxel.Corner c, int i ) {
      //  Debug.Log(" cornerHndl ?? ");
        var go = Instantiate(CrnrFab);
        var ret = go.transform;
        var ch = go.GetComponent<CrnrHndl>();
        ch.C = c;
        ch.V = v;
        ch.Ci = i;
        Crnrs.Add(ch);
        ret.localRotation = Quaternion.identity;
        ret.parent = v.Strct.Root.Trnsfrm;
        ret.localPosition = c.V;

        ret.parent = v.Strct.Trnsfrm;
        ret.localScale = Vector3.one * 0.1f;

        if(v.CrnrFlgs[i])
            go.GetComponent<MeshRenderer>().material = GizRed;

        return ret;
    }
    public void selectionUpdate(ref CtorUpCntx cntx) {
        var sel = cntx.Cu.Selected.Selection;

        foreach(var c in Crnrs) {
            if(c == null) continue;
            c.C.Hndl = null; 
            Destroy(c.gameObject);
        }
        Crnrs.Clear();

        //for( int i = 

        if(sel.Count > 0) {
            var p = Vector3.zero;
            foreach(var v in sel)
                p += v.transform.position;
            p /= (float)sel.Count;

            Quaternion r = sel[0].transform.rotation;
            sel[0].enSelected();
            sel[0].initCrnrs();
            int qc = 1;
            for(int i = sel.Count; --i > 0;) {
                var v = sel[i];

                v.enSelected();
                v.initCrnrs();
                var a = v.transform.rotation;
                if(Quaternion.Dot(a, r) < 0)
                    a = a.negated();

                r = Quaternion.Lerp(r, a, 1 / (float)(++qc));
            }



            if(Giz == null) {
                var go = Instantiate(GizmoFab);
                Giz = go.GetComponent<Gizmo>();

            }
            var cm = this;
           // var st = cntx.SHl.Root;
            Giz.transform.position = p;
            Giz.transform.rotation = r;

            Giz.setCB((Vector3 t) => {

                t = cm.Giz.transform.TransformDirection(t);
                cm.Giz.transform.position += t;
                foreach(var v in sel) {
                    v.transform.position += t;
                    v.Strct.dirty();
                }
                 
            }, (Vector3 t) => {
                //var strct = sel[0].transform.parent;
                foreach(var v in sel)
                    v.transform.parent = cm.Giz.transform;

                cm.Giz.transform.Rotate(t);

                foreach(var v in sel) {
                    v.transform.parent = v.Strct.Trnsfrm;
                    v.Strct.dirty();
                }
            }, (Vector3 t) => {
                foreach(var v in sel) {
                    v.transform.localScale += t;
                    v.Strct.dirty();
                }
            });

        } else if(Giz) {
            Destroy(Giz.gameObject);
        }
        DirtySelection = false;
    }
    [System.Serializable]
    public class CT_Manipulate : CtorTool {
        public CT_Manipulate() {

            Name = "Manipulator";
        }

        //public List<Voxel> sel = new List<Voxel>();

        public override void mouseUpdate(ref CtorUpCntx cntx, int bi) {

            if( cntx.Um.grabMouseDown(bi) ) {

                cntx.Cm. selectInEditor(cntx.VHl);

                bool selChange = false;
                var sel = cntx.Cu.Selected.Selection;
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                    if(cntx.VHl) {
                        if(sel.Contains(cntx.VHl)) {
                            sel.Remove(cntx.VHl);
                            cntx.VHl.OnPointerDeselect();

                        } else {
                            sel.Add(cntx.VHl);
                            cntx.VHl.OnPointerSelect();
                        }
                        selChange = true;
                    }
                   
                } else {

                    if( (sel.Count == 1 && sel[0] == cntx.VHl ) || (sel.Count == 0 && cntx.VHl == null) ) return;

                    foreach(var v in sel)
                        v.OnPointerDeselect();

                    sel.Clear();

                    if(cntx.VHl) {
                        sel.Add(cntx.VHl);
                        cntx.VHl.OnPointerSelect();

                        CtorMain.Singleton.UI.setSelect(cntx.SHl);
                    }
                    selChange = true;
                }

                if(selChange)
                    cntx.Cm.selectionUpdate( ref cntx );

               
            }
            /*if(Input.GetMouseButtonUp(bi)) {
                if(cm.Selected.V == cm.Hl.V && cm.Selected.V) {
                    cm.Selected.V.OnPointerClick();

                    //   extrudeClick();
                }
                // if(Selected.V) Selected.V.OnPointerDeselect();
                // Selected.V = null;
            } */
        }
    };
    [System.Serializable]
    public class CT_Extrude : CtorTool {
        public CT_Extrude() {

            Name = "Extrude";
        }

        public override void mouseUpdate(ref CtorUpCntx cntx, int bi) {
            var cm = CtorMain.Singleton;
            if(cntx.Um.grabMouseUp(bi)) {
                if(cntx.VHl) {

                    cm.extrudeClick( ref cntx );
                }
                // if(Selected.V) Selected.V.OnPointerDeselect();
                // Selected.V = null;
            }
        }
    };

    public List<CtorTool> ToolPool;

    public CtorTool LeftTool, RightTool;

    void Start() {

        ToolPool = new List<CtorTool>() {
            new CT_Manipulate(),
            new CT_Extrude(),
        };
        LeftTool = ToolPool[0];
        RightTool = ToolPool[1];
    }

    Voxel VHl;
    CrnrHndl CSel;

    public void clearSelection() {
        if(UI.Selected == null) return;
        foreach(var v in UI.Selected.Selection)
            v.OnPointerDeselect();
        UI.Selected.Selection.Clear();
    }

    void selectInEditor( Voxel v ) {
        if(v == null) return;
        if( v.Strct.Root.SelectInEditor)
            UnityEditor.Selection.objects = new Object[1] { v.gameObject };
    }
    public void update( UIMain um ) {

        var cntx = new CtorUpCntx(this, um, UI) { };

        if(cntx.SHl && (UI.Selected== null || cntx.SHl.Root != UI.Selected.Root )  ) {
            //if(CtorMain.Singleton.UI.Selected == null || CtorMain.Singleton
            if(cntx.Um.grabMouseDown(0) || cntx.Um.grabMouseDown(1) || cntx.Um.grabMouseDown(2)) {
                //if(cntx.SHl.Ui == null) CtorMain.Singleton.UI.add(cntx.SHl);
                clearSelection();
                CtorMain.Singleton.UI.setSelect(cntx.SHl);

                selectInEditor(cntx.VHl);

            }
        }

        var nHl = cntx.VHl;
        if(VHl != nHl) {
            if(VHl) VHl.lostHighlight(um);
            if(nHl) nHl.gotHighlight(um);
            VHl = nHl;
        } else if(VHl)
            VHl.keptHighlight(um);

        if(UI.Selected == null) return;

        var crnr = um.Highlight as CrnrHndl;

        if(crnr && crnr.V ) {
            if(crnr == CSel) {
                if(um.grabMouseUp(0)) {
                    Debug.Log("clicky");
                    
                    var flg = !crnr.V.CrnrFlgs[crnr.Ci];
                    foreach( var n in crnr.C.Vox )  {
                        for(int i = 8; i-- > 0;) {
                            if(n.Crnr[i] == crnr.C) {
                                n.CrnrFlgs[i] = flg;
                            }
                        }                        
                    }

                    crnr.V.Strct.dirty();
                }
            } else {
                CSel = null;
                if(um.grabMouseDown(0)) {
                    CSel = crnr;
                }
            }
        } else {
            CSel = null;
            LeftTool.mouseUpdate(ref cntx, 0);
            RightTool.mouseUpdate(ref cntx, 1);
        }
        /*
        if(Hl.V) {
            Debug.DrawLine(Hl.Wp, Hl.Wp + Hl.Nrm * 0.5f, Color.black);
        }
        if(Selected.V) {
            Debug.DrawLine(Selected.Wp, Selected.Wp + Selected.Nrm * 0.5f, Color.white);
        }*/

        if(DirtySelection) {
            selectionUpdate(ref cntx);
        }
    }
    public bool DirtySelection = false;

    public void extrudeClick( ref CtorUpCntx cntx ) {
        var v = cntx.VHl;
        var strct = v.Strct;
        if(cntx.FHl == null) return;
        /*
        var i = v.I + Voxel.faceOff(Hl_D.FaceI);
        
        if(strct.occupied(i)) {
            Debug.LogError("place occupied");
            return;
        } */


        var go = Instantiate(VoxelFab);

        go.transform.position = strct.Root.Trnsfrm.TransformPoint(cntx.FHl.Sf.Mid + cntx.FHl.Sf.Nrm * 0.5f);
        var nv = go.GetComponent<Voxel>();
        nv.init(strct );
        go.transform.localRotation = v.transform.localRotation;
        go.transform.localScale = v.transform.localScale;


    }
}
