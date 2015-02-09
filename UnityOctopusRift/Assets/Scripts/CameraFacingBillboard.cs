using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnWillRenderObject () {
		transform.LookAt(transform.position + Camera.current.transform.rotation * Vector3.back,
		                 Camera.current.transform.rotation * Vector3.up);
	}
}
