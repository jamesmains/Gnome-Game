using System;
using System.Collections;
using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Boots : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private GameObject PlayerSpawner;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Transform PlayerSpawnPoint;

    private UnityEvent OnGameStart = new();

    private void OnEnable() {
        OnGameStart.AddListener(GameStart);
    }

    private void OnDisable() {
        OnGameStart.RemoveListener(GameStart);
    }

    private void Start() {
        OnGameStart.Invoke();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.R))
            OnGameStart.Invoke();
    }

    [Button]
    private void GameStart() {
        print("Trying");
        if (PlayerCharacter.CurrentCharacter != null) {
            PlayerCharacter.CurrentCharacter.SelfEntity.Die(null);
        }
        Pooler.SpawnAt(PlayerSpawner,PlayerSpawnPoint.position);
    }
}
