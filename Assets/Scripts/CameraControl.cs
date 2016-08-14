using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	
	public GameObject Parent;
	
	public float sensitivityX = 15f;
	public float sensitivityY = 15f;
	public float sensitivityZ = -5f;
		
	float CamDis = 20.0f;
	
	
	public class InputHelper {
		
		public InputHelper( string name, bool initToggle = false ) {
			
		}
		
		public bool proc( Vector3 mMove ) {
						
			bool cPress = Input.GetButton( Name );
			bool ret = Toggle;
			float sMag = mMove.sqrMagnitude;
			
			if( sMag < 0.001f ) {
				ret = false;			
			} else  if( cPress ) {
				Drag += sMag;
				ret = true;		
			}
			
			if( !cPress ) {	
				if( LPress ) {
					//Debug.Log( "Drag "+Drag );
				}
				if( LPress && Drag < 5f ) {
					Toggle = !Toggle;
                    Cursor.visible = !Toggle;
                    Cursor.lockState = Toggle ? CursorLockMode.Locked : CursorLockMode.None;
				}
				Drag = 0f;
			}
					
			LPress = cPress;
						
			return LState = ret;
		}
		
		public bool LState = false;
		
		string Name = name;		
		bool Toggle = initToggle, LPress = false;		
		
		float Drag = 0f;	
	}
	
	
	InputHelper CamRotate = new InputHelper( "Fire3" );
    //public InputHelper SteerTgl = new InputHelper( "SteerTgl", true );


    void OnEnable() {
        CamDis = (Parent.transform.position - transform.position).magnitude;

    }
    public float VerticalOblique = 0.1f;

    public Vector3 FlyInput;

	void LateUpdate () {

        //Camera.main.ResetProjectionMatrix();
        Matrix4x4 mat = Camera.main.projectionMatrix;
       // mat[0, 2] = horizObl;
        mat[1, 2] = VerticalOblique;
       // Camera.main.projectionMatrix = mat;

        var mMove = new Vector3( - Input.GetAxis("Mouse Y") * sensitivityY, Input.GetAxis("Mouse X") * sensitivityX, 0 );
			
		if( CamRotate.proc( mMove) ) {	
			transform.rotation *= Quaternion.Euler( mMove );	
		}

		CamDis += CamDis * Input.GetAxis("Mouse ScrollWheel")*sensitivityZ;
        CamDis = Mathf.Max(CamDis, 0.01f);

        FlyInput = transform.forward * Input.GetAxis("Vertical")  + transform.right * Input.GetAxis("Horizontal")  + transform.up * Input.GetAxis("JumpCrouch");


        float yMod = (Mathf.Pow( Mathf.Abs( Quaternion.Dot( Parent.transform.rotation,transform.rotation) ), 2.0f ))*0.15f;
        //  Debug.Log("ymod  " + yMod);
            yMod = 0.15f;
		transform.position = Parent.transform.position + transform.up*CamDis*yMod + (transform.forward ) *-CamDis;		
	}
	
}
