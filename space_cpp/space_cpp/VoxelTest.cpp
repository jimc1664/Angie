#include "stdafx.h"

#include "World.h"
#include "Bullet.h"
#include "Body.h"

#include <Gem\Org\dlist.h>


class VoxelTest : public Body {
public:

	VoxelTest( const Transform_S &t, VoxelTest * l, VoxelTest * f, VoxelTest * b, const vec3f &vel  );
	//VoxelTest( const Transform_S &t, const MonoArray_V3 &verts );
	
	void deinit();
	~VoxelTest();

	template< class T, int Alignment> class Aligned { //todo -- __declspec(align(16)) 
	protected:
		typedef Aligned AlignHlpr;
	public:
		static T* create() {
			T* ret = (T*)_aligned_malloc(sizeof(T), Alignment);
			//static_cast<AccessViolator*>(ret)->ctor();
			new (ret)T(); 
			
		//	printf("alligned alloc %x  \n", (sizet)ret);
			return ret;


		}
		void destroy() {
			//static_cast<AccessViolator*>(static_cast<T*>(this))->ctor();
			auto t = static_cast<T*>(this);
			t->~T();
		//	printf("alligned free %x  \n", (sizet)static_cast<T*>(this));
			_aligned_free(t);
		}
	};

	__declspec(align(16)) struct Joint : Aligned<Joint, 16 >, Updateable, dListNode<Joint> {
	private: friend class AlignHlpr;
		Joint() {
			Assume(((unsigned long)&FB & 15) == 0 );
			memset(&FB, 0, sizeof(FB));

			
		}
		~Joint() {
			int a = 0;

		}
		vec3f Nrm, O1, O2;

		btQuaternion Rot;

		btVector3 Pos;
	public:


		void init() {
			//vec3f aaaa = Cnst->getFrameOffsetA().getOrigin();
			O1 = Cnst->getFrameOffsetA().getOrigin();
			O2 = Cnst->getFrameOffsetB().getOrigin();
			Nrm = (O1 - O2).normalise();
			Enabled = true;

			Rot = btQuaternion::getIdentity();
			Pos = Cnst->getFrameOffsetA().getOrigin();

		/*	float low = -SIMD_PI, high = -low;
			Cnst->setAngularLowerLimit(btVector3(low,low,low));


			Cnst->setAngularUpperLimit(btVector3(high,high,high));

			
			Cnst->setLimit(0, -SIMD_INFINITY, SIMD_INFINITY);
			Cnst->setLimit(1, -SIMD_INFINITY, SIMD_INFINITY);
			Cnst->setLimit(2, -SIMD_INFINITY, SIMD_INFINITY);
	*/
		}

		void destroy() { Aligned::destroy(); }
		btGeneric6DofConstraint *Cnst;

		bool Enabled;

		void update() override  {

			if(!Enabled) {
				/*Cnst->setEnabled(Enabled = true);	

				btTransform frameInA, frameInB;
				frameInA = btTransform::getIdentity();
				frameInB = btTransform::getIdentity();


				auto p1 = Cnst->getRigidBodyA().getCenterOfMassPosition(), p2 = Cnst->getRigidBodyB().getCenterOfMassPosition();
				auto diff = p1 - p2;

				auto t1 = Cnst->getRigidBodyA().getWorldTransform(), t2 = Cnst->getRigidBodyB().getWorldTransform();
				btQuaternion mid = t1.getRotation().slerp(t2.getRotation(), 0.5f ).normalize();

				 
				frameInB.setRotation(t1.getRotation() * mid.inverse() );
				frameInA.setRotation(t2.getRotation() * mid.inverse() );
				frameInA.setOrigin( O1.as<btVector3>() );
				frameInB.setOrigin( O2.as<btVector3>() );


				//frameInA = btTransform::getIdentity();
				//frameInB = 
				Cnst->setFrames(frameInA, frameInB);*/

				return;
			}

			static float max = 0.0f, maxt = 0; 
			//float imp = Cnst->getAppliedImpulse() ;
			 
			btVector3 imp = FB.m_appliedForceBodyA - FB.m_appliedForceBodyB;
		//	vec3f trq = (FB.m_appliedTorqueBodyA - FB.m_appliedTorqueBodyB);


			float trqLim = 1500.0f,  trqFlex = 1500.0f;

			trqLim = 7500; trqFlex = 19000;

			float t1 = FB.m_appliedTorqueBodyA.norm(),  t2 = FB.m_appliedTorqueBodyB.norm();
			float trq = t1 + t2;

			return;
			if( trq > trqFlex ) {		
				if( trq > trqLim ) {	

					auto aTrq = trq- trqLim ;

					aTrq /= -trq;

					Cnst->getRigidBodyA().applyTorque(FB.m_appliedTorqueBodyA * aTrq);
					Cnst->getRigidBodyB().applyTorque(FB.m_appliedTorqueBodyB * aTrq);
					Cnst->setEnabled(Enabled = false);
				} else {

					auto aTrq = trq- trqFlex ;

					aTrq /= -trq;

					aTrq *= 0.5f;

					Cnst->getRigidBodyA().applyTorque(FB.m_appliedTorqueBodyA * aTrq);
					Cnst->getRigidBodyB().applyTorque(FB.m_appliedTorqueBodyB * aTrq);
					//Cnst->setEnabled(Enabled = false);	

					btTransform frameInA, frameInB;
					frameInA = btTransform::getIdentity();
					frameInB = btTransform::getIdentity();



					auto t1 = Cnst->getRigidBodyA().getWorldTransform(), t2 = Cnst->getRigidBodyB().getWorldTransform();
					btQuaternion mid = t1.getRotation().slerp(t2.getRotation(), 0.5f ).normalize();

					auto diff = t1.getOrigin() - t2.getOrigin();

					auto r = t1.getRotation() * mid.inverse();
					frameInB.setRotation(r );
					frameInA.setRotation(r.inverse() );
					frameInA.setOrigin( diff *-0.5f );
					frameInB.setOrigin(diff *0.5f);

					frameInA.setOrigin( O1.as<btVector3>() );
					frameInB.setOrigin( O2.as<btVector3>() );

					//frameInA = btTransform::getIdentity();
					//frameInB = 
					Cnst->setFrames(frameInA, frameInB);
				}
			} 



			//if (abs(imp) > abs(max)) max = imp;
			//if (abs(trq) > abs(maxt)) maxt = trq;


//			printf("max %f    maxt %f   imp %f   trq %f    \n",max,maxt,imp,trq );
			//printf("a %f    \n",FB.m_appliedTorqueBodyA.dot( FB.m_appliedTorqueBodyB)) ;
		}
		btJointFeedback FB;
	};
	dList<Joint> Joints;
	//virtual void update2() {} 
	protected: 

