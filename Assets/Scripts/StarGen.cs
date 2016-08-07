using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.InteropServices;
//using System;

public class StarGen : MonoBehaviour {


    public GameObject OrbitLR, PlanetUI;

    public Sprite Unknown;
    public Sprite Rock;
    public Sprite Venusian;
    public Sprite Terrestrial;
    public Sprite GasGiant;
    public Sprite Martian;
    public Sprite Water;
    public Sprite Ice;
    public Sprite SubGasGiant;
    public Sprite SubSubGasGiant;
    public Sprite Asteroids;
    public Sprite OneFace;


    public GameObject Sphere;
    public GameObject Area;

#if UNITY_EDITOR
    public const string LibName = "plg_dbg";

    static StarGen _Singleton = null;
    public static StarGen Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<StarGen>();
            return _Singleton;
        }
        private set { } 
    }


    //  [DllImport(LibName)]
    // static extern void Core_drawGizmos([MarshalAs(UnmanagedType.FunctionPtr)] CB_DrawLine dl, [MarshalAs(UnmanagedType.FunctionPtr)] CB_DrawSphere ds);


    public bool Gen = false, Inc = false;
    public int Seed = 0;




    void OnDrawGizmos() {
        if(Inc) {
            Seed++;
            Gen = true;
            Inc = false;
        }
        if(Gen) {
            StarGen_gen( Seed, new CB_System(_CB_System), new CB_Planetoid(_CB_Planetoid));
            Gen = false;
        }
    }
#else
	public const string LibName = "space_cpp";
#endif

    void _CB_System(ref SunDat s) {
        Debug.Log("___star___");
        Debug.Log("luminosity  " + s.luminosity);
        Debug.Log("mass  " + s.mass);
        Debug.Log("life  " + s.life);
        Debug.Log("age  " + s.age);
        Debug.Log("r_ecosphere  " + s.r_ecosphere);
    }

    void _CB_Planetoid(ref PlanetDat p) {
        Debug.Log("___planet___");
        Debug.Log("a  " + p.a);
        Debug.Log("e  " + p.e);
        Debug.Log("axial_tilt  " + p.axial_tilt);
        Debug.Log("mass  " + p.mass);
        Debug.Log("gas_giant  " + p.gas_giant);
        Debug.Log("dust_mass  " + p.dust_mass);
        Debug.Log("gas_mass  " + p.gas_mass);
        Debug.Log("moon_a  " + p.moon_a);
        Debug.Log("moon_e  " + p.moon_e);
        Debug.Log("core_radius  " + p.core_radius);
        Debug.Log("radius  " + p.radius);
        Debug.Log("orbit_zone  " + p.orbit_zone);
        Debug.Log("density  " + p.density);
        Debug.Log("orb_period  " + p.orb_period);
        Debug.Log("day  " + p.day);
        Debug.Log("resonant_period  " + p.resonant_period);
        Debug.Log("esc_velocity  " + p.esc_velocity);
        Debug.Log("surf_accel  " + p.surf_accel);
        Debug.Log("surf_grav  " + p.surf_grav);
        Debug.Log("rms_velocity  " + p.rms_velocity);
        Debug.Log("molec_weight  " + p.molec_weight);
        Debug.Log("volatile_gas_inventory  " + p.volatile_gas_inventory);
        Debug.Log("surf_pressure  " + p.surf_pressure);
        Debug.Log("greenhouse_effect  " + p.greenhouse_effect);
        Debug.Log("boil_point  " + p.boil_point);
        Debug.Log("albedo  " + p.albedo);
        Debug.Log("exospheric_temp  " + p.exospheric_temp);
        Debug.Log("estimated_temp  " + p.estimated_temp);
        Debug.Log("estimated_terr_temp  " + p.estimated_terr_temp);
        Debug.Log("surf_temp  " + p.surf_temp);
        Debug.Log("greenhs_rise  " + p.greenhs_rise);
        Debug.Log("high_temp  " + p.high_temp);
        Debug.Log("low_temp  " + p.low_temp);
        Debug.Log("max_temp  " + p.max_temp);
        Debug.Log("min_temp  " + p.min_temp);
        Debug.Log("hydrosphere  " + p.hydrosphere);
        Debug.Log("cloud_cover  " + p.cloud_cover);
        Debug.Log("ice_cover  " + p.ice_cover);
        Debug.Log("gases  " + p.gases);
        Debug.Log("minor_moons  " + p.minor_moons);
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SunDat {
        public float luminosity;
        public float mass;
        public float life;
        public float age;
        public float r_ecosphere;
        //char		*name;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PlanetDat {
        public int planet_no;
        public float a;                    /* semi-major axis of solar orbit (in AU)*/
        public float e;                    /* eccentricity of solar orbit		 */
        public float axial_tilt;           /* units of degrees					 */
        public float mass;             /* mass (in solar masses)			 */
        public int gas_giant;          /* TRUE if the planet is a gas giant */
        public float dust_mass;            /* mass, ignoring gas				 */
        public float gas_mass;         /* mass, ignoring dust				 */
                                /*   ZEROES start here               */
        public float moon_a;               /* semi-major axis of lunar orbit (in AU)*/
        public float moon_e;               /* eccentricity of lunar orbit		 */
        public float core_radius;      /* radius of the rocky core (in km)	 */
        public float radius;               /* equatorial radius (in km)		 */
        public int orbit_zone;         /* the 'zone' of the planet			 */
        public float density;          /* density (in g/cc)				 */
        public float orb_period;           /* length of the local year (days)	 */
        public float day;              /* length of the local day (hours)	 */
        public int resonant_period;    /* TRUE if in resonant rotation		 */
        public float esc_velocity;     /* units of cm/sec					 */
        public float surf_accel;           /* units of cm/sec2					 */
        public float surf_grav;            /* units of Earth gravities			 */
        public float rms_velocity;     /* units of cm/sec					 */
        public float molec_weight;     /* smallest molecular weight retained*/
        public float volatile_gas_inventory;
        public float surf_pressure;        /* units of millibars (mb)			 */
        public int greenhouse_effect;  /* runaway greenhouse effect?		 */
        public float boil_point;           /* the boiling point of water (Kelvin)*/
        public float albedo;               /* albedo of the planet				 */
        public float exospheric_temp;  /* units of degrees Kelvin			 */
        public float estimated_temp;     /* quick non-iterative estimate (K)  */
        public float estimated_terr_temp;/* for terrestrial moons and the like*/
        public float surf_temp;            /* surface temperature in Kelvin	 */
        public float greenhs_rise;     /* Temperature rise due to greenhouse */
        public float high_temp;            /* Day-time temperature              */
        public float low_temp;         /* Night-time temperature			 */
        public float max_temp;         /* Summer/Day						 */
        public float min_temp;         /* Winter/Night						 */
        public float hydrosphere;      /* fraction of surface covered		 */
        public float cloud_cover;      /* fraction of surface covered		 */
        public float ice_cover;            /* fraction of surface covered		 */
                                    //sun*		sun;
        public int gases;              /* Count of gases in the atmosphere: */
                                       //gas*		atmosphere;
        public int type;				/* Type code						 */
        public int minor_moons;
        //planet_pointer first_moon;
        /*   ZEROES end here               */
        //planet_pointer next_planet;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CB_System(ref SunDat s);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CB_Planetoid(ref PlanetDat p);

    [DllImport(LibName)]
    public static extern void StarGen_gen( int i, [MarshalAs(UnmanagedType.FunctionPtr)] CB_System s, [MarshalAs(UnmanagedType.FunctionPtr)] CB_Planetoid p);




}
