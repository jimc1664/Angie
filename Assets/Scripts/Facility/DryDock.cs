using UnityEngine;
using System.Collections;

public class DryDock : MonoBehaviour {

    public MiningDrone InConstruction;

    public float Eta;


    void Update() {

        if(InConstruction) {
            Eta -= Time.deltaTime;
            if(Eta < 0) {

                var st = GetComponentInParent<Station>();

                InConstruction.init( st );

                Eta = 0;
                InConstruction = null;
            }
        }

    }
}
