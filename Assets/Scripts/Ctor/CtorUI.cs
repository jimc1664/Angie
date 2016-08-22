using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CtorUI : MonoBehaviour {

    public void leftDD(int i) {
        Debug.Log("  i  " + i);
    }

    public InputField NameBox;

    TreeView Tv;

    void Awake() {

        CtorMain.Singleton.UI = this;
        Tv = GetComponentInChildren<TreeView>();
        Tv.OnSelect = onSelect;
    }

    public void onSelect(object obj) {
        var s = obj as Structure;

        if(s == Selected) return;

        Selected = s;
        if(Selected) {
           // Debug.Log("selected " + Selected.name);
            NameBox.text = Selected.name;

        }

    }
    public void setSelect(Structure s) {
        if(s == Selected) return;

        Tv.select(s.Ui);
    }

    public Structure Selected;

    public void add(Structure s ) {
        TreeView_Ele p = null;
        if(s.Parent) p = s.Parent.Ui;
        s.Ui=Tv.add(s.name, s, p );
    }


    public void setName(string s) {
        s = NameBox.text;
      //  Debug.Log(" s " + s);
        if(!Selected) return;

        if(s.Equals(Selected.name)) return;
        Selected.name = s;
        Selected.Ui.name = NameBox.text = Selected.name;

        Tv.Dirty = true;
    }


    public void selectAll() {
        if(!Selected) return;
        Selected.selectAll();
    }
}
