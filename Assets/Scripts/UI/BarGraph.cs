using UnityEngine;
//using UnityEngine.UI;
using System.Collections.Generic;

public class BarGraph : MonoBehaviour {

    public float Value = 0.5f;
    public float Gain = 0.1f;

    public int Segments = 10;

    public GameObject BarFab;

    [System.Serializable]
    public class Bar {
        public RectTransform Tr;
    };

    public List<Bar> Bars;

	void Start () {
	
	}

    void recalc() {
        var rt = GetComponent<RectTransform>();
        float stride = rt.rect.size.x / (float)Segments;
        float h = rt.rect.size.y, h2 = h * 0.5f, w2 = rt.rect.size.x * 0.5f;
        for(int i = 0; i < Segments; i++) {
            var b = Bars[i];
            var bt = b.Tr;

            float v = Value + Gain * i;
//            bt.offsetMin = new Vector2(-stride * 0.5f, -h2);
            bt.offsetMax = new Vector2(bt.offsetMax.x, -h2 + h *v);
        }
    }
    void rebuild() {
        for( int i = transform.childCount; i-- >0;) {
            var t = transform.GetChild(i);
            t.parent = null;
            DestroyImmediate(t.gameObject);
        }

        var rt = GetComponent<RectTransform>();
        float stride = rt.rect.size.x / (float)Segments;
        float h = rt.rect.size.y, h2 = h * 0.5f, w2 =  rt.rect.size.x * 0.5f;
        Bars.Clear();
        for(int i = 0; i < Segments; i++ ) {
            var b = new Bar();
            b.Tr = Instantiate(BarFab).GetComponent<RectTransform>(); ;
            var bt = b.Tr;
            bt.SetParent(transform);
            bt.localScale = Vector3.one;
            bt.sizeDelta = Vector2.zero;
            bt.anchoredPosition = Vector2.zero;

            bt.offsetMin = new Vector2(-stride * 0.5f, -h2);
            bt.offsetMax = new Vector2(stride * 0.5f, h2);
            bt.localPosition = new Vector2(((float) i+0.5f)*stride -w2, 0);
            Bars.Add(b);
        }
    }

    void OnDrawGizmos() {

        if(Segments == transform.childCount && Segments == Bars.Count) {
            foreach(Bar b in Bars)
                if(b.Tr == null || b.Tr.parent != transform) {
                    rebuild();
                    break;
                }
        } else rebuild();


        recalc();
    }
}
