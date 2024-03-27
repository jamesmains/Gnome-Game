using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PopupManager : MonoBehaviour {

    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3 NumberVfxOffset;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Color StandardTextColor;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Color CritTextColor;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private float StandardFontSize;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private float CritFontSize;

    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject ValuePopupDisplayObject;

    private static PopupManager Instance;

    private void Awake() {
        Instance = this;
    }

    public static void DisplayWorldValuePopup(int value, Vector3 location, bool crit = false) {
        location += Instance.NumberVfxOffset;
        var c = crit ? Instance.CritTextColor : Instance.StandardTextColor;
        var size = crit ? Instance.CritFontSize : Instance.StandardFontSize;
        var obj = Pooler.Instance.SpawnObject(Instance.ValuePopupDisplayObject, location);
        obj.GetComponent<ValuePopupDisplay>().SetValue(value.ToString(), c, size);
    }
}