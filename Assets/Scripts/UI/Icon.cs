using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Icon  {


    public RectTransform UI, Icn, IconFrame;
    public ButtonRmb Bttn;
    public Slider Shield, Energy, Ammo;
    public Text Txt;
    public Graphic Mod;

    public virtual void onSelect() {
        Debug.LogError("err");
    }
    public virtual void onDeSelect() {
        Debug.LogError("err");
    }
    public void init(Sim.Drone d, RectTransform p  ) {
        //t = new Targetable();
        UI = GameObject.Instantiate(UIMain.Singleton.Icon_Frame).GetComponent<RectTransform>();
        UI.parent = p;
        UI.localRotation = Quaternion.identity;
        UI.localScale = Vector3.one;
        UI.localPosition = Vector3.zero;
        UI.anchorMin = UI.anchorMax = Vector2.one * 0.5f;
        UI.name = d.name;
        var Frame = UI.GetChild(0) as RectTransform;
        //GameObject.Destroy(Frame.gameObject);
        Frame.gameObject.SetActive(false);
        Bttn = UI.GetComponent<ButtonRmb>();
        //  Frame.SetAsFirstSibling();
        Icn = GameObject.Instantiate(UIMain.Singleton.Icon_Ship).transform as RectTransform;
        Icn.parent = UI;
        Icn.localRotation = Quaternion.identity;
        Icn.localScale = Vector3.one;
        Icn.localPosition = Vector3.zero;

        var bttnImg = Icn.GetComponent<Image>();

        Bttn.m_graphics.Add(bttnImg);


        Bttn.onClick.AddListener(onSelect);
        Bttn.onRClick.AddListener(onDeSelect);
       /*
       Bttn.onRClick.AddListener(() => {
           if(Plyr == null || Plyr.Drn == null || d == null || d.Drn.Ar != Plyr.Drn.Ar) return;

           if(Sel != null) {
               Sel.Drn = null;
               Debug.Assert(Sel.Trgt == t);
               Sel.Trgt = null;
           }
       });*/

       Color col = d.Owner.Col;
        //Frame.GetComponent<Image>().color = 
        bttnImg.color = col;
        IconFrame = Icn.transform.GetChild(0) as RectTransform;
        Mod = Icn.transform.GetChild(1).GetComponent<RawImage>();
        Txt = Icn.GetComponentInChildren<Text>(true);
        Txt.text = d.name;

        /*
        var pCan = p.GetComponentInParent<Canvas>();
        foreach( var c in UI.GetComponentsInChildren<Canvas>( true ) ) {
         //   c.worldCamera = pCan.worldCamera;
        }
        // Targets.Add(d.Drn, t);

        var cnv = Txt.gameObject.AddComponent<Canvas>();
        cnv.overrideSorting = true;
        cnv.sortingOrder = 1;
        */

        var sldrs = Icn.GetComponentsInChildren<Slider>(true);
        Shield = sldrs[0];
        Energy = sldrs[1];
        Ammo = sldrs[2];

        foreach( var s in sldrs )
            s.gameObject.SetActive(true);


    }
}
