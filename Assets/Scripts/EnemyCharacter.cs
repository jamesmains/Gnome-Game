using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : Character {

    [SerializeField] [FoldoutGroup("Hooks")]
    private Transform WeaponBulletOrigin;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Entity FocussedEntity;

    protected override void Update() {
        base.Update();
        if (FocussedEntity == null) return;
        float v = NavAgent.velocity.sqrMagnitude;
        bool canAttack = (v != 0 && CanAttackWhileMoving) ||
                         (v == 0 && CanAttackWhileNotMoving);
        if (!canAttack) return;
        var dest = FocussedEntity.transform.position;
        dest.y = WeaponBulletOrigin.position.y;
        HeldWeapon.FireTowards(WeaponBulletOrigin,dest);
    }

    protected override void HandleMovement() {
        if (FocussedEntity == null || FocussedEntity.IsDead) {
            FocussedEntity = SelfEntity.FindNearestEnemy();
            
        }
        TargetPosition = FocussedEntity.transform.position;
        if (TargetPosition != NavAgent.destination) {
            NavAgent.SetDestination(TargetPosition);
        }

        transform.position =
            Vector3.MoveTowards(transform.position, NavAgent.nextPosition, Time.deltaTime * NavAgent.speed);
    }
    
    
    public void FocusOnAttacker(Entity self, Entity attacker) {
        FocussedEntity = attacker;
    }
}