using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class ActorMotor : ActorComponent
{
    
    
    [SerializeField, FoldoutGroup("Dependencies"), ReadOnly]
    public FollowerEntity Agent;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public int FacingDirection; // -1 left, 1 right

    protected override void OnEnable() {
        base.OnEnable();
        if(Agent == null)
            Agent = GetComponent<FollowerEntity>();
        FacingDirection = 1;
        Actor.OnMoveActor += MoveAgent;
    }

    protected override void OnDisable() {
        base.OnDisable();
        Actor.OnMoveActor -= MoveAgent;
    }

    private void MoveAgent(Vector3 moveTarget, bool asDirection) {
        var targetPosition = asDirection ? transform.position + moveTarget: moveTarget;
        if (Agent.destination != targetPosition) {
            Agent.SetDestination(targetPosition);
            if(moveTarget.x != 0)
                FacingDirection = moveTarget.x > 0 ? -1 : 1;
        }
    }
}
