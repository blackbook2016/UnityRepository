﻿/////////////////////////////////////////////////////////////////////////////////
// 
//	ac_FPParkourEditor.cs
//	© GamersFrontier. All Rights Reserved.
//	https://twitter.com/thndstrm
//	http://www.gamersfrontier.my
//
//	description:	Custom editor for ac_FPParkour
//
//					Code re-used and modified with express permission
/////////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ac_FPParkour))]

public class ac_FPParkourEditor : Editor
{
	// target component
	public ac_FPParkour m_Component;
	
	// foldouts
	public static bool m_MotorDashFoldout;
	public static bool m_MotorDoubleJumpFoldout;
	public static bool m_MotorWallJumpFoldout;
	public static bool m_MotorWallRunFoldout;
	public static bool m_MotorWallHangFoldout;
	public static bool m_MotorGroundSlideFoldout;
	public static bool m_MotorLedgeGrabFoldout;

	public static bool m_StateFoldout;
	public static bool m_PresetFoldout = true;
	
	private static vp_ComponentPersister m_Persister = null;
	
	public virtual void OnEnable()
	{
		m_Component = (ac_FPParkour)target;

		if (m_Persister == null)
			m_Persister = new vp_ComponentPersister();
		m_Persister.Component = m_Component;
		m_Persister.IsActive = true;
		
		if (m_Component.DefaultState == null)
			m_Component.RefreshDefaultState();
	}

	/// <summary>
	/// disables the persister and removes its reference
	/// </summary>
	public virtual void OnDestroy()
	{
		
		m_Persister.IsActive = false;
		
	}

	public override void OnInspectorGUI()
	{
		GUI.color = Color.white;
			
		if (Application.isPlaying || m_Component.DefaultState.TextAsset == null)
		{	
			DoMotorDashFoldout();
			DoMotorDoubleJumpFoldout();
			DoMotorGroundSlideFoldout();
			DoMotorLedgeGrabFoldout();
			DoMotorWallRunFoldout();
			DoMotorWallJumpFoldout();
			DoMotorWallHangFoldout();
		}
		else
			vp_PresetEditorGUIUtility.DefaultStateOverrideMessage();

		// state
		m_StateFoldout = vp_PresetEditorGUIUtility.StateFoldout(m_StateFoldout, m_Component, m_Component.States, m_Persister);
		
		// preset
		m_PresetFoldout = vp_PresetEditorGUIUtility.PresetFoldout(m_PresetFoldout, m_Component);
		
		// update
		if (GUI.changed)
		{
			
			EditorUtility.SetDirty(target);
			
			// update the default state in order not to loose inspector tweaks
			// due to state switches during runtime
			if(Application.isPlaying)
				m_Component.RefreshDefaultState();
			
			if (m_Component.Persist)
				m_Persister.Persist();
			
		}
	}

	/// <summary>
	/// Dos the motor dash foldout.
	/// </summary>
	public virtual void DoMotorDashFoldout()
	{

		m_MotorDashFoldout = EditorGUILayout.Foldout(m_MotorDashFoldout, "Dash");
		
		if (m_MotorDashFoldout)
		{	
			m_Component.DashCount = EditorGUILayout.IntSlider("Count", m_Component.DashCount, 0, 10);
			m_Component.MotorDashForce = EditorGUILayout.Slider("Force", m_Component.MotorDashForce, 0, 3);
			m_Component.DashRecoverSpeed = EditorGUILayout.Slider("RecoverSpeed", m_Component.DashRecoverSpeed, 0, 10);
			m_Component.DashSensitivtiy = EditorGUILayout.Slider("Input Sensivity", m_Component.DashSensitivtiy, 0, 1);
			m_Component.DashCooldown = EditorGUILayout.Slider("Cooldown", m_Component.DashCooldown, 0, 10);
			vp_EditorGUIUtility.Separator();
		}
	}

