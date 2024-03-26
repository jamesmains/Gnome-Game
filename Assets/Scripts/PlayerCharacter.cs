using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : Character {
    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed = 250f;

    [SerializeField] [FoldoutGroup("Settings")]
    private float GroupTetherMaxDistance;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool DebugFlagAutoSelectCharacter = false;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Transform NonPcWeaponBulletOrigin;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 playerInput;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Entity FocussedEntity;

    public static PlayerCharacter CurrentCharacter;
    public static UnityEvent<PlayerCharacter> OnPlayerCharacterPossession = new();

    private void Start() {
        if (DebugFlagAutoSelectCharacter)
            Possess();
    }

    protected override void Update() {
        base.Update();
        if (CurrentCharacter == this) {
            if (Input.GetMouseButton(0) && !MouseOverUserInterfaceUtil.IsMouseOverUI)
                HeldWeapon.Fire(AimController.PlayerAim.BulletSpawnPoint);

            playerInput.x = Input.GetAxisRaw("Horizontal");
            playerInput.z = Input.GetAxisRaw("Vertical");
        }
        else {
            if (FocussedEntity == null) return;
            float v = NavAgent.velocity.sqrMagnitude;
            bool canAttack = (v != 0 && CanAttackWhileMoving) ||
                             (v == 0 && CanAttackWhileNotMoving);
            if (!canAttack) return;
            var dest = FocussedEntity.transform.position;
            dest.y = NonPcWeaponBulletOrigin.position.y;
            HeldWeapon.FireTowards(NonPcWeaponBulletOrigin,dest);
        }
    }

    protected override void HandleAnimation() {
        if (CurrentCharacter == this) {
            var isMoving = Rb.velocity.sqrMagnitude > 0;
            Anim.SetBool(IsMoving, isMoving);
        }
        else base.HandleAnimation();
    }

    protected override void HandleMovement() {
        var playerPosition = PlayerCharacter.CurrentCharacter.transform.position;
        if (CurrentCharacter == this) {
            var dir = playerInput.normalized * (Time.deltaTime * MoveSpeed);
            TargetPosition = transform.position + dir;
            Rb.AddForce(dir, ForceMode.Impulse);
        }
        else {
            if (FocussedEntity == null || FocussedEntity.IsDead) {
                FocussedEntity = SelfEntity.FindNearestEnemy();
            
            }
            
            TargetPosition = playerPosition;
            if (TargetPosition != NavAgent.destination &&
                Vector3.Distance(transform.position, playerPosition) > GroupTetherMaxDistance) {
                NavAgent.SetDestination(TargetPosition);
                print("setting destination");
            }

            transform.position =
                Vector3.MoveTowards(transform.position, NavAgent.nextPosition, Time.deltaTime * NavAgent.speed);
        }
    }

    [Button]
    public void Possess() {
        print(gameObject.name);
        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}