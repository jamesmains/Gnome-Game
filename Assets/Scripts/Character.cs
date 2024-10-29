using System;
using NUnit.Framework;
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
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    public Weapon HeldWeapon;

    [SerializeField] [FoldoutGroup("Dependencies")]
    public Sprite CharacterSprite;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
    protected NavMeshAgent NavAgent;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
    public Entity SelfEntity;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
    public Rigidbody Rb;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
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

    protected Vector3 Velocity => Rb.linearVelocity;
    protected bool ShouldFaceTarget => Mathf.Abs(TargetPosition.x - transform.position.x) > MinimumVelocityForAnimating;
    protected int TargetDirection => (TargetPosition.x - transform.position.x) > 0 ? -1 : 1;
    
    protected static readonly int IsMoving = Animator.StringToHash("IsMoving");

    protected virtual void Awake() {
        Init();
    }

    protected virtual void Init() {
        if (Rb == null) Rb = GetComponent<Rigidbody>();
        if (Anim == null) Anim = GetComponent<Animator>();
        if (SelfEntity == null) SelfEntity = GetComponent<Entity>(); // Wtf?
        if (NavAgent == null) NavAgent = GetComponent<NavMeshAgent>();
        
        CachedScale = transform.localScale.x;
        NavAgent.updatePosition = false;
        
        SetAnimationSpeed();
    }

    protected virtual void SetAnimationSpeed() {
        Anim.speed = Anim.speed / transform.localScale.x; // interesting idea to slow the speed of animations down by the scale of the character...
        Anim.speed = Mathf.Clamp(Anim.speed, 0.35f, 1f);
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
        FacingDirection = Velocity.x == 0 ? FacingDirection : Velocity.x > 0 ? -1 : 1;
        FacingDirection = ShouldFaceTarget ? TargetDirection : FacingDirection;
        SetScale(transform.localScale);
    }

    protected virtual void SetScale(Vector3 scale) {
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