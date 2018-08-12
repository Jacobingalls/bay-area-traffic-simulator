using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDetailView : MonoBehaviour {

	public Text carDetailText;

	public CarPathfinder activeCar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (activeCar == null) {
			return;
		}

		carDetailText.text = string.Format(
			"Origin: {0}\nDestination: {1}\nCurrent Location: {2}\n\nTime on Road: {3}\n\nStatus: {4}",
			activeCar.originalStart.location,
			activeCar.originalEnd.location,
			activeCar.GetCurrentLocation(),
			0.0f,
			"???"
		);
	}
}
