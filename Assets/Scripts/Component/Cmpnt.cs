using UnityEngine;
using System.Collections;

namespace Sim {
    [System.Serializable]
    public class Cmpnt : ScriptableObject {
        public global::Cmpnt Host;
    }
}

public class Cmpnt : MonoBehaviour {
    public Sim.Cmpnt Sc;
    public virtual void init( Sim.Drone drn, Sim.Cmpnt sc ) {
        Debug.LogError("err");
    }
    public virtual void build( Sim.Drone drn, int ci ) {
        Debug.LogError("err");
    }
}
