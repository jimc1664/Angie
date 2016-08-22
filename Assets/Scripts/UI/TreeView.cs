using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent( typeof(UnityEngine.UI.ScrollRect) )]
public class TreeView : MonoBehaviour {

    public GameObject EntryFab;

    // List<TreeView_Ele> Entries;


    public float YStride = 25;
    public float XOff = 5;
    public float XStride = 15;


    public string NewEle = "";
    public string NewParent = "";
    public bool Add = false;
    public bool Fix = false;

    public bool Dirty = false;


    UnityEngine.UI.ScrollRect Sr;

    void Awake() {
        Sr = GetComponent<UnityEngine.UI.ScrollRect>();
    }

    static TreeView_Ele findEntry( Transform _this,  string n ) {
        foreach(var e in _this.GetComponentsInChildren<TreeView_Ele>()) {
            //if(_this == e.transform) continue;
            if(e.name.Equals(n, System.StringComparison.OrdinalIgnoreCase)) {
                return e;
            }
            //var sub = findEntry(e.transform, n);
           // if(sub != null) return sub;

        }
        return null;
    }


    void placeEntry(TreeView_Ele ele, ref float yOff, ref float xOff ) {

        ele.transform.localPosition = new Vector3(xOff, -yOff);
        
       // Debug.Log("place  "+ele.name +"  "+ new Vector2(xOff, -yOff) );
        float xOff2 = XStride, yOff2 = YStride;

        ele.gameObject.SetActive(true);
        var tog = ele.Expand; //   ele.GetComponentInChildren<Toggle>();
        bool hasSub = false;
        foreach(Transform t in ele.transform ) {
            var e = t.GetComponent<TreeView_Ele>();
            if(e == null) {
                continue;
            }
            hasSub = true;

            if(tog.isOn)
                placeEntry(e, ref yOff2, ref xOff2);
            else
                e.gameObject.SetActive( false );  
        }
        if(!hasSub) tog.interactable = tog.isOn = false;
        else tog.interactable = true;

        ele.O.SetActive(!hasSub);
        ele.D.SetActive(hasSub && tog.isOn );
        ele.R.SetActive(hasSub && !tog.isOn );


        if(ele.Select.isOn)
            ele.Select.GetComponentInChildren<Text>().text = "<i><b>"+ele.name+ "</b></i>";
        else
            ele.Select.GetComponentInChildren<Text>().text = ele.name;

        yOff += yOff2;
    }


    void Update() {

        if(Add) {
            Sr = GetComponent<UnityEngine.UI.ScrollRect>();  //rem

         //   var e = Instantiate(EntryFab);
          //  e.name = NewEle;
            
           // var t = e.transform as RectTransform;


            var p = findEntry(Sr.content, NewParent);
         //   t.parent = p != null ? p.transform : Sr.content;
//            t.localPosition = Vector3.zero;

            add(NewEle, p);
        }

        if(Add || Fix || Dirty ) {
            Sr = GetComponent<UnityEngine.UI.ScrollRect>();  //rem
            float xOff = XOff, yOff = YStride +XOff;
            foreach( Transform t in Sr.content ) {
                var e = t.GetComponent<TreeView_Ele>();
                if(e == null) {
                    DestroyImmediate(t.gameObject);
                    continue;
                }
                placeEntry(e, ref yOff, ref xOff);
            }
            Dirty = Add = false;
        }


    }

    TreeView_Ele Selected;

    public delegate void OnSelect_Dlg(object o);
    public OnSelect_Dlg OnSelect;

    public void select(TreeView_Ele e) {
        if(Selected == e) return;

        if(Selected) {
            Selected.Select.interactable =  !(Selected.Select.isOn = false);
        }

        Selected = e;

        if(Selected) {
            Selected.Select.interactable = !(Selected.Select.isOn = true);
            if( OnSelect != null ) OnSelect(Selected.Handle);
        } else
            if(OnSelect != null) OnSelect(null);

        Dirty = true;
    }


    public TreeView_Ele add( string nm, object h, TreeView_Ele p = null ) {

        var e = Instantiate(EntryFab);
        e.name = nm;

        var t = e.transform as RectTransform;

        var ele = e.GetComponent<TreeView_Ele>();
        ele.Handle = h;
        //var p = findEntry(Sr.content, NewParent);
        t.parent = (p != null) ? p.transform : Sr.content;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        
        ele.Select.onValueChanged.AddListener((bool b) => {
            if(b)
                select(ele);
        });

        ele.Expand.onValueChanged.AddListener((bool b) => {
            Dirty = true;
        });


        Dirty = true;
        return ele;
    }
}


