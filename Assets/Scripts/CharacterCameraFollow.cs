using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCameraFollow : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character characterTarget;
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character heldTarget;
    
    [SerializeField] private Vector3 offset;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Image tempTransitionEffect;

    private void OnEnable() {
        PlayerCharacter.OnPlayerCharacterPossession.AddListener(SetCharacterTarget);
    }

    private void OnDisable() {
        PlayerCharacter.OnPlayerCharacterPossession.RemoveListener(SetCharacterTarget);
    }

    private void SetCharacterTarget(Character c) {
        heldTarget = c;
        characterTarget = null;
        if (Vector3.Distance(transform.position, heldTarget.transform.position) > 25f) {
            StopCoroutine(DoTransitionVfx());
            StartCoroutine(DoTransitionVfx());
        }
        else {
            characterTarget = c;
        }
    }

    private void FixedUpdate() {
        if (characterTarget == null) return;
        transform.position = Vector3.Lerp(transform.position, characterTarget.transform.position + offset,
            moveSpeed * Time.deltaTime);
    }

    IEnumerator DoTransitionVfx() {
        tempTransitionEffect.fillAmount = 0;
        while (tempTransitionEffect.fillAmount < 1) {
            tempTransitionEffect.fillAmount =
                Mathf.MoveTowards(tempTransitionEffect.fillAmount, 1, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.25f);
        tempTransitionEffect.fillAmount = 1;
        characterTarget = heldTarget;
        transform.position = characterTarget.transform.position + offset;
        
        while (tempTransitionEffect.fillAmount > 0) {
            tempTransitionEffect.fillAmount =
                Mathf.MoveTowards(tempTransitionEffect.fillAmount, 0, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
    }
}
