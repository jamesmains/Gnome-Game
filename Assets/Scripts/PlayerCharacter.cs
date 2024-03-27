using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : Character {
    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed = 250f;

    [SerializeField] [FoldoutGroup("Settings")]
    private float TetherMaxDistance;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool DebugFlagAutoSelectCharacter;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Transform NonPcWeaponBulletOrigin;

    [SerializeField] [FoldoutGroup("Status")] //[ReadOnly]
    private List<Entity> Group = new();

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
        HandleInputs();
    }

    protected virtual void HandleInputs() {
        if (CurrentCharacter == this) {
            HandlePlayerControlledInputs();
        }
        else {
            HandleComputerControlledInputs();
        }
    }
    
    protected virtual void HandlePlayerControlledInputs() {
        if (Input.GetMouseButton(0) && !MouseOverUserInterfaceUtil.IsMouseOverUI)
            HeldWeapon.Fire(AimController.PlayerAim.BulletSpawnPoint);

        playerInput.x = Input.GetAxisRaw("Horizontal");
        playerInput.z = Input.GetAxisRaw("Vertical");
    }
    
    protected virtual void HandleComputerControlledInputs() {
        if (FocussedEntity == null) return;
        float v = NavAgent.velocity.sqrMagnitude;
        bool canAttack = (v != 0 && CanAttackWhileMoving) ||
                         (v == 0 && CanAttackWhileNotMoving);
        canAttack = canAttack && SelfEntity.CanSeeEntity(FocussedEntity) &&
                    SelfEntity.WithinReachOfEntity(FocussedEntity, IdealInteractionRange);
        if (!canAttack) return;
        var dest = FocussedEntity.transform.position;
        dest.y = NonPcWeaponBulletOrigin.position.y;
        HeldWeapon.FireTowards(NonPcWeaponBulletOrigin, dest);
    }

    protected override void HandleAnimation() {
        if (CurrentCharacter == this) {
            var isMoving = Rb.velocity.sqrMagnitude > 0;
            Anim.SetBool(IsMoving, isMoving);
        }
        else base.HandleAnimation();
    }

    protected override void HandleMovement() {
        var thisPosition = transform.position;
        if (CurrentCharacter == this) {
            var dir = playerInput.normalized * (Time.deltaTime * MoveSpeed);
            TargetPosition = thisPosition + dir;
            Rb.AddForce(dir, ForceMode.Impulse);
        }
        else {
            if (FocussedEntity == null || FocussedEntity.IsDead ||
                !SelfEntity.WithinReachOfEntity(FocussedEntity, IdealInteractionRange) ||
                !SelfEntity.CanSeeEntity(FocussedEntity)) {
                FocussedEntity = SelfEntity.FindNearestEnemy();
            }

            var groupMemeber = Group.FirstOrDefault(o => o != SelfEntity);
            if (groupMemeber != null) {
                TargetPosition = groupMemeber.transform.position;
                if (TargetPosition != NavAgent.destination) {
                    var d = Vector3.Distance(thisPosition, TargetPosition);
                    var dir = (TargetPosition - transform.position);
                    if (d <= RepelFromOthersRange) {
                        TargetPosition = TargetPosition += dir;
                    }

                    NavAgent.SetDestination(TargetPosition);
                }
            }
            else if (FocussedEntity != null) {
                TargetPosition = FocussedEntity.transform.position;
                if (TargetPosition != NavAgent.destination) {
                    var d = Vector3.Distance(thisPosition, TargetPosition);
                    var dir = (transform.position - TargetPosition).normalized;
                    if (d <= RepelFromOthersRange)
                        TargetPosition = TargetPosition += dir;
                    NavAgent.SetDestination(TargetPosition);
                }
            }
            else {
                TargetPosition = thisPosition;
                if (TargetPosition != NavAgent.destination) {
                    NavAgent.SetDestination(TargetPosition);
                }
            }

            var dist = Vector3.Distance(thisPosition, TargetPosition);
            NavAgent.isStopped = (dist < TetherMaxDistance && dist > RepelFromOthersRange);

            transform.position =
                Vector3.MoveTowards(transform.position, NavAgent.nextPosition, Time.deltaTime * NavAgent.speed);
        }
    }

    public void FocusOnAttacker(Entity self, Entity attacker) {
        if (CurrentCharacter == this) return;
        FocussedEntity = attacker;
    }

    [Button]
    public void Possess() {
        if (CurrentCharacter != null)
            CurrentCharacter.TargetPosition = CurrentCharacter.transform.position;
        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}