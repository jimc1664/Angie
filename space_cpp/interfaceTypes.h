
#include <Gem\Math\Vec3.h>
#include <Gem\Math\Quaternion.h>

extern "C" {
typedef __declspec(dllexport) struct Transform_S {
	Gem::vec3f Pos;
	Gem::quatF Rot;	
} _Transform_S;


};


Template1 struct MonoArray {
public:
	const Gem::u32& size() const { return *(((Gem::u32*)this)-1); }
	T Data[1];
};

typedef MonoArray<Gem::vec3f> MonoArray_V3;


class PlayerShip; class Body; class VoxelTest;