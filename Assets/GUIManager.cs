using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public Text timeOfDayText;
	public CarDetailView selectedCarDetailView;

	private GameManager gameManager;

	private GameObject selectedObject;

	private RectTransform selectedObjectUIElement;


	// Use this for initialization
	void Start () {
		gameManager = GameManager.Instance;
	}

	// Update is called once per frame
	void Update () {
		int hour = gameManager.GetHour();
		string ampmString = "am";
		if(hour > GameManager.HOURS_PER_DAY / 2) {
			hour -= GameManager.HOURS_PER_DAY / 2;
			ampmString = "pm";
		}
		timeOfDayText.text = string.Format("{0}:{1}{2}", hour.ToString("D2"), gameManager.GetMinute().ToString("D2"), ampmString);
	
		if (selectedObject == null) {
			selectedObject = null;
			if(selectedObjectUIElement != null) {
				selectedObjectUIElement.gameObject.SetActive(false);
				selectedObjectUIElement = null;
			}
		}
	}

	private void ChangeSelectedObject(GameObject newSelectedObject, RectTransform newUIElement) {
		if(selectedObjectUIElement != null) {
			selectedObjectUIElement.gameObject.SetActive(false);
		}

		selectedObject = newSelectedObject;
		selectedObjectUIElement = newUIElement;

		if(selectedObjectUIElement != null) {
			selectedObjectUIElement.gameObject.SetActive(true);
		}
	}

	public void SelectNone() {
		selectedObject = null;

		if(selectedObjectUIElement != null) {
			selectedObjectUIElement.gameObject.SetActive(false);
			selectedObjectUIElement = null;
		}
	}

	public void SelectCar(CarPathfinder car) {
		if (selectedObject == car.gameObject) {
			return;
		}

		selectedCarDetailView.activeCar = car;
		ChangeSelectedObject(car.gameObject, selectedCarDetailView.GetComponent<RectTransform>());
	}
}
