using UnityEngine;
using System.Collections.Generic;

public class Recipe : MonoBehaviour {

    [System.Serializable]
    public struct Requirement {
        public int Amnt;
        public Recipe R;
    };
    public List<Requirement> Requirements;
    public GameObject Fab;


    public enum Type_E {
        Ore,
        Metal,
        Component, 
        Ship,
        Facility,
    }
    public Type_E Type;

    public float MaterialisationCost = -1;
    public float MaterialisationComplex = -1;
}
