using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class ActorBrain {
    public abstract void Update(Actor actor);
    public abstract void FixedUpdate(Actor actor);
    public abstract ActorBrain Create<T>();
}

public class MinionBrain : ActorBrain {
    public override ActorBrain Create<T>() {
        return new MinionBrain();
    }
    
    [SerializeField, FoldoutGroup("Debug"), ReadOnly] // Todo: add seek function for finding leader
    public Actor LeaderActor;
    
    [SerializeField, FoldoutGroup("Debug"), ReadOnly]
    private float LastTimeSignaled;
    
    public override void Update(Actor actor) {
        if (!(Time.time > LastTimeSignaled + 1f)) return;
        LastTimeSignaled = Time.time;
        Vector3 RandomNewPosition = Vector3.zero;
        RandomNewPosition.x = Random.Range(-1f, 1f);
        RandomNewPosition.z = Random.Range(-1f, 1f);
        if(LeaderActor == null)
            actor.OnMoveActor?.Invoke(RandomNewPosition, true);
        else {
            if (Vector3.Distance(actor.transform.position, LeaderActor.transform.position) < 1.5f) {
                var moveAwayDirection = (actor.transform.position - LeaderActor.transform.position).normalized;
                actor.OnMoveActor?.Invoke(moveAwayDirection, true);
            }
            else if (Vector3.Distance(actor.transform.position, LeaderActor.transform.position) > 2f) {
                actor.OnMoveActor?.Invoke(LeaderActor.transform.position, false);
            }
        }
    }

    public override void FixedUpdate(Actor actor) {
    }
}

public class LeaderBrain : ActorBrain {
    public override void Update(Actor actor) {
        
    }

    public override void FixedUpdate(Actor actor) {
        
    }

    public override ActorBrain Create<T>() {
        return new LeaderBrain();
    }
}