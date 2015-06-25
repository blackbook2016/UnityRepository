/////////////////////////////////////////////////////////////////////////////////
// 
//	ac_FPParkour.cs 1.0.5a
//	© GamersFrontier. All Rights Reserved.
//	https://twitter.com/thndstrm
//	http://www.gamersfrontier.my
//
//	description:	A class that allows a greater range of motion for parkour.
//					Note: Requires vp_FPController & ac_FPParkourEventHandler
//
//					Code re-used and modified with express permission
/////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(vp_FPController))]

public class ac_FPParkour : vp_Component 
{
	// anim
	public GameObject ParkourArmPrefab = null ;					// parkour arm prefab that will be seen during ledgegrab
	public AnimationClip ParkourArmClimbAnim = null;			// parkour's arm ledge climbing animation that will be played.
	public Vector3 ParkourArmPos = Vector3.zero;				

	// sounds
	protected vp_FootstepManager m_FootstepManager ;			// Can be used to cache the player vp_FootStepManager
	protected Terrain m_CurrentTerrain = null;					// caches the current terrain the controller is on
		
	// general
	protected CharacterController m_CharacterController = null;	// Can be used to cache the player CharacterController
	protected vp_FPController m_Controller = null; 				// Can be used to cache the players vp_FPController
	protected vp_FPCamera m_Camera = null; 						// Can be used to cache the players vp_FPCamera
	protected ac_FPParkourEventHandler m_Player = null; 		// Need this to access the above and the player.

	// dash
	public bool Dashing {get {return m_Dashing; } }
	public bool MotorDashDone { get {return m_MotorDashDone; } }
	public int CurrentDash {get {return m_CurrentDash; } }
	public float DashTimer {get {return m_DashTimer; } }
	
	public float DashCooldown = 1f;				
	public float MotorDashForce = 0.50f;		
	public int DashCount = 1;
	public float DashRecoverSpeed = 1.0f;
	public float DashSensitivtiy = 0.25f;
	protected bool m_MotorDashDone = true;
	protected int m_CurrentDash = 0; 
	protected float m_DashInputTimer = 0f;
	protected float m_DashTimer = 0f;
	protected bool m_Dashing = false;
	protected float m_CanDashAgain = 0f ;


	// walljump
	public string IgnoreWallJumpTag = "";						// tag that disallow Walljump
	public float WallJumpForwardForce = 0.05f;
	public float WallJumpForce = 0.1f;
	public float WallJumpUpForce = 0.3f;
	protected bool m_MotorWallJumpDone = true;
	protected bool m_WallJumpAble = false;
	protected Vector3 m_WallJumpHitNormal;						// contains info about the wall we collided

	// wallhang
	public string IgnoreWallHangTag = "";						// tag that disallow wallhang
	public bool IsWallHanging {get {return m_IsWallHanging; } }
	public float WallHangDuration = 3.0f;
	public float LosingGripStart = 0.75f;
	public float LosingGripGravity = 0.003f;
	protected float m_WallHangTimer = 0f;
	protected Vector3 m_WallHangHitNormal = Vector3.zero;
	protected bool m_CanWallHang = false;
	protected bool m_IsWallHanging = false;

	// doublejump
	public float DoubleJumpForwardForce = 0.15f ;
	public float DoubleJumpForce = 0.4f;
	public int DoubleJumpCount = 1;
	protected bool m_DoubleJumpAble = false;
	protected int m_currentDoubleJump = 1 ;
	protected bool m_MotorDoubleJumpDone = true;
	
	// groundslide
	public bool GroundSliding {get {return m_GroundSliding; } }
	public float MotorGroundSlideForce = 0.2f;
	public float MinBuildUp = 2.0f;
	public float MinSpeedToBuildup = 20.0f;
	public float GroundSlideSpeedMinimum = 2.0f;
	public float GroundSlideCooldown = 1.5f;
	protected float m_GroundSlideBuildup;
	protected float m_CanGroundslideAgain = 0f;
	protected Vector3 m_GroundSlideLookDirection = Vector3.zero;
	protected bool m_GroundSliding = false;
	
	// wallrun
	public string IgnoreWallRunTag = "";			// tag that disallow wallrun
	public Transform WallRunObject {get {return m_WallRunObject; } }
	public bool IsWallRunning {get { return m_IsWallRunning; } }
	public bool IsWallOnRight {get {return m_IsWallOnRight; } }
	public bool IsWallInFront {get {return m_IsWallInFront; } }

	public float WallRunGravity = 0.08f;
	public float WallRunTilt = 1.0f;
	public float WallRunDuration = 2.0f;
	public float WallRunUpForce = 0.1f;
	public float WallRunSpeedMinimum = 8.0f ;
	public float WallRunAgainTimeout = 1f;
	public float WallRunRange = 1.0f; 				// the distance to keep from the climbable while climbing
	public float WallRunDismountForce = 0.1f;		// force applied at end of wallrun
	public float WallAngle {get { return m_WallAngle; } }
	public bool WallRunInfinite = false;
	public bool WallRunAutoRotateYaw = false;		// if enabled, camera yaw will be rotated parallel of wallrun direction

	protected float m_WallAngle;
	protected float m_WallRunMaxSpeed = 0f;
	protected bool m_WallRunable = false ;
	protected bool m_IsWallOnRight = false;
	protected bool m_IsWallInFront = false;
	protected Transform m_WallRunObject;
	protected bool m_IsWallRunning = false;
	protected float m_WallRunTimer = 0f;
	protected float m_CanWallRunAgain = 0f ;
	protected Vector3 m_WallRunHitPoint = Vector3.zero;
	protected Vector3 m_WallRunHitNormal = Vector3.zero;
	protected Vector3 m_WallRunDismountDirection = Vector3.zero;
	protected Vector3 m_WallRunLookDirection = Vector3.zero;
	private bool autoYawed = false;
	
	// ledge grab
	public string IgnoreLedgeGrabTag = "";				// tag that disallow ledgegrab
	public bool LedgeGrabable { get {return m_LedgeGrabable; } }
	public bool IsLedgeGrabbing { get { return m_IsLedgeGrabbing; } }
	public bool IsHanging { get {return m_IsHanging; } }
	public Vector3 HangPosition {get {return m_HangPosition; } }
	public Vector3 WallNormal {get {return m_ClimbObjectNormal; } }
	public Vector3 ClimbObjectPosition {get {return m_ClimbObjectPosition; } }
	public Vector3 ClimbPositionOffset = Vector3.zero;	
	public float HeadOffset = 0.1f;						// how high the top-down raycast should start

	public float FirstPersonLedgeOffset = 0.4f;
	public float ThirdPersonLedgeOffset = 0.53f;
	public float PullUpSpeed = 0.08f;
	public float MinDistanceToVault =  1.0f;			// check if player can vault instead.
	public float MinDistanceToClimb = 10.0f ;			// check if player is within grabing distance
	public float DistancePlayerToTop ;					// gets the distance between player and the top of distance
	public float LedgeGrabRange = 1.5f;					// minimum ray casting range.
	public float LedgeGrabDuration = 0.5f;				// duration of the whole LedgeGrab coroutine
	public float ClimbAlignDuration = 0.4f;				// how fast to align to ledge
	public float ClimbDismountForce = 0.02f;			// additional force applied at end of climb

