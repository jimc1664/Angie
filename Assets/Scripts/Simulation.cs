using UnityEngine;
using System.Collections.Generic;
using Sim;


public class Simulation : MonoBehaviour {

    public float Glob_KineticScale = 1.0f;
    public float Glob_RotScale = 1.0f;
    public float Glob_AccelScale = 1.0f;

    public float Step = 0.05f;


    public float Timer = 0;
    public int FrameInd = 0;



    [System.Serializable]
    public class Event {
        public string Name = "invalid";

        public virtual void proc() {
            Debug.LogError("unimplemented");
        }

    }

    [System.Serializable]
    public class Create_Evnt : Event {

        public Recipe R;
        public int Amnt;
        public Facility F;


        public override void proc() {
            Debug.LogError("unimplemented");
        }

    }



    class Frame  {
        //int I;
        public List<Event> Events = new List<Event>();
    }


    SortedList<int, Frame> Frames = new SortedList<int, Frame>();

    public List<Sim.Drone> WarpingDrones = new List<Sim.Drone>();
    public List<Area> Areas = new List<Area>();
    //public List<Area> Areas = new List<Area>();


    public struct FrameCntx {

        public readonly float Delta;
        public readonly int FrameInd;
        public Simulation Sim;
        public FrameCntx(Simulation s, float d, int i ) {
            Sim = s;
            Delta = d;
            FrameInd = i;
        }
    };

    void Start() {
        Debug.Log("Start "  );
        foreach(var s in FindObjectsOfType<SimObj>()) {
            s.init();
        }
    }

    void Update () {
        Timer += Time.deltaTime;
        while(Timer > Step) {

            Timer -= Step;
           // Debug.Log("Frame " + FrameInd);
            if(Frames.Count > 0) {
                var n = Frames.Keys[0];
                if(n == FrameInd) {


                    var f = Frames.Values[0];
                    Frames.RemoveAt(0);

                    foreach(var e in f.Events) {
                        Debug.Log("Event "+e.Name);

                    }

                } else if(n < FrameInd) {
                    Debug.LogError("err");
                }
            }


            FrameCntx fc = new FrameCntx( this, Step, FrameInd);

            foreach(var a in Areas)
                a.proc( ref fc);

            foreach(var d in WarpingDrones)
                d.update(ref fc);

            foreach(var ds in DSwap) {
                //if(ds.A != null )

                if(ds.Ar != null) {
                    ds.Ar.addDrone(ds.Dr);
                    if(ds.A != null) {
                        Debug.Assert(ReferenceEquals(ds.A, WarpingDrones));
                        ds.A.Remove(ds.Dr);
                    }
                    Debug.Assert(ReferenceEquals(ds.B, ds.Ar.Drones));
                } else {
                    Debug.Assert(ds.Dr.Ar != null);
                    ds.Dr.Ar.remDrone(ds.Dr);
                    //ds.Dr._Ar = null;// ds.Ar;
                    if(ds.B != null) {
                        ds.B.Add(ds.Dr);
                        Debug.Assert(ReferenceEquals(ds.B, WarpingDrones));
                    }
                }
                ds.Dr.fd_Sync(fc.FrameInd);

                //if(ds.B != null )
                //                ds.A.Remove(ds.Dr);
                //                ds.B.Add(ds.Dr);
                //  Debug.Log("swap " + ds.Dr.Ar + "   ==== "+FrameInd);
            }
            DSwap.Clear();
            FrameInd++;

        }


    }

    public struct DroneSwap {
        public Sim.Drone Dr;
        public List<Sim.Drone> A, B;
        public Area Ar;
    };
    public List<DroneSwap> DSwap = new List<DroneSwap>();

    public void evnt( Event e, int delay  ) {
        int k = FrameInd + delay;
        Frame f;
        if(!Frames.TryGetValue(k, out f)) {
            f = new Frame();
            Frames.Add(k, f);               
        }
        f.Events.Add(e);
    }

    static Simulation _Singleton = null;
    public static Simulation Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<Simulation>();
            return _Singleton;
        }
        private set { }
    }


}
