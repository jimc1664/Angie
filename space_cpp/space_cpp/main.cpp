#include "stdafx.h"

unsigned int Reference = 0;

/*
template<bool B, class T = void>
struct enable_if {};
 
template<class T>
struct enable_if<true, T> { typedef T type; };


Template1A struct fooT {


	template<class T > fooT( const T &i,  typename enable_if<0,T>::type* = 0 ) {

		i = a;
	}
	T1A a;
};


void foo() {

	float z = 0;

	fooT<int>  a( z );



}
*/


char IF::Buffer[IF::BuffSize];

extern "C" {

__declspec(dllexport) void* accquire( int bs ) {
	if( bs > IF::BuffSize ) {
		return 0;
	}	
	if( Reference++ ) return IF::Buffer;


	return IF::Buffer;
}

__declspec(dllexport) void release() {
	if( --Reference ) return;
}

}
