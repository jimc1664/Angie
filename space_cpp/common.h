

#include "interfaceTypes.h"

#include <Gem\Basic.h>

using namespace Gem;

Template1 inline T function_def_ret() { u8 a[sizeof(T)] = { 0 }; return *(T*)a; } 
template<> inline void function_def_ret() { return; }
 