	/// <summary>
	/// Dos the motor double jump foldout.
	/// </summary>
	public virtual void DoMotorDoubleJumpFoldout()
	{
		m_MotorDoubleJumpFoldout = EditorGUILayout.Foldout(m_MotorDoubleJumpFoldout, "Double Jump");

		if(m_MotorDoubleJumpFoldout)
		{
			m_Component.DoubleJumpCount = EditorGUILayout.IntSlider("Count", m_Component.DoubleJumpCount, 0, 10);
			GUILayout.Label("How many times you can doublejump.", vp_EditorGUIUtility.NoteStyle);
	
			m_Component.DoubleJumpForce = EditorGUILayout.Slider("Force", m_Component.DoubleJumpForce, 0, 3);
			m_Component.DoubleJumpForwardForce = EditorGUILayout.Slider("Momentum Force", m_Component.DoubleJumpForwardForce, 0, 3);

			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Momentumforce is influenced InputMoveVector. " +
				"Set Momentumforce to 0 to disable changing of direction midair during doublejump.", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
		}
	}

	/// <summary>
	/// Dos the motor wall jump foldout.
	/// </summary>
	public virtual void DoMotorWallJumpFoldout()
	{
		m_MotorWallJumpFoldout = EditorGUILayout.Foldout(m_MotorWallJumpFoldout, "Wall Jump");

		if(m_MotorWallJumpFoldout)
		{
			m_Component.IgnoreWallJumpTag = EditorGUILayout.TagField("Ignore Tag",m_Component.IgnoreWallJumpTag);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Selected tag cannot be Wall Jump. Use this to help with level design", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			m_Component.WallJumpUpForce = EditorGUILayout.Slider("Up Force", m_Component.WallJumpUpForce, 0, 10);
			m_Component.WallJumpForce = EditorGUILayout.Slider("Wall Force", m_Component.WallJumpForce, 0, 3);
			m_Component.WallJumpForwardForce = EditorGUILayout.Slider("Forward Force", m_Component.WallJumpForwardForce, 0, 1);

			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Forward force is influenced by where camera is facing. Setting a small value helps player reach where they look at", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
		}
	}

	public virtual void DoMotorWallRunFoldout()
	{
		m_MotorWallRunFoldout = EditorGUILayout.Foldout(m_MotorWallRunFoldout, "Wall Run");

		if(m_MotorWallRunFoldout)
		{
			m_Component.IgnoreWallRunTag = EditorGUILayout.TagField("Ignore Tag",m_Component.IgnoreWallRunTag);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Selected tag cannot be Wallrun. Use this to help with level design", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			if(m_Component.WallRunInfinite)
				GUI.enabled = false;
			m_Component.WallRunDuration = EditorGUILayout.Slider("Duration", m_Component.WallRunDuration, 0, 10);
			GUI.enabled = true;
			m_Component.WallRunInfinite = EditorGUILayout.Toggle("Infinite Wallrun" , m_Component.WallRunInfinite);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Infinite Wallrun will turn off Duration. For best result set Gravity and Up Force to 0.", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			m_Component.WallRunGravity = EditorGUILayout.Slider("Gravity", m_Component.WallRunGravity, 0, 1);
			m_Component.WallRunSpeedMinimum = EditorGUILayout.Slider("Minimum Speed", m_Component.WallRunSpeedMinimum, 0, 10);
			m_Component.WallRunAgainTimeout = EditorGUILayout.Slider("Cooldown", m_Component.WallRunAgainTimeout, 0, 1);
			m_Component.WallRunUpForce = EditorGUILayout.Slider("Up Force", m_Component.WallRunUpForce, 0, 1);
			m_Component.WallRunDismountForce = EditorGUILayout.Slider("Dismount Force", m_Component.WallRunDismountForce, 0, 10);
			m_Component.WallRunTilt =  EditorGUILayout.Slider("Camera Tilt", m_Component.WallRunTilt, 0, 1);
			m_Component.WallRunRange = EditorGUILayout.Slider("Range", m_Component.WallRunRange, 0, 5);
			m_Component.WallRunAutoRotateYaw = EditorGUILayout.Toggle("Auto yaw", m_Component.WallRunAutoRotateYaw);
		}
	}