	protected bool m_LedgeGrabable = false;
	protected bool m_IsLedgeGrabbing = false;
	protected bool m_IsHanging = false;
	protected Vector3 m_HangPosOffset = Vector3.zero;
	protected Vector3 m_HangPosition = Vector3.zero;			// position to lerp to when hanging	
	protected Vector3 m_HangRotation = Vector3.zero;
	protected Vector3 m_ClimbObjectPosition = Vector3.zero;		// position to lerp to when climbing
	protected Vector3 m_ClimbObjectNormal = Vector3.zero;
	protected Vector3 m_CachedDirection = Vector3.zero; 		// cache the direction to keep proper distance from climbable
	protected Vector2 m_CachedRotation  = Vector2.zero;
	protected Vector3 m_LedgeLeft = Vector3.zero;
	protected Vector3 m_LedgeRight = Vector3.zero;
	protected Transform m_Platform = null;						// current rigidbody or object in the 'MovableObject' layer that we are standing on

	private bool shouldCrouch = false ;
	private bool isLiningUp = false;
	
	// physics
	float targetDamping = 0.15f;
	
	// misc
	public Vector3 LocalVelocity { get {return GetLocalVelocity(); } }
	protected int m_LastWeaponEquipped = 0;						// cache last weapon equipped

	// Use this for initialization
	protected override void  Awake () 
	{
		base.Awake();

		// initial setup
		m_Audio = GetComponent<AudioSource>();
		m_CharacterController = gameObject.GetComponent<CharacterController>();
		m_Controller = (vp_FPController)transform.root.GetComponentInChildren(typeof(vp_FPController));
		m_Player  = (ac_FPParkourEventHandler)transform.root.GetComponentInChildren(typeof(ac_FPParkourEventHandler));
		m_Camera = GetComponentInChildren<vp_FPCamera>();
		m_Transform = transform;
		m_FootstepManager = (vp_FootstepManager)transform.root.GetComponentInChildren(typeof(vp_FootstepManager));

		m_currentDoubleJump = DoubleJumpCount ;
		m_CanWallRunAgain = Time.time;
		m_CanDashAgain = Time.time;
		m_CanGroundslideAgain = Time.time;

		// spawn the parkour arm at world origin and hide it
		if(ParkourArmPrefab != null)
		{
			ParkourArmPrefab = (GameObject)Object.Instantiate(ParkourArmPrefab,Vector3.zero,Quaternion.identity);
			ParkourArmPrefab.SetActive(false);
		}


	}
	
	protected override void Start()
	{
		// setup the correct amount of dash count
		m_CurrentDash = DashCount ;

		// hacky way to setwallrun maxspeed from  preset
		m_Player.WallRun.Start();
		m_WallRunMaxSpeed = m_Controller.MotorAcceleration;
		m_Player.WallRun.Stop();

	}

	/// <summary>
	/// registers this component with the event handler (if any)
	/// </summary>
	protected override void OnEnable()
	{	

		if (m_Player != null)
			m_Player.Register(this);

		// add the wallrun footstep callback;
		m_Camera.BobStepCallback += WallRunFootstep;
	}
	
	/// <summary>
	/// unregisters this component from the event handler (if any)
	/// </summary>
	protected override void OnDisable()
	{

		// unregister this monobehaviour from the player event handler
		if (m_Player != null)
			m_Player.Unregister(this);	

		// remove the wallrun footstep callback
		m_Camera.BobStepCallback -= WallRunFootstep;
	}

	protected override void Update()
	{
		base.Update();

		// handles ledgegrab, walljump and doublejump input
		InputJump();
		
		// handles dash input
		InputDash();
		
		// handles ground slide input
		InputGroundSlide();
		
		// handles wallhang input
		InputWallHang();

	}

	protected override void FixedUpdate()
	{
		// handle ledgegrab raycast and collision check
		UpdateLedgeGrab();

		// handle wallrun raycast and motion during wallrunning
		UpdateWallrun();

		// apply a camera Z tilt when wallrun
		UpdateWallrunTilt();

		// limit groundsliding based on speed and duration
		UpdateGroundSliding();

		// update dash cooldown and dash regen
		UpdateDash();

		// handles a parkour hanging before transitioning to parkour climb
		UpdateLedgeHanging();

		// handles sticking to the wall
		UpdateWallHang();

//		UpdatePhysics();
	}
	
	/// <summary>
	/// Takes controllercollider hit and convert to raycast 
	/// before passing it over for wall parkour related activity
	/// as raycast is much more accurate then controllercolliderhit
	/// </summary>
	protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		RaycastHit raycastHit;

		Vector3 localpos = transform.root.position + new Vector3 (0,m_CharacterController.height*0.8f,0);

		Physics.Raycast( localpos , hit.point- localpos , out raycastHit, WallRunRange);

		// check if the surface allows wallrun.
		CheckWallRun(raycastHit);

		// check if the surface allows walljump and wallhang
		CheckWallJump(raycastHit);

