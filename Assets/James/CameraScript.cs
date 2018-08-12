using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	Vector3 lookAtVec;

	// Use this for initialization
	void Start () {
		lookAtVec = new Vector3(150.0f, 0.0f, 100.0f);
	}
	
	// Update is called once per frame
	void Update () {
		float speed = 1.0f;
		Vector3 dir = Vector3.zero;
		Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
		if (Input.GetKey (KeyCode.W)) {
			dir += forward;
		}
		if (Input.GetKey (KeyCode.S)) {
			dir -= forward;
		}
		if (Input.GetKey (KeyCode.A)) {
			dir -= right;
		}
		if (Input.GetKey (KeyCode.D)) {
			dir += right;
		}
		dir.Normalize ();

		Camera.main.transform.position += dir * speed;
	}
}
