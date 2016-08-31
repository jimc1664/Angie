using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AreaDisplay : MonoBehaviour {

    public static AreaDisplay Singleton;


    public float Scl = 5, IcoSpd = 15.0f, PushMul = 1, PushMod = 20;
    public bool DrawPosLines = false;
    public Sim.Area Ar;

    public static void set(Sim.Area a) {

        if(Singleton != null) {
            if(Singleton.Ar == a) return;
            Destroy(Singleton.gameObject);
        }
        Singleton = Instantiate(UIMain.Singleton.AreaDis).GetComponent<AreaDisplay>();
        Singleton.init(a);
    }

    [System.Serializable]
    public class DroneDat : Icon {
        public Sim.Drone Drn;
        public Vector3 Pos3;
        public Vector2 Pos;

        public Vector2 Push;
        public float Weight;
        public override void onSelect() {
            var ad = AreaDisplay.Singleton;
            if(ad == null || ad.Selected == this) return;

            ad.Selected = this;
        }
        public override void onDeSelect() {
            var ad = AreaDisplay.Singleton;
            if(ad == null || ad.Selected != this) return;

            ad.Selected = null;
        }

        public List<SimObj.ReportMessage> Msgs = new List<SimObj.ReportMessage>();
    }
    public DroneDat Selected;

    void init(Sim.Area a) {

        Ar = a;
        var rt = GetComponent<RectTransform>();
        rt.parent = SolMap.Singleton.Areas;
        rt.resetTransformation();


        foreach(var d in Ar.Drones) {
            addDrn(d);
        }
    }

    public void addDrn(Sim.Drone drn) {
        var d = new DroneDat() {
            Drn = drn,
            Weight = 0.00001f,
        };
        d.init(drn, transform as RectTransform );

        d.Pos3 = d.Drn.fdSmooth(Simulation.Singleton).Pos;
        d.Pos = new Vector2(d.Pos3.x, d.Pos3.z) * Scl;
        Drns.Add(d);
    }
    public List<DroneDat> Drns = new List<DroneDat>();

    DroneDat fetch(  Sim.Drone d ) {
        foreach(var dd in Drns)
            if(dd.Drn == d) return dd;
        return null;
    }

    static Vector3[] TmpCrns = new Vector3[4];
    Vector3 toWorldSpc(Vector2 p) {
        RectTransform rt = transform as RectTransform;
        rt.GetWorldCorners(TmpCrns);

       // Debug.DrawLine(TmpCrns[0], crns[1], Color.red);
       // Debug.DrawLine(crns[0], crns[2], Color.blue);
      //  Debug.DrawLine(crns[0], crns[3], Color.green);

      //  Vector3 wpBl = TmpCrns[0], wpX = TmpCrns[1] - TmpCrns[0], wpY = TmpCrns[3] - wpBl;

        p.x /= rt.sizeDelta.x;
        p.y /= rt.sizeDelta.y;

        p += Vector2.one * 0.5f;

        return Vector3.LerpUnclamped(Vector3.LerpUnclamped(TmpCrns[0], TmpCrns[1], p.y), Vector3.LerpUnclamped(TmpCrns[3], TmpCrns[2], p.y), p.x );
    }

    void LateUpdate() {
        var sim = Simulation.Singleton;
        for(int i = Drns.Count; i-- > 0;) {
            var d = Drns[i];

            if(d.Drn == null || d.Drn.Ar != Ar) {
                Destroy(d.UI.gameObject);
                Drns.RemoveAt(i);
                continue;
            }
            d.Pos3 = d.Drn.fdSmooth(sim).Pos;
            var wp = new Vector2(d.Pos3.x, d.Pos3.z) * Scl;
            d.Pos = Vector2.MoveTowards(d.Pos, wp, IcoSpd * Time.unscaledDeltaTime);

            d.Weight = Mathf.Lerp(d.Weight, d.Drn.Rad, Time.unscaledDeltaTime);

            for(int j = Drns.Count; --j > i;) {
                var d2 = Drns[j];

                var vec = (d.Pos - d2.Pos);

                var vm = vec.sqrMagnitude;

                if(vm > Math_JC.pow2(PushMod)) continue;

                if(vm < Mathf.Epsilon) {
                    vec = Random.insideUnitCircle.normalized;
                    vm = 1;
                } else {
                    vec = vec.normalized;
                }

                float wc = d.Weight + d2.Weight;
                vec = vec * Math_JC.pow2(PushMod - Mathf.Sqrt( vm)) / wc;

                d.Push += vec *d2.Weight;
                d2.Push -= vec *d.Weight;
            }
        }

        RectTransform lblP = SolMap.Singleton.Overlay;
        //float y = 0; 
        foreach(var m in Ar.Msgs) {
            if(m.UI == null) {
                if(m.Trns == null)
                    continue;

                DroneDat dd;
                foreach(var d in Drns) {
                    if(d.Drn.Host && d.Drn.Host.transform == m.Trns) {

                        //d.Msgs.Add(m);
                        dd = d;
                        goto label_Found;
                    }
                }
                continue;
                label_Found:;

                m.UI = Instantiate(UIMain.Singleton.Label).GetComponent<RectTransform>();
                m.UI.GetComponent<Text>().text = m.S;
                m.UI.parent = lblP;
                m.UI.resetTransformation();
                dd.Msgs.Add(m);
                
            }
            //m.UI.anchoredPosition = new Vector2( 0,             
        }

        foreach( var d in Drns ) {

            var wp = new Vector2(d.Pos3.x, d.Pos3.z) * Scl;

            d.Push *= PushMul;
            d.Pos += d.Push * Time.unscaledDeltaTime;

            if(DrawPosLines) {
                Debug.DrawLine(toWorldSpc(d.Pos), toWorldSpc(d.Pos + d.Push), Color.blue);
                Debug.DrawLine(toWorldSpc(d.Pos), toWorldSpc(wp), Color.black);
                Debug.DrawLine(toWorldSpc(Vector2.zero), toWorldSpc(wp), Color.red);
            }
            // d.Push = Vector2.zero;
            d.Push *= 0.75f;
            d.UI.anchoredPosition = d.Pos;

            float my = 0;
            for(int i = d.Msgs.Count; i-- > 0;) {
                var m = d.Msgs[i];
                if(m.Tm < 0) {
                    d.Msgs.RemoveAt(i);
                } else {

                    m.UI.anchoredPosition = d.UI.anchoredPosition + new Vector2(15 + m.UI.sizeDelta.x*0.5f, my);
                    my += 25;
                }
            }

            d.IconFrame.gameObject.SetActive( Selected == d );

            int chnc = (int) (d.Drn.Primary.LChanceToHit * 100.0f );

            bool vis = Selected == d || d.Bttn.isHighlighted;
            d.Txt.gameObject.SetActive(true);
            if(vis) {
                d.UI.SetAsLastSibling();
                d.Txt.text = d.Drn.name + chnc;
            } else {
                d.Txt.text = "" + chnc;
            }


            d.Shield.value = d.Drn.Hp / d.Drn.MaxHp;
            d.Energy.value = d.Drn.Power / d.Drn.MaxPower;
            d.Ammo.value = (float)d.Drn.Primary.AmmoCnt / (float) d.Drn.Primary.MaxAmmo;

            if(d.Drn.Primary && d.Drn.Primary.FireAt) {
                var w = d.Drn.Primary;
                var trgt = fetch(w.FireAt);
                if(trgt != null ) {
                    Debug.DrawLine(trgt.UI.position, d.UI.position, new Color(w.LChanceToHit, 0, 0.4f, 1));
                }

            }
        }



    }


}
