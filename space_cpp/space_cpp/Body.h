#ifndef BODY_H
#define BODY_H


class btCollisionShape; class btRigidBody; 

class Body {
public:

	Body( const Transform_S &t );
	Body( const Transform_S &t, const MonoArray_V3 &verts );
	void deinit();
	

	Transform_S getTransform( );

	virtual void update2() {} 
//protected: 
	Body() {}

	void initRB( const Transform_S &t, const f32 &mass );

	~Body();
	//vec3f Pos, Velocity; 
	//quatF Rot, RotInertia;


	btRigidBody *RBody;
	btCollisionShape *Shape;
};

#endif //BODY_H