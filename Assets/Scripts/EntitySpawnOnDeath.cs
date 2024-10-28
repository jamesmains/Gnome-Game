using System;
using System.Collections;
using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class EntitySpawnOnDeath : Entity {
    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject EntityToSpawnOnDeath;

    private void Awake() {
        OnDeath.AddListener(delegate { SpawnDeathEntity();});
    }

    private void SpawnDeathEntity() {
        // Todo: Check if game is over so it just removes them without adding a new entity
        Pooler.SpawnAt(EntityToSpawnOnDeath,transform.position);
    }
}
