using UnityEngine;
using System.Collections.Generic;

public class Facility : MonoBehaviour {

    public Station.Facility_E Type;

    public SortedList<Recipe, int> Inventory = new SortedList<Recipe, int>();

    public List<string> _Inventory;


    void Update() {

//        if(_Inventory.Count != Inventory.Count) {
            _Inventory.Clear();
            foreach(var kvp in Inventory)
                _Inventory.Add(kvp.Key.name + "    x  " + kvp.Value);
  //      }
    //    _Inventory
    }


}
