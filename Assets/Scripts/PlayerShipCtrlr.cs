using UnityEngine;
using System.Collections.Generic;

public class PlayerShipCtrlr : Drone {

    public CameraControl CamCntrl;
    public ShipHud UI;


    public override void init() {
        base.init();
        Drn.initBehavior = Sim.Drone.BehaviorT.Player;
    }

    new void OnEnable() {
        base.OnEnable();
        CamCntrl = FindObjectOfType<CameraControl>();
    }


    public void onAreaChange( Sim.Area old) {
        if(UI) {

            UI.WarpButton.set(warpCallback, Drn.Ar != null);
        }
        

    }
    List<PopupButton.Entry> warpCallback() {
        var ret = new System.Collections.Generic.List<PopupButton.Entry>();
        var _this = this;
        if(Drn.Ar == null) return ret;
        if( Drn.Ar.St != null && Drn.Ar.St.Ai == Drn.Ai) {

            Debug.Log("wcb 1");
            foreach(var a in Drn.Ai.TargetAreas) {
                var ar = a.Ar;
                ret.Add(new PopupButton.Entry() {
                    Name = a.Ar.name,                 
                    Callback = () => {
                        if(_this == null) return;
                        Drn.setWarpTarget(ar);
                    }
                });
                Debug.Log(" set cb " + a.Ar + "    n " + a.Ar.name);
            }
        } else {
            Debug.Log("wcb 2");
            foreach(var s in Drn.AreaD.StationsInRange) {
                ret.Add(new PopupButton.Entry() {
                    Name = s.name,
                    Callback = () => {
                        if(_this == null) return;
                        Drn.setWarpTarget(s.Ar);
                    }
                });
            }
        }
        return ret;
    }


}
