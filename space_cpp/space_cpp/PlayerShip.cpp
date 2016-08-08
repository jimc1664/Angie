#include "stdafx.h"

#include "PlayerShip.h"

#include <float.h>

#include "World.h"
#include "Bullet.h"


PlayerShip::PlayerShip( const Transform_S &t ) : Body(t) {

	printf( "PlayerShip ctor \n" );

	{ ControlDecl cd = { "DesRotation",		CT_Rotation, &DesRot };			Controls.push(cd); }
	{ ControlDecl cd = { "FwdThrottle",		CT_Scalar, &FwdThrottle };		Controls.push(cd); }
	{ ControlDecl cd = { "BckThrottle",		CT_Scalar, &BckThrottle };		Controls.push(cd); }
	{ ControlDecl cd = { "SpaceWingPwr",	CT_Scalar, &SpaceWingPwr };		Controls.push(cd); }
	{ ControlDecl cd = { "StrafeAxes",		CT_DoubleAxis, &StrafeAxes };	Controls.push(cd); }
	
	StrafeAxes = 0;
	StrafePower = FwdThrottle = BckThrottle= SpaceWingPwr = 0.f;

	RotInertia = Identity();


}
void PlayerShip::deinit() {
	printf( "PlayerShip::deinit \n" );
	 
	delete this;
}


quatF specLerp( quatF a, quatF b, const f32 &rate, const f32 &diff ) {

	a.normalise(); b.normalise();
	auto r = nLerp(a, b, rate/diff);

	for( int i = 3; i--; ) {
		r = sLerp(a, r, rate / acos(abs(dot(r, a)))).normalise();
	}
	return r;
}

/*

	void FU_Rotation() {

		if( rigidbody.angularVelocity.sqrMagnitude > 0.0001.f ) {
			
			Vector3 localAngularVelocity = transform.getInverseTransformDirection(rigidbody.angularVelocity);
			
			
			float mag = localAngularVelocity.magnitude;
			localAngularVelocity *= Mathf.Sqrt(mag) *0.3f / mag;
			var vel = Quaternion.Euler( localAngularVelocity  );
			//Debug.Log( "rigidbody.angularVelocity "+localAngularVelocity.magnitude );
			
			rotInertia *= vel;
			rigidbody.angularVelocity = new Vector3(0,0,0);
		}
		
		Transform camT = Camera.main.transform;

		
		if( Steer.LState ) {
			DesRot = camT.rotation; //Quaternion.Slerp( DesRot, camT.rotation, 0.2f );
		}
		
		var desTorque = Quaternion.getInverse( rigidbody.rotation ) *DesRot;
		
		float dot = Mathf.Abs( Quaternion.Dot( rigidbody.rotation, DesRot ) );
		float dot2 = Mathf.Abs( Quaternion.Dot( Quaternion.identity, rotInertia ) );
		float dot3 = Mathf.Abs( Quaternion.Dot( rotInertia, desTorque ) );
		
		
		float acc = 0.002f;
		float theta = Mathf.Acos( dot );  //desired amount of rotation
		float ang = Mathf.Acos( dot2 ); //current rotaional inertia
		float ang2 = Mathf.Acos( dot3 ); 
		
		float stepsToStop =  ang/acc; //(ang>0)? ang/acc : 1;
		
		//quad -  acc*t*t  + ang *t - theta;
		float disc = ang*ang - 4.0f* acc *(-theta ); //can not possibly be negative		
		float stepsToDesRot = (-ang + Mathf.Sqrt(disc))/(2.0f*acc);		
		
		if( stepsToStop <= 1.0f && stepsToDesRot <= 1.0f ) {
			rotInertia = desTorque;
		} else {
				
			if( stepsToStop >= stepsToDesRot ) {
				rotInertia = Quaternion.Slerp( rotInertia, Quaternion.identity,  1.0f/stepsToStop );	
			} else {
				rotInertia = Quaternion.Slerp( rotInertia, desTorque, acc/ang2  );	
			}
		
		}
		if( !float.IsNaN( rotInertia.w)   ) 
			rigidbody.MoveRotation( rigidbody.rotation * rotInertia );	
		else 
			rotInertia = Quaternion.identity;		
		//Debug.Log( "ang "+ ang+  "   stepsToStop "+ stepsToStop+" stepsToDesRot "+ stepsToDesRot );
	}
	
*/


