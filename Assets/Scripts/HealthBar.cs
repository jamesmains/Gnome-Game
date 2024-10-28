using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private Image FillBar;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float TargetValue;

    private void Update() {
        transform.LookAt(Camera.main.transform);
    }

    public void UpdateHealthBar(Entity Self) {
        TargetValue = (float)Self.Health / (float)Self.StartingHealth;
        FillBar.fillAmount = TargetValue;
    }
    
    public void UpdateHealthBar(Entity Self, Entity Ignore) {
        TargetValue = (float)Self.Health / (float)Self.StartingHealth;
        FillBar.fillAmount = TargetValue;
    }
}
