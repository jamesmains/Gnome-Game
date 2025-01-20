using Sirenix.OdinInspector;
using UnityEngine;

public class ActorVitalStats : ActorComponent {
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private float TotalDamageDealt;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private int KillCount;
    
    protected override void OnEnable() {
        base.OnEnable();
        Actor.OnDealDamage += HandleDealtDamage;
        Actor.OnTakeDamage += HandleTakeDamage;
    }

    // Dealt Damage -> Resolve things like tracked info or life steal
    private void HandleDealtDamage(DamageInfo damageInfo) {
        KillCount += damageInfo.DidKill ? 1 : 0;
        TotalDamageDealt += damageInfo.DamageDealt;
    }
    
    // Take Damage -> Resolve Damage against Weaknesses / Strengths
    private void HandleTakeDamage(ActorWeapon actorWeapon) {
        DamageInfo finalDamage = new DamageInfo(1, false);
        actorWeapon?.Actor?.OnDealDamage(finalDamage);
    }
    
    // Manage entity death -> Communicate with the Actor about it's status
}
