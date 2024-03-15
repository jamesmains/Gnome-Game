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

        var spawnedBullet = Pooler.Instance.SpawnObject(BulletObject, BulletOrigin.position);

        var projectile = spawnedBullet.GetComponentInChildren<Projectile>();
        var entity = spawnedBullet.GetComponentInChildren<Entity>();
        if(AssignToWeilder) entity.SetParentEntity(WieldingEntity);
        if(AssignToWeilderTeam) entity.SetTeam(WieldingEntity.Team);
        if(projectile != null) projectile.SetDirection(BulletOrigin.forward);
    }
}