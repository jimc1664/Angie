#ifndef WORLD_H
#define WORLD_H

#include <Gem\Org\ObjRefAry.h>

#include "Updateable.h"

class Body;

class btBroadphaseInterface;
class btCollisionShape;
class btOverlappingPairCache;
class btCollisionDispatcher;
class btConstraintSolver;
struct btCollisionAlgorithmCreateFunc;
class btDefaultCollisionConfiguration;
class btDiscreteDynamicsWorld;

struct PhysicsWorld {
	//btAlignedObjectArray<btCollisionShape*>	m_collisionShapes;
	PhysicsWorld();
	~PhysicsWorld();

	btBroadphaseInterface			*Broadphase;
	btCollisionDispatcher			*Dispatcher;
	btConstraintSolver				*Solver;
	btDefaultCollisionConfiguration	*CollisionConfiguration;
	btDiscreteDynamicsWorld			*DynWorld;
};

class World {
public:
	World();
	~World();
	int foo();

	void update( const float &delta );

	PhysicsWorld Bt;

	f32 TimeStep;
//private:
	
	dList<Updateable> UpdateList;
	ObjRefAry<Body*> Bodies;

	u32 Bodies_Ind; //single thread interface so can pass via global.. else use tls
						//.. or pass properly with manual className##deinit

};
extern World Wrld;

#endif //WORLD_H