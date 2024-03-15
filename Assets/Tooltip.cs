using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Vector3 offset;
    [FoldoutGroup("Details")][SerializeField] private RectTransform rect;
    [FoldoutGroup("Details")][SerializeField] private Canvas scaler;

    // protected override void OnEnable()
    // {
    //     base.OnEnable();
    //     GameEvents.OnShowTooltip += ShowMessage;
    //     GameEvents.OnHideTooltip += Hide;
    // }
    //
    // protected override void OnDisable()
    // {
    //     base.OnDisable();
    //     GameEvents.OnShowTooltip -= ShowMessage;
    //     GameEvents.OnHideTooltip -= Hide;
    // }

    private void ShowMessage(string message)
    {
        messageText.text = message;
        //Show();
    }

    private void Update()
    {
        //if (!isActive) return;
        var targetPosition = (Input.mousePosition / scaler.scaleFactor) +offset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, 0, 1920 - (rect.rect.width)); // TODO: investigate if this needs to be using reference resolution or actual
        targetPosition.y = Mathf.Clamp(targetPosition.y, 0, 1080 - (rect.rect.height));
        rect.anchoredPosition = targetPosition;
    }
}
