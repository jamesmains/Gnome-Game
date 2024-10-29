using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Weapon Setting", menuName = "Inventories and Items/Weapon Setting")]
public class WeaponSettings : ScriptableObject {
    public string WeaponName;
    [PreviewField] public Sprite WeaponIcon;

    [FoldoutGroup("Settings")] public Vector2 WeaponFireRate;
    [FoldoutGroup("Settings")] public float AttackRange = 3f;
    [FoldoutGroup("Settings")] public GameObject WeaponBullet;
    [FoldoutGroup("Settings")] public List<DamageSource> WeaponDamageSources = new();
    [FoldoutGroup("Settings")] public bool AssignToWielder = true;
    [FoldoutGroup("Settings")] public bool AssignToWielderTeam = true;
}