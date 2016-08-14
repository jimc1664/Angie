using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class ShipHud : MonoBehaviour {

    public PopupButton WarpButton;

    public RectTransform TargetIcons;

    public PlayerShipCtrlr Plyr;

    public Targetable FireTarget;

    void OnEnable() {
        Plyr = FindObjectOfType<PlayerShipCtrlr>();
        Plyr.UI = this;

        for( int i = TargetSels.Length; i-- >0; ) {
            int j = i;
            var ts = TargetSels[i];
            TargetSels[i].UI.GetComponent<ButtonRmb>().onClick.AddListener(() => {
                Debug.Log("clicky");
                if(ts.Drn == null || ts.Drn.Drn == null || Plyr == null  || Plyr.Drn == null ) return;
                if( ts.Drn.Drn.Owner != Plyr.Drn.Owner )
                    FireTarget = ts.Trgt;
            });
            TargetSels[i].UI.GetComponent<ButtonRmb>().onRClick.AddListener(() => {
                Debug.Log("Right clicky");
                ts.Drn = null;
            });
        }
    }

    
    public class Targetable {
        public RectTransform UI, Frame, Icon, IconFrame;
        public ButtonRmb Bttn;
        public Text Txt;
        public Graphic Mod;

        public bool Framed = false;
        public float Transition = 0;

        [System.NonSerialized]
        public TargetSel Sel;
    };

    public Dictionary<Sim.Body, Targetable> Targets = new Dictionary<Sim.Body, Targetable>();

    [System.Serializable]
    public class TargetSel {
        public TargetCam Cam;
        public RectTransform UI;
        public Graphic Border, Mod;
        public Text NameBox, RangeBox;

        Targetable _Trgt;
        public Targetable Trgt {
            get {
                return _Trgt;
            }
            set {
                if(Trgt != null && Trgt.Sel == this )
                    Trgt.Sel = null;

                _Trgt = value;
                if(Trgt != null)
                    Trgt.Sel = this;
            }
        }
        public Drone Drn;


    };

    public TargetSel[] TargetSels;
    public int ActiveTargets = 0;

    void set(TargetSel ts, Targetable t, Drone d) {
        ts.NameBox.text = d.name;

        ts.Drn = d;
        ts.Trgt = t;

        ts.Border.color = d.Drn.Owner.Col;
        ts.Cam.setTarget(d);       
    }
    void target(Targetable t, Drone d) {

        for(int i = ActiveTargets; i-- > 0;) 
            if(TargetSels[i].Drn == d) 
                return;
            
        

        int ti = ActiveTargets++;
        TargetSel ts;
        if(ti >= TargetSels.Length) {
            ti = (ActiveTargets = TargetSels.Length) - 1;
            ts = TargetSels[ti];

        } else {
            ts = TargetSels[ti];
            ts.UI.gameObject.SetActive(true);
            ts.Cam.gameObject.SetActive(true);
        }
        set(ts, t, d);
    }
   

    public void onAreaChange(Sim.Area old) {
        foreach(var k in Targets) {
            Destroy(k.Value.UI.gameObject);
        }
        Targets.Clear();

        foreach(var ts in TargetSels) ts.Drn = null;
    }


    void setMod(Graphic mod, bool flag ) {
        Color modCol = mod.color;
        if(flag) {
            modCol.a += (0.7f + Mathf.Sin(Time.time * 100.0f) * 0.3f - modCol.a) * Time.unscaledDeltaTime * 10.0f;
            mod.gameObject.SetActive(true);
        } else {
            modCol.a -= Time.unscaledDeltaTime * 5.0f;
            if(modCol.a < 0) {
                modCol.a = 0;
                mod.gameObject.SetActive(false);
            }
        }
        mod.color = modCol;

    }
    void LateUpdate () {

       // Color modCol = Vector4.zero;
        for(int i = ActiveTargets; i-- > 0;) {
            var ts = TargetSels[i];
            if(ts.Drn == null || ts.Drn.Drn.Ar != Plyr.Drn.Ar) {
                var clr = ts;
                ActiveTargets--;
                for(int j = i; j < ActiveTargets; j++) {
                    clr = TargetSels[j + 1];
                    set(TargetSels[j], clr.Trgt, clr.Drn);
                }
                clr.UI.gameObject.SetActive(false);
                clr.Cam.gameObject.SetActive(false);
                clr.Trgt = null;
                clr.Drn = null;
            } else {
                setMod(ts.Mod, ts.Trgt == FireTarget);
            }
        }
        if(FireTarget != null && (FireTarget.Sel == null || FireTarget.Sel.Trgt != FireTarget) )
            FireTarget = null;


        if(Plyr.Drn.Ar != null) {
            List<Sim.Body> toRem = null;
            foreach(var k in Targets.Keys)
                if(k.Ar != Plyr.Drn.Ar) {
                    if(toRem == null) toRem = new List<Sim.Body>();
                    toRem.Add(k);
                }
            if(toRem != null)
                foreach(var k in toRem) {
                    Destroy(Targets[k].UI.gameObject);
                   // Destroy(Targets[k].Frame.gameObject);
                    Targets.Remove(k);
                }

            var cm = Camera.main;


            float rMod = 300.0f / -Mathf.Tan(0.5f * cm.fieldOfView);

            //Debug.Log("   v1  " + v1 + "   v2  " + v2);
            foreach(var b in Plyr.Drn.Ar.Vis.GetComponentsInChildren<Body>()) {
                var d = b as Drone;
                if( d == null || d == Plyr) continue;

                Targetable t;
                if(!Targets.TryGetValue(d.Drn, out t)) {
                    t = new Targetable();
                    t.UI = Instantiate(UIMain.Singleton.Icon_Frame).GetComponent<RectTransform>();
                    t.UI.parent = TargetIcons;
                    t.UI.name = b.name;
                    t.Frame = t.UI.GetChild(0) as RectTransform;
                    t.Bttn = t.UI.GetComponent<ButtonRmb>();
                    //  t.Frame.SetAsFirstSibling();
                    t.Icon = Instantiate(UIMain.Singleton.Icon_Ship).transform as RectTransform;
                    t.Icon.transform.parent = t.UI;

                    var bttnImg = t.Icon.GetComponent<Image>();

                    t.Bttn.m_graphics.Add(bttnImg);

                    t.Bttn.onClick.AddListener(() => {
                        if(Plyr == null || Plyr.Drn == null || d == null || d.Drn.Ar != Plyr.Drn.Ar ) return;

                        if( t.Sel == null )
                            target(t, d);
                        else if(d.Drn.Owner != Plyr.Drn.Owner)
                            FireTarget = t;
                    });

                    t.Bttn.onRClick.AddListener(   () => {
                        if(Plyr == null || Plyr.Drn == null || d == null || d.Drn.Ar != Plyr.Drn.Ar) return;

                        if(t.Sel != null) {
                            t.Sel.Drn = null;
                            Debug.Assert(t.Sel.Trgt == t);
                            t.Sel.Trgt = null;
                        }
                    });

                    Color col = d.Drn.Owner.Col;
                    t.Frame.GetComponent<Image>().color = bttnImg.color = col;
                    t.IconFrame = t.Icon.transform.GetChild(0) as RectTransform;
                    t.Mod = t.Icon.transform.GetChild(1).GetComponent<RawImage>();
                    t.Txt = t.Icon.GetComponentInChildren<Text>(true);
                    t.Txt.text = b.name;

                    Targets.Add(d.Drn, t);
                }

                //cm. ViewportToScreenPoint
                Vector3 sp = cm.WorldToScreenPoint(b.transform.position);
                //Debug.Log(b.name + "   sp  " + sp + "   b.transform.position  " + b.transform.position);


                var osp = sp;

                float frameSz =5.0f + b.Rad * rMod / sp.z;

                float cmp = t.UI.sizeDelta.x * 0.5f;

                float spd = Time.unscaledDeltaTime * 4.0f;

                if(sp.z < 0) {
                    sp.x = sp.x > cm.pixelWidth * 0.5f ? cm.pixelWidth : 0;
                    sp.y = sp.y > cm.pixelHeight * 0.5f ? cm.pixelHeight : 0;
                }

                float border = 15.0f;
                if(sp.x < border) {
                    sp.x = border;
                    t.Framed = false;
                    cmp = float.MaxValue;
                } else if(sp.x > cm.pixelWidth - border) {
                    sp.x = cm.pixelWidth - border;
                    t.Framed = false;
                    cmp = float.MaxValue;
                }
                if(sp.y < border) {
                    sp.y = border;
                    t.Framed = false;
                    cmp = float.MaxValue;
                } else if(sp.y> cm.pixelHeight-border ) {
                    sp.y = cm.pixelHeight - border;
                    t.Framed = false;
                    cmp = float.MaxValue;
                }
              //  Debug.Log(b.name + "   osp  " + osp + "  sp  " +sp );
                if(t.Framed) {
                    if((t.Transition += spd) > 1) t.Transition = 1;
                    if(frameSz *1.1f < cmp) {
                        t.Framed = false;
                    }
                } else {
                    if((t.Transition -= spd ) < 0) {
                        t.Transition = 0;
                        t.Frame.gameObject.SetActive(false);
                    }

                    if(frameSz *0.9f > cmp ) {
                        t.Framed = true;
                        t.Frame.gameObject.SetActive(true);
                    }
                }
                frameSz *= t.Transition;

                (t.Icon as RectTransform) .anchoredPosition = Vector2.one * frameSz * 0.5f + t.UI.sizeDelta * 0.77f * 0.75f * 0.8f;
                

                t.UI.anchoredPosition = sp;
                t.Frame.sizeDelta = new Vector2(1.0f / t.Frame.localScale.x, 1.0f / t.Frame.localScale.y) *frameSz;


                t.IconFrame.gameObject.SetActive(t.Sel != null);

                t.Txt.gameObject.SetActive(t.Sel != null || t.Bttn.isHighlighted);

                setMod(t.Mod, t == FireTarget);

            }
        }
	}
}
