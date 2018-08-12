using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButtonDown (0)){ 
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if ( Physics.Raycast (ray,out hit)) {
				Clickable clickable = hit.transform.gameObject.GetComponent<Clickable>();
				if(clickable != null) {
					clickable.onLeftClick.Invoke();
				}
			}
		}

		if ( Input.GetMouseButtonDown (1)){ 
			RaycastHit hit; 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if ( Physics.Raycast (ray,out hit)) {
				Clickable clickable = hit.rigidbody.gameObject.GetComponent<Clickable>();
				if(clickable != null) {
					clickable.onRightClick.Invoke();
				}
			}
		}
	}
}
