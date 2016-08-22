using UnityEngine;
using System.Collections;

public class FlyCam : Cam_Beh {

    // Use this for initialization
    void Start() {

    }
    public float Speed = 5;
    public Vector2 RotateSpd = new Vector2(10, 10);

    // Update is called once per frame
    void Update() {

        float delta = 0;
        //if(Time.timeScale > Mathf.Epsilon)
        //    delta = Time.deltaTime / Time.unscaledDeltaTime;

        delta = Time.unscaledDeltaTime;

        float spd = delta * Speed;

        var Trnsfrm = transform;
        Trnsfrm.position += Trnsfrm.forward * Input.GetAxis("Vertical") * spd + Trnsfrm.right * Input.GetAxis("Horizontal") * spd + Trnsfrm.up *Input.GetAxis("JumpCrouch") * spd;


        if(Input.GetMouseButton(2)) { //EventSystem.current.IsPointerOverGameObject()

            var euler = transform.localEulerAngles + new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * delta, Input.GetAxis("Mouse X") * RotateSpd.y * delta);

            if(euler.x > 180.0f) euler.x -= 360.0f;
            if(Mathf.Abs(euler.x) > 80) euler.x = Mathf.Sign(euler.x) * 80;

            euler.z = 0;
            transform.localEulerAngles = euler;
            //transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * Time.unscaledDeltaTime, Input.GetAxis("Mouse X") * RotateSpd.y * Time.unscaledDeltaTime));


        }
    }
}