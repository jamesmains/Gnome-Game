using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    
    [SerializeField] [FoldoutGroup("Settings")]
    protected float FireRate;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Transform BulletOrigin;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Entity WieldingEntity;

    private float fireRateRechargeTimer;

    protected virtual void Update() {
        if (fireRateRechargeTimer > 0) fireRateRechargeTimer -= Time.deltaTime;
    }

    public virtual void Fire(Transform Origin) {
        BulletOrigin = Origin;
        fireRateRechargeTimer = FireRate;
    }

    protected virtual bool CanFire() {
        return !(fireRateRechargeTimer > 0);
    }
}