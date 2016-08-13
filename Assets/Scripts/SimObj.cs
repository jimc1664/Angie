using UnityEngine;
using System.Collections;

public class SimObj : MonoBehaviour {

    
    public void foo(ref Simulation.FrameCntx fc,  Sim.Body.FooDlg act){
        act(ref fc);
    }

   public virtual void init() {

        Debug.LogError("err");
    }

}
