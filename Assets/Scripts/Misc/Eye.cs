using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour {

	public ParticleSystem deathParticles;  	
	float cutoffValue = 0f;
	bool triggered = false;

	void Update () {
		cutoffValue = Mathf.Lerp(cutoffValue, 1f, 0.8f * Time.deltaTime);
		GetComponent<Renderer>().materials[0].SetFloat("_Cutoff", cutoffValue);

		if (cutoffValue >= 0.8f && !triggered) {
			deathParticles.Stop();
			Destroy(gameObject, 1.5f);
			triggered = true;
		}
	}
}
