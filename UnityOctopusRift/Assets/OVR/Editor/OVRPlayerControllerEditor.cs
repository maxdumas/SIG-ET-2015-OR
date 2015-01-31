﻿/************************************************************************************

Filename    :   OVRPlayerControllerEditor.cs
Content     :   Player controller interface. 
				This script adds editor functionality to the OVRPlayerController
Created     :   January 17, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(OVRPlayerController))]

//-------------------------------------------------------------------------------------
// ***** OVRPlayerControllerEditor
//
// OVRPlayerControllerEditor adds extra functionality in the inspector for the currently
// selected OVRPlayerController.
//
public class OVRPlayerControllerEditor : Editor
{
	// target component
	private OVRPlayerController m_Component;

	// OnEnable
	void OnEnable()
	{
		m_Component = (OVRPlayerController)target;
	}

	// OnDestroy
	void OnDestroy()
	{
	}

	// OnInspectorGUI
	public override void OnInspectorGUI()
	{
		GUI.color = Color.white;
		
		Undo.RecordObject(m_Component, "OVRPlayerController");

		{
			m_Component.Acceleration 	  = EditorGUILayout.Slider("Acceleration", 			m_Component.Acceleration, 	  0, 1);
			m_Component.Damping 		  = EditorGUILayout.Slider("Damping", 				m_Component.Damping, 		  0, 1);
			m_Component.BackAndSideDampen = EditorGUILayout.Slider("Back and Side Dampen", 	m_Component.BackAndSideDampen,0, 1);
//			m_Component.JumpForce 		  = EditorGUILayout.Slider("Jump Force", 			m_Component.JumpForce, 		  0, 10);
			m_Component.RotationAmount 	  = EditorGUILayout.Slider("Rotation Amount", 		m_Component.RotationAmount,   0, 5);
				
			OVREditorGUIUtility.Separator();

			m_Component.GravityModifier = EditorGUILayout.Slider("Gravity Modifier", 		m_Component.GravityModifier, 0, 1);

			OVREditorGUIUtility.Separator();
		}
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(m_Component);
		}
	}		
}

