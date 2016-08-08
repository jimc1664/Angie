#ifndef DLL_INTERFACE
#error "define DLL_INTERFACE you fool!"
#endif

DLL_INTERFACE( int, World, foo, (), (), ()  )
DLL_INTERFACE( void, World, update, ( float delta ), ( delta ), ( delta )  )
