using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestParticleController : MonoBehaviour {
	public ParticleSystem DefaultEmitter;
	public CustomParticleEmitter CustomEmitter;
	public Text Display;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(CustomEmitter != null) {
			if(Input.touchCount > 0 || Input.anyKey) {
				CustomEmitter.CreateRandomParticle();
			}

			Display.text = string.Format("{0} | {1:F0}", CustomEmitter.CurrentParticles, 1 / Time.deltaTime);
		} else if(DefaultEmitter != null) {
			if(Input.touchCount > 0 || Input.anyKey) {
				DefaultEmitter.Emit(1);
			}
			
			Display.text = string.Format("{0} | {1:F0}", DefaultEmitter.particleCount, 1 / Time.deltaTime);
		}
	}
}
