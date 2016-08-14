using UnityEngine;
using System.Collections;

public class TargetCam : MonoBehaviour {

    Drone Target;
    PlayerShipCtrlr Plyr;

    void Awake() {
        Plyr = FindObjectOfType<PlayerShipCtrlr>();
    }
    public void setTarget(Drone d) {
        Target = d;

        var cm = Plyr.transform;
        var tp = Target.transform.position;
        var vec = tp - cm.position;

        transform.position = tp - vec * Target.Rad * 1.5f;
    }

    void LateUpdate() {
        if(Target == null) {
            return;
        }

        var cm = Plyr.transform;
        var tp = Target.transform.position;
        var vec = tp - (transform.position +cm.position)*0.5f;

        transform.rotation = Quaternion.LookRotation( vec, cm.up  );

        transform.position = Vector3.Lerp(transform.position, tp - transform.forward * Target.Rad * 2.5f , 25.0f *Time.deltaTime );
    }
}