float StepM = 0;
quatF NextRot = Identity(), ORot= Identity();

void PlayerShip::updateRotation() {
	
	//float step = 0.2f;

		 
	RBody->setAngularVelocity( btVector3(0,0,0) );

	auto r2 = RBody->getOrientation();
	quatF rot = RBody->getOrientation();
	/*auto lr = rot;
	if( (StepM+=step) < 0.9999f ) {

		rot = sLerp(ORot, NextRot, StepM);
	//	rot = ORot;
	} else {
		StepM = 0;*/
		//rot = NextRot;

		/*
		if( rigidbody.angularVelocity.sqrMagnitude > 0.0001.f ) {
			
			Vector3 localAngularVelocity = transform.getInverseTransformDirection(rigidbody.angularVelocity);
			
			
			float mag = localAngularVelocity.magnitude;
			localAngularVelocity *= Mathf.Sqrt(mag) *0.3f / mag;
			var vel = Quaternion.Euler( localAngularVelocity  );
			//Debug.Log( "rigidbody.angularVelocity "+localAngularVelocity.magnitude );
			
			rotInertia *= vel;
			rigidbody.angularVelocity = new Vector3(0,0,0);
		}
		
		Transform camT = Camera.main.transform;

		
		if( Steer.LState ) {
			DesRot = camT.rotation; //Quaternion.Slerp( DesRot, camT.rotation, 0.2f );
		}
	
	
		**************************************************************

		var desTorque = Quaternion.getInverse( rigidbody.rotation ) *DesRot;
	
		float dot = Mathf.Abs( Quaternion.Dot( rigidbody.rotation, DesRot ) );
		float dot2 = Mathf.Abs( Quaternion.Dot( Quaternion.identity, rotInertia ) );
		float dot3 = Mathf.Abs( Quaternion.Dot( rotInertia, desTorque ) );

		float acc = 0.002f;
		float theta = Mathf.Acos( dot );  //desired amount of rotation
		float ang = Mathf.Acos( dot2 ); //current rotaional inertia
		float ang2 = Mathf.Acos( dot3 ); 
		
		float stepsToStop =  ang/acc; //(ang>0)? ang/acc : 1;
		
		//quad -  acc*t*t  + ang *t - theta;
		float disc = ang*ang - 4.0f* acc *(-theta ); //can not possibly be negative		
		float stepsToDesRot = (-ang + Mathf.Sqrt(disc))/(2.0f*acc);		
		
		if( stepsToStop <= 1.0f && stepsToDesRot <= 1.0f ) {
			rotInertia = desTorque;
		} else {
				
			if( stepsToStop >= stepsToDesRot ) {
				rotInertia = Quaternion.Slerp( rotInertia, Quaternion.identity,  1.0f/stepsToStop );	
			} else {
				rotInertia = Quaternion.Slerp( rotInertia, desTorque, acc/ang2  );	
			}
		
		}
		if( !float.IsNaN( rotInertia.w)   ) 
			rigidbody.MoveRotation( rigidbody.rotation * rotInertia );	
		else 
			rotInertia = Quaternion.identity;		
		//Debug.Log( "ang "+ ang+  "   stepsToStop "+ stepsToStop+" stepsToDesRot "+ stepsToDesRot );
	
	
		*/

		//Rot.normalise();
		//rot = ORot = NextRot;

		auto desTorque = rot.getInverse() * DesRot;  
		desTorque.normalise();

		/*
		rot *= desTorque;  
		printf( "rot %f  %f  %f  %f -- desRot %f  %f  %f  %f -- desTorque %f  %f  %f  %f \n"
			, rot.x, rot.y, rot.z, rot.w
			, DesRot.x, DesRot.y, DesRot.z, DesRot.w
			, desTorque.x, desTorque.y, desTorque.z, desTorque.w );
		
		btTransform trans = RBody->getCenterOfMassTransform();
		trans.setRotation( rot.as<btQuaternion>() );
		RBody->proceedToTransform( trans );

		RotM = RBody->getCenterOfMassTransform().getBasis();
		RotM = rot;


		return; //*/

		float dot1 = abs( dot( rot, DesRot ) );
		float dot2 = abs( dot( RotInertia, Identity() ) );
		float dot3 = abs( dot( RotInertia, desTorque ) );


		float acc = 0.1625f * Wrld.TimeStep;
		 acc = 0.05f;
		 acc = 0.02f;
		float theta = acos( dot1 );  //desired amount of rotation
		float ang = acos( dot2 ); //current rotaional inertia
		float ang2 = acos( dot3 ); 
		
		float stepsToStop =  ceil(ang/acc);

		//quad -  acc*t*t  + ang *t - theta;
		float disc = ang*ang - 4.0f* acc *(-theta ); //can not possibly be negative		
		float stepsToDesRot = (-ang + sqrt(disc))/(2.0f*acc);		
	
		int actStepsToStop = 0, actStepsToStop2 = 0, actStepsToStop3 = 0, actStepsToRot = 0;

		f32 lAng = 0;
		/*{ quatF ri = RotInertia;
			for (;; actStepsToStop ++ ) {
				float dot2 = abs( dot( ri, Identity() ) );
				float ang = acos( dot2 ); //current rotaional inertia
				float stepsToStop =  ceil(ang/acc);
				printf("ang %f    change %f   steps %f \n", ang, abs(ang - lAng) , stepsToStop); lAng = ang;
				if(stepsToStop < 1.1f) break;
				ri = sLerp( ri, quatF::identity(), (stepsToStop-1.f)/stepsToStop );	
			}	
		}	
		{ quatF ri = RotInertia;
			for (;; actStepsToStop2 ++ ) {
				float dot2 = abs( dot( ri, Identity() ) );
				float ang = acos( dot2 ); //current rotaional inertia

				float stepsToStop =  ceil(ang/acc);
				printf("ang %f    change %f   steps %f \n", ang, abs(ang - lAng) , stepsToStop); lAng = ang;
				if(stepsToStop < 1.1f) break;
				ri = nLerp( ri, quatF::identity(), (stepsToStop-1.f)/stepsToStop );	
			}	
		}		*/
		/*{ quatF ri = RotInertia;
			for (;; actStepsToStop3 ++ ) {
				float dot2 = abs( dot( ri, Identity() ) );
				float ang = acos( dot2 ); //current rotaional inertia

				float stepsToStop =  ceil(ang/acc);


				f32 cosTheta = -2;

				if( stepsToStop >= 1.1f)  {
					ri = cLerp( ri, quatF::identity(),  acc );	

				//template <class f32> quat_T<f32> cLerp(const quat_T<f32> &a, const quat_T<f32> &c, const f32 &f32 ) { //sLerps at a constant speed towards c, ie f32 radians
				/*	const quat_T<f32> &a = ri; 
					const quat_T<f32> &c = quatF::identity(); 
					const f32 &t = acc;
					//f32 
						cosTheta = dot(a, c);

					quat_T<f32> b;
					if( cosTheta < 0 ) {
						b = -c;
						cosTheta = -cosTheta;
					} else
						b = c;

					if(cosTheta > 0.999999f) //return c;
						ri = c;
					else {
						f32 sinTheta = sqrt(((f32)1.0) - cosTheta*cosTheta);
						f32 theta = atan2(sinTheta, cosTheta);

						if (theta < t) //return c; 
							ri = c;
						else {

							f32 invSinTheta = (f32)1.0 / sinTheta;
							f32 d1 = sin(theta - t) *invSinTheta;
							f32 d2 = sin(t) *invSinTheta;
	

							//return
								ri = quat_T<f32>( d1*a.x + d2*b.x, d1*a.y + d2*b.y, d1*a.z + d2*b.z, d1*a.w + d2*b.w );
						}
					}* /

					ri.normalise();
				//}

				}
				printf("ang %f    change %f   steps %f  cosTheta  %f\n", ang, abs(ang - lAng) , stepsToStop, cosTheta); lAng = ang;

				if(stepsToStop < 1.1f) break;
			

			}	
		}	/ /* /
		{ quatF ri = RotInertia, rot = RBody->getOrientation();
			float lTheta = 0;

				auto desTorque = rot.getInverse() * DesRot;  
				desTorque.normalise();

				float dot1 = min( 0.99999999f, abs( dot( rot, DesRot ) ) );
				float theta = acos( dot1 );  //desired amount of rotation
				float dot2 = min( 0.99999999f, abs( dot( ri, Identity() ) ));
				float dot3 = min( 0.99999999f, abs( dot( ri, desTorque ) ));
				float ang = acos( dot2 ); //current rotaional inertia
				float ang2 = acos( dot3 ); 

			for (;; actStepsToRot ++ ) {
			 
				float disc = ang*ang - 4.0f* acc *(-theta ); //can not possibly be negative		
				float stepsToDesRot = (-ang + sqrt(disc))/(2.0f*acc);	
				if (stepsToDesRot < 1.1) break;
				if (actStepsToRot > 25) break;
				//ri = //sLerp( ri, desTorque, 1.f/stepsToDesRot );	
					 //cLerp( ri, desTorque, acc  );	
				//ri = desTorque;
				if (theta < ang ) break;
				printf("ang %f    change %f   steps %f     ang %f  ang2 %f  dot1 %f \n", theta, abs(theta - lTheta), stepsToDesRot, ang, ang2, dot1 ); 
			
				ang += acc;
				theta -= ang;
			
				lTheta = theta;
				//if (_finite(ri.w)) //todo = still needed?
					//rigidbody.MoveRotation( rigidbody.rotation * rotInertia );	
				//	rot *= ri;
				//else 
				//	break;
			}	
		}
		//printf(" stepsToStop  %f  stepsToDesRot %f  actStepsToStop  %i actStepsToRot %i \n", stepsToStop, stepsToDesRot, actStepsToStop, actStepsToRot);
		//printf(" stepsToStop  %f  actStepsToStop  %i actStepsToStop2 %i actStepsToStop3 %i \n", stepsToStop, actStepsToStop, actStepsToStop2, actStepsToStop3);
		printf(" stepsToDesRot  %f  actStepsToRot  %i \n", stepsToDesRot,actStepsToRot ); //*/
		//printf(" stepsToDesRot  %f  stepsToStop  %f \n", stepsToDesRot,stepsToStop ); 
		if( stepsToStop <= 1.0f && stepsToDesRot <= 1.0f ) {
			rot = DesRot;
			RotInertia =  quatF::identity();	
		} else {	
			//stepsToStop *= 0.2f;
			if( stepsToStop >= stepsToDesRot ) {
				RotInertia = //cLerp( RotInertia, quatF::identity(),  acc );	
					nLerp( RotInertia, quatF::identity(),  1.0f/stepsToStop );	
			} else {
				auto ri = RotInertia.normalise();

				RotInertia = cLerp( ri, desTorque, acc  ).normalise();
					//nLerp( RotInertia, desTorque, acc/ang2  );	

				//auto ri2 = nLerp(ri, desTorque, acc / ang2).normalise();
				//auto ri3 = specLerp(ri, desTorque, acc, ang2 ).normalise();

				//printf(" d1  %f  d2  %f   d3  %f  fot  %f\n", acos(dot(ri,RotInertia))/acc,acos(dot(ri,ri2))/acc,acos(dot(ri,ri3))/acc , dot3 ); 
			}	

			if( _finite( RotInertia.w) ) { //todo = still needed?
				//rigidbody.MoveRotation( rigidbody.rotation * rotInertia );
				RotInertia.normalise();
				rot *= RotInertia;
			} else 
				RotInertia =  quatF::identity();	
			//rot = DesRot;
		}

		/*NextRot = rot.normalise();
		rot = ORot;

		quatF n1 = NextRot, n2 = NextRot, n3 = NextRot, n4 = NextRot; 
		//n2 *= invSqrt3(dot(NextRot, NextRot));
		//n3 *= invSqrt2(dot(NextRot, NextRot));

		f32 l = sqrt(dot(NextRot, NextRot));

		n4.normalise();

		f32 l2 = invSqrt3(dot(NextRot, NextRot));
		n3.x *= l2;
		n3.y *= l2;
		n3.z *= l2;
		n3.w *= l2;

		l2 = invSqrt(dot(NextRot, NextRot));
		n1.x *= l2;
		n1.y *= l2;
		n1.z *= l2;
		n1.w *= l2;

		l2 = invSqrt2(dot(NextRot, NextRot));
		n2.x *= l2;
		n2.y *= l2;
		n2.z *= l2;
		n2.w *= l2;


		printf("dot2   %f  %f  %f  %f   %f \n", dot(n1, n1), dot(n2, n2), dot(n3, n3), dot(n4, n4), l);
	} */

	//f32 d1 = dot(rot, rot), d2 = r2.dot(r2);
	//printf("dot %f  %f \n", d1, d2 );
	btTransform trans = RBody->getCenterOfMassTransform();
	trans.setRotation( rot.as<btQuaternion>() );
	RBody->proceedToTransform( trans );

	RotM = RBody->getCenterOfMassTransform().getBasis();
	RotM = rot;
}

