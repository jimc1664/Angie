using UnityEngine;
using System.Collections.Generic;

public class CtorPhys : MonoBehaviour {

    public bool Gen = false;

    public Transform Dat;


    void gen( bool giz = false ) {
        if(Dat) Destroy(Dat.gameObject);

        var strct = GetComponent<Structure>();

        Dat = new GameObject("PhysStuff").transform;
        Dat.parent = transform;
        Dat.transform.resetTransformation();


        Dictionary<Structure.Vertex, CrnrDat> crnrs = new Dictionary<Structure.Vertex, CrnrDat>(new IdentityEqualityComparer<Structure.Vertex>());

        foreach(var v in strct.Vox) {
            if(v.Strct != strct) return;

            if(v.State == Voxel.StateE.Invalid) continue;

            for(int i = 8; i-- > 0;) {
                if(v.CrnrFlgs[i] || v.Crnr[i] == null) continue;

                //int faceC = 0;
                var crnr = v.Crnr[i];
                Vector3 cp = crnr.V;

                CrnrDat cd;
                if(!crnrs.TryGetValue(crnr.Vrtx, out cd)) {
                    cd = new CrnrDat();
                    crnrs.Add(crnr.Vrtx, cd);
                    cd.P = crnr.V;
                }

                foreach(var f in v.Faces) {
                    if(f.Sf.F2 != null) continue;

                    for(int j = f.Vi.Length; j-- > 0;) {
                        if(f.Vi[j] == i) {

                            Vector3 nrm = f.Sf.Nrm;

                            if(FaceLines && giz ) {
                                Gizmos.color = Color.black;
                                Gizmos.DrawLine(transform.TransformPoint(f.Sf.Mid), transform.TransformPoint(f.Sf.Mid + nrm));

                                Gizmos.color = Color.grey;
                                Gizmos.DrawLine(transform.TransformPoint(f.Sf.Mid), transform.TransformPoint(v.Crnr[i].V));
                            }


                            float d = Vector3.Dot(nrm, f.Sf.Mid);

                            Vector4 pln = nrm;
                            pln.w = d;
                            cd.RPlane.Add(pln);
                            break;
                        }
                    }
                }

            }
        }

        foreach(var cd in crnrs.Values) {

            var cp = cd.P;

            if(cd.RPlane.Count > 0) {
                foreach(var pln in cd.RPlane) {
                    //Vector4 pln = faceD[j % faceC];
                    Vector3 nrm = pln;
                    var op = cp;
                    cp -= nrm * (CrnrRad + Vector3.Dot(nrm, cp) - pln.w);

                    if(giz) {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(transform.TransformPoint(cp), transform.TransformPoint(op));
                    }
                }
                Vector3 mp = Vector3.zero;

                foreach(var pln in cd.RPlane) {
                    Vector3 nrm = pln;

                    mp += cp - nrm * (CrnrRad + Vector3.Dot(nrm, cp) - pln.w);
                }
                cd.P = mp /= cd.RPlane.Count;
            }
            if(giz) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.TransformPoint(cd.P), CrnrRad);
            } else {

                var sphr = Instantiate(CtorMain.Singleton.Sphere);
                var t = sphr.transform;
                t.parent = Dat;
                t.localPosition = cd.P;
                t.localScale = Vector3.one * CrnrRad;
                t.localRotation = Quaternion.identity;
                sphr.layer = 12;
                cd.Body = sphr.AddComponent<Rigidbody>();
            }
        }



        foreach(var e in strct.Edges) {
            if(e == null) continue;

            var c = Color.red;
            c.a = 0.5f;
            if(giz) {
                Gizmos.color = c;
                Gizmos.DrawLine(transform.TransformPoint(e.V1.V), transform.TransformPoint(e.V2.V));

            } else {

                Rigidbody b1 = crnrs[e.V1].Body, b2 = crnrs[e.V2].Body;

                var j = b1.gameObject.AddComponent< FixedJoint>();
                j.connectedBody = b2;

                for(var v = e.V1; ; v = e.V2) {

                    CrnrDat cd;
                    if(!crnrs.TryGetValue(v, out cd)) {
                        cd = new CrnrDat();
                        if(giz) {
                            c = Color.green;
                            c.a = 0.5f;
                            Gizmos.color = c;
                            Gizmos.DrawWireSphere(transform.TransformPoint(v.V), 0.3f);
                        }
                        crnrs.Add(v, cd);
                    }

                    if(v == e.V2) break;
                }
            }
        }
    }

    class CrnrDat {
        public Vector3 P = Vector3.zero;
        public List<Vector4> RPlane = new List<Vector4>(8);
        internal Rigidbody Body;
    };
    class IdentityEqualityComparer<T> : IEqualityComparer<T> where T : class {
        public bool Equals(T v1, T v2) {
            return object.ReferenceEquals(v1, v2);
        }
        public int GetHashCode(T v) {
            return v.GetHashCode();
        }
    }
    public float CrnrRad = 0.25f;

    public bool FaceLines = false;
    public bool Gizmo = false;

    void OnDrawGizmos() {
        if(!Gizmo) return;

        gen(true);
    }
	void Update () {

        if(Gen) {
            gen();
            Gen = false;
        }
	}
}
