using UnityEngine;
using System.Collections.Generic;

public class MasterAi : MonoBehaviour {


    // public List<Station> Stations;

    public class Task {

    }

    public class BuildShip_Task : Task{

    }

    [System.Serializable]
    public class StationDat {

        public Station Station;
    };
    public List<StationDat> Stations = new List<StationDat>();

    [System.Serializable]
    public class SysDat {
        public List<StationDat> Stations = new List<StationDat>();
        public StarSystem Sys;



        public float Threat = 0, Intel = 0;
    };
    public List<SysDat> StarSystems = new List<SysDat>() ;

    //area 


    void Update () {
	
	}

    public void init(Station st) {
        var sd = new StationDat() { Station = st };
        Stations.Add(sd);

        var sysD = new SysDat() { Sys = st.Sys };
        sysD.Stations.Add(sd);
        StarSystems.Add(sysD);


        //////

        var ms = Spawnables.Singleton.Recipes[2];
        Facility f = null; 
        foreach( var fa in st.GetComponentsInChildren<Facility>()) {
            if(fa.Type == Station.Facility_E.DryDock) {
                f = fa;
                break;
            }
        }
        var e = new Simulation.Create_Evnt() { Amnt = 1, R = ms, F = f, Name ="Build ship" };


        Simulation.Singleton.evnt(e, 5 );
    }
}