/*
	float sCurve_01( float x ) {  //0...1   = 0...1   (but curved, so as 0 and 1 are horizontal tangents)
		return 0.5f- Mathf.Cos( x*Mathf.PI)*0.5f;
	}	
	float sCurve( float x ) {  //x-mirrored sigmoid curve   
		return 1.f/(1.f+Mathf.Exp(x) );	
	}	
	void applyForce( Vector3 oldVel, ref Vector3 nVel, Vector3 force, float max  ) {
		

		//float max = 100f;
			
		//var ov = oldVel.magnitude; var ovd = oldVel/ov;
		var fv = force.magnitude; var fcd = force/fv;
		
		float dt = Vector3.Dot( oldVel, fcd );
		//fs(5x -0.642) *1.45  +0.05
		//ft3(x)	fs(5x +0.0513 ) *1.95  +0.05

		float pow = ( sCurve(  5f*(dt/max) -0.513f ) )*1.95f +0.05f;
		
		nVel += force*pow;
	}
	*/
float sCurve_01( const float &x ) {  //0...1   = 0...1   (but curved, so as 0 and 1 are horizontal tangents)
	return 0.5f- cos( x*PIf)*0.5f;
}	
float sCurve( const float &x ) {  //x-mirrored sigmoid curve   
	return 1.f/(1.f+exp(x) );	
}		
void PlayerShip::applyForce( const vec3f &oldVel, vec3f &nVel, const vec3f &force, const float &max  ) {

	//var ov = oldVel.magnitude; var ovd = oldVel/ov;
		
	float dt =dot( oldVel, force.getNormal() );
	//fs(5x -0.642) *1.45  +0.05
	//ft3(x)	fs(5x +0.0513 ) *1.95  +0.05

	float pow = ( sCurve(  5.f*(dt/max) -0.513f ) )*1.95f +0.05f;
		
	nVel += force*pow;
}
/*
	void aFixedUpdate() {

		FU_Rotation();
		
		float mod = Time.deltaTime;
		float forwardThrust = 50.0f  * mod;
		float reverseThrust = 25.0f  * mod;
		
		//bool spaceWings = true;
		
		var forward = rigidbody.transform.forward;
		//var force = new Vector3(0,0,0);
		
		var oldVel = rigidbody.velocity; var addVel = Vector3.zero;
		
		
		if( FwdThrottle > 0.0f ) {
			applyForce(oldVel,ref addVel, forward * FwdThrottle * forwardThrust, 150f );
		}
		if( BckThrottle > 0.0f ) {
			applyForce(oldVel,ref addVel, -forward * BckThrottle * reverseThrust, 100f );
		}
		
		float maxStrafe = 20f;
		StrafePower += Time.deltaTime * maxStrafe /1.f;
		if( StrafePower > maxStrafe ) StrafePower = maxStrafe;
		
		float vrt = Input.GetAxis( "Vertical" );
		float hrz = Input.GetAxis( "Horizontal" );
		float pwr = 0f;
		if( Mathf.Abs( vrt + hrz) > 0.01.f ) {
			Vector3 dir = (rigidbody.transform.up * vrt + rigidbody.transform.right * hrz).normalized;
			pwr = Mathf.Min( Mathf.Min( (new Vector2(vrt,hrz)).magnitude, 1.f ) * StrafePower*5f, 40f )*Time.deltaTime;
			if( pwr < StrafePower ) StrafePower -= pwr;
			else { 			pwr = StrafePower; StrafePower = 0f; }
			
			applyForce(oldVel,ref addVel,dir*pwr, 50f ); 
			
		}
		//Debug.Log( "strafe "+pwr+ "   "+StrafePower );
		
		
		 
		//var velocity = oldVel;


		if( SpaceWingPwr > 0.01.f ) {
			//float vel = rigidbody.velocity.magnitude;
			float sw_max = 50.0f *mod *SpaceWingPwr, sw_min = 1.0f *mod  * (0.5f+SpaceWingPwr*0.5f);
			float sw_res = 0.02f, sw_eff = 0.5f, sw_cutoffHigh = 1.f, sw_cutoffLow = 0f;
				
			var mag = oldVel.magnitude;					
			float dot = Vector3.Dot( forward, oldVel / mag ) , absDt = Mathf.Abs(dot);
			
			if( absDt < sw_cutoffHigh && absDt > sw_cutoffLow ) {
				var transversal = oldVel - forward * dot * mag;
				
					//linear
				// 1 = absDt(sw_cutoffHigh)*a +b    // 0 = absDt(sw_cutoffLow)*a +b
				// sw_cutoffHigh*a +b -1.f = sw_cutoffLow*a +b   // a  = 1.f/(sw_cutoffHigh - sw_cutoffLow)  // b = - sw_cutoffLow*a				
				float aMd = 1.f/(sw_cutoffHigh - sw_cutoffLow), bMd = - sw_cutoffLow*aMd;		
				float dirM = sCurve_01( absDt*aMd +bMd );
				
				var effect = transversal*sw_res *(dirM*0.9f+0.1.f);
				
				var effMag = effect.magnitude; 
				
				//Debug.Log( "effMag  "+effMag/mod );
				if( effMag > sw_min ) {
					effect *= 1.f/effMag;
					
					effMag -= sw_min;			
					if( effMag > sw_max )  effMag = sw_max;
					
					effMag = sCurve_01( effMag/sw_max )*sw_max;
					
					effect *= effMag;
					
					var av =addVel;
					
					applyForce(oldVel,ref addVel, -effect, 30f ); 
					
					float actEffect = (addVel-av).magnitude;
				
							
					float add = actEffect * sw_eff * dirM;
					if( dot > 0 ) {
						applyForce(oldVel,ref addVel, forward * add, 40f ); 
					} else {
						applyForce(oldVel,ref addVel, -forward * add, 40f ); 
					}					
				}
			}
			
		}
		if( float.IsNaN( addVel.x ) ) addVel = Vector3.zero; //todo -- what caused this???
		
		oldVel *= 1.f-Time.deltaTime*0.01.f;  // damp		
		oldVel *= 1.f / ( 1.f+addVel.magnitude *0.001.f );
		
		//Debug.Log( "vel  "+ (oldVel+addVel).magnitude  + "  acc "+ (addVel.magnitude)+"    strafe "+StrafePower );
		rigidbody.velocity = oldVel + addVel;
		
		//float vel = rigidbody.velocity.magnitude;
		
		//if( vel > lvel ) Debug.Log( "inc" );
		//if( vel < lvel ) Debug.Log( "dec" );
		//lvel = vel;
		//Debug.Log( "vel  " + rigidbody.velocity.magnitude + "     force  " + force.magnitude );
		//rigidbody.AddForce( force, ForceMode.VelocityChange );
		
	} */

