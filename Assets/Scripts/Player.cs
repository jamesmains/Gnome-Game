using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Possible fix for Local Multiplayer device detection
/// Get device from InputSystem.onEvent -> Func(InputEventPtr, InputDevice) -> store inputDevice
/// Check device with SomeCompareFuncViaContext(InputAction.CallbackContext) -> callbackContext.control.device == storedDevice
/// </summary>
public class Player : MonoBehaviour {
    [SerializeField, FoldoutGroup("Debug")]
    private float CameraMoveSpeed = 10f;
    
    [SerializeField, FoldoutGroup("Debug")]
    private Actor DebugTargetActor;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Actor CurrentActor;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Vector3 MoveInput;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Vector3 LookInput;

    private InputSystem_Actions Input;

    private void OnEnable() {
        if (Input == null) {
            RegisterNewInputSystem();
        }

        Actor.OnPossessed += PossessActor;
        Actor.OnReleasePossession += HandleReleasePossession;
    }


    private void OnDisable() {
        if (Input != null) {
            UnregisterInputSystem();
        }

        Actor.OnPossessed -= PossessActor;
        Actor.OnReleasePossession -= HandleReleasePossession;
    }

    private void RegisterNewInputSystem() {
        Input = new InputSystem_Actions();
        Input.Enable();
        Input.Player.Move.performed += Move;
        Input.Player.Move.canceled += Move;
        Input.Player.Look.performed += Aim;
        Input.Player.Look.canceled += Aim;
        Input.Player.Attack.performed += Attack;
    }

    private void UnregisterInputSystem() {
        Input.Player.Move.performed -= Move;
        Input.Player.Move.canceled -= Move;
        Input.Player.Look.performed -= Aim;
        Input.Player.Look.canceled -= Aim;
        Input.Player.Attack.performed -= Attack;
        Input.Disable();
        Input = null;
    }

    private void Update() {
        CurrentActor?.OnMoveActor?.Invoke(MoveInput,true);
        CurrentActor?.OnAimWeapon?.Invoke(LookInput);
    }

    private void FixedUpdate() {
        // Todo: replace with camera controller if needed
        var targetPosition = CurrentActor ? CurrentActor.transform.position : transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * CameraMoveSpeed);
    }

    [Button]
    private void TryPossess() {
        Actor.OnTryPossess.Invoke(DebugTargetActor);
    }

    private void PossessActor(Actor actor, List<ActorComponent> actorComponents) {
        if (CurrentActor != null) Actor.OnReleasePossession.Invoke(CurrentActor);
        CurrentActor = actor;
    }

    private void HandleReleasePossession(Actor releasedActor) {
        if (CurrentActor == releasedActor) {
            CurrentActor = null;
        }
    }

    private void Move(InputAction.CallbackContext callbackContext) {
        var moveDir = (Vector3)callbackContext.ReadValue<Vector2>();
        moveDir.z = moveDir.y;
        moveDir.y = 0;
        MoveInput = moveDir;
    }

    private void Aim(InputAction.CallbackContext callbackContext) {
        LookInput = callbackContext.ReadValue<Vector2>();
    }

    private void Attack(InputAction.CallbackContext callbackContext) {
        CurrentActor?.OnUseWeapon?.Invoke();
    }
}