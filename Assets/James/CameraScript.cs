using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	[Range(5.0f, 100.0f)]
	public float minY = 5.0f;

	[Range(5.0f, 100.0f)]
	public float maxY = 100.0f;

	[Range(0.0f, 5.0f)]
	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		float multiplier = (Camera.main.transform.position.y / maxY);
		float speedMultiplier = speed * multiplier;
		Vector3 dir = Vector3.zero;
		Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
		Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
		if(Input.mouseScrollDelta != Vector2.zero) {
			Vector3 newPos = Camera.main.transform.position + transform.forward * Input.mouseScrollDelta.y * speedMultiplier;

			if(newPos.y <= maxY && newPos.y >= minY) {
				Camera.main.transform.position = newPos;
			}
		}
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
			dir += forward;
		}
		if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
			dir -= forward;
		}
		if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
			dir -= right;
		}
		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
			dir += right;
		}
		dir.Normalize ();

		Camera.main.transform.position += dir * speedMultiplier;
	}
}
