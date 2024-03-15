using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private Button SelectButton;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Image SelectHighlightImage;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Image CharacterImage;

    private static Image currentSelectedHighlight;

    public void AssignCharacter(PlayerCharacter character) {
        SelectButton.onClick.RemoveAllListeners();
        SelectButton.onClick.AddListener(delegate { SelectCharacter(character); });
        CharacterImage.sprite = character.CharacterSprite;
    }

    private void SelectCharacter(PlayerCharacter character) {
        if (currentSelectedHighlight != null) currentSelectedHighlight.gameObject.SetActive(false);
        currentSelectedHighlight = SelectHighlightImage;
        currentSelectedHighlight.gameObject.SetActive(true);
        character.Possess();
    }
}