using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour, IDamageable, IKillable {
    [SerializeField] [FoldoutGroup("Settings")]
    public EntityTeams Team;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public bool IsLeader;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public bool IsMinion;

    [SerializeField] [FoldoutGroup("Settings")]
    public EntityType Type;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool IgnoreDamage;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool IgnoreTeam;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool Detectable = true;

    [SerializeField] [FoldoutGroup("Settings")]
    public bool WaitForTriggerToCommitDie;

    [SerializeField] [FoldoutGroup("Settings")] [Range(-1,999)]
    protected int StartingHealth;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float DamageBuffer = 0.1f;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Takes off a percentage of the damage")]
    protected List<DamageTypeModifier> Resistances;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Add on a percentage of the damage")]
    protected List<DamageTypeModifier> Weaknesses;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected AudioClip SpawnSfx;
    
    [SerializeField] [FoldoutGroup("Hooks")]
    protected GameObject HurtVfx;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected GameObject DeathVfx;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public int Health;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Entity ParentEntity;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected bool ImmuneToDamage;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public bool IsGrouped;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public bool IsDead;

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent OnSpawn = new();

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity, Entity> OnHit = new(); // Self, Attacker -- Damage Applied NOT required

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity, Entity> OnTakeDamage = new(); // Self, Attacker -- Damage must be applied

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity> OnDeath = new();
    
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
        // print($"Nearest Ally for {this.gameObject.name} is {FindNearestAlly()}");
        // print($"Nearest Enemy for {this.gameObject.name} is {FindNearestEnemy()}");
    }

    protected virtual void FixedUpdate() {
    }

    private void Spawn() {
        OnSpawn.Invoke();
        IsDead = false;
        Health = StartingHealth;
        ImmuneToDamage = false;
        AudioManager.OnPlayClip.Invoke(SpawnSfx);
    }

    public void SetParentEntity(Entity entity) {
        ParentEntity = entity;
    }

    public void SetTeam(EntityTeams team) {
        Team = team;
    }

    public Entity FindNearestAlly(bool seekMinion = false) {
        if (seekMinion) return FindNearestMinion();
        else return AllEntities.Where(o => o.Team == Team && o != this && o.Detectable)
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    private Entity FindNearestMinion() {
        return AllEntities.Where(o => o.Team == Team && o != this && o.Detectable && o.IsMinion && !o.IsGrouped)
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    public Entity FindNearestEnemy(Entity filter = null) {
        return AllEntities.Where(o => o.Team != Team && o != this && o.Detectable && o != filter)
            .OrderBy(o => Vector3.Distance(transform.position, o.transform.position)).FirstOrDefault();
    }

    public bool CanSeeEntity(Entity targetEntity) {
        var targetDirection = targetEntity.transform.position - transform.position;
        if (!Physics.Raycast(transform.position,targetDirection, out var hit,
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
        if (ImmuneToDamage || IgnoreDamage) return false;
        if (attacker != null && attacker == this && !canHarmAttacker) return false;
        if (attacker != null && attacker.Team == Team && !IgnoreTeam) return false;

        if(this.gameObject.activeSelf)
            StartCoroutine(ProcessDamageBuffer());

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
        Pooler.Instance.SpawnObject(HurtVfx,
            transform.position);
        OnTakeDamage.Invoke(this, attacker);
        return true;
    }

    IEnumerator ProcessDamageBuffer() {
        ImmuneToDamage = true;
        yield return new WaitForSeconds(DamageBuffer);
        ImmuneToDamage = false;
    }

    public virtual void Die(Entity killer) {
        if (IsDead) return;
        if (killer != null)
            print($"{this.gameObject.name} was slain by {killer.gameObject.name}");
        IsDead = true;
        OnDeath.Invoke(this);
        if (WaitForTriggerToCommitDie) return;
        CommitDie();
    }

    public virtual void CommitDie() {
        Pooler.Instance.SpawnObject(DeathVfx, transform.position);
        gameObject.SetActive(false);
    }
}