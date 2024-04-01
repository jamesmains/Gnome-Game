using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Character : SerializedMonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    protected float ResizeSpeed = 5;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float MinimumVelocityForAnimating = 0.15f;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float InteractionRange;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public float AttackRange;
    
    [SerializeField] [FoldoutGroup("Settings")]
    protected float RepelFromOthersRange;
    
    [SerializeField] [FoldoutGroup("Settings")]
    protected bool CanAttackWhileMoving = true;
    
    [SerializeField] [FoldoutGroup("Settings")]
    protected bool CanAttackWhileNotMoving = true;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Entity SelfEntity;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    protected NavMeshAgent NavAgent;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    protected Weapon HeldWeapon;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Sprite CharacterSprite;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Rigidbody Rb;

    [SerializeField] [FoldoutGroup("Hooks")]
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
        if (SelfEntity == null) SelfEntity = GetComponent<Entity>();
        CachedScale = transform.localScale.x;
        NavAgent.updatePosition = false;
        NavAgent.stoppingDistance = 0;
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
        var velocity = Rb.velocity;
        FacingDirection = velocity.x == 0 ? FacingDirection : velocity.x > 0 ? -1 : 1;
        var shouldFaceTarget = Mathf.Abs(TargetPosition.x - transform.position.x) > MinimumVelocityForAnimating;
        var targetDirection = TargetPosition.x - transform.position.x;
        targetDirection = targetDirection > 0 ? -1 : 1;
        FacingDirection = shouldFaceTarget ? (int)targetDirection : FacingDirection;
        var scale = transform.localScale;
        var targetScale = scale;
        targetScale.x = FacingDirection * CachedScale;
        scale = Vector3.MoveTowards(scale, targetScale, Time.deltaTime * ResizeSpeed);
        transform.localScale = scale;
    }

    protected virtual void HandleAnimation() {
        var isMoving = NavAgent.velocity.sqrMagnitude > 0;
        Anim.SetBool(IsMoving, isMoving);
    }
}