using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target; 	
	public float smoothing = 5f;     
	public GameObject minimap;
	Vector3 offset;

	void Start() {
		offset = transform.position - target.position;
	}

	void FixedUpdate () {
		Vector3 targetCamPos = target.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}

	void Update () {
		if (Input.GetButtonDown("MiniMap")) {
			minimap.SetActive(!minimap.activeInHierarchy);
		}
	}
}