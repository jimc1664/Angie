using UnityEngine;
using System.Collections.Generic;

namespace Sim {
    public class MasterAi : MonoBehaviour {
        public Faction Owner;

        // public List<Station> Stations;

        public class Task {

        }

        public class BuildShip_Task : Task {

        }

        [System.Serializable]
        public class StationDat {

            public global::Station Station;
        };
        public List<StationDat> Stations = new List<StationDat>();

        [System.Serializable]
        public class SysDat {
            public List<StationDat> Stations = new List<StationDat>();
            public StarSystem Sys;



            public float Threat = 0, Intel = 0;
        };
        public List<SysDat> StarSystems = new List<SysDat>();

        //area 

        [System.Serializable]
        public class AreaDat {
            public Area Ar;
            public float LastVisit = -1000;

            public int Count = 0;
            public List<Station> StationsInRange = new List<Station>();

            /*
            public enum StatusE {
                Owned, 
                Present,
                Contested,
                Unknown, 
            };
           // public StatusE Status; */
        };

        public List<AreaDat> TargetAreas = new List<AreaDat>();
        public SortedDictionary<Area, AreaDat> Areas = new SortedDictionary<Area, AreaDat>( new MonoBehaviourComparer<Area>() );

        public Area getTargetArea() {

            float d = float.MinValue;
            Area ret = null;

            return TargetAreas[Random.Range(0, TargetAreas.Count)].Ar;
        }

        public void spotted( Drone d, AreaDat at ) {
            Debug.Log(name +" ::  "+ Owner + "    spotted " + d.Host + "   in " + at.Ar.name);
        }

        void Update() {

        }

        public void init( global::Station st) {

            // 
            Owner = st.Owner;

            var sd = new StationDat() { Station = st };
            Stations.Add(sd);
            var sAd = new AreaDat() { Ar = st.Bdy.Ar, Count = 1 };//Status = AreaDat.StatusE.Owned, 
            Areas.Add( st.Bdy.Ar, sAd );
            st.Bdy.Ar.Occupiers.Add(this, sAd);

            var sysD = new SysDat() { Sys = st.Sys };
            sysD.Stations.Add(sd);
            StarSystems.Add(sysD);


            //////

            var ms = Spawnables.Singleton.Recipes[2];
            Facility f = null;
            foreach(var fa in st.GetComponentsInChildren<Facility>()) {
                if(fa.Type == global::Station.Facility_E.DryDock) {
                    f = fa;
                    break;
                }
            }
            var e = new Simulation.Create_Evnt() { Amnt = 1, R = ms, F = f, Name = "Build ship" };


            //Simulation.Singleton.evnt(e, 5 );


            foreach(var a in FindObjectsOfType<Area>()) {
                if(a.GetComponentInChildren<Asteroid>()) {
                    if( (st.transform.position - a.transform.position ).magnitude  > st.WarpRange ) continue;
                    var ad = new AreaDat() { Ar = a, };
                    ad.StationsInRange.Add(st.Sm);
                    TargetAreas.Insert(Random.Range(0, TargetAreas.Count), ad );
                    Areas.Add(a, ad);
                }
            }

            
        }
    }

}