		// check if the surface allows wallhang
		CheckWallHang(raycastHit);
	}

	protected virtual bool CanStart_Run()
	{
		// can only run from a forward 
		if (m_Player.InputMoveVector.Get().y <= 0)
			return false;
		// can't start running while crouching
		if (m_Player.Crouch.Active)
			return false;
		
		return true;
		
	}

	protected virtual bool CanStart_Crouch()
	{
		// can't start running while crouching
		if (m_Player.WallRun.Active)
			return false;
		
		return true;
		
	}
	
	protected virtual bool CanStart_WallRun()
	{
		if(WallRunDuration == 0)	
			return false;
		if(Time.time < m_CanWallRunAgain)	// wallrun is refreshing
			return false;
		if (m_Controller.Grounded)			// Can't wallrun on the ground
			return false;
		if (m_LedgeGrabable)				// If can ledge grab, should ledge grab instead
			return false;
		if (m_IsWallRunning )				// Already wallrunning		
			return false;
		if (!m_WallRunable)					// Surface is not wallrunable
			return false;
		if (m_Player.Velocity.Get().magnitude < WallRunSpeedMinimum) // Not fast not enough
			return false;
//		if (!m_Player.Run.Active)			// Must start from a Run
//			return false;
		if (m_Player.Crouch.Active)			// cannot wallrun while crouching
			return false;
		if (GetLocalVelocity().z < -1)		// cannot wallrun backwards
			return false;

		return true;
		
	}

	protected virtual bool CanStart_WallHang()
	{
		if(m_Controller.Grounded)			// still grounded
			return false;
		if(!m_CanWallHang)					// not near a wall
			return false;
		if(m_IsWallHanging)					// already wallhanging
			return false;

		return true;
	}

	protected virtual bool CanStart_GroundSlide()
	{
		if(m_GroundSlideBuildup < MinBuildUp)				// run distance too short
			return false;
		if(Time.time < m_CanGroundslideAgain)			// groundslide is still refreshing
			return false;
		if (vp_Input.GetAxisRaw("Vertical") != 1)		// Can only groundslide if going forward
			return false;
		if (!m_Controller.Grounded)						// Not on ground, cannot groundslide
			return false;
		if(!m_Player.Run.Active)						// Can only groundslide from a run
			return false;

		return true;
	}

	protected virtual bool CanStart_Dash()
	{
		// ran out of dash counter
		if (m_CurrentDash <= 0)
			return false;

		if (!m_MotorDashDone)
			return false;

		if (Time.time < m_CanDashAgain)
			return false;

		// if there's no movement input at all
		if (vp_Input.GetAxisRaw("Vertical") == 0 && vp_Input.GetAxisRaw("Horizontal") == 0)
			return false;

		if (m_IsHanging)
			return false;

		if (m_Dashing)
			return false;

		if (m_IsWallRunning)
			return false;

		if (m_IsLedgeGrabbing)
			return false;

		return true;
	}
	
	protected virtual bool CanStart_WallJump()
	{
		if (!m_Player.MotorJumpDone.Get())	// if we are holding jump
			return false;

		if (!m_WallJumpAble)				// can't wall jump without wall contact
			return false;

		if (m_WallJumpHitNormal == new Vector3 (0,0,0)) // we can't walljump from ground
			return false;

		if (m_LedgeGrabable)
			return false;

		if (m_Controller.Grounded)			// Debug.Log ("Still on ground");
			return false;
		return true;
	}
	
	protected virtual bool CanStart_DoubleJump()
	{

		if (m_WallJumpAble)					// check if we can walljump instead
			return false;

		if (!m_MotorDoubleJumpDone)			// Still double jumping
			return false;

		if (!m_DoubleJumpAble)				// Cannot doublejump
			return false;

		if (m_currentDoubleJump <= 0)			// Ran out of airjump counter
			return false;
	
		if (m_Controller.Grounded)			// Are we still on the ground
			return false;

		if (!m_Player.MotorJumpDone.Get())	// Still jumping
			return false;

		if (!m_MotorWallJumpDone)			// Still wall jumping
			return false;

		if (m_IsWallHanging)				// Still wall hanging
			return false;
		
		if (m_IsLedgeGrabbing)			// Still parkour climbing
			return false;

		if (m_IsWallRunning)				// Still wallrunning
			return false;

		return true;
		
	}

	/// <summary>
	/// Prevent automatically jumping upon loading from a wall jump or doublejump if player holds jump
	/// </summary>
	protected virtual bool CanStart_Jump()
	{
		if(!m_MotorDoubleJumpDone)
			return false;

		if(!m_MotorWallJumpDone)
			return false;

		return true;
	}
	
	protected virtual bool CanStart_LedgeGrab()
	{

		if(m_Controller.Grounded)			// can't ledgegrab if grounded
			return false;
		if(m_IsLedgeGrabbing)				// is already ledgegrabbing
		{
			m_Player.LedgeGrab.TryStop();
			return false;
		}
		if(m_LedgeGrabable == false)
			return false;

		return true;
		
	}

	/// <summary>
	/// Once WallRun is approved, this will setup all the variables to allow wallrunning
	/// </summary>
	protected virtual void OnStart_WallRun()
	{
		// setup the variables
		m_IsWallRunning = true;
		m_CanWallHang = true;
		m_WallJumpAble = true;

		// reset camera initial rotation ,otherwise the camera
		// will be rotated with the offset it had at spawn-time
		m_Camera.SetRotation(m_Camera.Transform.eulerAngles, false, true);

		m_Player.Jump.Stop();
		
		// stop any movement on our controller. 
		// Comment this off if you want to maintain the momentum when wallrunning
		m_Player.Stop.Send();

		// Gives a nice push upwards at start of wallrunning
		m_Controller.AddSoftForce(transform.up*WallRunUpForce,WallRunDuration*0.5f);

		m_CachedDirection = m_Transform.forward;
		
	}

	protected virtual void OnStart_WallHang()
	{
		// setup the variables
		m_CanWallHang = false;
		m_IsWallHanging = true;
		m_WallJumpAble = true;

		// stop wallrun
		m_Player.WallRun.Stop();

		// stop controller completely
		m_Controller.Stop();
	}

	protected virtual void OnStart_GroundSlide()
	{
		m_Player.Run.Stop();
		m_GroundSliding = true;

		// stores current look direction
		m_GroundSlideLookDirection = m_Controller.transform.forward;
		m_CachedRotation = m_Player.Rotation.Get ();

		// pushes player forward over a 10 frames
		m_Controller.AddSoftForce(transform.forward*MotorGroundSlideForce,10f);

	}

	protected virtual void OnStart_Dash()
	{
		// decrease available dashcount by one
		m_CurrentDash --;
		m_Dashing = true;
		m_MotorDashDone = false;

		// stores player inputMove vector as dashDirection
		Vector3 dashDirection = transform.TransformDirection(new Vector3 (m_Player.InputMoveVector.Get().x,0,
		                                                                  m_Player.InputMoveVector.Get().y));

		// apply force to controller based dashDirection
		m_Controller.AddSoftForce(dashDirection*MotorDashForce,5f);

	}

	protected virtual void OnStart_WallJump()
	{
		// stop jumping to prevent creating a double wall jump
		m_Player.Jump.Stop();
		// if we are wallrunning or wallhanging, stops immediately
		m_Player.WallHang.Stop();
		m_Player.WallRun.Stop();

		// setup for walljump
		m_Player.MotorJumpDone.Set(false);
		m_MotorWallJumpDone = false;
		m_CanWallHang = false;
		m_WallJumpAble = false;

		// store local velocity
//		Vector3 localVelocity = m_Controller.Transform.InverseTransformDirection( m_Player.Velocity.Get() );

		Vector3 localVelocity = GetLocalVelocity() * 0.1f;

		// store MotorThrottle from walljump Normal 
		Vector3 tempMotorThrottle = (m_WallJumpHitNormal*WallJumpForce) +										// wall jump normal direction
			(m_Camera.transform.TransformDirection(Vector3.forward)*(WallJumpForwardForce * localVelocity.z)) + // nudge it towards camera direction
									new Vector3 (0,(WallJumpUpForce / Time.timeScale),0);						// wall jump up force

		// perform impulse jump
		m_Player.FallSpeed.Set( (Physics.gravity.y * (m_Controller.PhysicsGravityModifier * 0.002f)) );
		m_Player.MotorThrottle.Set(tempMotorThrottle);
	}

	protected virtual void OnStart_DoubleJump()
	{
		// setup for double jump
		m_MotorDoubleJumpDone = false ;
		m_DoubleJumpAble = false;

		// decrease the current double jump count left by 1
		m_currentDoubleJump -= 1;

		m_Controller.StopSoftForce();

		// store current MotorThrottle 
		Vector3 tempMotorThrottle = m_Player.MotorThrottle.Get();
		tempMotorThrottle.y = (DoubleJumpForce / Time.timeScale);

		// perform impulse jump
		m_Player.FallSpeed.Set( (Physics.gravity.y * (m_Controller.PhysicsGravityModifier * 0.002f)) );
		m_Player.MotorThrottle.Set(tempMotorThrottle);


		// give it's a little nudge towards player's direction
		Vector3 doubleJumpDirection = transform.TransformDirection(new Vector3 (
			m_Player.InputMoveVector.Get().x,0,m_Player.InputMoveVector.Get().y));

		m_Controller.AddForce( doubleJumpDirection * DoubleJumpForwardForce);

	}

	protected virtual void OnStart_LedgeGrab()
	{
		// setup for ledgegrab
		m_CanWallHang = false;
		m_IsWallHanging = false ;
		m_IsLedgeGrabbing = true;
//		if(m_Platform != null)
//			m_Player.Platform.Set(m_Platform);


		// stop jump
		m_Player.Jump.Stop();

		// stop the controller
		m_Player.Stop.Send();

		// disallow player control while ledgegrab
		m_Player.InputAllowGameplay.Set(false);

		// check the height of the obstacle, if it's over vault height then it will do ledge grab
		if(DistancePlayerToTop < MinDistanceToClimb && DistancePlayerToTop > MinDistanceToVault)
			StartCoroutine(LineUpLedge());
		else
			StartCoroutine(ClimbLedge(ClimbPositionOffset));	//else it will auto vault

	}

	/// <summary>
	/// This lerps the camera towards the direction of the wallrunning
	///
	protected virtual IEnumerator LineUpWallRun()
	{

		Quaternion startingRotation = m_Camera.transform.rotation; // cache start rotation
		Quaternion endRotation ; // cache end rotation
		
		endRotation = Quaternion.LookRotation(m_WallRunLookDirection,Vector3.up);

		float t = 0;
		
		float duration = 0.2f;
		while(t < 1)
		{	
			t += Time.deltaTime/duration;
			
			Quaternion newRotation = Quaternion.Slerp(startingRotation, endRotation, t);
			m_Player.Rotation.Set(new Vector2( newRotation.eulerAngles.x , WallRunAutoRotateYaw ? newRotation.eulerAngles.y : m_Player.Rotation.Get().y ));
			
			yield return new WaitForEndOfFrame();
		}

		m_CachedDirection = m_Camera.Transform.forward;
		
	}

	/// <summary>
	/// Lerps players towards the ideal ledgegrab position
	/// </summary>
	protected virtual IEnumerator LineUpLedge()
	{
		// setup for line up ledge
		isLiningUp = true;
		m_IsHanging = true;
		
		m_Camera.SetRotation(m_Camera.Transform.eulerAngles, false, true);
		
		// stop player 
		m_Player.Stop.Send();
		
		// store the current weapon
		m_LastWeaponEquipped = m_Player.CurrentWeaponIndex.Get();
		
		// put the weapon away
		vp_Timer.In(0.0f, delegate() { m_Player.SetWeapon.TryStart(0); } );
		
		// play parkour animation.
		if(ParkourArmPrefab != null && ParkourArmClimbAnim != null)
			vp_Timer.In(0.1f, ParkourArmClimb);
		
		Quaternion startingRotation = m_Camera.transform.rotation; // cache start rotation
		Quaternion endRotation = Quaternion.LookRotation(m_HangRotation); // cache end rotation

		Vector3 startPosition = m_Player.Position.Get(); // cache the start position

		// position player about 0.5f in negative z away from ledgegrab position to
		// minimize player being too close to the wall
		Vector3 posOff ;

		posOff = m_ClimbObjectNormal*(m_Player.IsFirstPerson.Get() ? FirstPersonLedgeOffset : ThirdPersonLedgeOffset);

		Vector3 endPosition = m_HangPosition + posOff; // cache the end position

		float t = 0;
		
		while(t < 1)
		{
			t += Time.deltaTime/(ClimbAlignDuration);

			Quaternion newRotation = Quaternion.Slerp(startingRotation, endRotation, t);
			m_Player.Rotation.Set(new Vector2( newRotation.eulerAngles.x , newRotation.eulerAngles.y));

			// Slerp players from current position to target position by t
			Vector3 newPosition = Vector3.Slerp(startPosition, endPosition, t);

			m_Player.Position.Set(newPosition);

			m_Player.Stop.Send();
			yield return new WaitForEndOfFrame();
		}
		m_CachedDirection = m_Camera.Transform.forward;
		m_CachedRotation = m_Player.Rotation.Get();


//		m_CachedRotation = new Vector2 (m_HangRotation.x, m_HangRotation.y);

		m_HangPosOffset = m_HangPosition + new Vector3 (0,0.5f,0);
		isLiningUp = false;

	}

	/// <summary>
	/// This is the actual function that will be used to climb obstacle or vault obstacle
	/// It is triggered after OnStart_ClimbLedge
	/// Takes a vector3 as a position offset
	/// </summary>
	protected virtual IEnumerator ClimbLedge(Vector3 posOff)
	{
		// disable player input and setup for climbledge
		m_Player.InputAllowGameplay.Set(false);
		m_IsHanging = false;
		m_Player.Stop.Send();

		m_Player.ClimbingLedge.Send();
		Vector3 startPosition = m_Player.Position.Get(); // cache the start position
		m_CachedRotation = m_Player.Rotation.Get();	// cache the start rotation

		// convert positionOffset into controller's local space
		posOff = m_Controller.Transform.TransformDirection(posOff);
		
		Vector3 endPosition = m_ClimbObjectPosition + posOff; // cache the end position


		float t = 0;
		
		while(t < 1)
		{
			// lerp player current position to target position
			t += Time.deltaTime/LedgeGrabDuration;
			Vector3 newPosition = Vector3.Slerp(startPosition, endPosition, t);
			m_Player.Position.Set(newPosition);
			m_Player.Stop.Send();
			yield return new WaitForEndOfFrame();
		}

		// turn off ledgegrabbing, as we are done climbing
		m_IsLedgeGrabbing = false;

		// equip cache weapon
		m_Player.SetWeapon.TryStart(m_LastWeaponEquipped);
		Vector3 force = gameObject.transform.root.forward * ClimbDismountForce;
		force.y = ClimbDismountForce * 0.5f;	// dismounting at top: add slight up-force (push forward & up)
		m_Player.Stop.Send();
		m_Controller.AddForce(force);
		m_Player.LedgeGrab.Stop();
		m_Player.SetState("Default", true, true);
		m_LedgeGrabable = false;

		// return control back to player
		m_Player.InputAllowGameplay.Set(true);
	}
	
	protected virtual IEnumerator ShimmyLedge(Vector3 posOff) 
	{
		// disable player input and setup for climbledge
		m_Player.InputAllowGameplay.Set(false);
		m_Player.Stop.Send();
		
		Vector3 startPosition = m_Player.Position.Get(); // cache the start position

		Vector3 endPosition = posOff; // cache the end position
		
		
		float t = 0;
		
		while(t < 1)
		{
			// lerp player current position to target position
			t += Time.deltaTime/1f;
			Vector3 newPosition = Vector3.Slerp(startPosition, endPosition, t);
			m_Player.Position.Set(newPosition);
			m_Player.Stop.Send();
			yield return new WaitForEndOfFrame();
		}

		// return control back to player
		m_Player.InputAllowGameplay.Set(true);
	}

	protected virtual void OnStop_WallRun()
	{
		autoYawed = false;
		m_WallRunTimer = 0f;
		m_CanWallHang = false;
		m_WallJumpAble = false;
		m_IsWallRunning = false;
		m_DoubleJumpAble = true;
		m_currentDoubleJump = DoubleJumpCount ;
		
		m_Player.Interactable.Set(null);

		m_CanWallRunAgain = Time.time + WallRunAgainTimeout;
		m_Player.SetState("Default");

	}

	protected virtual void OnStop_WallHang()
	{
		m_IsWallHanging = false;
	}
	
	protected virtual void OnStop_GroundSlide()
	{
		m_Player.Run.Stop();
		m_GroundSliding = false;
		m_CanGroundslideAgain = Time.time + GroundSlideCooldown;
		m_CachedRotation = m_Player.Rotation.Get();
	}

	protected virtual void OnStop_Dash()
	{
		m_Dashing = false;
		m_MotorDashDone = true;
		m_CanDashAgain = Time.time + DashCooldown;
	}

	protected virtual void OnStop_DoubleJump()
	{
		m_DoubleJumpAble = true;
		m_MotorDoubleJumpDone = true;
		
	}

	protected virtual void OnStop_WallJump()
	{
		m_Player.MotorJumpDone.Set(true);
		m_MotorWallJumpDone = true ;
		m_CanWallHang = false;
		m_WallJumpAble = false;
		m_DoubleJumpAble = true;
		m_currentDoubleJump = DoubleJumpCount ;
	}

	protected virtual void OnStop_LedgeGrab()
	{
		m_LedgeGrabable = false;
		m_IsLedgeGrabbing = false;
		m_IsHanging = false;

//		m_Platform = null ;
//		m_Player.Platform.Set(m_Platform);

		m_Player.WallHang.Stop ();
		if(shouldCrouch)
		{
			m_Player.Crouch.Start();
		}
		else
			m_Player.SetState("Default", true, true);

		// hides parkour arm
		if(ParkourArmPrefab != null)
			ParkourArmPrefab.SetActive(false);

	}

	/// <summary>
	/// Handles when player is hanging on to the ledge before they climb up.
	/// </summary>
	protected virtual void UpdateLedgeHanging()
	{		 
		// no need to run through here if grounded
		if(m_Controller.Grounded)
			return;

		// no need to run through here if we aren't climbing
		if(m_Player == null || !m_IsLedgeGrabbing)
			return;

		// still lining up 
		if(isLiningUp)
			return;

		// we aren't hanging so no point coming through here
		if(!m_IsHanging)
			return;

		// stops all run and wallrun activity
		m_Player.Run.Stop();
		m_Player.WallRun.Stop();

		// set gravity to 0
		m_Controller.PhysicsGravityModifier = 0.0f;

		// limit player's view so they could only see 90 degrees to the left and right only
		m_Camera.RotationYawLimit = new Vector2(m_CachedRotation.y - 50,  m_CachedRotation.y + 50);
		m_Camera.RotationPitchLimit = new Vector2(90, -90);

		// stops player controller
		m_Controller.Stop();

		// store players current position
		Vector3 pos = m_Player.Position.Get() ;

		// Below is to simulate player building up momentum from the hanging to climbing up the ledge

		//if we reach the threshold, climb up the ledge
		if(vp_Input.GetAxisRaw("Vertical") > 0) 
		{
			StartCoroutine(ClimbLedge(ClimbPositionOffset));
			return;
		}

		// Build up momentum when pressing forward
		if(vp_Input.GetAxisRaw("Vertical") > 0 && pos.y < m_HangPosOffset.y)
		{
			pos += new Vector3(0,PullUpSpeed,0);
			m_Player.Position.Set(pos); // simulate moving upward
		}
		// drops down the default hanging position if there's no input
		if(vp_Input.GetAxisRaw("Vertical") == 0 && pos.y > m_HangPosition.y)
		{
			pos -= new Vector3(0,PullUpSpeed,0);
			m_Player.Position.Set(pos); // move back down
		}

		// stop hanging and drop player down
		if(vp_Input.GetAxisRaw("Vertical") < 0 || vp_Input.GetButton("Crouch"))
		{
			m_Player.LedgeGrab.Stop();
			m_CachedRotation = m_Player.Rotation.Get();
			m_Camera.SetRotation(m_Camera.Transform.eulerAngles, false, true);
			m_Controller.AddForce(-m_Controller.Transform.forward * ClimbDismountForce);

			// reset back to default and give player control
			m_Player.SetState("Default");
			m_Player.SetWeapon.TryStart(m_LastWeaponEquipped);
			m_Player.InputAllowGameplay.Set(true);

			// hide parkour arm
			if(ParkourArmPrefab != null)
				ParkourArmPrefab.SetActive(false);
		}
	}

	/// <summary>
	/// Handles the logic for wallhanging.
	/// </summary>
	protected virtual void UpdateWallHang()
	{
		if(!m_Player.WallHang.Active)						// we are not currently wallhanging
			return;
		if(m_WallHangTimer >= WallHangDuration*LosingGripStart)			// if we are near the end of the wallhang duration
			m_Controller.PhysicsGravityModifier = LosingGripGravity;	// set gravity to a low value to simulate slipping

		if(m_WallHangTimer >= WallHangDuration)				// we are over the duration allowed
		{
			m_WallHangTimer = 0;							// reset the timer
			m_Player.WallHang.TryStop();					// and stop wallhanging
			return;
		}

		m_WallHangTimer += Time.deltaTime;			

	}

	/// <summary>
	/// Handles the logic to stop groundsliding when it is active
	/// </summary>
	protected virtual void UpdateGroundSliding()
	{
		// This checks if player has been running forward for a predetermined amount of seconds
		// If true, then only could player attempt to groundslide.
		// This is to prevent from spamming groundslide infinitely.
		if(!m_Player.GroundSlide.Active && m_Player.Velocity.Get().magnitude > MinSpeedToBuildup)
			m_GroundSlideBuildup += Time.deltaTime;
		else 
			m_GroundSlideBuildup = 0f;

		if(m_Player.GroundSlide.Active)
		{
			// constraint player transform to only look forward
			transform.rotation = Quaternion.LookRotation(m_GroundSlideLookDirection,Vector3.up);

			// limit camera to only be able to left and right
			m_Camera.RotationYawLimit = new Vector2(m_CachedRotation.y - 120,  m_CachedRotation.y + 120);
			
			// stops groundsliding if we are too slow, preventing sliding up ramp or infinite sliding
			if(m_Player.Velocity.Get().magnitude < GroundSlideSpeedMinimum )
				m_Player.GroundSlide.Stop();
		}
	}

	/// <summary>
	/// Handles the cooldown logic for dashing 
	/// </summary>
	protected virtual void UpdateDash()
	{

		if(m_CurrentDash > DashCount)		// we are over the dashcount
		{
			m_DashTimer = 0 ;				// reset timer
			m_CurrentDash = DashCount ;		// set it to the max alloawed
			return;
		}

		// we reached cooldown
		if(m_CurrentDash < DashCount && m_DashTimer >= DashRecoverSpeed)
		{
			m_DashTimer = 0 ;				// reset timer
			m_CurrentDash ++;				// add one more dash counter
			return;
		}

		// buildup  to cooldown
		if(m_CurrentDash < DashCount && m_DashTimer < DashRecoverSpeed)
			m_DashTimer += Time.deltaTime;
	}

	/// <summary>
	/// Handles logic for LedgeGrab. Cast a ray that is in front of the controller
	/// and returns the climbing position if within climbing distance
	/// Depending on the height of the obstacle, will either auto climb or go to ledge hanging
	/// </summary>
	protected virtual void UpdateLedgeGrab()
	{
//		if(m_Controller.Grounded)			// we are still on the ground
//			return;
//
//		if(m_IsLedgeGrabbing)				// already ledgegrabbing
//			return;
//
//		if(m_IsHanging)						// we are currently hanging
//			return;

		if(LedgeGrabRange == 0 )			// ledgegrab disabled.
			return;

		RaycastHit checkCeilingHit ;		

		// check if there's a ceiling above while attempting to ledgegrab
		// if true, we are inside building, abort ledge grab
		if(Physics.SphereCast(transform.position,m_CharacterController.radius,Vector3.up, out checkCeilingHit, 2f))
				return;

		// in the air and we are moving forward.
		if(!m_Controller.Grounded && GetLocalVelocity().z > 0)
		{
			m_Player.LedgeGrab.TryStart();	// automatically try to start LedgeGrab
		}

		
		RaycastHit hit;						// stores collider hit in front of controller	
		RaycastHit topHit;					// stores ceiling hit on top of climb position
		GameObject climbObject;

		// cast a ray forward about chest height to see if there's any climbable object
		if (Physics.Raycast( transform.root.position + new Vector3 (0,m_CharacterController.height*0.8f,0), 
		                       transform.root.forward, out hit, LedgeGrabRange, vp_Layer.Mask.ExternalBlockers))
		{
			if(!string.IsNullOrEmpty(IgnoreLedgeGrabTag))		// If tag is not empty
			{
				if(hit.transform.tag == IgnoreLedgeGrabTag)		// Hit a ignore ledgegrab tag object
					return;
			}

			Debug.DrawRay(hit.point, hit.normal, Color.green);

			climbObject = hit.transform.gameObject;

			if(hit.normal.y >  0.5f)	// wall is not vertically flat
				return;

//			float height = hit.transform.localScale.y * hit.collider.bounds.size.y;// finds the height of collider
			float height = m_CharacterController.height + HeadOffset;
			float topMost;									// get the top climbing position based on raycast and climbObjectHeight

			Vector3 offset = hit.point + transform.root.TransformDirection(0,height,0.05f);

			Debug.DrawRay(offset, -transform.root.up, Color.blue);


			if(Physics.Raycast( offset, -transform.root.up, out topHit, LedgeGrabRange*2f, vp_Layer.Mask.ExternalBlockers))
			{
				if(topHit.normal.y < 0.9f)
					return;

				if(topHit.point.y < hit.point.y || topHit.point.y < transform.position.y)
					return;

				if(topHit.transform.gameObject != climbObject)
					return;

				Debug.DrawRay(topHit.point, topHit.normal, Color.red);

//				if (topHit.collider.gameObject.layer == vp_Layer.MovableObject)
//				{
//					m_Platform = topHit.transform;
//				}
//				else 
//					m_Platform = null;

				topMost = topHit.point.y+0.1f;

				if(m_Player.IsFirstPerson.Get())
					m_HangPosition = new Vector3(hit.point.x,topMost-2.2f,hit.point.z);
				else
					m_HangPosition = new Vector3(hit.point.x,topMost-1.8f,hit.point.z);

				m_HangRotation = Quaternion.AngleAxis(-180,Vector3.up) * hit.normal;
				m_ClimbObjectPosition = new Vector3(hit.point.x,topMost,hit.point.z);
				m_ClimbObjectNormal = hit.normal;
				DistancePlayerToTop = Vector3.Distance(m_ClimbObjectPosition,new Vector3(m_ClimbObjectPosition.x,transform.root.position.y,m_ClimbObjectPosition.z));

				Vector3 posOff = m_Controller.Transform.TransformDirection(ClimbPositionOffset);

				if (DistancePlayerToTop < MinDistanceToClimb )
				{
					if(Physics.SphereCast(new Ray(	m_ClimbObjectPosition+posOff, Vector3.up),
					                      m_CharacterController.radius,
					                      m_CharacterController.height - (m_CharacterController.radius) + 0.01f))
						shouldCrouch = true;
					else
					{
						shouldCrouch = false;
					}

					m_LedgeGrabable = true ;					
					
				}
				else
					m_LedgeGrabable = false;
			}			
		}
		else
			m_LedgeGrabable = false;
	}

	/// <summary>
	/// Handles the logic for Wallrun.
	/// Happens once OnStart_WallRun setup all the variables
	/// Will constraint the transform to the normal of the wall
	/// </summary>
	protected virtual void UpdateWallrun()
	{	
		if(m_IsLedgeGrabbing)
			return;

		if(m_IsHanging)
			return;

		if(!m_IsWallRunning)
			return;

		if(m_Player.Crouch.Active)
			return;

		// get the angle between the wallrun surface and the player's facing direction
		// used for controlling animationblending
		
		Vector3 cv = Vector3.Cross(-m_WallRunHitNormal, transform.forward);
		m_WallAngle = Vector3.Angle(-m_WallRunHitNormal, transform.forward);
		
		if(cv.y > 0) m_WallAngle = -m_WallAngle;

		RaycastHit hit;
		Vector3 raycastdirection = m_WallRunHitPoint - transform.position;

		// determines where is the wallrun in relation to the camera 
		// this will determine where the raycast should be projected
		// so if wall is on camera right, the raycast will projected to camera right.

//		if(m_IsWallInFront )
//			raycastdirection = transform.root.forward;
//		else if (m_IsWallOnRight)
//			raycastdirection = transform.root.right;
//		else if (!m_IsWallOnRight)
//			raycastdirection = -transform.root.right;
	
		// stop wallrun if we ran out of time
		if(m_WallRunTimer >= WallRunDuration && !WallRunInfinite)
		{
			m_WallRunTimer = 0f;
			m_IsWallRunning = false;

			m_Player.WallRun.Stop ();

			m_Controller.AddForce (m_WallRunDismountDirection * WallRunDismountForce);
			
			return;
		}

		// override controller's gravity
		m_Controller.PhysicsGravityModifier = WallRunGravity;

		m_WallRunTimer += Time.deltaTime;

		float wallrunRelativeSpeed = Vector3.Dot(m_Camera.transform.forward, m_WallRunLookDirection);
		wallrunRelativeSpeed *= 10f;
		wallrunRelativeSpeed = Mathf.Round (wallrunRelativeSpeed);
		wallrunRelativeSpeed *= 0.1f;

		// rotates player towards wallrunning direction.
		if(m_WallAngle > 50f || m_WallAngle < -50f )
		{
			transform.rotation = Quaternion.LookRotation(m_WallRunLookDirection,Vector3.up);
			m_Controller.MotorAcceleration = Mathf.Lerp(0.2f,m_WallRunMaxSpeed,wallrunRelativeSpeed);
		}


		Vector2 tempInput = m_Player.InputMoveVector.Get();
		tempInput.x = 0f;
		m_Player.InputMoveVector.Set(tempInput);

		if(	Physics.Raycast( transform.root.position + new Vector3 (0,m_CharacterController.height*0.8f,0) , raycastdirection, out hit, WallRunRange))
		{
			if(hit.collider != null)
				m_WallRunObject = hit.transform;
		}

		if (!Physics.CheckSphere(transform.root.position + new Vector3 (0,m_CharacterController.height*0.5f,0), 1.5f,vp_Layer.Mask.ExternalBlockers)) 
		{
			m_WallRunTimer = 0f;
			m_IsWallRunning = false;
			m_IsHanging = false ;
			m_Player.WallRun.Stop ();
			m_Controller.AddForce (m_WallRunDismountDirection * WallRunDismountForce);

		}
	}
		
	void UpdateWallrunTilt()
	{
		if(!m_Player.WallRun.Active)
			return;

		// disabled
		if(WallRunTilt == 0)
			return;

//		print (WallAngle);****************************************************************************************************************************************************************************

		Vector3 localVelocity = GetLocalVelocity() * 0.016f ;

		if(WallAngle > 25)
			m_Camera.AddRollForce(Mathf.Clamp(localVelocity.z * WallRunTilt,0f,WallRunTilt));
		else if (WallAngle < -25)
			m_Camera.AddRollForce(Mathf.Clamp(localVelocity.z * -WallRunTilt,-WallRunTilt,0f));


//		if(m_IsWallOnRight)
//			m_Camera.AddRollForce(Mathf.Clamp(localVelocity.z * WallRunTilt,0f,WallRunTilt));
//		else
//			m_Camera.AddRollForce(Mathf.Clamp(localVelocity.z * -WallRunTilt,-WallRunTilt,0f));
		
	}

	/// <summary>
	/// If press, will start LedgeGrab or Walljump or DoubleJump
	/// This utilizes GetButtonDown so it will only trigger once
	/// </summary>
	protected virtual void InputJump()
	{
		// uses default "Jump" button to check for double jumping.
		// feel free to change "Jump" to some other buttons to suit your project
		if(vp_Input.GetButtonDown("Jump"))
		{
			m_Player.LedgeGrab.TryStart();
			m_Player.WallJump.TryStart();
			m_Player.DoubleJump.TryStart();
		}
		else if(vp_Input.GetButtonUp("Jump"))
		{
			m_Player.WallJump.TryStop();
			m_Player.DoubleJump.TryStop();

		}

	}

	/// <summary>
	/// Input command for executing dash
	/// The main logic happens in the GetButtonUp, and if within the 
	/// </summary>
	protected virtual void InputDash()
	{

		// uses default "Run" button to check for dashing 
		if(vp_Input.GetButton("Run"))
		{
			m_DashInputTimer += Time.deltaTime;
		}
		// checks if we release "Run" button within the time frame allowed to trigger dash
		else if (vp_Input.GetButtonUp("Run") && m_DashInputTimer <= DashSensitivtiy)
		{
			m_Player.Dash.TryStart();
			m_DashInputTimer=0f;
		}
		else	
		{
			m_Player.Dash.TryStop();
			m_DashInputTimer=0f;
		}

	}

	protected virtual void InputGroundSlide()
	{

		//if (vp_Input.GetButton("Crouch"))	// suggested input axis
		if (vp_Input.GetButton("Crouch") || Input.GetAxisRaw("Crouch") != 0)
		{
			m_Player.GroundSlide.TryStart();
			m_Player.WallHang.TryStop();
		}
		else
		{
			m_Player.GroundSlide.Stop();
		}

	}

	protected virtual void InputWallHang()
	{
		if (vp_Input.GetButtonDown("Zoom"))
		{
			if(!m_IsWallHanging)
				m_Player.WallHang.TryStart();
			else
				m_Player.WallHang.TryStop();
		}
	}
	
	/// <summary>
	/// Helper to check if the collision surface can be wallrun
	/// </summary>
	protected virtual void CheckWallRun(RaycastHit hit)
	{
		if(m_Controller.Grounded)
			return;

		if(m_IsLedgeGrabbing)
			return;

		if(!string.IsNullOrEmpty(IgnoreWallRunTag))			// If tag is not empty
		{
			if(hit.transform.tag == IgnoreWallRunTag &&
			   hit.transform.tag != "Untagged")				// Hit a un-wallrunable object
				return;
		}

		if(m_WallRunHitNormal == hit.normal)
		{
			m_CanWallRunAgain = Time.time + WallRunAgainTimeout;
			return;
		}
	
		if(hit.normal.y >  0.2f || hit.normal.y < -0.2f)
		{
			m_WallRunable = false;
			return;
		}

		if(hit.collider == null)
			return;

		m_WallRunHitNormal = hit.normal;
		m_WallRunHitPoint = hit.point;

		float colliderNormalForward ;
		float colliderNormalRight ;

		colliderNormalForward = Vector3.Dot (transform.forward,hit.normal);
		colliderNormalRight = Vector3.Dot (transform.right,hit.normal);
	
		if (colliderNormalForward < -0.95)		// check if we are facing the wall		
		{
			m_IsWallInFront = true;

			m_WallRunLookDirection = Quaternion.AngleAxis(-90,Vector3.forward) * m_WallRunHitNormal;
			m_WallRunDismountDirection = m_WallRunHitNormal;
			m_IsWallOnRight = false;
			m_WallRunable = true;


			m_Player.WallRun.TryStart();
			return;

		}
		else
		{
			m_IsWallInFront = false;
		}

		if(colliderNormalRight > 0)			// check if wall is on the left
		{
			m_WallRunLookDirection = Quaternion.AngleAxis(-89,Vector3.up) * m_WallRunHitNormal;
			m_WallRunDismountDirection = Quaternion.AngleAxis(-15,Vector3.up) * m_WallRunHitNormal;
			m_IsWallOnRight = false;
		}
		
		else if (colliderNormalRight < 0)	// else it's on the right
		{
			m_WallRunLookDirection = Quaternion.AngleAxis(89,Vector3.up) * m_WallRunHitNormal;
			m_WallRunDismountDirection = Quaternion.AngleAxis(15,Vector3.up) * m_WallRunHitNormal;
			m_IsWallOnRight = true;
		}
		
		m_WallRunable = true;

		if(m_IsWallRunning && !autoYawed && !m_IsWallInFront && WallRunAutoRotateYaw)// are we already wallrunning?
		{
//			autoYawed = true;
			StartCoroutine(LineUpWallRun());	// align the camera to the new vector instead.
			return;
		}
		else
			m_Player.WallRun.TryStart();

	}

	protected virtual void UpdatePhysics()
	{
		if(m_Controller.Grounded)
			return;
		if(m_Player.DoubleJump.Active)
			return;
		if(m_Player.WallRun.Active)
			return;

		m_Controller.MotorDamping = targetDamping;
	}

	/// <summary>
	/// Helper to check if the collision surface can be walljump
	/// </summary>
	protected virtual void CheckWallJump(RaycastHit hit)
	{
		if(!string.IsNullOrEmpty(IgnoreWallJumpTag))			// If tag is not empty
		{
			if(hit.transform.tag == IgnoreWallJumpTag &&
			   hit.transform.tag != "Untagged")					// Hit a un-walljumpable object
				return;
		}

//		// hit the same wall, we can't walljump the same place twice
//		if(m_WallJumpHitNormal == hit.normal)
//		{
//			m_WallJumpAble = false;
//			return;
//		}

		if (!m_Controller.Grounded)
		{
			if(hit.normal.y <  0.1f && hit.normal.y > -0.1f) //if surface's orientation is flat like a wall
			{
				m_WallJumpAble = true;
				m_WallJumpHitNormal = hit.normal;
				m_DoubleJumpAble = false;
			}
		}
		else
			m_WallJumpAble = false;
	}

	/// <summary>
	/// Helper to check if the collision surface can be wallhang
	/// </summary>
	protected virtual void CheckWallHang(RaycastHit hit)
	{
		if(!string.IsNullOrEmpty(IgnoreWallHangTag))		// If tag is not empty
		{
			if(hit.transform.tag == IgnoreWallHangTag &&
			   hit.transform.tag != "Untagged")				// Hit a un-wallhangable object
				return;
		}

		// hit the same wall, we can't wallhang the same place twice
		if(m_WallHangHitNormal == hit.normal)
			return;

		if (!m_Controller.Grounded)
		{
			if(hit.normal.y <  0.1f && hit.normal.y > -0.1f) //if surface's orientation is flat like a wall
			{
				m_CanWallHang = true;
				m_WallHangHitNormal = hit.normal ;
			}
		}
		else
			m_CanWallHang = false;
	}
	


	protected virtual void OnMessage_FallImpact(float impact)
	{
		// we are touching the ground again, reset all the variables
		m_CanWallRunAgain = Time.time;
		m_DoubleJumpAble = true;
		m_currentDoubleJump = DoubleJumpCount ;
		m_IsHanging = false;
		m_IsLedgeGrabbing = false;
		m_IsWallHanging = false;
		m_Player.LedgeGrab.Stop();
		m_Player.WallRun.Stop();
		m_Player.SetState("Default", true, true);
		m_Player.InputAllowGameplay.Set(true);
		m_WallRunHitNormal = new Vector3(0,0,0);

		
	}

		protected virtual Texture OnValue_WallTexture
	{
		get
		{	
			if (WallRunObject == null)
				return null;
			
			// return if no renderer and no terrain under the controller
			if(WallRunObject.GetComponent<Renderer>() == null && m_CurrentTerrain == null)
				return null;

			int terrainTextureID = -1;

			// check to see if a main texture can be retrieved from the terrain
			if(m_CurrentTerrain != null)
			{
				terrainTextureID = vp_FootstepManager.GetMainTerrainTexture( m_Player.Position.Get(), m_CurrentTerrain );
				if(terrainTextureID > m_CurrentTerrain.terrainData.splatPrototypes.Length - 1)
					return null;
			}

			// return the texture
			return m_CurrentTerrain == null ? WallRunObject.GetComponent<Renderer>().material.mainTexture : m_CurrentTerrain.terrainData.splatPrototypes[ terrainTextureID ].texture;
			
		}
	}

	/// <summary>
	/// This is copy from FootStepManager, derived from Footstep and made to work only with wallrun
	/// </summary>
	protected virtual void WallRunFootstep()
	{
		if(m_Controller.Grounded)
			return;

		// return if there no texture or surface type is found
		if(m_Player.WallTexture.Get() == null )
			return;
		
		// return if not wallrunning
		if(!m_Player.WallRun.Active)
			return;

		// loop through the surfaces
		foreach(vp_FootstepManager.vp_SurfaceTypes st in m_FootstepManager.SurfaceTypes)
		{
			// loop through the surfaces textures
			foreach(Texture tex in st.Textures)
			{
				// if the texture is the same as the wall texture...
				if(tex == m_Player.WallTexture.Get())
				{
					// play random surface sound
					m_FootstepManager.PlaySound( st );
					break;
				}
			}
		}
	}

	protected virtual Vector3 GetLocalVelocity()
	{
		Vector3 localVelocity = m_Controller.Transform.InverseTransformDirection( m_Player.Velocity.Get() );
		return localVelocity;
	}

	protected virtual Vector3 GetNewPosition()
	{
		Vector3 newPosition = m_Controller.Transform.position;

		RaycastHit hit;

		Vector3 offSetPos = transform.InverseTransformDirection(new Vector3(0,0,1));
		Vector3 lastWallRunNormal = Quaternion.AngleAxis(-180,Vector3.up) * m_WallRunHitNormal;

		Ray ray = new Ray(offSetPos, lastWallRunNormal);
		Physics.Raycast(ray, out hit, 10f);

		newPosition = hit.point;
				
		return newPosition;
	}
	
	/// <summary>
	/// Plays parkour arm animation.
	/// </summary>
	protected virtual void ParkourArmClimb()
	{
		if(ParkourArmPrefab != null )
		{
			ParkourArmPrefab.SetActive(true);

			Vector3 armPosition = m_HangRotation ;

			RaycastHit hit;

			Physics.Raycast(transform.position, armPosition , out hit);

//			ParkourArmPrefab.transform.position = new Vector3(hit.point.x, m_ClimbObjectPosition.y, hit.point.z) + transform.InverseTransformDirection(ParkourArmPos);

			ParkourArmPrefab.transform.position = m_ClimbObjectPosition + transform.InverseTransformDirection(ParkourArmPos);
			
			ParkourArmPrefab.transform.rotation = Quaternion.LookRotation(m_HangRotation,Vector3.up);
			ParkourArmPrefab.GetComponent<Animation>().CrossFade(ParkourArmClimbAnim.name);	// play anim
		}

	}

}