	//void initRB( const Transform_S &t, const f32 &mass );

};


void fixRef( VoxelTest * &p ) {

	if( p ) 
		if( VoxelTest* r = dynamic_cast<VoxelTest*>(Wrld.Bodies.sf_get( (u32)(sizet)p -1) ) ) {
			p = r;
		} else {
			printf( "Err VoxelTest ::fixRef  Not valid object  %i \n",(sizet)(p )-1  );					\
		}
}

class RigidBody : public btRigidBody {
public:
	RigidBody( btRigidBody::btRigidBodyConstructionInfo & a ) : btRigidBody(a) {
	
		
	}

	bool checkExceptions( const RigidBody* co ) const {
		return !Exceptions.contains( const_cast<RigidBody*>( co)) ;
	}

	bool checkCollideWithOverride(const btCollisionObject* co ) const override {

		const RigidBody * rb = static_cast<const RigidBody*>(co);
		if ((sizet)this > (sizet)rb)
			return checkExceptions(rb);
		else
			return  rb->checkExceptions(this);
	}

	void addException( RigidBody *rb  ) {
		m_checkCollideWith = rb->m_checkCollideWith =  1;
		if ((sizet)this > (sizet)rb)
			Exceptions.add(rb); 
		else
			rb->Exceptions.add(this);
	}

	ary< RigidBody* > Exceptions;
};



VoxelTest::VoxelTest( const Transform_S &t, VoxelTest * l, VoxelTest * d, VoxelTest * b, const vec3f &vel )  {
	printf( " VoxelTest ctor %d \n",Wrld.Bodies_Ind+1 );


	fixRef(l);fixRef(d);fixRef(b);
	 
	//Shape = new btBoxShape(btVector3(0.5f, 0.5f, 0.5f));
	Shape = new btSphereShape(0.45f);

	//mat3f rm = t.Rot;

	//RBody->setLinearVelocity( (rm.z *10.f).as<btVector3>() );

	btTransform transform;
	transform.setIdentity();
	transform.setOrigin( t.Pos.as<btVector3>() );
	transform.setRotation( t.Rot.as<btQuaternion>() );

	//We can also use DemoApplication::localCreateRigidBody, but for clarity it is provided here:
	
	btVector3 localInertia(0,0,0);
	auto mass = 1.0f;
	Shape->calculateLocalInertia(mass,localInertia);

	//using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
//	btDefaultMotionState* myMotionState = new btDefaultMotionState(transform);
	btRigidBody::btRigidBodyConstructionInfo rbInfo(mass,0,Shape,localInertia);
	rbInfo.m_startWorldTransform = transform;
	RBody = new RigidBody(rbInfo);

	RBody->setLinearVelocity( vel.as<btVector3>() );

	//j->setParam(BT_CONSTRAINT_STOP_CFM, myCFMvalue, index)



	Wrld.Bt.DynWorld->addRigidBody(RBody);

	auto sub = [&]( VoxelTest* o ) {
		if (!o ) return;
		//return;
		btTransform frameInA, frameInB;
		frameInA = btTransform::getIdentity();
		frameInB = btTransform::getIdentity();


		auto p1 = RBody->getCenterOfMassPosition(), p2 = o->RBody->getCenterOfMassPosition();
		auto diff = p1 - p2;

		frameInA.setOrigin( diff *-0.5f );
		frameInB.setOrigin(diff *0.5f);

		auto joint = new btGeneric6DofConstraint(*RBody, *o->RBody, frameInA, frameInB, true);

		joint->setAngularLowerLimit(btVector3(0,0,0));
		joint->setAngularUpperLimit(btVector3(0,0,0));
		
		for( int i = 5; i--; ) 
			joint->setParam(BT_CONSTRAINT_STOP_ERP, 0.1f, i);

		//joint->set
		auto j = Joint::create();

		j->Cnst = joint;
		j->init();
		joint->setJointFeedback(&j->FB);
		Joints.add(j);
		joint->enableFeedback(true);

		//RBody->checkCollideWithOverride

		//((RigidBody*)RBody)->addException(((RigidBody*)o->RBody));

		Wrld.Bt.DynWorld->addConstraint(joint);
	};
	sub(l);
	sub(d);
	sub(b);
}



VoxelTest::~VoxelTest() {
	Joints.deleteAll();
}
void VoxelTest::deinit() {
	printf( "Body_deinit \n" );	
	delete this;
}


void* VoxelTest_init( Transform_S t, VoxelTest * l, VoxelTest * d, VoxelTest * b,  vec3f vel ) {
	return (void*)(sizet)(Wrld.Bodies_Ind=1+Wrld.Bodies.add( new VoxelTest(t,l,d,b,vel) )); }


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
#include "../interface_VoxelTest.h"
}

