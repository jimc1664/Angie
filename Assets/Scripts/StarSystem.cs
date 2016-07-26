using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StarSystem : MonoBehaviour {


    public int Seed = 1;

    public bool Regen = false;
    public bool Inc = false;



    void gen() {
        foreach(var s in GetComponentsInChildren<Star>())
            DestroyImmediate(s.gameObject);
        foreach(var s in GetComponentsInChildren<Planetoid>())
            DestroyImmediate(s.gameObject);

        var ui = UIMain.Singleton.SolSys;
        for(int i = ui.childCount; i-- > 0;)
            DestroyImmediate(ui.GetChild(i).gameObject);


        Star star = null;

        int pi = 0;
        float px = 0, py = -1;
        RectTransform lpui = null; Transform lp =null;
        StarGen.StarGen_gen(Seed, (ref StarGen.SunDat s) => {
            var go = new GameObject();
            go.transform.parent = transform;
            star = go.AddComponent<Star>();

            star.init(ref s);
        }, (ref StarGen.PlanetDat p) => {
            var go = new GameObject();
            go.transform.parent = star.transform;
            var planet = go.AddComponent<Planetoid>();

            var uiGo = Instantiate(StarGen.Singleton.PlanetUI);


            var spr = uiGo.GetComponent<UnityEngine.UI.Image>();
           
            var s = Mathf.Pow(p.radius / 6378.0f, 1.0f/3.0f) * 40.0f;
            var uit = uiGo.GetComponent<RectTransform>();

            uit.localScale = Vector3.one *s;
           // uit.sizeDelta = Vector2.zero;
            uit.anchoredPosition = Vector2.zero;

           // uit.offsetMin = new Vector2(0, 0);
           // uit.offsetMax = new Vector2(s, s);

            if(p.planet_no < 0) {
                uit.parent = ui;
                lpui = uit;
           
                px += s * 0.51f;
                uit.localPosition = new Vector2( px, 0);
                px += s * 0.51f;
                py = -s * 0.6f;

                pi++;

                lp = go.transform;
            } else {
                uit.parent = lpui;

                py += -s * 0.51f;
               // Debug.Log("py " + py);
                uit.localPosition = new Vector2(0,py / lpui.localScale.x);
                py += -s * 0.51f;


                go.transform.parent = lp;
                go.transform.localPosition = Vector3.down * p.moon_e;
            }

            spr.sprite = Planetoid.type_sprite((Planetoid.Planet_Type)p.type);

            planet.init(ref p, uiGo );


        });
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

}
