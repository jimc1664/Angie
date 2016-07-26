using UnityEngine;
using System.Collections;

public class SimObj : MonoBehaviour {

    public delegate void FooDlg(ref Simulation.FrameCntx fc);
    public void foo(ref Simulation.FrameCntx fc,  FooDlg act){
        act(ref fc);
    }

   public virtual void init() {

        Debug.LogError("err");
    }

}
