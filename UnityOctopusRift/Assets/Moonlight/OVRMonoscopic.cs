/************************************************************************************

Filename    :   OVRResetOrientation.cs
Content     :   Helper component that can be dropped onto a GameObject to assist
			:	in resetting device orientation
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;

public class OVRMonoscopic : MonoBehaviour {
	
	public OVRGamepadController.Button			toggleButton = OVRGamepadController.Button.B;

	public OVRCameraController					cameraController = null;

	// Note: This functionality will eventually be moved to the CameraController editor.
	private bool								Monoscopic = false;

	/// <summary>
	/// Initialization
	/// </summary>
	void Awake() {
		if ( cameraController == null ) {
			Debug.LogError ( "ERROR: missing camera controller reference on " + name );
			enabled = false;
			return;
		}
	}

	/// <summary>
	/// Check input and toggle monoscopic rendering mode if necessary
	/// See the input mapping setup in the Unity Integration guide
	/// </summary>
	void Update() {
		// NOTE: some of the buttons defined in OVRGamepadController.Button are not available on the Android game pad controller
		if ( Input.GetButtonDown( OVRGamepadController.ButtonNames[(int)toggleButton] ) ) {
			//*************************
			// toggle monoscopic rendering mode
			//*************************
			Monoscopic = !Monoscopic;
			cameraController.Monoscopic = Monoscopic;
		}
	}

}
