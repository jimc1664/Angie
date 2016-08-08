
#include <Gem\Math\Vec3.h>


extern "C" {
typedef __declspec(dllexport) struct SunDat {
	float	luminosity;
	float	mass;
	float life;
	float age;
	float r_ecosphere;
	//char		*name;
} _SunDat;



typedef __declspec(dllexport) struct PlanetDat {
	int			planet_no;
	float a;					/* semi-major axis of solar orbit (in AU)*/
	float e;					/* eccentricity of solar orbit		 */
	float	axial_tilt;			/* units of degrees					 */
	float mass;				/* mass (in solar masses)			 */
	int 		gas_giant;			/* TRUE if the planet is a gas giant */
	float	dust_mass;			/* mass, ignoring gas				 */
	float	gas_mass;			/* mass, ignoring dust				 */
									/*   ZEROES start here               */
	float moon_a;				/* semi-major axis of lunar orbit (in AU)*/
	float moon_e;				/* eccentricity of lunar orbit		 */
	float	core_radius;		/* radius of the rocky core (in km)	 */
	float radius;				/* equatorial radius (in km)		 */
	int 		orbit_zone;			/* the 'zone' of the planet			 */
	float density;			/* density (in g/cc)				 */
	float orb_period;			/* length of the local year (days)	 */
	float day;				/* length of the local day (hours)	 */
	int 		resonant_period;	/* TRUE if in resonant rotation		 */
	float	esc_velocity;		/* units of cm/sec					 */
	float	surf_accel;			/* units of cm/sec2					 */
	float	surf_grav;			/* units of Earth gravities			 */
	float	rms_velocity;		/* units of cm/sec					 */
	float molec_weight;		/* smallest molecular weight retained*/
	float	volatile_gas_inventory;
	float	surf_pressure;		/* units of millibars (mb)			 */
	int		 	greenhouse_effect;	/* runaway greenhouse effect?		 */
	float	boil_point;			/* the boiling point of water (Kelvin)*/
	float	albedo;				/* albedo of the planet				 */
	float	exospheric_temp;	/* units of degrees Kelvin			 */
	float estimated_temp;     /* quick non-iterative estimate (K)  */
	float estimated_terr_temp;/* for terrestrial moons and the like*/
	float	surf_temp;			/* surface temperature in Kelvin	 */
	float	greenhs_rise;		/* Temperature rise due to greenhouse */
	float high_temp;			/* Day-time temperature              */
	float low_temp;			/* Night-time temperature			 */
	float max_temp;			/* Summer/Day						 */
	float min_temp;			/* Winter/Night						 */
	float	hydrosphere;		/* fraction of surface covered		 */
	float	cloud_cover;		/* fraction of surface covered		 */
	float	ice_cover;			/* fraction of surface covered		 */
	//sun*		sun;
	int			gases;				/* Count of gases in the atmosphere: */
	//gas*		atmosphere;
	int type;				/* Type code						 */
	int			minor_moons;
	//planet_pointer first_moon;
									/*   ZEROES end here               */
	//planet_pointer next_planet;
} _PlanetDat;


typedef void ( __stdcall *CB_System ) ( const SunDat& s );
typedef void ( __stdcall *CB_Planetoid ) ( const PlanetDat& p );
}
