using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour, IDamageable, IKillable {
    [Title("Entity Settings")] [SerializeField] [FoldoutGroup("Settings")]
    public EntityTeams Team;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool IsLeader;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool Detectable = true;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool Searchable = true;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool WaitForTriggerToCommitSpawn;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public bool WaitForTriggerToCommitDie;

    [SerializeField] [FoldoutGroup("Settings")] [Range(-1, 999)]
    public int StartingHealth = 10;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Takes off a percentage of the damage")]
    protected List<DamageTypeModifier> Resistances;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Add on a percentage of the damage")]
    protected List<DamageTypeModifier> Weaknesses;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected Transform SightLine;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected SpawnableEffect SpawnEffects;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected SpawnableEffect HurtEffects;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected SpawnableEffect DeathEffects;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public int Health;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Entity ParentEntity;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public bool IsGrouped;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public bool IsDead;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public int EnemiesDefeated;

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity> OnSpawn = new();

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity, Entity> OnHit = new(); // Self, Attacker -- Damage Applied NOT required

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity, Entity> OnTakeDamage = new(); // Self, Attacker -- Damage must be applied

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity> OnDeath = new(); // Attacker (well killer in this case)

    private static List<Entity> AllEntities = new();
    private const int EntityLayer = ~ 6; // *shrug* idk either

    protected virtual void OnEnable() {
        AllEntities.Add(this);
        Spawn();
    }

    protected void OnDisable() {
        AllEntities.Remove(this);
    }

    protected virtual void Update() {
    }

    protected virtual void FixedUpdate() {
    }

    public void SetParentEntity(Entity entity) {
        ParentEntity = entity;
    }

    public void SetTeam(EntityTeams team) {
        Team = team;
    }

    public Entity FindNearestEntity(float maxDistance = Mathf.Infinity) {
        return AllEntities.Where(o => o.Searchable && WithinReachOfEntity(o, maxDistance))
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    public Entity FindNearestAlly(bool seekMinion = false, float maxDistance = Mathf.Infinity) {
        return AllEntities.Where(o =>
                o.Team == Team && o != this && o.Searchable && WithinReachOfEntity(o, maxDistance) &&
                (!seekMinion || !o.IsLeader && !o.IsGrouped))
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    public Entity FindNearestEnemy(Entity filter = null) {
        return AllEntities.Where(o => o.Team != Team && o != this && o.Searchable && o != filter)
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    public bool CanSeeEntity(Entity targetEntity) {
        var targetDirection = targetEntity.transform.position - SightLine.position;
        if (!Physics.Raycast(SightLine.position, targetDirection, out var hit,
                Mathf.Infinity, EntityLayer)) return false;
        hit.collider.gameObject.TryGetComponent<Entity>(out var e);
        if (e == null) return false;
        return e == targetEntity;
    }

    public bool WithinReachOfEntity(Entity targetEntity, float maxDistance) {
        return Vector3.Distance(transform.position, targetEntity.transform.position) < maxDistance;
    }

    public virtual bool TakeDamage(List<DamageSource> damageSources, Vector3 hitPoint, Entity attacker = null,
        bool canHarmAttacker = false) {
        if (attacker != null && attacker == this && !canHarmAttacker) return false;

        var dmg = 0;
        foreach (var source in damageSources) {
            var sourceDmg = source.DealtDamage();
            bool isResisted = false;
            bool isCritical = false;
            foreach (int value in from r in Resistances
                     where r.ModifierType == source.damageType
                     let value = sourceDmg
                     select value * r.Percent) {
                sourceDmg -= value;
                isResisted = true;
            }

            foreach (int value in from r in Weaknesses
                     where r.ModifierType == source.damageType
                     let value = sourceDmg
                     select value * r.Percent) {
                sourceDmg += value;
                isCritical = true;
            }

            sourceDmg = (int) Mathf.Clamp(sourceDmg, 0, Mathf.Infinity);
            dmg += sourceDmg;
            PopupManager.DisplayWorldValuePopup(sourceDmg, hitPoint, isCritical);
        }

        dmg = (int) Mathf.Clamp(dmg, 0, Mathf.Infinity);
        Health -= dmg;
        if (Health <= 0) Die(attacker);
        HurtEffects.PlayEffect(transform.position);
        OnTakeDamage.Invoke(this, attacker);
        return true;
    }

    public virtual bool TakeDamage(int damage, Vector3 hitPoint, Entity attacker = null,
        bool canHarmAttacker = false) {
        if (attacker != null && attacker == this && !canHarmAttacker) return false;
        Health -= damage;
        if (Health <= 0) Die(attacker);
        HurtEffects.PlayEffect(transform.position);
        OnTakeDamage.Invoke(this, attacker);
        return true;
    }

    protected virtual void Spawn() {
        IsDead = false;
        Health = StartingHealth;
        OnSpawn.Invoke(this);
        if (WaitForTriggerToCommitSpawn) return;
        CommitSpawn();
    }

    public virtual void CommitSpawn() {
        SpawnEffects.PlayEffect(transform.position);
    }
    
    public virtual void Die(Entity killer) {
        if (IsDead) return;
        if (killer != null)
            killer.EnemiesDefeated++;
        IsDead = true;
        OnDeath.Invoke(this);
        if (WaitForTriggerToCommitDie) return;
        CommitDie();
    }

    public virtual void CommitDie() {
        DeathEffects.PlayEffect(transform.position);
        gameObject.SetActive(false);
    }
}