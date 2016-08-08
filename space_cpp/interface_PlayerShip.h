#ifndef DLL_INTERFACE
#error "define DLL_INTERFACE you fool!"
#endif

DLL_INTERFACE( void, PlayerShip, deinit, (ptr _this), (_this), ()  )

DLL_INTERFACE( ptr, PlayerShip, getControlOffset, (ptr _this,  const char * name, u32 type ), (_this, name, type), (name, (PlayerShip::ControlType)type)  )

