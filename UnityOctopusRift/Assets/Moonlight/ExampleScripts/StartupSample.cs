/************************************************************************************

Filename    :   StartupSample.cs
Content     :   An example of how to set up your game for fast loading with a
			:	black splash screen, and a small logo screen that triggers an
			:	async main scene load
Created     :   June 27, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;				// required for Coroutines

public class StartupSample : MonoBehaviour {
	
	public float				delayBeforeLoad = 0.0f;
	
	/// <summary>
	/// Start a delayed scene load
	/// </summary>
	void Start () {
		// all applications should run at 60fps
		Application.targetFrameRate = 60;
		
		// start the main scene load
		StartCoroutine( DelayedSceneLoad() );
	}
	
	/// <summary>
	/// Asynchronously start the main scene load
	/// </summary>
	IEnumerator DelayedSceneLoad() {
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		// this is *ONLY* here for example as our 'main scene' will load too fast
		// remove this for production builds or set the time to 0.0f
		yield return new WaitForSeconds( delayBeforeLoad );
		
		//*************************
		// load the scene asynchronously.
		// this will allow the player to 
		// continue looking around in your loading screen
		//*************************
		float startTime = Time.realtimeSinceStartup;
		AsyncOperation async = Application.LoadLevelAsync( 1 );
		yield return async;
		Debug.Log( "[SceneLoad] Completed: " + ( Time.realtimeSinceStartup - startTime ).ToString( "F2" ) + " sec(s)" );
	}
	
}
