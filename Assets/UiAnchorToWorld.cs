using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UiAnchorToWorld : MonoBehaviour {
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 Padding;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private bool KeepWithinScreen = true;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject WorldAnchor;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    private RectTransform ChainedObject;

    private void Update() {
        var uiPos = Camera.main.WorldToScreenPoint(WorldAnchor.transform.position);
        var offset = ChainedObject.sizeDelta + Padding;
        if (KeepWithinScreen) {
            uiPos.x = Mathf.Clamp(uiPos.x, 0+offset.x, Screen.width-offset.x);
            uiPos.y = Mathf.Clamp(uiPos.y, 0+offset.y, Screen.height-offset.y);    
        }
        ChainedObject.transform.position = uiPos;
    }
}