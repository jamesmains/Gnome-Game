using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerCharacter : Character {
    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Minimum distance from Leader")]
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
    private List<Entity> MinionSeekIgnoreList = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Entity> FocusIgnoreList = new();

    public static PlayerCharacter CurrentCharacter; // Pray that multiplayer is never a thing...
    public static UnityEvent<PlayerCharacter> OnPlayerCharacterPossession = new();

    private bool NeedsToMoveToFocusTarget => FocussedEntity != null && SelfEntity.CanSeeEntity(FocussedEntity) &&
                                             !SelfEntity.WithinReachOfEntity(FocussedEntity,
                                                 HeldWeapon.Settings.AttackRange);

    private bool IsTooFarFromLeader => Leader != null && !SelfEntity.IsLeader &&
                                       Vector3.Distance(Leader.transform.position, transform.position) > LeashDistance;

    private bool CanAttack(float v) => ((v != 0 && CanAttackWhileMoving) ||
                                        (v == 0 && CanAttackWhileNotMoving)) &&
                                       SelfEntity.CanSeeEntity(FocussedEntity) &&
                                       SelfEntity.WithinReachOfEntity(FocussedEntity, HeldWeapon.Settings.AttackRange);

    private bool CanReachFocusTarget => FocussedEntity != null &&
                                        NavAgent.CalculatePath(FocussedEntity.transform.position, new NavMeshPath());

    private bool HasWeapon => HeldWeapon != null && HeldWeapon.Settings != null;

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
        // There is an issue with Entity Scaling. Large entites cannot see nor hit enemies
        if (FocussedEntity == null || !HasWeapon) return;
        if (!CanAttack(NavAgent.velocity.sqrMagnitude)) return;
        var dest = FocussedEntity.transform.position;
        HeldWeapon.FireTowards(NonPcWeaponBulletOrigin, dest);
    }

    protected override void HandleMovement() {
        if (SelfEntity.IsLeader && Time.time > NextMinionSeek) {
            SeekMinions();
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

    private void SeekMinions() {
        var nearestAlly =
            SelfEntity.FindNearestAlly(filter: MinionSeekIgnoreList, seekMinion: true, maxDistance: SightDistance);
        NextMinionSeek = Time.time + SeekMinionRate;

        // Reset Search
        if (nearestAlly == null) {
            MinionSeekIgnoreList.Clear();
            return;
        }

        if (!SelfEntity.CanSeeEntity(nearestAlly)) {
            MinionSeekIgnoreList.Add(nearestAlly);
            return;
        }

        if (nearestAlly != null && nearestAlly.TryGetComponent<PlayerCharacter>(out var character)) {
            character.AssignLeader(this);
            nearestAlly.IsGrouped = true;
            MinionSeekIgnoreList.Clear();
        }
    }

    /// <summary>
    /// FocusIgnoreList is unused at the moment.
    /// </summary>
    private void SeekFocus() {
        if (!SelfEntity.CanSeeEntity(FocussedEntity)) {
            FocussedEntity = null;
        }

        if (FocussedEntity != null && !FocussedEntity.IsDead &&
            CanReachFocusTarget) return;

        if (Leader != null) {
            FocussedEntity = Leader == CurrentCharacter
                ? SelfEntity.FindNearestEnemy(maxDistance: SightDistance)
                : Leader.FocussedEntity;
        }

        FocussedEntity = SelfEntity.FindNearestEnemy(filter: FocusIgnoreList, maxDistance: SightDistance);
        if (FocussedEntity == null) {
            FocusIgnoreList.Clear();
        }
    }

    private void HandlePlayerMovement() {
        var thisPosition = transform.position;

        TargetPosition = thisPosition + PlayerInput.normalized / 2;
        if (NavAgent.destination != TargetPosition)
            NavAgent.SetDestination(TargetPosition);
    }

    private void HandleComputerMovement() {
        SeekFocus();

        bool followLeader = Leader != null && (IsTooFarFromLeader || FocussedEntity == null);

        TargetPosition = followLeader ? Leader.transform.position :
            NeedsToMoveToFocusTarget ? FocussedEntity.transform.position : transform.position;

        var dir = (TargetPosition - transform.position).normalized * TetherDistance;

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

    [Button]
    [PropertyOrder(99)]
    public void Possess() {
        if (CurrentCharacter != null) {
            CurrentCharacter.TargetPosition = CurrentCharacter.transform.position;
        }

        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}