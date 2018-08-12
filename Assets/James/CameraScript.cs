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
		transform.position = lookAtVec + new Vector3(150 * Mathf.Cos(Time.time / 4.0f), 50.0f, 150 * Mathf.Sin(Time.time / 4.0f));
		transform.LookAt(lookAtVec);
	}
}
