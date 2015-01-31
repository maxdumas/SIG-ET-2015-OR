/************************************************************************************

Filename    :   IPDAdjustSample.cs
Content     :   An example of how to interactively adjust the IPD 
Created     :   June 30, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;

public class IPDAdjustSample : MonoBehaviour {

	public OVRCameraController		cameraController = null;
	public float   					IPDIncrement = 0.0025f;

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
	/// Interactively adjust the IPD
	/// </summary>
	void Update() {
		
		if ( Input.GetButtonDown( "Right Shoulder" ) ) {
			//*************************
			// Increase IPD
			//*************************
			cameraController.IPD += IPDIncrement;
		} else if ( Input.GetButtonDown( "Left Shoulder" ) ) {
			//*************************
			// Decrease IPD
			//*************************
			cameraController.IPD -= IPDIncrement;
			if ( cameraController.IPD < 0.0f ) {
				cameraController.IPD = 0.0f;
			}
		} else if ( Input.GetButtonDown( "Start" ) ) {
			//*************************
			// Reset the IPD
			//*************************
			float defaultIPD = 0.063f;	// women = 0.062f, men = 0.064f, avg = 0.063f
			if ( OVRDevice.GetIPD( ref defaultIPD ) ) {
				cameraController.IPD = defaultIPD;
			}
		}

		//Debug.LogError ( "IPD: " + cameraController.IPD.ToString ( "F4" ) );
	}

	
}
