using UnityEngine;
using System.Collections;


public class Cam_Beh : MonoBehaviour {

    public Camera Cam;


    void Awake() {
        Cam = GetComponent<Camera>();
    }
}
public class OrbitCam : Cam_Beh {

    public float Dis;
    public Transform Target;
    public float D;
    public Vector2 RotateSpd = new Vector2(10,10);
    public float ZoomSpd = 10;


    Vector3 targetP() {
        Vector3 p = Vector3.zero;
        if(Target) p = Target.position;
        return p;
    }
    Quaternion CRot;
    public void setTarget(Transform t) {
        Target = t;
        Vector3 p = targetP();
        CRot = Quaternion.LookRotation( p - transform.position);
        D = Mathf.Sqrt((transform.position - p).magnitude);

    }

    void OnEnable() {
        Vector3 p = targetP();
        D = Mathf.Sqrt( (transform.position - p).magnitude );
        CRot = transform.rotation;

   
    }
    void Start() {

        UIMain.Singleton.SolCam = this;
    }
    
    void Update() {
        Vector3 p = targetP();

        if(Input.GetMouseButton(2)) { //EventSystem.current.IsPointerOverGameObject()
           // Vector3 target = Vector3.zero; //this is the center of the scene, you can use any point here
            var euler = CRot.eulerAngles + new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * Time.unscaledDeltaTime, Input.GetAxis("Mouse X") * RotateSpd.y * Time.unscaledDeltaTime);

            if(euler.x > 180.0f) euler.x -= 360.0f;
            if(Mathf.Abs(euler.x) > 80) euler.x = Mathf.Sign(euler.x) * 80;

            euler.z = 0;

            var nr = Quaternion.Euler(euler);
            Quaternion diff = (Quaternion.Inverse(CRot) * nr).normalised();
            //CRot = nr;
            CRot *= diff;
          //  CRot = CRot.normalised();
            transform.rotation *= diff;
            //transform.localEulerAngles = euler;
            //transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * RotateSpd.x * Time.unscaledDeltaTime, Input.GetAxis("Mouse X") * RotateSpd.y * Time.unscaledDeltaTime));            
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, CRot, Time.unscaledDeltaTime * 3.0f);
       // transform.rotation = CRot;
        D += Input.GetAxis("Mouse ScrollWheel") * ZoomSpd * Time.unscaledDeltaTime;

        transform.position = p - CRot* Vector3.forward * D*D;

        Dis = D * D;
    }


}
