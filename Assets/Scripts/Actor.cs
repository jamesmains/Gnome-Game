using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Actor : MonoBehaviour {
    [SerializeField, FoldoutGroup("Debug")]
    private ActorDetails DebugIncomingActor;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private ActorDetails CurrentActorDetails;

    // This may not be required
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<ActorComponent> AttachedComponents;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private bool BrainActive;

    // Maybe more of a debug?
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private string BrainType;

    private ActorBrain Brain;

    public ActorDetails Details {
        get => CurrentActorDetails;
        set {
            CurrentActorDetails = value;
            SwapActor();
        }
    }

    // Setup
    public Action<ActorDetails> OnActorSet;
    // Possession
    public static Action<Actor> OnTryPossess;
    public static Action<Actor, List<ActorComponent>> OnPossessed;
    public static Action<Actor> OnReleasePossession;
    // Movement
    public Action<Vector3, bool> OnMoveActor;
    // Weapon
    public Action<Vector2> OnAimWeapon;
    public Action OnUseWeapon;
    // Vitals
    public Action<DamageInfo> OnDealDamage; // Get information back on damage dealt to another Actor
    public Action<ActorWeapon> OnTakeDamage; // Get information on incoming damage from another Actor

    private void Awake() {
        AttachedComponents = GetComponents<ActorComponent>().ToList();
        BrainActive = true;
    }

    private void OnEnable() {
        OnTryPossess += HandlePossession;
        OnReleasePossession += HandleReleasePossession;
    }

    private void OnDisable() {
        OnTryPossess -= HandlePossession;
        OnReleasePossession -= HandleReleasePossession;
    }

    private void Update() {
        // Todo: replace with actor behavior
        if (Brain == null || !BrainActive) return;
        Brain.Update(this);
    }

    private void FixedUpdate() {
        if (Brain == null || !BrainActive) return;
        Brain.FixedUpdate(this);
    }

    private void HandlePossession(Actor actor) {
        if (actor != this) return;
        OnPossessed.Invoke(this, AttachedComponents);
        BrainActive = false;
    }

    [Button]
    private void HandleReleasePossession(Actor actor) {
        if (actor != this) return;
        BrainActive = true;
    }

    private void SwapActor() {
        OnActorSet.Invoke(Details);
        Brain = Details?.BrainBehavior?.Create<ActorBrain>();
        BrainType = Brain?.GetType().Name;
    }

    [Button]
    public void DebugSwapActor() {
        Details = DebugIncomingActor;
    }
}