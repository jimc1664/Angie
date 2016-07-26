using UnityEngine;
using System.Collections.Generic;

public class Beacon : MonoBehaviour {


    public float Energy = 0;

    public float EnergyGain = 1;

    public Area Ar;

    [System.Serializable]
    public class AreaDat {
        public Area Ar;

        public float LastVisit = -1000;


    };

    public List<AreaDat> TargetAreas;


    public Area getTargetArea() {

        float d = float.MinValue;
        Area ret = null;

        return TargetAreas[Random.Range(0, TargetAreas.Count)].Ar;
    }


    void Start () {

        Ar = GetComponentInParent<Area>();


        foreach(var a in FindObjectsOfType<Area>()) {
            if(a.GetComponentInChildren<Asteroid>()) {
                TargetAreas.Insert( Random.Range(0,TargetAreas.Count), new AreaDat() { Ar = a });
            }
        }
	}

	void Update () {


        Energy += EnergyGain * Time.deltaTime;

        if(Energy > 20) {
            var md = (Instantiate(Spawnables.Singleton.Buzzer, transform.position, transform.rotation) as GameObject).GetComponent<Buzzer>();
            md.init( this );
            Energy -= 20;
        }
	}
}
