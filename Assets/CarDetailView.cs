using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDetailView : MonoBehaviour {

	public Text carDetailText;

	public CarPathfinder activeCar;

	private RectTransform rectTransform;

	public Vector3 offset = new Vector3(0.0f, 2.5f, 0.0f);

	// Use this for initialization
	void Start () {
		rectTransform = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (activeCar == null) {
			return;
		}

		rectTransform.position = Camera.main.WorldToScreenPoint(activeCar.transform.position + offset) + offset * 15.0f;

		carDetailText.text = string.Format(
			"Origin: {0}\nDestination: {1}\nCurrent Location: {2}",
			activeCar.originalStart.location,
			activeCar.originalEnd.location,
			activeCar.GetCurrentLocation()
		);
	}
}
