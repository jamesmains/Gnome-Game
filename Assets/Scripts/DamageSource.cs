using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DamageSource {
    [SerializeField] private Vector2 damageRange;
    public DamageType damageType;

    public int DealtDamage() {
        return (int) Random.Range(damageRange.x, damageRange.y+1);
    }
}