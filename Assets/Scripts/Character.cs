using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    protected float ResizeSpeed = 5;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float MinimumVelocityForAnimating = 0.15f;

    [SerializeField] [FoldoutGroup("Settings")]
    protected bool CanAttackWhileMoving = true;

    [SerializeField] [FoldoutGroup("Settings")]
    protected bool CanAttackWhileNotMoving = true;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Weapon HeldWeapon;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Sprite CharacterSprite;

    [SerializeField] [FoldoutGroup("Hooks")] [ReadOnly]
    protected NavMeshAgent NavAgent;

    [SerializeField] [FoldoutGroup("Hooks")] [ReadOnly]
    public Entity SelfEntity;

    [SerializeField] [FoldoutGroup("Hooks")] [ReadOnly]
    public Rigidbody Rb;

    [SerializeField] [FoldoutGroup("Hooks")] [ReadOnly]
    protected Animator Anim;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Vector3 TargetPosition;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected int FacingDirection = 1;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected bool IsLocked;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected float CachedScale;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public Entity FocussedEntity;

    protected static readonly int IsMoving = Animator.StringToHash("IsMoving");

    protected virtual void Awake() {
        if (Rb == null) Rb = GetComponent<Rigidbody>();
        if (Anim == null) Anim = GetComponent<Animator>();
        Anim.speed = Anim.speed / transform.localScale.x; // interesting idea to slow the speed of animations down by the scale of the character...
        Anim.speed = Mathf.Clamp(Anim.speed, 0.35f, 1f);
        if (SelfEntity == null) SelfEntity = GetComponent<Entity>();
        if (NavAgent == null) NavAgent = GetComponent<NavMeshAgent>();
        CachedScale = transform.localScale.x;
        NavAgent.updatePosition = false;
    }

    protected virtual void OnEnable() {
    }

    protected virtual void OnDisable() {
    }

    protected virtual void Update() {
    }

    protected virtual void FixedUpdate() {
        if (IsLocked) return;
        HandleMovement();
        HandleLookDirection();
        HandleAnimation();
    }

    public virtual void ToggleLock(bool state) {
        IsLocked = state;
    }

    protected virtual void HandleMovement() {
    }

    protected virtual void HandleLookDirection() {
        var velocity = Rb.linearVelocity;
        FacingDirection = velocity.x == 0 ? FacingDirection : velocity.x > 0 ? -1 : 1;
        var shouldFaceTarget = Mathf.Abs(TargetPosition.x - transform.position.x) > MinimumVelocityForAnimating;
        var targetDirection = TargetPosition.x - transform.position.x;
        targetDirection = targetDirection > 0 ? -1 : 1;
        FacingDirection = shouldFaceTarget ? (int) targetDirection : FacingDirection;
        var scale = transform.localScale;
        var targetScale = scale;
        targetScale.x = FacingDirection * CachedScale;
        scale = Vector3.MoveTowards(scale, targetScale, Time.deltaTime * ResizeSpeed);
        transform.localScale = scale;
    }

    protected virtual void HandleAnimation() {
        var isMoving = (Vector3.Distance(transform.position, NavAgent.destination) > 0.2f);
        Anim.SetBool(IsMoving, isMoving);
    }
}