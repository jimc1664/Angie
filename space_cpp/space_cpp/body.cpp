#include "stdafx.h"

#include "Body.h"


#include "World.h"
#include "Bullet.h"

int ShipI = 0;


Body::Body( const Transform_S &t )  {
	//printf( " Body ctor % d \n", sizeof(t) );
	printf( "Body_init \n" );

	if(  ShipI++ ) {

		Shape = new btSphereShape( btScalar(8.f) );	
		ShipI = 0;
	} else {
		//auto cs = new btCompoundShape();
		auto s = new btCapsuleShapeZ(12.0f, 70.0f);
		//btTransform tr = btTransform::getIdentity();
	//	tr.setOrigin(btVector3(0, -6.0f, 20.0f));
	//	cs->addChildShape(tr, s);
		Shape = s;
	}
	
	//mat3f rm = t.Rot;

	//RBody->setLinearVelocity( (rm.z *10.f).as<btVector3>() );

	initRB( t, 1.f );
}

Body::Body( const Transform_S &t, const MonoArray_V3 &verts  )  {
	//printf( " Body ctor % d \n", sizeof(t) );

	auto s = new btConvexHullShape( (const f32*) &verts.Data[0], verts.size(), sizeof( verts.Data[0] ) );
	Shape = s;
	

	//Shape = new btSphereShape( btScalar(8.f) );	
	//mat3f rm = t.Rot;

	//RBody->setLinearVelocity( (rm.z *10.f).as<btVector3>() );

	initRB( t, 50.f );
}

void Body::initRB( const Transform_S &t, const f32 &mass ) {


	btTransform transform;
	transform.setIdentity();
	transform.setOrigin( t.Pos.as<btVector3>() );
	transform.setRotation( t.Rot.as<btQuaternion>() );

	//We can also use DemoApplication::localCreateRigidBody, but for clarity it is provided here:
	
	btVector3 localInertia(0,0,0);

	Shape->calculateLocalInertia(mass,localInertia);

	//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
//	btDefaultMotionState* myMotionState = new btDefaultMotionState(transform);
	btRigidBody::btRigidBodyConstructionInfo rbInfo(mass,0,Shape,localInertia);
	rbInfo.m_startWorldTransform = transform;
	RBody = new btRigidBody(rbInfo);

	//add the body to the dynamics world
	Wrld.Bt.DynWorld->addRigidBody(RBody);

}

Body::~Body() {
	Wrld.Bodies.free( Wrld.Bodies_Ind );
	Wrld.Bt.DynWorld->removeRigidBody(RBody); RBody = null;
	delete Shape; Shape = null;
}

void Body::deinit() {
	printf( "Body_deinit \n" );	
	delete this;
}

Transform_S Body::getTransform( ) {
	Transform_S t;

	t.Pos = RBody->getCenterOfMassPosition(); 
	t.Rot = RBody->getOrientation();
	return t;
}	

void* Body_init( Transform_S t ) { return (void*)(sizet)(1+Wrld.Bodies.add( new Body(t) )); }
void* Body_initMesh( Transform_S t, MonoArray_V3 *a ) { return (void*)(sizet)(1+Wrld.Bodies.add( new Body(t, *a) )); }

extern "C" {
#define DLL_INTERFACE(  returnType, className, funcName, paramsT, paramsV, paramsC )	\
	__declspec(dllexport) returnType className##_##funcName paramsT {					\
		try { \
			if( className* p = dynamic_cast<className*>(Wrld.Bodies.sf_get( Wrld.Bodies_Ind=  (u32)(sizet)(_this )-1) ))	\
				return p->funcName paramsC ;												\
			printf( "Err "#className"::"#funcName"Not valid object  %i \n",(sizet)(_this )-1  );					\
		} \
		catch( std::exception& e ) {\
			printf( e.what() );\
			printf( "\n" );\
		}\
		catch( ... ) {\
			printf("unknown exception!\n");\
		}\
		return function_def_ret<returnType>(); \
	}
#include "../interface_Body.h"
}

