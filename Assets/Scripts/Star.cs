using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

    public float luminosity;
    public float mass;
    public float life;
    public float age;
    public float r_ecosphere;
    //char		*name;

    public void init(ref StarGen.SunDat s) {

        luminosity  = s.luminosity;
        mass  = s.mass;
        life  = s.life;
        age  = s.age;
        r_ecosphere  = s.r_ecosphere;

        transform.localPosition = Vector3.zero;

        name = "A Sun";
    }

}
