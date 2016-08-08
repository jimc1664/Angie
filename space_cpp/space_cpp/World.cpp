#include "stdafx.h"

#include "World.h"

#include "Body.h"

#include "btBulletDynamicsCommon.h"

#ifdef x64
#ifdef DEBUG
#define BulletLib( a ) "..\\bullet\\lib\\"##a##"_vs2010_x64_debug.lib"
#else
#define BulletLib( a ) "..\\bullet\\lib\\"##a##"_vs2010_x64.lib"
#endif
#else
#ifdef DEBUG
#define BulletLib( a ) "..\\bullet\\lib\\"##a##"_vs2010_debug.lib"
#else
#define BulletLib( a ) "..\\bullet\\lib\\"##a##"_vs2010.lib"
#endif
#endif

#pragma comment( lib, BulletLib( "BulletCollision"  ) )
#pragma comment( lib, BulletLib( "BulletDynamics" ) )
#pragma comment( lib, BulletLib( "LinearMath" ))

World::World() {
	printf( "World ctor \n" );

	//TimeStep = 0.032f;
	TimeStep = 0.016666667f;
	TimeStep = 0.02f;
	TimeStep = 0.01f;
}

World::~World() {
		
}

void starGenTest();
int World::foo() {


	starGenTest();
	return 1337;
}

//called at start of unity frame
void World::update( const float &delta ) {
	static float t = 0;
	
	for( t+= delta; t > TimeStep; t -= TimeStep ) {
		for( auto it = UpdateList.start(); it; it++ ) {
			it->update();
		}
	}

	Bt.DynWorld->stepSimulation( delta, 9999, TimeStep );
}

PhysicsWorld::PhysicsWorld() {

	CollisionConfiguration = new btDefaultCollisionConfiguration();
	//m_collisionConfiguration->setConvexConvexMultipointIterations();

	///use the default collision dispatcher. For parallel processing you can use a diffent dispatcher (see Extras/BulletMultiThreaded)
	Dispatcher = new btCollisionDispatcher(CollisionConfiguration);

	Broadphase = new btDbvtBroadphase();

	///the default constraint solver. For parallel processing you can use a different solver (see Extras/BulletMultiThreaded)
	btSequentialImpulseConstraintSolver* sol = new btSequentialImpulseConstraintSolver;
	Solver = sol;

	DynWorld = new btDiscreteDynamicsWorld(Dispatcher,Broadphase,Solver,CollisionConfiguration);
	//DynamicsWorld->setDebugDrawer(&gDebugDraw);
	
	DynWorld->setGravity(btVector3(0,0,0));

	btContactSolverInfo& info = DynWorld->getSolverInfo();
	info.m_numIterations = 60;
		
	auto groundShp = new btBoxShape(btVector3(100.0f, 0.5f, 100.0f));
	
	//mat3f rm = t.Rot;

	//RBody->setLinearVelocity( (rm.z *10.f).as<btVector3>() );

	btTransform transform;
	transform.setIdentity();
	//transform.setOrigin( t.Pos.as<btVector3>() );
	//transform.setRotation( t.Rot.as<btQuaternion>() );


	btVector3 localInertia(0,0,0);
	//auto mass = 1.0f;
	groundShp->calculateLocalInertia(1,localInertia);

	//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
//	btDefaultMotionState* myMotionState = new btDefaultMotionState(transform);
	btRigidBody::btRigidBodyConstructionInfo rbInfo(0,0,groundShp,localInertia);
	rbInfo.m_startWorldTransform = transform;
	auto groundBdy = new btRigidBody(rbInfo);


	//add the body to the dynamics world
	Wrld.Bt.DynWorld->addRigidBody(groundBdy);
	///create a few basic rigid bodies
	//btBoxShape* groundShape = new btBoxShape(btVector3(btScalar(50.),btScalar(50.),btScalar(50.)));
}

PhysicsWorld::~PhysicsWorld() {

	delete DynWorld;
	delete Solver;
	delete Broadphase;
	delete Dispatcher;
	delete CollisionConfiguration;
}

Updateable::Updateable() {
	Wrld.UpdateList.add( this );
	
}
Updateable::~Updateable() {
	Wrld.UpdateList.detach( this );
}

World Wrld;    

extern "C" {
#define DLL_INTERFACE(  returnType, className, funcName, paramsT, paramsV, paramsC )	\
	__declspec(dllexport) returnType className##_##funcName paramsT { \
		return Wrld.funcName paramsC ; \
	}
#include "../interface_World.h"
}
