using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Galaxy : MonoBehaviour {


    /// <summary>
    /// Calculates the intersection line segment between 2 lines (not segments).
    /// Returns false if no solution can be found.
    /// </summary>
    /// <returns></returns>
    public static bool CalculateLineLineIntersection(Vector3 line1Point1, Vector3 line1Point2,
        Vector3 line2Point1, Vector3 line2Point2, out Vector3 resultSegmentPoint1, out Vector3 resultSegmentPoint2) {
        // Algorithm is ported from the C algorithm of 
        // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
        resultSegmentPoint1 = Vector3.zero;
        resultSegmentPoint2 = Vector3.zero;

        Vector3 p1 = line1Point1;
        Vector3 p2 = line1Point2;
        Vector3 p3 = line2Point1;
        Vector3 p4 = line2Point2;
        Vector3 p13 = p1 - p3;
        Vector3 p43 = p4 - p3;

        Vector3 p21 = p2 - p1;

        float d1343 = p13.x * (float)p43.x + (float)p13.y * p43.y + (float)p13.z * p43.z;
        float d4321 = p43.x * (float)p21.x + (float)p43.y * p21.y + (float)p43.z * p21.z;
        float d1321 = p13.x * (float)p21.x + (float)p13.y * p21.y + (float)p13.z * p21.z;
        float d4343 = p43.x * (float)p43.x + (float)p43.y * p43.y + (float)p43.z * p43.z;
        float d2121 = p21.x * (float)p21.x + (float)p21.y * p21.y + (float)p21.z * p21.z;

        float denom = d2121 * d4343 - d4321 * d4321;
        if(Mathf.Abs((float)denom) < Mathf.Epsilon) {
            Debug.LogError("unhandled");
            //return false;
        }
        float numer = d1343 * d4321 - d1321 * d4343;

        float mua = numer / denom;
        float mub = (d1343 + d4321 * (mua)) / d4343;

        mua = Mathf.Clamp01(mua);
        mub = Mathf.Clamp01(mub);

        resultSegmentPoint1.x = (float)(p1.x + mua * p21.x);
        resultSegmentPoint1.y = (float)(p1.y + mua * p21.y);
        resultSegmentPoint1.z = (float)(p1.z + mua * p21.z);
        resultSegmentPoint2.x = (float)(p3.x + mub * p43.x);
        resultSegmentPoint2.y = (float)(p3.y + mub * p43.y);
        resultSegmentPoint2.z = (float)(p3.z + mub * p43.z);

        return true;
    }

    public static float calculateSegementSegemntSqDis(Vector3 line1Point1, Vector3 line1Point2,
        Vector3 line2Point1, Vector3 line2Point2) {
        Vector3 resultSegmentPoint1, resultSegmentPoint2;
        CalculateLineLineIntersection(line1Point1, line1Point2, line2Point1, line2Point2, out resultSegmentPoint1, out resultSegmentPoint2);
        return (resultSegmentPoint1 - resultSegmentPoint2).sqrMagnitude;
    }


    float dist_Point_to_Segment(Vector3 p, Vector3 l1, Vector3 l2) {
        Vector3 v = l2 - l1;
        Vector3 w = p - l1;

        float c1 = Vector3.Dot(w, v);
        if(c1 <= 0)
            return (p - l1).sqrMagnitude;

        float c2 = Vector3.Dot(v, v);
        if(c2 <= c1)
            return (p - l2).sqrMagnitude;

        float b = c1 / c2;
        Vector3 Pb = l1 + b * v;
        return (p - Pb).sqrMagnitude;
    }


    public Texture2D Mask;
    public int _BaseSeed = 1, _SeedX = 7919, _SeedY = 7057, _SeedZ = 4957, _Samples = 5;
    [HideInInspector]
    [SerializeField]
    int BaseSeed = 1, SeedX = 7919, SeedY = 7057, SeedZ = 4957, Samples = 5;

    public int _DimX = 10, _DimY = 1, _DimZ = 10, _CX = 0, _CZ = 0, _InitRad = 9999;
    [HideInInspector]
    [SerializeField]
    int DimX = 10, DimY = 1, DimZ = 10, X1 = 0, X2 = 0, Z1 = 0, Z2 = 0;

    public bool Hyperlane_StarBlock = true, Hyperlane_CrossIntersect = true;
    public float Hyperlane_MaxDis = 1;

    public bool DimIsSize = true;
    public bool Regen = false;
    public bool Inc = false;

    public bool DrawSamples = false;
    public int StarCnt = -1, PossibleHyperlanes = -1;

    Transform Trnsfrm;

    void initSec( int x, int y, int z, Vector3 sz, Vector3 hSz, Color [] pix, Sector [,] secAry, int nMsk ) {
        var lp = new Vector3((x - DimX * 0.5f + 0.5f) / DimX, (y - DimY * 0.5f + 0.5f) / DimY, (z - DimZ * 0.5f + 0.5f) / DimZ);
        var c = Trnsfrm.position + new Vector3((x - DimX * 0.5f + 0.5f) * sz.x, (y - DimY * 0.5f + 0.5f) * sz.y, (z - DimZ * 0.5f + 0.5f) * sz.z);

        var ss = Random.seed = BaseSeed + x * SeedX + y * SeedY + z * SeedZ;
        float d = 0;
        var lp2 = new Vector3((x + 0.5f) / DimX, (y + 0.5f) / DimY, (z + 0.5f) / DimZ);

        var st = Instantiate(GalaxyGen.Singleton.SectorFab).transform;
        // st.position = c;
        st.parent = Trnsfrm;
        st.localPosition = lp;
        st.localScale = sz;
        var sec = st.GetComponent<Sector>();
        sec.MissedStars = new System.Collections.Generic.List<Sector.Line>();
        sec.Nbrs = new Sector[8];

        int[,] secNOffset = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 1, 1 } };
        for(int i = 8; i-- > 0;) {
            int nx = x + secNOffset[i, 0];
            int nz = z + secNOffset[i, 1];
            if(nx < X1 || nx > X2 || nz < Z1 || nz > Z2) continue;
            sec.Nbrs[i] = secAry[nx - X1, nz - Z1];
        }
        secAry[x - X1, z - Z1] = sec;
        for(int i = Samples; i-- > 0;) {

            var p = lp2 + new Vector3(Random.Range(-hSz.x, hSz.x), 0, Random.Range(-hSz.z, hSz.z));

            int px = Mathf.RoundToInt(p.x * (Mask.width - 1)), py = Mathf.RoundToInt(p.z * (Mask.height - 1));
            if((uint)(px + py * Mask.width) > (uint)pix.Length) {
                Debug.Log("err px  " + px + "  py  " + py);
                continue;
            }
            var col = pix[Mathf.RoundToInt(p.x * (Mask.width - 1)) + Mathf.RoundToInt(p.z * (Mask.height - 1)) * Mask.width];
            d += Mathf.Clamp01(col.r - Mathf.Pow(col.b, 1.2f) * 0.7f);


            /*
            sec.C.Add(col);
            sec.Pix.Add(new Vector2(px, py));
            sec.So.Add(new Vector2(p.x, p.z)); */
        }
        d /= Samples;

        sec.EffSeed = ss;
        sec.EffDensity = d;

        d = Mathf.Pow(sec.EffDensity - 0.001f, 0.90f) * 30;
        int sc = Mathf.FloorToInt(d);


        Vector3 sp = st.position, scl = st.localScale;
        float mnD = scl.magnitude * 0.1f; mnD *= mnD;

        float yBias = 0.2f;
        for(int i = sc; i-- > 0;) {

            var o = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f) *Random.Range(0.5f, 1.0f), Random.Range(-0.5f, 0.5f));
            o.Scale(scl);


            var sp2 = sp + o; sp2.y *= yBias;

            bool pass = true;
            sec.forNearSector((Sector nSec) => {
                for(int j = nSec.Stars.Count; j-- > 0;) {
                    // if(nSec == sec) break;
                    var tp = nSec.transform.position + nSec.Stars[j].P; tp.y *= yBias;

                    if((tp - sp2).sqrMagnitude < mnD) {
                        sec.MissedStars.Add(new Sector.Line() { P1 = sp + o, P2 = nSec.transform.position + nSec.Stars[j].P });
                        return pass = false;
                    }
                }
                return true;
            });
            if(pass) {
                sec.Stars.Add(new Sector.StarSys() { P = o });
                StarCnt++;
            }

        }
        sec.Stt = Sector.State.Gen;
    }
    void initHyperlanes(int x, int y, int z, Vector3 sz, Vector3 hSz, Color[] pix, Sector[,] secAry, int nMsk) {


        var sec = secAry[x - X1, z - Z1];
        var st = sec.transform;

       // sec.HyperLanes = new System.Collections.Generic.List<Sector.Line>();
        sec.MissedHyperLanes = new System.Collections.Generic.List<Sector.Line>();

        Vector3 sp = st.position, scl = st.localScale;
        float mnD = scl.magnitude * 0.095f; mnD *= mnD;


        int[,] secNOffset = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 1, 1 } };
        for(int i = 8; i-- > 0;) {
            int nx = x + secNOffset[i, 0];
            int nz = z + secNOffset[i, 1];
            if(nx < X1 || nx > X2 || nz < Z1 || nz > Z2) continue;
            sec.Nbrs[i] = secAry[nx - X1, nz - Z1];
        }

        sec.forNearSector((Sector nSec, int ni ) => {
            if(nSec.Stt != Sector.State.Gen) return true;
            for(int i = sec.Stars.Count; i-- > 0;) {
                var o = sec.Stars[i];

                // var sp2 = sp + o.P; sp2.y *= 0.33333f;

                int sc = nSec.Stars.Count;
                if(nSec == sec) sc = i;
                for(int j = sc; j-- > 0;) {

                   // var tp = nSec.transform.position + nSec.Stars[j].P; tp.y *= 0.33333f;

                    Vector3 p1 = sp + o.P, p2 = nSec.transform.position + nSec.Stars[j].P;
                    var vec = (p1 - p2); vec.y *= 0.3f;
                    if( vec.sqrMagnitude > Hyperlane_MaxDis * Hyperlane_MaxDis ) continue;
                     
                    if(Hyperlane_StarBlock) {
                        bool pass = true;
                        sec.forNearSector((Sector nSec2) => {
                            var ns2p = nSec2.transform.position;
                            foreach(var ns in nSec2.Stars) {
                                var nsP = ns.P + ns2p;
                                if((nsP - p1).sqrMagnitude < Mathf.Epsilon || (nsP - p2).sqrMagnitude < Mathf.Epsilon) continue;
                                if(dist_Point_to_Segment(nsP, p1, p2) < mnD) {
                                    sec.MissedHyperLanes.Add(new Sector.Line() { P1 = p1, P2 = p2 });
                                    sec.MissedHyperLanes.Add(new Sector.Line() { P1 = nsP, P2 = (p1 + p2) * 0.5f });
                                    return pass = false;
                                }
                            }

                            return true;
                        }); 
                        if(!pass) continue;
                    }
                    if(Hyperlane_CrossIntersect) {
                        bool pass = true;
                        sec.forNearSector((Sector nSec2) => {
                            //if(nSec2.HyperLanes == null) return true;
                            var ns2p = nSec2.transform.position;
                            foreach(var ns in nSec2.Stars) {
                                foreach(var l in ns.Links) {
                                    var lSec = nSec2;
                                    if(l.Ni >= 0) lSec = lSec.Nbrs[l.Ni];

                                    Vector3 lp1 = ns.P + ns2p, lp2 = lSec.transform.position + lSec.Stars[l.Si].P;
                                    if((lp1 - p1).sqrMagnitude < Mathf.Epsilon || (lp1 - p2).sqrMagnitude < Mathf.Epsilon || (lp2 - p1).sqrMagnitude < Mathf.Epsilon || (lp2 - p2).sqrMagnitude < Mathf.Epsilon) continue;
                                    if(calculateSegementSegemntSqDis(lp1, lp2, p1, p2) < mnD) {

                                        sec.MissedHyperLanes.Add(new Sector.Line() { P1 = p1, P2 = p2 });
                                        sec.MissedHyperLanes.Add(new Sector.Line() { P1 = (lp1 + lp2) * 0.5f, P2 = (p1 + p2) * 0.5f });

                                        return pass = false;
                                    }
                                }
                            }
                            return true; 
                        });

                        if(!pass) continue;
                    }

                    o.Links.Add(new Sector.StarSys.PossHyperLink() { Ni = ni, Si = j });
                   // sec.HyperLanes.Add(new Sector.Line() { P1 = p1, P2 = p2 });
                    PossibleHyperlanes++; 
                    
                    //if((tp - sp2).sqrMagnitude < mnD) {
               
                    //goto label_fail;
                    //}
                }
            }
            return true;
        });
        
        sec.Stt = Sector.State.Done;

    }
    void gen() {
        BaseSeed = _BaseSeed;
        SeedX = _SeedX;
        SeedY = _SeedY;
        SeedZ = _SeedZ;
        Samples = _Samples;

        DimX = _DimX;
        DimY = _DimY;
        DimZ = _DimZ;
        X1 = Mathf.Max(0, _CX - _InitRad) & ~1;
        Z1 = Mathf.Max(0, _CZ - _InitRad) & ~1;
        X2 = Mathf.Min(DimX-1, _CX + _InitRad) & ~1;
        Z2 = Mathf.Min(DimZ-1, _CZ + _InitRad) & ~1;

        //Debug.Log("a x1  " + X1 + "  x2 " + X2);

        foreach(var s in GetComponentsInChildren<Sector>())
            DestroyImmediate(s.gameObject);

        Vector3 sz = new Vector3(1.0f / (float)DimX, 1.0f / (float)DimY, 1.0f / (float)DimZ);
        var hSz = sz * 0.5f;
        Trnsfrm = transform;

        if(DimIsSize) {
            sz = Vector3.one;
            Trnsfrm.localScale = new Vector3((float)DimX, (float)DimY, (float)DimZ);
        } else
            sz.Scale(Trnsfrm.localScale);

        Trnsfrm.rotation = Quaternion.identity;

        Texture2D tex = Mask;// GetComponentInChildren<SpriteRenderer>().sprite.texture;
        var pix = tex.GetPixels();

        var os = Random.seed;



        var secAry = new Sector[X2 - X1 + 1, Z2 - Z1 + 1];
        //for(int y = DimY; y-- > 0;) 
        int y = 0;

        StarCnt = 0;
        for(int x = X1; x <= X2; x += 2)
            for(int z = Z1; z <= Z2; z += 2)
                initSec(x, y, z, sz, hSz, pix, secAry, 0);
        for(int x = X1 + 1; x <= X2; x += 2)
            for(int z = Z1; z <= Z2; z += 2)
                initSec(x, y, z, sz, hSz, pix, secAry, 1);
        for(int x = X1; x <= X2; x += 2)
            for(int z = Z1 + 1; z <= Z2; z += 2)
                initSec(x, y, z, sz, hSz, pix, secAry, 1);
        for(int x = X1 + 1; x <= X2; x += 2)
            for(int z = Z1 + 1; z <= Z2; z += 2)
                initSec(x, y, z, sz, hSz, pix, secAry, 1);

        PossibleHyperlanes = 0; 
        for(int x = X1; x <= X2; x += 2)
            for(int z = Z1; z <= Z2; z += 2)
                initHyperlanes(x, y, z, sz, hSz, pix, secAry, 0);
        for(int x = X1 + 1; x <= X2; x += 2)
            for(int z = Z1; z <= Z2; z += 2)
                initHyperlanes(x, y, z, sz, hSz, pix, secAry, 1);
        for(int x = X1; x <= X2; x += 2)
            for(int z = Z1 + 1; z <= Z2; z += 2)
                initHyperlanes(x, y, z, sz, hSz, pix, secAry, 1);
        for(int x = X1 + 1; x <= X2; x += 2)
            for(int z = Z1 + 1; z <= Z2; z += 2)
                initHyperlanes(x, y, z, sz, hSz, pix, secAry, 1);


        Random.seed = os;
    }

    void Update() {
        if(Inc) {
            BaseSeed++;
            Regen = true;
            Inc = false;
        }
        if(Regen) {
            gen();
            Regen = false;
        }

    }

    void OnDrawGizmos() {
        Vector3 sz = new Vector3(1.0f / (float)DimX, 1.0f / (float)DimY, 1.0f / (float)DimZ);
        var t = transform;

        if(DimIsSize) {

            sz = Vector3.one;
            t.localScale = new Vector3( (float)DimX, (float)DimY, (float)DimZ);
        } else 
            sz.Scale(t.localScale);

        t.rotation = Quaternion.identity;

        Gizmos.color = Color.cyan;
        var os = Random.seed;
        var hSz = sz * 0.5f;
        for(int x = X1; x <= X2; x++)
            for(int y = DimY; y-- > 0;)
                for(int z = Z1; z <= Z2; z++) {

                    var c = t.position + new Vector3((x - DimX * 0.5f + 0.5f) * sz.x, (y - DimY * 0.5f + 0.5f) * sz.y, (z - DimZ * 0.5f + 0.5f) * sz.z);
                    
                    Gizmos.DrawWireCube( c, sz);

                    if(DrawSamples) {
                        Gizmos.color = Color.red;

                        Random.seed = BaseSeed + x * SeedX + y * SeedY + z * SeedZ;
                        for(int i = Samples; i-- > 0;) {

                            var p = c + new Vector3(Random.Range(-hSz.x, hSz.x), 0, Random.Range(-hSz.z, hSz.z));

                            Gizmos.DrawWireSphere(p, sz.magnitude * 0.1f);
                        }
                        Gizmos.color = Color.cyan;
                    } 
                }

        Random.seed = os;

    }
}
