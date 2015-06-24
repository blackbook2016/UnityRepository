/////////////////////////////////////////////////////////////////////////////////
// 
//	ac_FPParkourEventHandler.cs
//	© GamersFrontier. All Rights Reserved.
//	https://twitter.com/thndstrm
//	http://www.gamersfrontier.my
//
//	description:	This class adds the following states to the event system
//					Dash, WallJump, DoubleJump, WallRun, GroundSlide, and LedgeGrab
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class ac_FPParkourEventHandler : vp_FPPlayerEventHandler 
{
	// custom player activities
	public vp_Activity WallJump;
	public vp_Activity WallRun;
	public vp_Activity WallHang;
	public vp_Activity LedgeGrab;
	public vp_Activity GroundSlide;
	public vp_Activity Dash;
	public vp_Activity DoubleJump;

	public vp_Message ClimbingLedge;
	public vp_Value<Texture> WallTexture;

	protected override void Awake()
	{
		
		base.Awake();

		// custom activity state bindings
		BindStateToActivity(WallJump);		// custom walljump
		BindStateToActivity(WallRun);		// custom wallrun
		BindStateToActivity(WallHang);		// custom wallrun
		BindStateToActivity(LedgeGrab);		// custom parkour climbing 
		BindStateToActivity(GroundSlide);	// custom ground sliding
		BindStateToActivity(Dash);			// custom dashing
		BindStateToActivity(DoubleJump);	// custom double jump


	}
}
