using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PopupManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3 CritVfxOffset;

    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3 NumberVfxOffset;

    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject ValuePopupDisplayObject;

    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject CritHitObject;

    private static PopupManager Instance;

    private void Awake() {
        Instance = this;
    }

    public static void DisplayCritVfx(Vector3 location) {
        location += Instance.CritVfxOffset;
        Pooler.Instance.SpawnObject(Instance.CritHitObject, location);
    }

    public static void DisplayWorldValuePopup(int value, Vector3 location) {
        location += Instance.NumberVfxOffset;
        var obj = Pooler.Instance.SpawnObject(Instance.ValuePopupDisplayObject, location);
        obj.GetComponent<ValuePopupDisplay>().SetValue(value.ToString());
    }
}