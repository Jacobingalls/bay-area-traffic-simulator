using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject
{
    public event System.Action OnValuesUpdates;
    public bool autoUpdate;


    protected virtual void OnValidate()
    {
        if(autoUpdate)
        {
    #if UnityEditor

            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
    #endif
        }
    }

    public void NotifyOfUpdatedValues()
    {
    #if UnityEditor

        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
    #endif

        if (OnValuesUpdates != null)
        {
            OnValuesUpdates();
        }
    }
}