void PlayerShip::updatePosition() {
	

	float mod = Wrld.TimeStep;
	float forwardThrust = 50.0f  * mod;
	float reverseThrust = 25.0f  * mod;
		
	const vec3f oldVel = RBody->getLinearVelocity();
	const vec3f forward = RotM.row[2];
	//printf( "fwd %f  %f  %f  \n" , forward.x, forward.y, forward.z );

	vec3f addVel(0.f);
			
	if( FwdThrottle > 0.0f ) {
		applyForce(oldVel, addVel, forward * FwdThrottle * forwardThrust, 150.f );
	}
	if( BckThrottle > 0.0f ) {
		applyForce(oldVel, addVel, -forward * BckThrottle * reverseThrust, 100.f );
	}

	float maxStrafe = 20.f;
	StrafePower += Wrld.TimeStep * maxStrafe /1.f;
	if( StrafePower > maxStrafe ) StrafePower = maxStrafe;
		
	//float pwr = 0f;
	if( StrafeAxes.sqrLeng() > 0.01f ) {
		vec3f dir = ( RotM.row[1] * StrafeAxes.y + RotM.row[0] * StrafeAxes.x).normalise();
		float pwr = min( min( StrafeAxes.leng(), 1.f ) * StrafePower*5.f, 40.f )*Wrld.TimeStep;
		if( pwr < StrafePower ) StrafePower -= pwr;
		else { 			pwr = StrafePower; StrafePower = 0.f; }
			
		/*
		printf("dir %f  %f  %f -- x %f  %f  %f -- y %f  %f  %f -- z %f  %f  %f  \n"
			, dir.x, dir.y, dir.z
			, RotM.x.x, RotM.x.y, RotM.x.z
			, RotM.y.x, RotM.y.y, RotM.y.z
			, RotM.z.x, RotM.z.y, RotM.z.z); */


		applyForce( oldVel, addVel, dir*pwr, 50.f ); 		
	}
	//Debug.Log( "strafe "+pwr+ "   "+StrafePower );
					 

	if( SpaceWingPwr > 0.01f ) {
		//float vel = rigidbody.velocity.magnitude;
		float sw_max = 50.0f *mod *SpaceWingPwr, sw_min = 1.0f *mod  * (0.5f+SpaceWingPwr*0.5f);
		float sw_res = 0.02f, sw_eff = 0.5f, sw_cutoffHigh = 1.f, sw_cutoffLow = 0.f;
				
		float mag = oldVel.leng();					
		float dt = dot( forward, oldVel / mag ) , absDt = abs(dt);
			
		if( absDt < sw_cutoffHigh && absDt > sw_cutoffLow ) {
			vec3f transversal = oldVel - forward * dt * mag;
				
				//linear
			// 1 = absDt(sw_cutoffHigh)*a +b    // 0 = absDt(sw_cutoffLow)*a +b
			// sw_cutoffHigh*a +b -1.f = sw_cutoffLow*a +b   // a  = 1.f/(sw_cutoffHigh - sw_cutoffLow)  // b = - sw_cutoffLow*a				
			float aMd = 1.f/(sw_cutoffHigh - sw_cutoffLow), bMd = - sw_cutoffLow*aMd;		
			float dirM = sCurve_01( absDt*aMd +bMd );
				
			vec3f effect = transversal*sw_res *(dirM*0.9f+0.1f);
				
			float effMag = effect.leng(); 
				
			//Debug.Log( "effMag  "+effMag/mod );
			if( effMag > sw_min ) {
				effect *= 1.f/effMag;
					
				effMag -= sw_min;			
				if( effMag > sw_max )  effMag = sw_max;
					
				effMag = sCurve_01( effMag/sw_max )*sw_max;
					
				effect *= effMag;
					
				vec3f av =addVel;
					
				applyForce(oldVel, addVel, -effect, 30.f ); 
					
				float actEffect = (addVel-av).leng();
				
							
				float add = actEffect * sw_eff * dirM;
				if( dt > 0 ) {
					applyForce(oldVel, addVel, forward * add, 40.f ); 
				} else {
					applyForce(oldVel, addVel, -forward * add, 40.f ); 
				}					
			}
		}
			
	}

	vec3f vel = oldVel;

	// dampening	
	vel *= 1.f-Wrld.TimeStep*0.01f;	
	vel /=  1.f+addVel.sqrLeng() *0.001f;
		
	vel += addVel;

	RBody->setLinearVelocity( vel.as<btVector3>() );
	RBody->setActivationState(ACTIVE_TAG);
	//Pos += Velocity * Wrld.TimeStep;
}

void PlayerShip::update() {
	//Pos.z += 0.01.f;
	updateRotation();
	updatePosition();
}


ptr PlayerShip::getControlOffset( const char * name, ControlType type ) {

	for( auto it = Controls.start(); it; it++ ) {
		if( strcmp( name, it->Name ) == 0 ) {
			if( it->Type == type ) return it->Offset;

			printf( " PlayerShip::getControlOffset  %s  -- incorrect type  given(%d)  act(%d) \n", name, type, it->Type );
			return 0;			
		}
	}
	printf( " PlayerShip::getControlOffset  %s  -- not found \n", name);
	return 0;
}

void* PlayerShip_init( Transform_S t ) { 
	try { 
		Assert( (void*)0 ==  (Body*)(PlayerShip*)0, "Error - inheritance offset not handled c# side" );
		sizet p= (sizet)Wrld.Bodies.add( new PlayerShip(t) )+1;
		printf("PlayerShip_init  %x \n", p);
		return (void*)p; 
	}
	catch( std::exception& e ) {
		printf( e.what() );
		printf( "\n" );
	}
	catch( ... ) {
		printf("unknown exception!\n");
	}
	return 0;
}
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
#include "../interface_PlayerShip.h"
}
