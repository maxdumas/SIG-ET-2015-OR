/************************************************************************************

Filename    :   HomeBattery.cs
Content     :   An example of how to show the remaining battery level on the menu
Created     :   June 30, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;

public class HomeBattery : MonoBehaviour {

	public Gradient		batteryTempGradient = new Gradient();

	private Transform	juiceLevel = null;
	private Material	batteryMaterial = null;

	/// <summary>
	/// Initialization
	/// </summary>
	void Awake() {
		juiceLevel = transform.FindChild( "juice" );
		if ( juiceLevel == null ) {
			Debug.LogError ( "ERROR: battery juice child not found " + name );
			enabled = false;
			return;
		}
		// clone the battery material
		batteryMaterial = juiceLevel.renderer.material;
		OnRefresh();
	}

	/// <summary>
	/// Clean up cloned material
	/// </summary>	
	void OnDestroy() {
		if ( batteryMaterial != null ) {
			Destroy( batteryMaterial );
		}
	}

	/// <summary>
	/// Message handler that is called before the menu is redisplayed
	/// </summary>
	void OnRefresh() {
		Debug.Log( "> Device Battery Level: " + OVRDevice.GetBatteryLevel() );
		Vector3 scale = juiceLevel.localScale;
		scale.x = OVRDevice.GetBatteryLevel();
		juiceLevel.localScale = scale;

		Debug.Log( "> Battery Temp: " + OVRDevice.GetBatteryTemperature() + "C" );
		// 30 degrees C == green/cool, 45 degrees C == red/hot
		float colorScale = Mathf.InverseLerp( 30.0f, 45.0f, OVRDevice.GetBatteryTemperature() );
		Color juiceColor = batteryTempGradient.Evaluate( colorScale );
		batteryMaterial.color = juiceColor;

	}
}
