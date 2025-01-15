using Sirenix.OdinInspector;
using UnityEngine;

public class ActorComponent: MonoBehaviour {
    
    [SerializeField, FoldoutGroup("Dependencies"),ReadOnly]
    public Actor Actor;

    protected virtual void OnEnable() {
        if(Actor == null) Actor = GetComponent<Actor>();
        
        Actor.OnActorSet += HandleActorChanged;
    }
    
    protected virtual void OnDisable() {
        Actor.OnActorSet -= HandleActorChanged;
    }
    
    protected virtual void HandleActorChanged(ActorDetails newDetails) {
        
    }
}