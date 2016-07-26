using UnityEngine;
using System.Collections;

// Require these components when using this script
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class BotControlScript : MonoBehaviour
{

    public CharacterMotor Body;


    public Transform Trnsfrm { get; private set; }
    void Awake() {
        Trnsfrm = transform;
    }

	[System.NonSerialized]					
	public float lookWeight;					// the amount to transition when using head look
	
	[System.NonSerialized]
	public Transform enemy;						// a transform to Lerp the camera to during head look
	
	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed
	public float lookSmoother = 3f;				// a smoothing setting for camera motion
	public bool useCurves;						// a setting for teaching purposes to show use of curves

	
	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	private CapsuleCollider col;					// a reference to the capsule collider of the character
	

	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
	static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
	static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
	static int fallState = Animator.StringToHash("Base Layer.Fall");
	static int rollState = Animator.StringToHash("Base Layer.Roll");
	static int waveState = Animator.StringToHash("Layer2.Wave");
	

	void Start ()
	{
		// initialising reference variables
		anim = GetComponent<Animator>();					  
		col = GetComponent<CapsuleCollider>();				
		//enemy = GameObject.Find("Enemy").transform;	
		if(anim.layerCount ==2)
			anim.SetLayerWeight(1, 1);


	}

    float SoftDis = 0;
	void FixedUpdate () {
        float h = 0;
        float v = 0;
        Time.timeScale = 1;// 0.25f;
        var fwd = Trnsfrm.forward;

        var velVec = Body.movement.velocity; velVec.y = 0;
        var vel = velVec.magnitude; velVec /= vel;
        if(Vector3.Dot(velVec, Body.Trnsfrm.forward) < -0.5) vel = -vel;

        v = vel;



        h = Vector3.Dot(velVec, Trnsfrm.right);// *1.5f;//, -1.0f, 1.0f);
        // h *= Mathf.Abs(h);

        float maxSpeed = 5.3f;

        if(v > 0.0f && v < maxSpeed *0.8f ) {
            
            if(Vector3.Dot(velVec, Trnsfrm.forward) < 0.1f) h += Mathf.Sign(h);

        }


        Vector3 vec = Body.Trnsfrm.position - Trnsfrm.position; //vec.y = 0;
        var dis = vec.magnitude;
        if(dis > 0.0001f) { 
            float limit = 0.35f;
            if(dis > limit) {
                vec *= limit/dis;
                GetComponent<Rigidbody>().MovePosition(Body.Trnsfrm.position - vec );
                dis = limit;
            }
            vec = Trnsfrm.InverseTransformDirection(vec);
            // if(Vector3.Dot(vec, fwd) < 0) dis = -dis;

            SoftDis = vec.z;//Mathf.Lerp(SoftDis, vec.z, 2.0f * Time.deltaTime);

            float mod =  0.1f * (1.0f + Mathf.Abs(v) * 10.0f);
            v += SoftDis *mod*2.0f;
            if(Mathf.Abs(h) < 0.5f) {
                if(v < 0) mod = mod * 3.0f;
                h += Mathf.Clamp( vec.x * mod * 0.25f, -0.5f, 0.5f ) *(0.5f-Mathf.Abs(h) )*2.0f;
            }           
            
        }


        if(v < 0) {
            maxSpeed = 1.3f;
        }
        animSpeed = 1.0f;
        v /= maxSpeed;

        anim.SetBool("FlipDir", false);
        if(v > 0.1f) {

           
            if(Vector3.Dot(fwd, velVec.normalized) < -0.5f) {
                anim.SetBool("FlipDir", true);
             //   v *= 2.0f;
            }
        }
        if(Mathf.Abs(v) > 1.0f) {
            animSpeed = Mathf.Abs(v);
            v = Mathf.Sign(v);

            
        }
        if(Mathf.Abs(v) > 0.1f) {
            Trnsfrm.rotation = Quaternion.Lerp(Trnsfrm.rotation, Quaternion.LookRotation(velVec * v), Time.deltaTime * (Mathf.Abs(v)-0.05f) * 2.0f);
        }
        float sl = 0.6f;
        if(Mathf.Abs(h) < sl) {
            //var ph = h;
            var a = 1.0f - Mathf.Abs(h) / sl; a *= a; a = 1 - a;
         //   h = h * a;

           // Debug.Log("ph " + ph + "  h " + h +"  a "+a);
        }
		anim.SetFloat("Speed", v);							
		anim.SetFloat("Direction", h); 						
		anim.speed = animSpeed;								

        float ang = 0;
        if(Mathf.Abs(v) < 0.1f) {
            ang = Body.Trnsfrm.localEulerAngles.y - Trnsfrm.localEulerAngles.y;

            if(ang > 180.0f) ang = ang - 360;
            else if(ang < -180.0f) ang = 360 + ang;
        }
      //  ang = 0;
       // Debug.Log("ang " + ang + "  dot " + Vector3.Dot(velVec, Body.Trnsfrm.forward) + "  softDis " + SoftDis + "  adis " + dis + "  spd " + v + "  h " + h);
        anim.SetFloat("IdleTurnAng", ang);

       
       
        if(Input.GetButtonDown("Fire1")) {
           
        }
       
        return;
        /*anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
		if(anim.layerCount ==2)		
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation
		
		
		// LOOK AT ENEMY
		
		// if we hold Alt..
		if(Input.GetButton("Fire2"))
		{
			// ...set a position to look at with the head, and use Lerp to smooth the look weight from animation to IK (see line 54)
			//anim.SetLookAtPosition(enemy.position);
			lookWeight = Mathf.Lerp(lookWeight,1f,Time.deltaTime*lookSmoother);
		}
		// else, return to using animation for the head by lerping back to 0 for look at weight
		else
		{
			lookWeight = Mathf.Lerp(lookWeight,0f,Time.deltaTime*lookSmoother);
		}
		
		// STANDARD JUMPING
		
		// if we are currently in a state called Locomotion (see line 25), then allow Jump input (Space) to set the Jump bool parameter in the Animator to true
		if (currentBaseState.nameHash == locoState)
		{
			if(Input.GetButtonDown("Jump"))
			{
				anim.SetBool("Jump", true);
			}
		}
		
		// if we are in the jumping state... 
		else if(currentBaseState.nameHash == jumpState)
		{
			//  ..and not still in transition..
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					// ..set the collider height to a float curve in the clip called ColliderHeight
					col.height = anim.GetFloat("ColliderHeight");
				
				// reset the Jump bool so we can jump again, and so that the state does not loop 
				anim.SetBool("Jump", false);
			}
			
			// Raycast down from the center of the character.. 
			Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				// ..if distance to the ground is more than 1.75, use Match Target
				if (hitInfo.distance > 1.75f)
				{
					
					// MatchTarget allows us to take over animation and smoothly transition our character towards a location - the hit point from the ray.
					// Here we're telling the Root of the character to only be influenced on the Y axis (MatchTargetWeightMask) and only occur between 0.35 and 0.5
					// of the timeline of our animation clip
					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		
		
		// JUMP DOWN AND ROLL 
		
		// if we are jumping down, set our Collider's Y position to the float curve from the animation clip - 
		// this is a slight lowering so that the collider hits the floor as the character extends his legs
		else if (currentBaseState.nameHash == jumpDownState)
		{
			col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
		}
		
		// if we are falling, set our Grounded boolean to true when our character's root 
		// position is less that 0.6, this allows us to transition from fall into roll and run
		// we then set the Collider's Height equal to the float curve from the animation clip
		else if (currentBaseState.nameHash == fallState)
		{
			col.height = anim.GetFloat("ColliderHeight");
		}
		
		// if we are in the roll state and not in transition, set Collider Height to the float curve from the animation clip 
		// this ensures we are in a short spherical capsule height during the roll, so we can smash through the lower
		// boxes, and then extends the collider as we come out of the roll
		// we also moderate the Y position of the collider using another of these curves on line 128
		else if (currentBaseState.nameHash == rollState)
		{
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					col.height = anim.GetFloat("ColliderHeight");
				
				col.center = new Vector3(0, anim.GetFloat("ColliderY"), 0);
				
			}
		}
		// IDLE
		
		// check if we are at idle, if so, let us Wave!
		else if (currentBaseState.nameHash == idleState)
		{
			if(Input.GetButtonUp("Jump"))
			{
				anim.SetBool("Wave", true);
			}
		}
		// if we enter the waving state, reset the bool to let us wave again in future
		if(layer2CurrentState.nameHash == waveState)
		{
			anim.SetBool("Wave", false);
		}*/
	}
}
