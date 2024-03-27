using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ValuePopupDisplay : MonoBehaviour {
    
    [SerializeField] [FoldoutGroup("Hooks")] private TextMeshProUGUI ValueText;
    
    public void SetValue(string value, Color c, float size) {
        ValueText.text = value;
        ValueText.color = c;
        ValueText.fontSize = size;
        transform.position += new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), Random.Range(-.1f, .1f));
    }

    public void Disable() {
        gameObject.SetActive(false);
    }
}
