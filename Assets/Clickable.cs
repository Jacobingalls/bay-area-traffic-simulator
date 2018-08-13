using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour {
	public UnityEvent onLeftClick;
	public UnityEvent onRightClick;

	void Start()
    {
        if (onLeftClick == null)
            onLeftClick = new UnityEvent();

		if (onRightClick == null)
            onRightClick = new UnityEvent();
    }

}
