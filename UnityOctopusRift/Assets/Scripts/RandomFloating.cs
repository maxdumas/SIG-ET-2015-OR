using UnityEngine;
using System.Collections;

public class RandomFloating : MonoBehaviour {

	public float MaxVelocity = 1f;
	public float MaxRadianDelta = 0.001f;
	public float MaxMagnitudeDelta = 0.0005f;
	public Vector3 CurrentVelocity;
	private Vector3 _nextVelocity;

	// Use this for initialization
	void Start () {
		CurrentVelocity = Random.insideUnitSphere * MaxVelocity;
		_nextVelocity = Random.insideUnitSphere * MaxVelocity;
	}
	
	// Update is called once per frame
	void Update () {
//		CurrentVelocity = MaxVelocity * new Vector3(Mathf.PerlinNoise(transform.position.x, 0), Mathf.PerlinNoise(0, transform.position.y));
		if((CurrentVelocity - _nextVelocity).sqrMagnitude < 0.05) {
			CurrentVelocity = _nextVelocity;
			_nextVelocity = Random.insideUnitSphere * MaxVelocity;
		} else {
			CurrentVelocity = Vector3.RotateTowards(CurrentVelocity, _nextVelocity, MaxRadianDelta, MaxMagnitudeDelta);
		}

		transform.position += CurrentVelocity * Time.deltaTime;
	}
}
