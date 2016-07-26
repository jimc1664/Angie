using UnityEngine;
using System.Collections;

public class Asteroid : Body {


    public float RotScl = 1;

    public override void init() {

        var ar = GetComponentInParent<Area>();
        Trnsfrm = transform;
        var sim = Simulation.Singleton;
        Bdy = new Sim.Body() {
            /*ArriveEp = ArriveEp,
            MaxVel = MaxVel * sim.Glob_KineticScale,
            MaxSteer = MaxSteer * sim.Glob_KineticScale * sim.Glob_AccelScale,
            MaxAVel = MaxAVel * sim.Glob_KineticScale * sim.Glob_RotScale,
            MaxASteer = MaxASteer * sim.Glob_KineticScale * sim.Glob_AccelScale * sim.Glob_RotScale,
             */
            Rad = Rad,

            initPos = Trnsfrm.position,
            initVel = Vector3.zero,
            initRot = Trnsfrm.rotation,
            initAVel = Quaternion.Euler((Random.value - 0.5f) * RotScl, (Random.value - 0.5f) * RotScl, (Random.value - 0.5f) * RotScl),
            _Ar = ar,
            Host = this,
        };
        ar.Bodies.Add(Bdy);
    }


}
