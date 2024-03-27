using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntityDamageSourceVolume : Entity {
    [SerializeField] [FoldoutGroup("Settings")]
    private bool TriggerOnEntry = true;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool TriggerOnExit;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool TriggerOnStay;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool IgnoreFriendlies;

    [SerializeField] [FoldoutGroup("Settings")]
    private LayerMask IgnoredLayers;

    [SerializeField] [FoldoutGroup("Settings")]
    private List<DamageSource> DamageSources = new();

    private void OnTriggerEnter(Collider other) {
        if (!TriggerOnEntry) return;
        ProcessHit(other);
    }

    private void OnTriggerExit(Collider other) {
        if (!TriggerOnExit) return;
        ProcessHit(other);
    }


    private void OnTriggerStay(Collider other) {
        if (!TriggerOnStay) return;
        ProcessHit(other);
    }

    private void ProcessHit(Collider other) {
        var hitLayer = other.transform.gameObject.layer;
        if ((IgnoredLayers.value & (1 << hitLayer)) > 0) return;

        var hitPoint = other.ClosestPoint(transform.position);
        var entity = other.gameObject.GetComponentInChildren<Entity>();
        if (entity != null) {
            if (ParentEntity != null && IgnoreFriendlies && entity.Team == ParentEntity.Team) return;
            entity.OnHit.Invoke(entity,null);
            if (!entity.Detectable) return;
            entity.TakeDamage(DamageSources, hitPoint, ParentEntity);
        }

        if (Health == -1) return;
        Health -= 1;
        if (Health <= 0) Die(null);
    }
}