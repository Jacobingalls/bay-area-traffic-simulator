using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(50 * Mathf.Cos(Time.time), 0.0f, 50 * Mathf.Sin(Time.time));
		transform.LookAt(Vector3.zero);
	}
}
