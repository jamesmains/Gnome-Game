using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType {
    Generic
}

[CreateAssetMenu(fileName = "Item", menuName = "Inventories and Items/Item")]
public class Item : SerializedScriptableObject {
    [FoldoutGroup("Settings")] [SerializeField] [PreviewField]
    public Sprite Icon;

    [FoldoutGroup("Settings")] [SerializeField]
    public string Name;

    [FoldoutGroup("Settings")] [SerializeField] [TextArea(4, 10)]
    public string Description;

    [FoldoutGroup("Settings")] [SerializeField]
    public ItemType ItemType;

    [FoldoutGroup("Settings")] [Title("Buy,Sell")] [SerializeField]
    public Vector2 Value;

    [FoldoutGroup("Settings")] [SerializeField]
    public bool IsConsumable;

    [FoldoutGroup("Status")] [SerializeField]
    public bool IsIdentified;

    [SerializeField] [FoldoutGroup("Events")]
    public UnityEvent OnPickupItem = new();

    [SerializeField] [FoldoutGroup("Settings")]
    public List<ItemEffect> ItemEffects = new();

    [SerializeField] [FoldoutGroup("Settings")]
    public int StackLimit = 999;
}

[Serializable]
public abstract class ItemEffect {
    public string effectText;
    public abstract void OnConsume();
    [PropertyOrder(100)] public UnityEvent OnConsumeEvent = new();
}


public class AddItemEffect : ItemEffect {
    [SerializeField] private Inventory inventory;
    [SerializeField] private int amount;
    [SerializeField] private Item item;

    public override void OnConsume() {
        inventory.TryAddItem(item, amount);
    }
}

public class RemoveItemEffect : ItemEffect {
    [SerializeField] private Inventory inventory;
    [SerializeField] private int amount;
    [SerializeField] private Item item;

    public override void OnConsume() {
        inventory.TryRemoveItem(item, amount);
    }
}

public class ItemStatusEffect : ItemEffect {
    [SerializeField] private float tickRate;
    [SerializeField] private float duration;
    [FoldoutGroup("Debug"), ReadOnly] protected float tickTimer;
    [FoldoutGroup("Debug"), ReadOnly] protected float durationTimer;
    [FoldoutGroup("Debug"), ReadOnly] protected bool isActive;

    public override void OnConsume() {
        durationTimer = duration;
        isActive = true;
    }

    public virtual bool OnTick() {
        if (!isActive) OnConsume();
        if (tickTimer > 0)
            tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
            OnTickEffect();

        if (durationTimer > 0)
            durationTimer -= Time.deltaTime;
        if (durationTimer <= 0) {
            OnRemoveEffect();
            return true;
        }

        return false;
    }

    protected virtual void OnTickEffect() {
        tickTimer = tickRate;
    }

    public virtual void OnRemoveEffect() {
        isActive = false;
    }
}