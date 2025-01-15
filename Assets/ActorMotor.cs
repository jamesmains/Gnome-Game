using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class ActorMotor : ActorComponent
{
    [SerializeField, FoldoutGroup("Dependencies"), ReadOnly]
    public NavMeshAgent Agent;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    public int FacingDirection; // -1 left, 1 right

    protected override void OnEnable() {
        base.OnEnable();
        if(Agent == null)
            Agent = GetComponent<NavMeshAgent>();
        FacingDirection = 1;
        Actor.OnMoveActor += MoveAgent;
    }

    protected override void OnDisable() {
        base.OnDisable();
        Actor.OnMoveActor -= MoveAgent;
    }

    private void MoveAgent(Vector3 moveDirection) {
        if (Agent.destination != transform.position + moveDirection) {
            Agent.SetDestination(transform.position + moveDirection);
            if(moveDirection.x != 0)
                FacingDirection = moveDirection.x > 0 ? -1 : 1;
        }
    }
}
