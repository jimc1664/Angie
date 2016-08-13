using UnityEngine;
using System.Collections.Generic;

public class ShipHud : MonoBehaviour {

    public PopupButton WarpButton;

    public PlayerShipCtrlr Plyr;

    void OnEnable() {
        Plyr = FindObjectOfType<PlayerShipCtrlr>();
        Plyr.UI = this;
    }

    // Update is called once per frame
    void Update () {
	
	}
}
