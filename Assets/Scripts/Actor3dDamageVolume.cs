using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum DamageVolumeMode {
    WeaponEffect,
    Persistent
}

public class Actor3dDamageVolume : MonoBehaviour {
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private DamageVolumeMode Mode;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private ActorWeapon SourceWeapon;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private float DisposeTime;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private List<Actor> OverlappingActors = new();

    public void SetVolumeWeaponMode(ActorWeapon volumeSource) {
        SourceWeapon = volumeSource;
        Mode = DamageVolumeMode.WeaponEffect;
        DisposeTime = Time.time + SourceWeapon.Details.DamageLinger;
    }

    public void SetVolumePersistentMode(float lingerTime) {
        SourceWeapon = null;
        Mode = DamageVolumeMode.Persistent;
        DisposeTime = lingerTime > 0 ? Time.time + lingerTime : -1;
    }

    private void OnEnable() {
    }

    private void OnDisable() {
        Reset();
    }

    private void Update() {
        if (DisposeTime > 0 && Time.time >= DisposeTime) {
            gameObject.SetActive(false);
        }
        else {
            foreach (var actorInVolume in OverlappingActors) {
                actorInVolume.OnTakeDamage.Invoke(SourceWeapon);
            }
        }
    }

    private void Reset() {
        OverlappingActors.Clear();
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.TryGetComponent(out Actor overlappingActor)) return;
        Debug.Log(SourceWeapon.Actor.Details);
        if (!OverlappingActors.Contains(overlappingActor) && (Mode == DamageVolumeMode.WeaponEffect &&
                                                              (overlappingActor?.Details.FriendlyWithTeam &
                                                               SourceWeapon.Actor.Details.FriendlyWithTeam) == 0 ||
                                                              Mode == DamageVolumeMode.Persistent)) {
            OverlappingActors.Add(overlappingActor);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!other.TryGetComponent(out Actor overlappingActor)) return;
        if (OverlappingActors.Contains(overlappingActor)) {
            OverlappingActors.Remove(overlappingActor);
        }
    }
}