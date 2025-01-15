using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorWeapon : ActorComponent {
    // Todo: Add weapon slot for flexible weapon options
    // Todo: Add functionality for firing weapon -> Check Player.cs, Actor.cs
    
    // Todo: Move to player controlled settings menu
    [SerializeField, FoldoutGroup("Debug")]
    private float AimSpeed = 12f;
    
    // Todo: Move to weapon settings as Range
    [SerializeField, FoldoutGroup("Debug")]
    private float DebugBounds;
    
    [SerializeField, FoldoutGroup("Dependencies")]
    private Transform AimTransform;

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
        AimTransform.localPosition = Vector3.ClampMagnitude(AimTransform.localPosition, DebugBounds);
    }

    private void UseWeapon() {
        
    }
}
