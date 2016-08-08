#ifndef DLL_INTERFACE
#error "define DLL_INTERFACE you fool!"
#endif

DLL_INTERFACE( void, Body, deinit, (void* _this), (_this), ()  )
DLL_INTERFACE( Transform_S, Body, getTransform, (void* _this), (_this), (  )  )


