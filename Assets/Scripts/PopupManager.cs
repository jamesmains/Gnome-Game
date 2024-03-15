using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PopupManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject ValuePopupDisplayObject;

    private static PopupManager Instance;

    private void Awake() {
        Instance = this;
    }

    public static void DisplayWorldValuePopup(int value, Vector3 location) {
        var obj = Pooler.Instance.SpawnObject(Instance.ValuePopupDisplayObject, location);
        obj.GetComponent<ValuePopupDisplay>().SetValue(value.ToString());
    }
}
