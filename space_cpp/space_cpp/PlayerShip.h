#ifndef PLAYERSHIP_H
#define PLAYERSHIP_H

#include "Body.h"
#include <Gem/Org/Ary.h>
#include <Gem/Math/Matrix3.h>
#include <Gem/Math/Vec2.h>

#include "Updateable.h"

/*
	enum ControlType : int {
		Rotation = 1, 	//ie quaternion
		Axis,   		// -1 <-> 1
		DoubleAxis,   	// two perpendicular Axes
		Scaler,			// 0 - 1
	}
	[DllImport("plg_dbg")] static extern IntPtr PlayerShip_init( Transform_S trns ); 
	[DllImport("plg_dbg")] static extern void PlayerShip_deinit( IntPtr _this ); 
	[DllImport("plg_dbg")] static extern IntPtr PlayerShip_getControlOffset( IntPtr _this, string name, ControlType type ); 	
*/

class PlayerShip : public Body, public Updateable  {
public:

	enum ControlType {
		CT_Rotation = 1, 	//ie quaternion
		CT_Axis,   			// -1 <-> 1
		CT_DoubleAxis,   	// two perpendicular Axes
		CT_Scalar,			// 0 - 1

		__ForceInt = 0xffffffff,
	};

	PlayerShip( const Transform_S &t );
	void deinit();
	ptr getControlOffset( const char * name, ControlType type );


private:
	void update() override;
	void update2() override {}
	//Transform_S getTransform( );

	void updateRotation();
	void updatePosition();

	void applyForce( const vec3f &oldVel, vec3f &nVel, const vec3f &force, const float &max  );

	struct ControlDecl  {
		char * Name;
		ControlType Type;
		ptr Offset;
	};

	ary<ControlDecl,Ctor::SimpleZeroed> Controls;

	float StrafePower;

	float FwdThrottle, BckThrottle, SpaceWingPwr;
	vec2f StrafeAxes;

	quatF DesRot;

	quatF RotInertia;

	mat3f RotM; //cache matrix to get directions

};

#endif //PLAYERSHIP_H