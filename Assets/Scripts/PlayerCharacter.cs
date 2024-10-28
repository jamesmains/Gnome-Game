using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerCharacter : Character {
    
    [SerializeField] [FoldoutGroup("Settings")] 
    private float TetherDistance = 1f;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Furthest the minion can get away from the Leader")]
    private float LeashDistance = 5f;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private float SightDistance = 5f;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Transform NonPcWeaponBulletOrigin;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character Leader;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 PlayerInput;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float NextMinionSeek;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Entity> SeekIgnoreList = new();
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Entity> FocusIgnoreList = new();

    public static PlayerCharacter CurrentCharacter;
    public static UnityEvent<PlayerCharacter> OnPlayerCharacterPossession = new();

    private const float SeekMinionRate = 0.25f;

    protected override void OnDisable() {
        base.OnDisable();
        SelfEntity.IsGrouped = false;
        Leader = null;
    }

    protected override void Update() {
        base.Update();
        HandleInputs();
    }

    protected virtual void HandleInputs() {
        if (SelfEntity.IsDead) return;
        if (CurrentCharacter == this) {
            HandlePlayerControlledInputs();
        }
        else {
            HandleComputerControlledInputs();
        }
    }

    // Todo - Convert to new input system
    protected virtual void HandlePlayerControlledInputs() {
        if (Input.GetMouseButton(0) && !
                MouseOverUserInterfaceUtil.IsMouseOverUI && HeldWeapon != null &&
            HeldWeapon.Settings != null)
            HeldWeapon.Fire(AimController.PlayerAim.BulletSpawnPoint);

        PlayerInput.x = Input.GetAxisRaw("Horizontal");
        PlayerInput.z = Input.GetAxisRaw("Vertical");
    }

    protected virtual void HandleComputerControlledInputs() {
        if (FocussedEntity == null) return;
        float v = NavAgent.velocity.sqrMagnitude;
        if (HeldWeapon == null || HeldWeapon.Settings == null) return;
        bool canAttack = (v != 0 && CanAttackWhileMoving) ||
                         (v == 0 && CanAttackWhileNotMoving);
        canAttack = canAttack && SelfEntity.CanSeeEntity(FocussedEntity) &&
                    SelfEntity.WithinReachOfEntity(FocussedEntity, HeldWeapon.Settings.AttackRange);
        if (!canAttack) return;
        var dest = FocussedEntity.transform.position;
        HeldWeapon.FireTowards(NonPcWeaponBulletOrigin, dest);
    }

    protected override void HandleMovement() {
        if (SelfEntity.IsLeader && Time.time > NextMinionSeek) {
            HandleSeek();
        }

        if (CurrentCharacter == this) {
            HandlePlayerMovement();
        }
        else {
            HandleComputerMovement();
        }

        if (NavAgent.isStopped && this != CurrentCharacter) return;
        transform.position =
            Vector3.MoveTowards(transform.position, NavAgent.nextPosition, Time.deltaTime * NavAgent.speed);
    }


    private void HandleSeek() {
        var nearestAlly = SelfEntity.FindNearestAlly(filter: SeekIgnoreList, seekMinion: true, maxDistance:SightDistance);
        if (nearestAlly == null) {
            SeekIgnoreList.Clear();
            NextMinionSeek = Time.time + SeekMinionRate;
            return;
        }

        if (!SelfEntity.CanSeeEntity(nearestAlly)) {
            SeekIgnoreList.Add(nearestAlly);
            NextMinionSeek = Time.time + SeekMinionRate;
            return;
        }

        if (nearestAlly != null && nearestAlly.TryGetComponent<PlayerCharacter>(out var character)) {
            character.AssignLeader(this);
            nearestAlly.IsGrouped = true;
            SeekIgnoreList.Clear();
        }

        NextMinionSeek = Time.time + SeekMinionRate;
    }

    private void HandlePlayerMovement() {
        var thisPosition = transform.position;

        TargetPosition = thisPosition + PlayerInput.normalized / 2;
        if (NavAgent.destination != TargetPosition)
            NavAgent.SetDestination(TargetPosition);
    }

    private void HandleComputerMovement() {
        var thisPosition = transform.position;

        if (Leader != null && !SelfEntity.IsLeader) {
            FocussedEntity = Leader == CurrentCharacter ? SelfEntity.FindNearestEnemy(maxDistance:SightDistance) : Leader.FocussedEntity;
            bool needsToMoveToFocusTarget = FocussedEntity != null && SelfEntity.CanSeeEntity(FocussedEntity) &&
                                            !SelfEntity.WithinReachOfEntity(FocussedEntity,
                                                HeldWeapon.Settings.AttackRange);
            TargetPosition = needsToMoveToFocusTarget
                ? FocussedEntity.transform.position
                : Leader.transform.position;
            var targetPositionOverride = Vector3.Distance(Leader.transform.position, TargetPosition) > LeashDistance;
            if (targetPositionOverride) {
                TargetPosition = Leader.transform.position;
                FocussedEntity = null;
            }
        }

        else {
            var canReach = false;
            if (FocussedEntity != null) {
                var path = new NavMeshPath();
                canReach = NavAgent.CalculatePath(FocussedEntity.transform.position, path);    
            }
            
            if (FocussedEntity == null || FocussedEntity.IsDead ||
                !canReach) {
                if (FocussedEntity != null) {
                    FocusIgnoreList.Add(FocussedEntity);
                }
                FocussedEntity = SelfEntity.FindNearestEnemy(filter:FocusIgnoreList,maxDistance:SightDistance);
                if (FocussedEntity == null) {
                    FocusIgnoreList.Clear();
                }
            }

            TargetPosition = FocussedEntity ? FocussedEntity.transform.position : thisPosition;
        }

        var dir = (TargetPosition - thisPosition).normalized * TetherDistance;

        TargetPosition -= dir;
        NavAgent.SetDestination(TargetPosition);
    }

    public void AssignLeader(Character newLeader) {
        if (Leader != null) return;
        Leader = newLeader;
    }

    public void FocusOnAttacker(Entity self, Entity attacker) {
        if (CurrentCharacter == this) return;
        FocussedEntity = attacker;
    }

    [Button] [PropertyOrder(99)]
    public void Possess() {
        if (CurrentCharacter != null) {
            CurrentCharacter.TargetPosition = CurrentCharacter.transform.position;
        }

        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}