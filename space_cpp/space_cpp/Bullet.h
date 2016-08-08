#ifndef BULLET_H
#define BULLET_H

#include <Gem/Math/Quaternion.h>
#include <Gem/Math/vec3.h>
#include <Gem/Math/matrix3.h>

#include "btBulletDynamicsCommon.h"


template<> quatF Gem::convert<quatF,btQuaternion>( const btQuaternion &b ) {
	return quatF( b.x(), b.y(), b.z(), b.w() );
}
template<> btQuaternion Gem::convert<btQuaternion,quatF>( const quatF &b ) {
	return btQuaternion(b.x,b.y,b.z,b.w);
}
template<> struct Is_ValidConversion<quatF,btQuaternion> { typedef ptr Valid; };
template<> struct Is_ValidConversion<btQuaternion,quatF> { typedef ptr Valid; };

template<> vec3f Gem::convert<vec3f,btVector3>( const btVector3 &b ) {
	return vec3f(b.x(),b.y(),b.z());
}
template<> btVector3 Gem::convert<btVector3,vec3f>( const vec3f &b ) {
	return btVector3(b.x,b.y,b.z);
}
template<> struct Is_ValidConversion<vec3f,btVector3> { typedef ptr Valid; };
template<> struct Is_ValidConversion<btVector3,vec3f> { typedef ptr Valid; };



template<> mat3f Gem::convert<mat3f,btMatrix3x3>( const btMatrix3x3 &b ) {
	mat3f m;
	auto x = b.getRow(0);
	auto y = b.getRow(1);
	auto z = b.getRow(2);
	m.row[0] = vec3f(x.x(), x.y(), x.z() );
	m.row[1] = vec3f(y.x(), y.y(), y.z() );
	m.row[2] = vec3f(z.x(), z.y(), z.z() );
	return m;
}
template<> struct Is_ValidConversion<mat3f,btMatrix3x3> { typedef ptr Valid; };
//template<> struct Is_ValidConversion<btQuaternion,quatF> { typedef ptr Valid; };


#endif//BULLET_H