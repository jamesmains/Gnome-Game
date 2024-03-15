using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private Transform CharacterSelectContent;

    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject CharacterSelectObj;

    void Start() {
        var playerCharacters = FindObjectsOfType<PlayerCharacter>();
        foreach (var character in playerCharacters) {
            var obj = Instantiate(CharacterSelectObj, CharacterSelectContent);
            obj.GetComponent<CharacterSelectButton>().AssignCharacter(character);
        }
    }
}