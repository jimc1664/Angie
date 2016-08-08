#ifndef DLL_INTERFACE
#error "define DLL_INTERFACE you fool!"
#endif

DLL_INTERFACE( ptr, Body,		init, ( Transform_S t ), ( t ), _ )
DLL_INTERFACE( ptr, Body,		initMesh, ( Transform_S t, MonoArray_V3 *a ), ( t, a ), _ )

		
DLL_INTERFACE( ptr, PlayerShip, init, ( Transform_S t ), ( t ), _ )

DLL_INTERFACE( ptr, VoxelTest,		init, ( Transform_S t, VoxelTest * l, VoxelTest * d, VoxelTest * b, Gem::vec3f vel ), ( t,l,d,b, vel ), _ )


