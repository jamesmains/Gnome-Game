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
        foreach (var c in RawText) {
            if (index >= LineLimit || (int)c == 10) {
                if (c == ' ' || (int)c == 10) {
                    s += "<br>";
                    index = 0;
                    continue;
                }
                else {
                    s += c; 
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
