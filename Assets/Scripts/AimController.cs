using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AimController : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed = 10;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private LayerMask GroundLayer;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Transform AimIndicatorOriginPoint;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Transform BulletSpawnPoint;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    public Transform AimIndicatorDestinationPoint;

    [SerializeField] [FoldoutGroup("Hooks")]
    private Camera Cam;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 TargetPosition;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 CachedMousePosition;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector3 CachedPlayerPosition;

    public static AimController PlayerAim;

    private void Awake() {
        if (Cam == null)
            Cam = GetComponent<Camera>();
        PlayerAim = this;
    }

    private void Update() {
        if (PlayerCharacter.CurrentCharacter == null) return;
        var playerPosition = PlayerCharacter.CurrentCharacter.transform.position;
        if (CachedMousePosition == Input.mousePosition && CachedPlayerPosition == playerPosition) return;
        CachedMousePosition = Input.mousePosition;
        CachedPlayerPosition = playerPosition;
        
        var ray = Cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, 100,GroundLayer)) return;
        var dir = hit.point - playerPosition;
        var range = Vector3.ClampMagnitude(dir, PlayerCharacter.CurrentCharacter.AttackRange);
        TargetPosition = playerPosition + range;
    }

    private void FixedUpdate() {
        AimIndicatorDestinationPoint.position = Vector3.Lerp(AimIndicatorDestinationPoint.position, TargetPosition, Time.deltaTime * MoveSpeed);
        AimIndicatorOriginPoint.transform.LookAt(AimIndicatorDestinationPoint);
        if (PlayerCharacter.CurrentCharacter == null) return;
        
        AimIndicatorOriginPoint.transform.position = PlayerCharacter.CurrentCharacter.transform.position;
    }
}