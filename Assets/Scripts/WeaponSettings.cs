using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Setting", menuName = "Inventories and Items/Weapon Setting")]
public class WeaponSettings : ScriptableObject {
    public string WeaponName;
    [PreviewField] public Sprite WeaponIcon;

    [FoldoutGroup("Settings")] public Vector2 WeaponFireRate;
    [FoldoutGroup("Settings")] public float AttackRange = 3f;
    [FoldoutGroup("Settings")] public GameObject WeaponBullet;
    [FoldoutGroup("Settings")] public List<DamageSource> WeaponDamageSources = new();
    [FoldoutGroup("Settings")] public bool AssignToWeilder = true;
    [FoldoutGroup("Settings")] public bool AssignToWeilderTeam = true;
}