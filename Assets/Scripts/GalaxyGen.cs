using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GalaxyGen : MonoBehaviour {

    public GameObject SectorFab;
    
    public List<Vector3> P;
    static GalaxyGen _Singleton = null;
    public static GalaxyGen Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<GalaxyGen>();
            return _Singleton;
        }
        private set { }
    }


}
