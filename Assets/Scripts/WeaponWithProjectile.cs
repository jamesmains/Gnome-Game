using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponWithProjectile : Weapon {

    [SerializeField] [FoldoutGroup("Settings")]
    private bool AssignToWeilder = true;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private bool AssignToWeilderTeam = true;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject BulletObject;

    public override void Fire(Transform Origin) {
        if (!CanFire()) return;
        base.Fire(Origin);
        
        AttachProjectile(LaunchProjectile(BulletOrigin.forward).GetComponentInChildren<Entity>());
    }
    
    public override void FireTowards(Transform Origin, Vector3 Destination) {
        if (!CanFire()) return;
        base.FireTowards(Origin, Destination);
        
        var dir = (Destination - BulletOrigin.position).normalized;
        AttachProjectile(LaunchProjectile(dir).GetComponentInChildren<Entity>());
    }

    private GameObject LaunchProjectile(Vector3 dir) {
        var spawnedBullet = Pooler.Instance.SpawnObject(BulletObject, BulletOrigin.position);
        var projectile = spawnedBullet.GetComponentInChildren<Projectile>();
        if(projectile != null) projectile.SetDirection(dir);
        return spawnedBullet;
    }

    private void AttachProjectile(Entity entity) {
        if(AssignToWeilder) entity.SetParentEntity(WieldingEntity);
        if(AssignToWeilderTeam) entity.SetTeam(WieldingEntity.Team);
    }
}