using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Details", menuName = "GNOME/Weapon Details")]
public class WeaponDetails : SerializedScriptableObject {
    [SerializeField, BoxGroup("Settings"), PreviewField]
    public Sprite WeaponIcon;

    [SerializeField, BoxGroup("Settings")]
    public string WeaponName;

    [SerializeField, BoxGroup("Settings")]
    public GameObject SpawnOnUseObject;

    [SerializeField, BoxGroup("Settings")]
    public float Range = 1f;

    [SerializeField, BoxGroup("Settings")] // Todo: Move to weapon behavior
    public float DamageLinger = 0.1f;

    [SerializeField, BoxGroup("Settings")]
    public StatType WeaponStatType;

    [SerializeField, BoxGroup("Settings")]
    public List<StatValue> BaseStats;
}