using System;
using UnityEngine;

[Serializable]
public class DamageTypeModifier {
    public DamageType ModifierType;
    [Range(0,1)] public float Percent;
}