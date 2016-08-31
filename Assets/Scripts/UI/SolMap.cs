using UnityEngine;
using System.Collections;

public class SolMap : MonoBehaviour {



    public RectTransform Areas, Overlay, SolSys;

    static SolMap _Singleton = null;
    public static SolMap Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<SolMap>();
            return _Singleton;
        }
        private set { }
    }

}