	public virtual void DoMotorWallHangFoldout()
	{
		m_MotorWallHangFoldout = EditorGUILayout.Foldout(m_MotorWallHangFoldout, "Wall Hang");
		
		if(m_MotorWallHangFoldout)
		{
			m_Component.IgnoreWallHangTag = EditorGUILayout.TagField("Ignore Tag",m_Component.IgnoreWallHangTag);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Selected tag cannot be Wall Hang. Use this to help with level design", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			m_Component.WallHangDuration = EditorGUILayout.Slider("Duration", m_Component.WallHangDuration, 0, 10);
			m_Component.LosingGripStart = EditorGUILayout.Slider("Lost Grip Start", m_Component.LosingGripStart, 0, 1);
			m_Component.LosingGripGravity = EditorGUILayout.Slider("Lost Grip Gravity", m_Component.LosingGripGravity, 0, 0.01f);
		}
	}

	public virtual void DoMotorGroundSlideFoldout()
	{
		m_MotorGroundSlideFoldout = EditorGUILayout.Foldout(m_MotorGroundSlideFoldout, "Ground Slide");
		
		if(m_MotorGroundSlideFoldout)
		{			
			m_Component.MotorGroundSlideForce = EditorGUILayout.Slider("Force", m_Component.MotorGroundSlideForce, 0, 10);
			m_Component.MinBuildUp = EditorGUILayout.Slider("Minimum Run Duration", m_Component.MinBuildUp, 0, 10);
			m_Component.MinSpeedToBuildup = EditorGUILayout.Slider("Minimum Run Speed", m_Component.MinSpeedToBuildup, 0, 10);
			m_Component.GroundSlideCooldown = EditorGUILayout.Slider("Cooldown", m_Component.GroundSlideCooldown, 0, 3);
			m_Component.GroundSlideSpeedMinimum = EditorGUILayout.Slider("Minimum Speed", m_Component.GroundSlideSpeedMinimum, 0, 3);
		}
	}

	public virtual void DoMotorLedgeGrabFoldout()
	{
		m_MotorLedgeGrabFoldout = EditorGUILayout.Foldout(m_MotorLedgeGrabFoldout, "Ledge Grab");

		if(m_MotorLedgeGrabFoldout)
		{
			m_Component.IgnoreLedgeGrabTag = EditorGUILayout.TagField("Ignore Tag",m_Component.IgnoreLedgeGrabTag);
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.HelpBox("Selected tag cannot be LedgeGrab. Use this to help with level design", MessageType.Info);
			GUILayout.Space(10);
			GUILayout.EndHorizontal();
			m_Component.ParkourArmPrefab = (GameObject)EditorGUILayout.ObjectField("Arm Prefab", m_Component.ParkourArmPrefab, typeof(GameObject), false);
			m_Component.ParkourArmClimbAnim = (AnimationClip)EditorGUILayout.ObjectField("Arm Animation", m_Component.ParkourArmClimbAnim, typeof(AnimationClip), false);
			m_Component.ParkourArmPos = EditorGUILayout.Vector3Field("Arm Position Offset", m_Component.ParkourArmPos);
			m_Component.HeadOffset = EditorGUILayout.Slider("Head Offset", m_Component.HeadOffset, 0, 1);
			m_Component.ClimbPositionOffset = EditorGUILayout.Vector3Field("Climb Position Offset", m_Component.ClimbPositionOffset);
			m_Component.LedgeGrabDuration = EditorGUILayout.Slider("LedgeGrab Duration", m_Component.LedgeGrabDuration, 0, 10);
			m_Component.PullUpSpeed = EditorGUILayout.Slider("Pullup Speed", m_Component.PullUpSpeed,0, 0.1f);
			m_Component.MinDistanceToVault = EditorGUILayout.Slider("Auto Vault Height", m_Component.MinDistanceToVault,0, m_Component.MinDistanceToClimb-0.1f);
			m_Component.MinDistanceToClimb = EditorGUILayout.Slider("Climb Height", m_Component.MinDistanceToClimb,0, 10);
			m_Component.LedgeGrabRange = EditorGUILayout.Slider("Climb Range", m_Component.LedgeGrabRange,0, 5);
			m_Component.ClimbAlignDuration = EditorGUILayout.Slider("Align Ledge Duration", m_Component.ClimbAlignDuration,0,1);
			m_Component.ClimbDismountForce = EditorGUILayout.Slider("Dismount Force", m_Component.ClimbDismountForce,0, 1);

		}
	}
}