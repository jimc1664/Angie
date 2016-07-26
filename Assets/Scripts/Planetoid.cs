using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class Planetoid : MonoBehaviour {


    public enum Planet_Type {
        Unknown,
        Rock,
        Venusian,
        Terrestrial,
        GasGiant,
        Martian,
        Water,
        Ice,
        SubGasGiant,
        SubSubGasGiant,
        Asteroids,
        OneFace
    };

    public Planet_Type PType;

    public GameObject Pui, Spr; 

    public void init(ref StarGen.PlanetDat p, GameObject pUI ) {

        planet_no = p.planet_no;
        a = p.a;
        e = p.e;
        axial_tilt = p.axial_tilt;
        mass = p.mass;
        gas_giant = p.gas_giant;
        dust_mass = p.dust_mass;
        gas_mass = p.gas_mass;
        moon_a = p.moon_a;
        moon_e = p.moon_e;
        core_radius = p.core_radius;
        radius = p.radius;
        orbit_zone = p.orbit_zone;
        density = p.density;
        orb_period = p.orb_period;
        day = p.day;
        resonant_period = p.resonant_period;
        esc_velocity = p.esc_velocity;
        surf_accel = p.surf_accel;
        surf_grav = p.surf_grav;
        rms_velocity = p.rms_velocity;
        molec_weight = p.molec_weight;
        volatile_gas_inventory = p.volatile_gas_inventory;
        surf_pressure = p.surf_pressure;
        greenhouse_effect = p.greenhouse_effect;
        boil_point = p.boil_point;
        albedo = p.albedo;
        exospheric_temp = p.exospheric_temp;
        estimated_temp = p.estimated_temp;
        estimated_terr_temp = p.estimated_terr_temp;
        surf_temp = p.surf_temp;
        greenhs_rise = p.greenhs_rise;
        high_temp = p.high_temp;
        low_temp = p.low_temp;
        max_temp = p.max_temp;
        min_temp = p.min_temp;
        hydrosphere = p.hydrosphere;
        cloud_cover = p.cloud_cover;
        ice_cover = p.ice_cover;
        gases = p.gases;
        type = p.type;
        minor_moons = p.minor_moons;


        PType = (Planet_Type)type;

        name = type_string(PType);

        Pui = pUI;
        float oa = a;

        if(planet_no < 0) {
            transform.localPosition = Vector3.right * oa;

        } else {
            oa = moon_a;
            transform.localPosition = Vector3.back * oa;
        }

        /*
        var go = new GameObject(); go.name = "Sprite";
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = type_sprite(PType);
        var st = go.transform;
        st.parent = transform;
        float scl = Mathf.Sqrt(radius / 6378.0f) * 30.0f / sr.sprite.rect.max.x;
        st.localScale = Vector3.one *scl;
        st.localPosition = new Vector3(0, Mathf.Sqrt(radius / 6378.0f) * 30.0f / 200.0f, 0);
        */
        initSpr();

        Instantiate(StarGen.Singleton.OrbitLR).GetComponent<OrbitPath>().init(transform.parent, oa );
    }


    void initSpr() {

        var pui = Instantiate(Pui);
        pui.AddComponent<Canvas>();
        pui.AddComponent<UnityEngine.UI.CanvasScaler>();
        pui.AddComponent<UnityEngine.UI.GraphicRaycaster>();//.ignoreReversedGraphics = false;
        Spr = pui;
        Debug.Log("Spr " + Spr);
        var img = pui.GetComponent<UnityEngine.UI.Image>();
       // img.sprite = type_sprite(PType);
        var st = img.transform;
        float scl = Mathf.Sqrt(radius / 6378.0f) * 30.0f / 100;
        st.localScale = Vector3.one * scl;
        st.parent = transform;
        st.localPosition = new Vector3(0, scl *0.5f, 0);

        PairedButton b1 = Pui.GetComponentInChildren<PairedButton>(), b2 = Spr.GetComponentInChildren<PairedButton>();
        b1.setOther(b2);
    }

    void Update() {
       // Debug.Log("Spr " + Spr);
        if(Spr == null)
            return;
            UnityEngine.UI.Button b1 = Pui.GetComponentInChildren<UnityEngine.UI.Button>(), b2 = Spr.GetComponentInChildren<UnityEngine.UI.Button>();
      //  Debug.Log("selected == " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
        //if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == b1.gameObject)
          //  Debug.Log("selected == " + UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
        //  if( b1.GetComponent< UnityEngine.UI.Selectable >().IsHigh
        var c = new Color(0, 0, 0, 0);

        
        var cb = b1.colors;
        cb.normalColor = c;


    //    b1.colors = cb;


    }
    void LateUpdate() {
        if(Spr == null) return;

        Spr.transform.rotation = Quaternion.LookRotation( Camera.main.transform.forward  );
    }

    public static string type_string( Planet_Type type) {
        string typeString = "Really Unknown";

        switch(type) {
            case Planet_Type.Unknown: typeString = "Unknown"; break;
            case Planet_Type.Rock: typeString = "Rock"; break;
            case Planet_Type.Venusian: typeString = "Venusian"; break;
            case Planet_Type.Terrestrial: typeString = "Terrestrial"; break;
            case Planet_Type.SubSubGasGiant: typeString = "GasDwarf"; break;
            case Planet_Type.SubGasGiant: typeString = "Sub-Jovian"; break;
            case Planet_Type.GasGiant: typeString = "Jovian"; break;
            case Planet_Type.Martian: typeString = "Martian"; break;
            case Planet_Type.Water: typeString = "Water"; break;
            case Planet_Type.Ice: typeString = "Ice"; break;
            case Planet_Type.Asteroids: typeString = "Asteroids"; break;
            case Planet_Type.OneFace: typeString = "1Face"; break;
        }
        return typeString;
    }

    public static Sprite type_sprite(Planet_Type type) {
        var sg = StarGen.Singleton;

        switch(type) {
            case Planet_Type.Unknown: return sg.Unknown;
            case Planet_Type.Rock: return sg.Rock;
            case Planet_Type.Venusian: return sg.Venusian;
            case Planet_Type.Terrestrial: return sg.Terrestrial;
            case Planet_Type.SubSubGasGiant: return sg.SubSubGasGiant;
            case Planet_Type.SubGasGiant: return sg.SubGasGiant;
            case Planet_Type.GasGiant: return sg.GasGiant;
            case Planet_Type.Martian: return sg.Martian;
            case Planet_Type.Water: return sg.Water;
            case Planet_Type.Ice: return sg.Ice;
            case Planet_Type.Asteroids: return sg.Asteroids;
            case Planet_Type.OneFace: return sg.OneFace;
        }
        //err
        return sg.Unknown;
    } 

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
}
