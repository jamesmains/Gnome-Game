using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Character : SerializedMonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    protected float ResizeSpeed = 5;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float MinimumVelocityForAnimating = 0.15f;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Sprite CharacterSprite;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Rigidbody Rb;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected Animator Anim;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected int FacingDirection = 1;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected bool IsLocked;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected float CachedScale;

    protected static readonly int IsMoving = Animator.StringToHash("IsMoving");

    protected virtual void Awake() {
        if (Rb == null) Rb = GetComponent<Rigidbody>();
        if (Anim == null) Anim = GetComponent<Animator>();
        CachedScale = transform.localScale.x;
    }

    public virtual void ToggleLock(bool state) {
        IsLocked = state;
    }
    
    protected virtual void Update() {
        
    }
    
    protected virtual void FixedUpdate() {
        if (IsLocked) return;
        var velocity = Rb.velocity;
        FacingDirection = velocity.x == 0 ? FacingDirection : velocity.x > 0 ? -1 : 1;
        var isMoving = Mathf.Abs(velocity.x) > MinimumVelocityForAnimating ||
                       Mathf.Abs(velocity.z) > MinimumVelocityForAnimating;
        Anim.SetBool(IsMoving, isMoving);
        if (!(Math.Abs(transform.localScale.x - FacingDirection) > 0)) return;
        var scale = transform.localScale;
        var targetScale = scale;
        targetScale.x = FacingDirection * CachedScale;
        scale = Vector3.MoveTowards(scale, targetScale, Time.deltaTime * ResizeSpeed);
        transform.localScale = scale;
    }
}