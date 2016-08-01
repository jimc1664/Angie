using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Spawnables : MonoBehaviour {

    public GameObject MiningShip, Buzzer, FighterDrn;


    public List<Recipe> Recipes; 

    static Spawnables _Singleton = null;
    public static Spawnables Singleton {
        get {
            if(_Singleton == null)
                _Singleton = FindObjectOfType<Spawnables>();
            return _Singleton;
        }
        private set { }
    }


    void OnDrawGizmos() {

    }
}
