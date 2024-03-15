using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyCharacter : Character {
    [SerializeField] [FoldoutGroup("Hooks")]
    private Weapon EnemyWeapon;

    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveSpeed;

    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 IdealAttackRange;

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (IsLocked) return;
        var playerPosition = PlayerCharacter.CurrentCharacter.transform.position;
        var pos = transform.position;
        var dir = playerPosition - pos;
        var dist = Vector3.Distance(playerPosition, pos);
        dir = dir.normalized;
        dir = dist > IdealAttackRange.x ? dir : dist < IdealAttackRange.y ? dir * -1 : Vector3.zero;
        Rb.AddForce(dir * (Time.deltaTime * MoveSpeed), ForceMode.Impulse);
    }
}