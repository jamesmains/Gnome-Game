using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacter : Character {
    [SerializeField] [FoldoutGroup("Settings")]
    private float TetherDistance;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Furthest the minion can get away from the Leader")]
    private float LeashDistance;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Transform NonPcWeaponBulletOrigin;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Character Leader;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 playerInput;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Entity LastFoccussedEntity;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float NextMinionSeek;

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
                    SelfEntity.WithinReachOfEntity(FocussedEntity, HeldWeapon.Settings.AttackRange);
        if (!canAttack) return;
        var dest = FocussedEntity.transform.position;
        dest.y = NonPcWeaponBulletOrigin.position.y;
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
        
        if (NavAgent.isStopped) return;
        transform.position =
            Vector3.MoveTowards(transform.position, NavAgent.nextPosition, Time.deltaTime * NavAgent.speed);
    }
    

    private void HandleSeek() {
        var nearestAlly = SelfEntity.FindNearestAlly(seekMinion: true);
        if (nearestAlly != null && nearestAlly.TryGetComponent<PlayerCharacter>(out var character)) {
            character.AssignLeader(this);
            nearestAlly.IsGrouped = true;
        }

        NextMinionSeek = Time.time + SeekMinionRate;
    }

    private void HandlePlayerMovement() {
        var thisPosition = transform.position;

        TargetPosition = thisPosition + playerInput.normalized;

        var dist = Vector3.Distance(thisPosition, TargetPosition);
        NavAgent.isStopped = (dist < 0.01f);
        if (NavAgent.destination != TargetPosition)
            NavAgent.SetDestination(TargetPosition);
    }

    private void HandleComputerMovement() {
        var thisPosition = transform.position;
        float maximumDistance;

        if (Leader != null && !SelfEntity.IsLeader) {
            FocussedEntity = Leader == CurrentCharacter ? SelfEntity.FindNearestEnemy() : Leader.FocussedEntity;
            bool needsToMoveToFocusTarget = FocussedEntity != null && SelfEntity.CanSeeEntity(FocussedEntity) &&
                                            !SelfEntity.WithinReachOfEntity(FocussedEntity,
                                                HeldWeapon.Settings.AttackRange);
            TargetPosition = needsToMoveToFocusTarget
                ? FocussedEntity.transform.position
                : Leader.transform.position;
            maximumDistance = needsToMoveToFocusTarget ? HeldWeapon.Settings.AttackRange : TetherDistance;
            var targetPositionOverride = Vector3.Distance(Leader.transform.position, TargetPosition) > LeashDistance;
            if (targetPositionOverride) {
                maximumDistance = TetherDistance;
                TargetPosition = Leader.transform.position;
                FocussedEntity = null;
            }
        }

        else {
            if (FocussedEntity == null || FocussedEntity.IsDead ||
                !SelfEntity.CanSeeEntity(FocussedEntity)) {
                LastFoccussedEntity = FocussedEntity = SelfEntity.FindNearestEnemy(LastFoccussedEntity);
            }

            TargetPosition = FocussedEntity ? FocussedEntity.transform.position : transform.position;
            maximumDistance = FocussedEntity ? HeldWeapon.Settings.AttackRange : 0;
        }

        var dist = Vector3.Distance(thisPosition, TargetPosition);
        NavAgent.isStopped = (dist < maximumDistance);
        if (NavAgent.destination != TargetPosition)
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
    public void Possess() {
        if (CurrentCharacter != null)
            CurrentCharacter.TargetPosition = CurrentCharacter.transform.position;
        CurrentCharacter = this;
        OnPlayerCharacterPossession.Invoke(this);
    }
}