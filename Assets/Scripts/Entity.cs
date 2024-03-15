using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public enum EntityTeams {
    IgnoreTeam,
    IgnoreDamage,
    Player,
    Enemies,
    Projectiles
}

public enum EntityType {
    Undefined,
    Humanoid,
    Undead
}

public class Entity : MonoBehaviour, IDamageable, IKillable {
    [SerializeField] [FoldoutGroup("Settings")]
    public EntityTeams Team;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public EntityType Type;
    
    [SerializeField] [FoldoutGroup("Settings")]
    public bool WaitForTriggerToCommitDie;

    [SerializeField] [FoldoutGroup("Settings")]
    protected int MaxHealth;

    [SerializeField] [FoldoutGroup("Settings")]
    protected int Health;

    [SerializeField] [FoldoutGroup("Settings")]
    protected bool StartAtMaxHealth = true;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float DamageBuffer = 0.1f;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Takes off a percentage of the damage")]
    protected List<DamageTypeModifier> Resistances;

    [SerializeField] [FoldoutGroup("Settings")] [Tooltip("Add on a percentage of the damage")]
    protected List<DamageTypeModifier> Weaknesses;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected GameObject HurtVfx;

    [SerializeField] [FoldoutGroup("Hooks")]
    protected GameObject DeathVfx;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected Entity ParentEntity;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected bool ImmuneToDamage;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    protected bool IsDead;

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent OnSpawn = new();
    
    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent OnTakeDamage = new();
    
    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent<Entity> OnDeath = new();

    protected virtual void OnEnable() {
        Spawn();
    }

    protected void OnDisable() {
    }

    protected virtual void Update() {
    }

    protected virtual void FixedUpdate() {
    }

    private void Spawn() {
        OnSpawn.Invoke();
        IsDead = false;
        Health = StartAtMaxHealth ? MaxHealth : Health;
        ImmuneToDamage = false;
    }
    
    public void SetParentEntity(Entity entity) {
        ParentEntity = entity;
    }

    public void SetTeam(EntityTeams team) {
        Team = team;
    }

    public virtual bool TakeDamage(List<DamageSource> damageSources, Vector3 hitPoint, Entity attacker = null,
        bool canHarmAttacker = false) {
        if (ImmuneToDamage || Team == EntityTeams.IgnoreDamage) return false;
        if (attacker != null && attacker == this && !canHarmAttacker) return false;
        if (attacker != null && attacker.Team == Team && attacker.Team != EntityTeams.IgnoreTeam) return false;

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
            PopupManager.DisplayWorldValuePopup(sourceDmg, hitPoint);
        }

        dmg = (int) Mathf.Clamp(dmg, 0, Mathf.Infinity);
        Health -= dmg;
        if (Health <= 0) Die();
        Pooler.Instance.SpawnObject(HurtVfx,
            transform.position);
        OnTakeDamage.Invoke();
        return true;
    }

    IEnumerator ProcessDamageBuffer() {
        ImmuneToDamage = true;
        yield return new WaitForSeconds(DamageBuffer);
        ImmuneToDamage = false;
    }

    public virtual void Die() {
        if (IsDead) return;
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