using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Hooks")]
    private List<GameObject> SpawnPool;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("X: Min distance, Y: Max distance")]
    private Vector2 SpawnRange;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("How many can be spawned together")]
    private Vector2 SpawnGrouping;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("X: Min time till next spawn, Y: Max time till next spawn")]
    private Vector2 SpawnFrequency;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("How many things can be out at once")]
    private int SpawnLimit;

    [SerializeField]
    [FoldoutGroup("Settings")]
    [Tooltip("How many total spawns this spawner can provide. NOTE: -1 == Limitless")]
    private int AvailableSpawns = -1;

    [SerializeField] [FoldoutGroup("Settings")][HideIf("DepleteOnLastDeath")]
    private bool DepleteOnLastSpawn;
    
    [SerializeField] [FoldoutGroup("Settings")] [HideIf("DepleteOnLastSpawn")]
    private bool DepleteOnLastDeath;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Entity> SpawnedEntities;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float TimeUntilNextSpawn;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnSpawnDepletion;

    private void Awake() {
    }

    private void Update() {
        if (AvailableSpawns <= 0 && AvailableSpawns != -1 || SpawnedEntities.Count >= SpawnLimit) return;
        if (TimeUntilNextSpawn > 0) {
            TimeUntilNextSpawn -= Time.deltaTime;
        }
        else Spawn();
    }

    private void Spawn() {
        int groupCount = Random.Range((int) SpawnGrouping.x, (int) SpawnGrouping.y + 1);
        print(groupCount);
        int maxAvailableSpawns = SpawnLimit > AvailableSpawns ? SpawnLimit : AvailableSpawns;
        groupCount = Mathf.Clamp(groupCount, 0, maxAvailableSpawns);

        AvailableSpawns -= groupCount;

        for (int i = 0; i < groupCount; i++) {
            int r = Random.Range((int) 0, (int) SpawnPool.Count);
            var pos = transform.position;
            var rX = Random.Range(SpawnRange.x, SpawnRange.y);
            var rY = Random.Range(SpawnRange.x, SpawnRange.y);
            var flipX = Random.Range(0f, 100f) > 50;
            var flipY = Random.Range(0f, 100f) > 50;
            rX = flipX ? rX * -1 : rX;
            rY = flipY ? rY * -1 : rY;
            pos += new Vector3(rX, pos.y, rY);
            var entity = Pooler.Instance.SpawnObject(SpawnPool[r], pos).GetComponent<Entity>();
            entity.OnDeath.AddListener(RemoveEntityFromList);
            SpawnedEntities.Add(entity);
        }

        if (AvailableSpawns <= 0 && AvailableSpawns != -1 && DepleteOnLastSpawn) {
            OnSpawnDepletion.Invoke();
        }
        
        TimeUntilNextSpawn = Random.Range(SpawnFrequency.x, SpawnFrequency.y);
    }

    private void RemoveEntityFromList(Entity e) {
        SpawnedEntities.Remove(e);
        if (AvailableSpawns <= 0 && AvailableSpawns != -1 && DepleteOnLastDeath) {
            OnSpawnDepletion.Invoke();
        }
    }
}