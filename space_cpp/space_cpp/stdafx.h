#include "stdio.h"
#include "../common.h"

extern "C" {

#define DLL_INTERFACE(  returnType, className, funcName, paramsT, paramsV, paramsC )	\
	__declspec(dllexport) returnType className##_##funcName paramsT;
#include "../interface_main.h"
#undef DLL_INTERFACE
}


struct IF {
	static const int BuffSize = 256;
	static char Buffer[BuffSize];
}; 

