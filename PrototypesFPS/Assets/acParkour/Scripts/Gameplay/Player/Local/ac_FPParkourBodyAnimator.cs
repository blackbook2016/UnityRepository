/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPBodyAnimator.cs
//	© GamersFrontier. All Rights Reserved.
//	https://twitter.com/thndstrm
//	http://www.gamersfrontier.my
//
//	description:	A modified version of vp_FPBodyAnimator to work with acParkour.
//
//					Code re-used and modified with express permission
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ac_FPParkourBodyAnimator : vp_FPBodyAnimator 
{
	public Vector3 LedgeGrabOffsetPosition ;					// tweak this to get for more accurate placement during ledge grab

	protected CharacterController charactercontroller = null;
	protected vp_FPController m_Controller = null;

	protected ac_FPParkour m_Parkour = null;
	protected ac_FPParkour Parkour	{ get{return m_Parkour;	}}

	protected float m_climbLedgeFeetAdjustAngle = 95;
	protected float m_originalFeetAdjustAngle ;

	protected override void Awake()
	{
		base.Awake();
		m_Parkour = (ac_FPParkour)transform.root.GetComponentInChildren(typeof(ac_FPParkour));

		charactercontroller = (CharacterController)transform.root.GetComponentInChildren(typeof(CharacterController));
		m_Controller = (vp_FPController)transform.root.GetComponentInChildren(typeof(vp_FPController));
	}

	protected virtual void Start ()
	{
		m_originalFeetAdjustAngle = FeetAdjustAngle;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
			
		UpdateLedgeGrabPosition();

	}

	protected override void UpdateAnimator()
	{

		base.UpdateAnimator();

		// setup the animator for acParkour's animation
		m_Animator.SetBool("StartLedgeGrab", m_Parkour.IsLedgeGrabbing );
		m_Animator.SetBool("IsLedgeGrabbing", m_Parkour.IsLedgeGrabbing );
		m_Animator.SetBool("IsLedgeHanging", m_Parkour.IsHanging );
		m_Animator.SetBool("Hanging", m_Parkour.IsHanging);
		m_Animator.SetFloat("ParkourHeight", m_Parkour.DistancePlayerToTop ) ;
		m_Animator.SetBool("WallRunning", m_Parkour.IsWallRunning ) ;
		m_Animator.SetBool("Hanging", m_Parkour.IsHanging );
		m_Animator.SetBool("StartDash", m_Parkour.Dashing );
		m_Animator.SetBool("IsDashing", m_Parkour.Dashing );
		m_Animator.SetBool("StartGroundSlide", m_Parkour.GroundSliding );
		m_Animator.SetBool("IsGroundSliding", m_Parkour.GroundSliding );
		m_Animator.SetBool("StartWallHang", m_Parkour.IsWallHanging );
		m_Animator.SetBool("IsWallHanging", m_Parkour.IsWallHanging );
		m_Animator.SetFloat("WallAngle", m_Parkour.WallAngle);
	
	}

	protected virtual void UpdateLedgeGrabPosition()
	{
		if (Parkour.IsLedgeGrabbing)
		{
			transform.position += LedgeGrabOffsetPosition;
			Vector3 m_HangRotation = Quaternion.AngleAxis(-180,Vector3.up) * Parkour.WallNormal;
			transform.rotation = Quaternion.LookRotation(m_HangRotation,Vector3.up);

		}
	}

	protected override void UpdateBody()
	{
		if(Parkour.IsHanging || Parkour.IsLedgeGrabbing )
			return;
		else
			base.UpdateBody();
	}

	protected override void UpdateSpine()
	{
		if(Parkour.IsHanging || Parkour.IsLedgeGrabbing )
			return;
		else
			base.UpdateSpine();
	}

	protected virtual void OnStart_LedgeGrab()
	{
		m_originalFeetAdjustAngle = FeetAdjustAngle;
		FeetAdjustAngle = m_climbLedgeFeetAdjustAngle;
	}

	protected virtual void OnStop_LedgeGrab()
	{
		FeetAdjustAngle = m_originalFeetAdjustAngle;
	}

}
