using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Actor : MonoBehaviour
{
    
    [SerializeField, FoldoutGroup("Debug")]
    private bool BrainActive;
    
    [SerializeField, FoldoutGroup("Debug"), ReadOnly]
    private float LastTimeSignaled;
    
    [SerializeField, FoldoutGroup("Debug")]
    private ActorDetails DebugIncomingActor;
    
    [SerializeField, FoldoutGroup("Status"),ReadOnly]
    private ActorDetails CurrentActorDetails;

    // This may not be required
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<ActorComponent> AttachedComponents;

    public ActorDetails Details {
        get => CurrentActorDetails;
        set { CurrentActorDetails = value; SwapActor();}
    }
    
    public Action<ActorDetails> OnActorSet;
    public static Action<Actor> OnTryPossess;
    public static Action<Actor,List<ActorComponent>> OnPossessed;
    public static Action<Actor> OnReleasePossession;

    public Action<Vector3> OnMoveActor;

    public Action<Vector2> OnAimWeapon;
    public Action OnUseWeapon;

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
        if (!BrainActive) return;
        if (Time.time > LastTimeSignaled + 1) {
            LastTimeSignaled = Time.time;
            Vector3 RandomNewPosition = Vector3.zero;
            RandomNewPosition.x = Random.Range(-1f, 1f);
            RandomNewPosition.z = Random.Range(-1f, 1f);
            OnMoveActor?.Invoke(RandomNewPosition);
        }
    }

    private void HandlePossession(Actor actor) {
        if (actor != this) return;
        OnPossessed.Invoke(this,AttachedComponents);
        BrainActive = false;
    }

    [Button]
    private void HandleReleasePossession(Actor actor) {
        if (actor != this) return;
        BrainActive = true;
    }

    private void SwapActor() {
        print(Details.ActorName);
        OnActorSet.Invoke(Details);
    }

    [Button]
    public void DebugSwapActor() {
        Details = DebugIncomingActor;
    }   
}
