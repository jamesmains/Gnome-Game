using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {
    
    [SerializeField] [FoldoutGroup("Settings")]
    protected WeaponSettings Settings;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Transform BulletOrigin;

    [SerializeField] [FoldoutGroup("Hooks")]
    public Entity WieldingEntity;

    private float fireRateRechargeTimer;

    protected virtual void Update() {
        if (fireRateRechargeTimer > 0) fireRateRechargeTimer -= Time.deltaTime;
    }

    public virtual void Fire(Transform Origin) {
        CommitAttack(Origin);
    }

    public virtual void FireTowards(Transform Origin, Vector3 Destination) {
        CommitAttack(Origin);
    }

    protected virtual void CommitAttack(Transform Origin) {
        BulletOrigin = Origin;
        fireRateRechargeTimer = Random.Range(Settings.WeaponFireRate.x,Settings.WeaponFireRate.y);
    }

    protected virtual bool CanFire() {
        return !(fireRateRechargeTimer > 0);
    }
}