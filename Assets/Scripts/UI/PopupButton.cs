using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupButton : Button {

    public GameObject Panel;

    PopupButton Parent; 

    public class Entry {
        public string Name;
        public UnityEngine.Events.UnityAction Callback;
        public System.Func<List<Entry>> SubListGen;
    };

    System.Func<List<Entry>> ListGen;

    public void set(System.Func<List<Entry>> lg, bool active) {
        ListGen = lg;
        interactable = active;
        if(Panel != null) Destroy(Panel);
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        if(Panel != null) return;

        // string[] strs = { "Alpha1", "Beta7", "Charlie12", "Charlie13", "Charlie14", "Charlie15" };
        if(ListGen == null) {

            interactable = false;
            return;
        }

        var list = ListGen();
        if(list.Count == 0) return;
        var rt = transform as RectTransform;
        rt.SetAsLastSibling();

        Panel = Instantiate(UIMain.Singleton.Popup_Panel);
        var prt = Panel.transform as RectTransform;
        float spc = 1.5f;
        var rct = prt.rect;
        rct.height = spc*3 + list.Count * ( (UIMain.Singleton.Popup_Button.transform as RectTransform).rect.size.y + spc);
        prt.sizeDelta = rct.size;
        int i = 0;
        foreach(var _e in list) {
            var e = _e;
            i++;
            GameObject b;
            if(e.SubListGen != null) {
                b = Instantiate(UIMain.Singleton.Popup_ButtonEx);
                var ch = b.GetComponent<PopupButton>();
                ch.ListGen = e.SubListGen;
                ch.Parent = this;
            } else {
                b = Instantiate(UIMain.Singleton.Popup_Button);
                b.GetComponent<Button>().onClick.AddListener( () => {
                    clearRecursiveUp();
                    e.Callback();
                } );
            }
            var brt = b.transform as RectTransform;
            brt.transform.parent = prt;
          
            brt.localPosition = new Vector3( 0, (brt.rect.size.y + spc) * (list.Count -i) + brt.rect.size.y  * 0.5f + spc*2 - prt.rect.height *0.5f , 0);
            brt.GetComponentInChildren<Text>().text = e.Name;
            brt.localRotation = Quaternion.identity;
            brt.localScale = Vector3.one;
        }
        prt.parent = transform;
        prt.localPosition= new Vector3( prt.rect.width*0.7f, prt.rect.height*0.5f, 0 );
        prt.localRotation = Quaternion.identity;
        prt.localScale = Vector3.one;
        // if(Other != null) Other.baseOnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        clear();
    }
    void clear() {
        if(Panel != null) Destroy(Panel);
    }
    void clearRecursiveUp() {
        for(var t = this; ;) {
            t.clear();
            t = t.Parent;
            if(t == null) break;
        }
    }
}
