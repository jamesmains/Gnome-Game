using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponWithProjectile : Weapon {

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
        var spawnedBullet = Pooler.SpawnAt(Settings.WeaponBullet, BulletOrigin.position);
        var projectile = spawnedBullet.GetComponentInChildren<Projectile>();
        if(Settings.WeaponDamageSources != null)
            spawnedBullet.GetComponentInChildren<EntityDamageSourceVolume>().DamageSources = Settings.WeaponDamageSources;
        if(projectile != null) projectile.SetDirection(dir);
        return spawnedBullet;
    }

    private void AttachProjectile(Entity entity) {
        if(Settings.AssignToWeilder) entity.SetParentEntity(WieldingEntity);
        if(Settings.AssignToWeilderTeam) entity.SetTeam(WieldingEntity.Team);
    }
}