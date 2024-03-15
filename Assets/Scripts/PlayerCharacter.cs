using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : Character {

    [SerializeField] [FoldoutGroup("Hooks")]
    private Weapon CurrentWeapon;

    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed = 250f;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 IdealAttackRange;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool DebugFlagAutoSelectCharacter = false;

    private Vector3 playerInput;
    public static Character CurrentCharacter;
    public static UnityEvent<Character> OnPlayerCharacterPossession = new();

    protected override void Awake() {
        base.Awake();
        if (DebugFlagAutoSelectCharacter)
            Possess();
    }

    public void FixedUpdate() {
        if (CurrentCharacter == this) {
            Rb.AddForce(playerInput.normalized * (Time.deltaTime * MoveSpeed), ForceMode.Impulse);    
        }
        else {
            
            var playerPosition = CurrentCharacter.transform.position;
            var pos = transform.position;
            var dir = playerPosition - pos;
            var dist = Vector3.Distance(playerPosition, pos);
            dir = dir.normalized;
            dir = dist > IdealAttackRange.x ? dir : dist < IdealAttackRange.y ? dir * -1 : Vector3.zero;
            Rb.AddForce(dir * (Time.deltaTime * MoveSpeed), ForceMode.Impulse);
        }
    }

    protected override void Update() {
        base.Update();
        if (CurrentCharacter != this) return;
        
        if(Input.GetMouseButton(0) && !MouseOverUserInterfaceUtil.IsMouseOverUI)
            CurrentWeapon.Fire(AimController.PlayerAim.BulletSpawnPoint);
        
        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
    }

    [Button]
    public void Possess() {
        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}