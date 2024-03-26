using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TextDisplayFilter : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private TextMeshProUGUI TextDisplay;
    [SerializeField] [FoldoutGroup("Settings")]
    private int LineLimit;
    [SerializeField] [FoldoutGroup("Status")] [Multiline]
    private string RawText;
    
    [SerializeField] [FoldoutGroup("Debug")]
    private int index;

    [Button]
    private void ValidateText() {
        string s = "";
        index = 0;
        TextDisplay.text = s;
        foreach (var c in RawText) {
            if (index >= LineLimit) {
                if (c != ' ') {
                    s += c; 
                    continue;
                }
                else {
                    s += "<br>";
                    index = 0;
                    continue;
                }
            }
            index++;
            s += c;
        }

        TextDisplay.text = s;
    }

    private void OnValidate() {
        //ValidateText();
    }
}
