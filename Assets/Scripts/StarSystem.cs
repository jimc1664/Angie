using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class StarSystem : MonoBehaviour {


    public int Seed = 1;

    public bool Regen = false;
    public bool Inc = false;

    public float OrbitalPow = 0.6f;
    public float LunarScale = 0.33333f;

    class Area : IQuadTreeObject {

        public Vector2 Pos;
        public Vector2 GetPosition() {
                return Pos;// * 10.0f;
        }
        public float Rad {
            get {
                return _Rad;
            }
            set {
                _Rad = value;
            }
        }
        float _Rad;
        public Vector3 Pos3 {
            get {
                return new Vector3( Pos.x,0, Pos.y);
            }
        }
        public List<Area> Nbrs;

        public int Island = -1;
    };

    List<Area> Areas;
    public float DensityMod = 0.2f, DensityPow = 1.75f;
    public float MinIsoDistance = 2;
    public bool KillIso = false;

    float areaDensityFunc( float a ) {
        return Mathf.Pow(a, DensityPow ) * Mathf.PI* DensityMod;
    }
    void gen() {
        foreach(var s in GetComponentsInChildren<Star>())
            DestroyImmediate(s.gameObject);
        foreach(var s in GetComponentsInChildren<Planetoid>())
            DestroyImmediate(s.gameObject);

        var ui = UIMain.Singleton.SolSys;
        for(int i = ui.childCount; i-- > 0;)
            DestroyImmediate(ui.GetChild(i).gameObject);


       
        Star star = null;

        int pi = 0, mi = 0;
        float px = 0, py = -1;
        RectTransform lpui = null; Transform lp =null;

        Random.seed = Seed;
        float areaPass = 0, areaTheta = Random.value * Mathf.PI * 2;

        Areas = new List<Area>();
        //float bounds = 50;
        QuadTree<Area> qt = new QuadTree<Area>(3); //, new Rect( -bounds, -bounds, bounds*2, bounds*2) );
        //Debug.Log("xxx " + qt.QuadRect);

        List<int> islandRedir = new List<int>( 32 );

        StarGen.StarGen_gen(Seed, (ref StarGen.SunDat s) => {
            var go = new GameObject();
            go.transform.parent = transform;
            star = go.AddComponent<Star>();

            star.init(ref s);// , Seed );
           
        }, (ref StarGen.PlanetDat p) => {

            p.a = Mathf.Pow(p.a +1, OrbitalPow) - 1;
            p.moon_a = (Mathf.Pow(p.moon_a + 1, OrbitalPow) - 1) * LunarScale;

            var go = new GameObject();
            go.transform.parent = star.transform;
            var planet = go.AddComponent<Planetoid>();
            
            var sphr = Instantiate(StarGen.Singleton.Sphere).transform;
            sphr.parent = go.transform;
            sphr.localPosition = Vector3.zero;
            sphr.localScale = Vector3.one * 2 * p.radius * Math_JC.Km_Au;

            var uiGo = Instantiate(StarGen.Singleton.PlanetUI);


            var spr = uiGo.GetComponent<UnityEngine.UI.Image>();
           
            var s = Mathf.Pow(p.radius / 6378.0f, 1.0f/3.0f) * 40.0f;
            var uit = uiGo.GetComponent<RectTransform>();

            uit.localScale = Vector3.one *s;
           // uit.sizeDelta = Vector2.zero;
            uit.anchoredPosition = Vector2.zero;

            // uit.offsetMin = new Vector2(0, 0);
            // uit.offsetMax = new Vector2(s, s);
            //int pSeed = Seed;
            if(p.planet_no < 0) {


                if(lp != null) {

                    Random.seed = Seed + pi * 829 - 773;
                    var lPlanet = lp.GetComponent<Planetoid>();

                    float a1 = lPlanet.a * lPlanet.a * Mathf.PI, a2 = p.a * p.a * Mathf.PI;
                    float c = (lPlanet.a + p.a) * Mathf.PI;
                    areaPass += areaDensityFunc(p.a) - areaDensityFunc(lPlanet.a);
                    Debug.Log("area mod = " + areaPass + "  (a2 - a1) " + (a2 - a1) + "  a1 " + a1 + "  a2 " + a2);

                    float iterCnt = Mathf.Floor(areaPass);
                    areaPass -= iterCnt;
                    for(int i = (int)iterCnt; i-- > 0;) {
                        var ar = Instantiate(StarGen.Singleton.Area).transform;
                        ar.transform.parent = star.transform;

                        float lrp = Random.Range(-1.0f, 1.0f) * (Random.value*0.3f+0.7f) * 0.4f + 0.5f;
                        float a = Mathf.Lerp(lPlanet.a, p.a, lrp);

                        //var theta = Random.value * Mathf.PI * 2;
                        areaTheta += Random.value + Mathf.PI;
                        float x = a * Mathf.Cos(areaTheta);
                        float y = a * Mathf.Sin(areaTheta);
                        var ap = new Vector2(x, y);
                        ar.transform.localPosition = new Vector3( x, 0, y );

                        var nbrs = qt.RetrieveObjectsInArea(ap, MinIsoDistance);
                        int island = islandRedir.Count;

                        foreach(var n in nbrs) {

                            while(n.Island != islandRedir[n.Island])
                                n.Island = islandRedir[n.Island];

                            if(n.Island < island) {
                                if(island < islandRedir.Count)
                                    islandRedir[island] = n.Island;
                                island = n.Island;
                            } else {
                                n.Island = islandRedir[n.Island] = island;
                            }
                        }

                        if(island == islandRedir.Count)
                            islandRedir.Add(island);
                        var ar2 = new Area() { Pos = ap, Nbrs = nbrs, Island = island, Rad = 0.1f };
                        Areas.Add(ar2);
                        
                        qt.Insert(ar2);
                    }
                } else {
                    areaPass = areaDensityFunc(p.a);
                }

                uit.parent = ui;
                lpui = uit;
           
                px += s * 0.51f;
                uit.localPosition = new Vector2( px, 0);
                px += s * 0.51f;
                py = -s * 0.6f;

                pi++;
                mi = 0;
                lp = go.transform;
            } else {
                uit.parent = lpui;

                py += -s * 0.51f;
               // Debug.Log("py " + py);
                uit.localPosition = new Vector2(0,py / lpui.localScale.x);
                py += -s * 0.51f;

                mi++;
                go.transform.parent = lp;



               // go.transform.localPosition = Vector3.down * p.moon_e;
            }


            spr.sprite = Planetoid.type_sprite((Planetoid.Planet_Type)p.type);

            planet.init(ref p, uiGo, Seed + pi* 829 + mi* 947);

            {
                var ap3 = planet.transform.position;
                var ar2 = new Area() { Pos = new Vector2(ap3.x, ap3.z), Nbrs = new List<Area>(), Island = 0, Rad = 0.3f };
                Areas.Add(ar2);

                // qt.Insert(ar2)
            }

        });
        // Debug.Log("xxx2 " + qt.QuadRect);
        for(int ai = Areas.Count; ai-- > 0;) {
            var n = Areas[ai];
            while(n.Island != islandRedir[n.Island])
                n.Island = islandRedir[n.Island];

            if(KillIso && n.Island != 0) {
                Areas.RemoveAt(ai);

            }
        }
    }

    void Update() {
        if(Inc) {
            Seed++;
            Regen = true;
            Inc = false;
        }
        if(Regen) {

            gen();
            Regen = false;
        }

    }
    void OnDrawGizmos() {

        if(Areas == null) return;

        foreach(var a1 in Areas) {
            Gizmos.color = (a1.Island == 0) ? Color.green : Color.red;

            Gizmos.DrawWireSphere(a1.Pos3, a1.Rad);
            foreach(var a2 in a1.Nbrs)
                Gizmos.DrawLine(a1.Pos3, a2.Pos3);

        }
    }
}
