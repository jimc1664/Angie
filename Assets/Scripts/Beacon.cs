using UnityEngine;
using System.Collections.Generic;

public class Beacon : MonoBehaviour {


    public float Energy = 0;

    public float EnergyGain = 1;

    public Sim.Area Ar;

    [System.Serializable]
    public class AreaDat {
        public Sim.Area Ar;
        public float LastVisit = -1000;
    };

    public List<AreaDat> TargetAreas;


    public Sim.Area getTargetArea() {

        float d = float.MinValue;
        Sim.Area ret = null;

        return TargetAreas[Random.Range(0, TargetAreas.Count)].Ar;
    }


    void Start () {
        Ar = GetComponentInParent<Sim.Area>();
        foreach(var a in FindObjectsOfType<Sim.Area>()) {
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
