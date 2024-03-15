using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
    public bool TakeDamage(List<DamageSource> damageSources, Vector3 hitPoint, Entity attacker = null, bool canHarmAttacker = false);
}

public interface IKillable {
    public void Die();
}