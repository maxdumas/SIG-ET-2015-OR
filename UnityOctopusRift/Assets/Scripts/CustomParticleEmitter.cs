using UnityEngine;
using System.Collections;

public class CustomParticleEmitter : MonoBehaviour {
	public GameObject ParticlePrefab;

	public int MaxParticles = 500;
	public int CurrentParticles = 0;
	public bool PreWarm = true;
	
	private Bounds _emitterBounds;
	private Collider _attachedCollider;

	// Use this for initialization
	void Start () {
		// We use a Collider component attached to the game object to define the bounds
		// in which to generate particles. Note that these bounds will always define a box,
		// even if the attached Collider is not a BoxCollider.
		_attachedCollider = GetComponent<Collider>();
		if(_attachedCollider == null) {
			Debug.LogError("No Collider component attached to Custom Particle Emitter!");
		} else {
			_emitterBounds = _attachedCollider.bounds;
		}
		
		if(PreWarm) {
			while(CurrentParticles < MaxParticles) {
				CreateRandomParticle();
			}
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Generates a particle at a random position within the bounds of the emitter
	/// </summary>
	public void CreateRandomParticle () {
		Vector3 max = _emitterBounds.max;
		Vector3 min = _emitterBounds.min;

		Vector3 p = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

		CreateParticle(p);
	}

	/// <summary>
	/// Genereates a particle at the specified coordinates
	/// </summary>
	/// <param name="p">The point at which to generate the particle</param>
	/// <param name="ignoreBounds">If set to <c>false</c> points outside the bounds of this emitter
	/// will be adjusted to fit into the emitter bounds.</param>
	public void CreateParticle(Vector3 p, bool ignoreBounds = false) {
		if(!ignoreBounds && !_emitterBounds.Contains(p)) {
			Debug.Log ("Bad point");
			p = _attachedCollider.ClosestPointOnBounds(p);
		}

		var g = (GameObject) Instantiate(ParticlePrefab, p, Quaternion.identity);
		g.transform.parent = this.transform;
		++CurrentParticles;
	}
}
