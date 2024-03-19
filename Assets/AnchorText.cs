using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class AnchorText : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 TextPadding;
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject AnchorObject;

    [SerializeField] [FoldoutGroup("Hooks")]
    private TextMeshProUGUI AnchoredText;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    private RectTransform AnchoredTextRect;

    private void Update() {
        var uiPos = Camera.main.WorldToScreenPoint(AnchorObject.transform.position);
        var offset = AnchoredTextRect.sizeDelta + TextPadding;
        uiPos.x = Mathf.Clamp(uiPos.x, 0+offset.x, Screen.width-offset.x);
        uiPos.y = Mathf.Clamp(uiPos.y, 0+offset.y, Screen.height-offset.y);
        AnchoredText.transform.position = uiPos;
    }
}