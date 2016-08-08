using UnityEngine;
using System.Collections;

public struct SimpleRandom {



    //public SimpleRandom() { randomize(); }
    // SimpleRandom(Randomize ) { randomize(); }
    public SimpleRandom(uint seed) { Seed = seed; }
    public SimpleRandom( int seed) { Seed = (uint)seed; }

    public uint rand( uint scale = 32768 ) { return  rawRand()%scale; } //returns between 0 and scale-1, max scle = 32768
    public float randf() { return (float)((int)rand()) / 32767.0f; }  //returns between 0.0 and 1.0

    public void setSeed(uint s) { Seed = s; }
    public void setSeed(int s) { Seed = (uint)s; }
    public void randomize() { setSeed( Time.frameCount ); } //value returned by current makes a pretty good seed

    public uint rawRand() {
        Seed = Seed * 1103515245 + 12345;
        return (Seed / 65536);
    }
    uint Seed;

}
