using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCameraFollow : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private float PanSpeed = 12;

    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed = 12;

    [SerializeField] [FoldoutGroup("Settings")]
    private float ScreenBorderPadding = 5;

    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3 Offset;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Image TransitionEffect;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character CharacterTarget;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character HeldTarget;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 TargetPosition;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector2 ControlInput;


    private void OnEnable() {
        PlayerCharacter.OnPlayerCharacterPossession.AddListener(SetCharacterTarget);
    }

    private void OnDisable() {
        PlayerCharacter.OnPlayerCharacterPossession.RemoveListener(SetCharacterTarget);
    }

    private void SetCharacterTarget(Character c) {
        HeldTarget = c;
        CharacterTarget = null;
        if (Vector3.Distance(transform.position, HeldTarget.transform.position) > 25f) {
            StopCoroutine(DoTransitionVfx());
            StartCoroutine(DoTransitionVfx());
        }
        else {
            CharacterTarget = c;
        }
    }

    private void Update() {
        if (CharacterTarget != null) return;
        var mousePos = Input.mousePosition;
        ControlInput.x = mousePos.x < ScreenBorderPadding ? -1 :
            mousePos.x > Screen.width - ScreenBorderPadding ? 1 : 0;
        ControlInput.y = mousePos.y < ScreenBorderPadding ? -1 :
            mousePos.y > Screen.height - ScreenBorderPadding ? 1 : 0;
        TargetPosition += new Vector3(ControlInput.x * PanSpeed * Time.deltaTime, 0,
            ControlInput.y * PanSpeed * Time.deltaTime);
    }

    private void FixedUpdate() {
        if (CharacterTarget != null) {
            TargetPosition = CharacterTarget.transform.position;
        }

        transform.position = Vector3.Lerp(transform.position, TargetPosition + Offset,
            MoveSpeed * Time.deltaTime);
    }

    IEnumerator DoTransitionVfx() {
        TransitionEffect.fillAmount = 0;
        while (TransitionEffect.fillAmount < 1) {
            TransitionEffect.fillAmount =
                Mathf.MoveTowards(TransitionEffect.fillAmount, 1, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.25f);
        TransitionEffect.fillAmount = 1;
        CharacterTarget = HeldTarget;
        transform.position = CharacterTarget.transform.position + Offset;

        while (TransitionEffect.fillAmount > 0) {
            TransitionEffect.fillAmount =
                Mathf.MoveTowards(TransitionEffect.fillAmount, 0, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }
}