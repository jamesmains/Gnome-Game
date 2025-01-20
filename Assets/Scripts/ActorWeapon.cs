using System;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorWeapon : ActorComponent {
    // Todo: Add functionality for firing weapon -> Check Player.cs, Actor.cs (HIGH PRIORITY)
    
    // Todo: Move to player controlled settings menu (LOW PRIORITY)
    [SerializeField, FoldoutGroup("Debug")]
    private float AimSpeed = 12f;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private Transform AimTransform;

    [SerializeField, FoldoutGroup("Dependencies")]
    public WeaponDetails Details;

    protected override void OnEnable() {
        base.OnEnable();
        Actor.OnUseWeapon += UseWeapon;
        Actor.OnAimWeapon += AimWeapon;
    }

    protected override void OnDisable() {
        base.OnDisable();
        Actor.OnUseWeapon -= UseWeapon;
        Actor.OnAimWeapon -= AimWeapon;
    }

    private void AimWeapon(Vector2 aimDirection) {
        Vector3 aim = aimDirection;
        aim.z = aim.y;
        aim.y = 0;
        
        // needed because mouse will make it go nuts, investigate better solution?
        // mouse aimDirection comes in as screen space positions
        aim = Vector3.ClampMagnitude(aim, 1); 
        
        // Todo: Move the actual setting and clamping in FixedUpdate
        AimTransform.localPosition += aim * (Time.deltaTime * AimSpeed);
        AimTransform.localPosition = Vector3.ClampMagnitude(AimTransform.localPosition, Details.Range);
    }

    private void UseWeapon() {
        Debug.Log("Using weapon!");
        Pooler.SpawnAt(Details.SpawnOnUseObject,AimTransform.position).GetComponent<Actor3dDamageVolume>().SetVolumeWeaponMode(this);
    }
}
