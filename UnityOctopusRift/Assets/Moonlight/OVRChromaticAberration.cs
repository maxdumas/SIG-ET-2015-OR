/************************************************************************************

Filename    :   OVRResetOrientation.cs
Content     :   Helper component that can be dropped onto a GameObject to assist
			:	in resetting device orientation
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Runtime.InteropServices;		// required for DllImport

public class OVRChromaticAberration : MonoBehaviour {
	
	public OVRGamepadController.Button			toggleButton = OVRGamepadController.Button.X;	

	// Note: This functionality will eventually be moved to the CameraController editor.
	private bool								Chromatic = false;

	[DllImport("OculusPlugin")]
	private static extern void OVR_TW_EnableChromaticAberration( bool enable );

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
#if (UNITY_ANDROID && !UNITY_EDITOR)
		// Enable/Disable Chromatic Aberration Correction
		// NOTE: Enabling Chromatic Aberration has a large performance cost.
		OVR_TW_EnableChromaticAberration( Chromatic );
#endif
	}

	/// <summary>
	/// Check input and toggle chromatic aberration correction if necessary
	/// See the input mapping setup in the Unity Integration guide
	/// </summary>
	void Update() {
		// NOTE: some of the buttons defined in OVRGamepadController.Button are not available on the Android game pad controller
		if ( Input.GetButtonDown( OVRGamepadController.ButtonNames[(int)toggleButton] ) ) {
			//*************************
			// toggle chromatic aberration correction
			//*************************
			Chromatic = !Chromatic;
#if (UNITY_ANDROID && !UNITY_EDITOR)
			OVR_TW_EnableChromaticAberration( Chromatic );
#endif
		}
	}

}
