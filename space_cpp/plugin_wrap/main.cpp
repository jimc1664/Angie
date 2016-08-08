
#include <Windows.h>
#include <stdio.h>
#include <Gem\Basic.h>

#include "iostream"
#include "io.h"
#include "fcntl.h"

using namespace Gem;

void initConsole() {

	AllocConsole();	

	HANDLE handle_out = GetStdHandle(STD_OUTPUT_HANDLE);
	s32 hCrt = _open_osfhandle((long) handle_out, _O_TEXT);
	FILE* hf_out = _fdopen(hCrt, "w");
	setvbuf(hf_out, NULL, _IONBF, 1);
	*stdout = *hf_out;

	HANDLE handle_in = GetStdHandle(STD_INPUT_HANDLE);
	hCrt = _open_osfhandle((long) handle_in, _O_TEXT);
	FILE* hf_in = _fdopen(hCrt, "r");
	setvbuf(hf_in, NULL, _IONBF, 128);
	*stdin = *hf_in;

}


#include "..\config.h"
#include "..\interfaceTypes_starGen.h"

void function_def( char * s ) {
	int a = 0;
	printf( s );  
}



/*
void def_foo2( int ) { return (void)0; }
typedef void (*foo2) (int);
foo2 foo2_dec;
*/

#define DLL_INTERFACE( returnType, className, funcName, paramsT, paramsV, paramsC )	\
	returnType _##className##_##funcName##_Def paramsT	{				\
		function_def( "Err: - "#className "_" #funcName" not loaded.." );  \
		return function_def_ret<returnType>();							\
	}													\
	typedef returnType (*_##className##_##funcName##_FuncT) paramsT;	\
	_##className##_##funcName##_FuncT _##className##_##funcName##_Dec = _##className##_##funcName##_Def;
#include "../interface.h"



extern "C" {


/*	 __declspec(dllexport) int foo() {
		return 1337;
	}
*/

#define DLL_INTERFACE( returnType, className, funcName, paramsT, paramsV, paramsC )\
__declspec(dllexport) returnType __stdcall className##_##funcName paramsT {	\
	return _##className##_##funcName##_Dec paramsV; \
} 
#include "../interface.h"



typedef void* (*AccquireFnc) (int);
typedef void (*ReleaseFnc) ();

unsigned int Reference = 0;

HMODULE Lib = 0;
bool First = 1;

void * Buffer = 0;
int BuffSize;

__declspec(dllexport) void* __stdcall accquire( int bs ) {
	
	if( Reference++ ) return Buffer;

	if( First ) {
		initConsole();
		First = 0;
	}

	printf( "plugin wrap says hi (to the world) \n" );

	Lib = LoadLibrary( LIBFILE );

	if( !Lib ) {
		printf( "  Can't find "LIBFILE"\n" );  
		Reference = 0;
		return Buffer;
	}

	
	if( auto p = (AccquireFnc) GetProcAddress( Lib, "accquire" ) )  Buffer = p( BuffSize = bs);
	if( !Buffer ) {
		printf( "   Failed to initate \n" );  
		Reference = 0;
		FreeLibrary( Lib ); Lib = 0;
		return Buffer;
	}
	//foo2_dec = GetProcAddress( Lib, "foo" );
#define DLL_INTERFACE( returnType, className, funcName, paramsT, paramsV, paramsC )\
	if( FARPROC proc = GetProcAddress(Lib, #className"_"#funcName) )	\
		_##className##_##funcName##_Dec = (_##className##_##funcName##_FuncT) proc;
#include "../interface.h"
	
	return Buffer;
}

__declspec(dllexport) void __stdcall release() {
	if (Lib == null) return;
	if( --Reference ) return;
	printf( "plugin wrap released \n" );

#define DLL_INTERFACE( returnType, className, funcName, paramsT, paramsV, paramsC ) \
	_##className##_##funcName##_Dec = _##className##_##funcName##_Def;
#include "../interface.h"

	Buffer = 0;
	if( auto p = (ReleaseFnc) GetProcAddress( Lib, "release" ) )  p( );

	if( Lib ) {
		FreeLibrary( Lib ); Lib = 0;
	}
}


}

