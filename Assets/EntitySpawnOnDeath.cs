using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntitySpawnOnDeath : Entity {
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject DeathSpawn;

    private void Awake() {
        OnDeath.AddListener(delegate { SpawnDeathEntity();});
    }

    private void SpawnDeathEntity() {
        // Todo: Check if game is over so it just removes them without adding a new entity
        Pooler.Instance.SpawnObject(DeathSpawn,transform.position);
    }
}
