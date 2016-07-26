using UnityEngine;
using System.Collections;

public class OrbitCam : MonoBehaviour {

    public float D;
    public Vector2 RotateSpd = new Vector2(10,10);
    public float ZoomSpd = 10;

    void OnEnable() {
        D = Mathf.Sqrt( transform.position.magnitude );
    }

    void LateUpdate() {
        if(Input.GetMouseButton(2)) { //EventSystem.current.IsPointerOverGameObject()
            Vector3 target = Vector3.zero; //this is the center of the scene, you can use any point here
            var euler = transform.localEulerAngles + new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * Time.deltaTime, Input.GetAxis("Mouse X") * RotateSpd.y * Time.deltaTime);

            if(euler.x > 180.0f) euler.x -= 360.0f;
            if(Mathf.Abs(euler.x) > 80) euler.x = Mathf.Sign(euler.x) * 80;

            euler.z = 0;
            transform.localEulerAngles = euler;
            //transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * Time.deltaTime, Input.GetAxis("Mouse X") * RotateSpd.y * Time.deltaTime));


        }

        D += Input.GetAxis("Mouse ScrollWheel") * ZoomSpd * Time.deltaTime; ;

        transform.position = -transform.forward * D*D;
    }


}
