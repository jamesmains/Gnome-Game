using System;
using System.Collections;
using System.Collections.Generic;
using I302.Manu;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Pickup : Entity {
    [FoldoutGroup("Settings")][SerializeField] private float ExpulseForce;
    [FoldoutGroup("Settings")][SerializeField] private float ExpulseHeightMultiplier = 1;
    [FoldoutGroup("Settings")][SerializeField] private float ActivationTime = 1f;
    [FoldoutGroup("Settings")][SerializeField] private float DespawnTime;
    [FoldoutGroup("Hooks")] public Inventory PlayerInventory;
    [FoldoutGroup("Hooks")][SerializeField] private Rigidbody Rb;
    [FoldoutGroup("Hooks")][SerializeField] private SpriteRenderer Renderer;
    [FoldoutGroup("Status")] [ReadOnly] public Item Item;
    [FoldoutGroup("Status")] [ReadOnly] public int Amount;

    private bool _canPickup = false;

    public void Setup(Item incomingItem, int quantity)
    {
        Item = incomingItem;
        Amount = quantity;
        Renderer.sprite = Item.Icon;
        ExpulsePickup();
        StopAllCoroutines();
        StartCoroutine(DelayActivate());
        StartCoroutine(Despawn());
    }

    [Button]
    private void ExpulsePickup()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(0.25f, 1f)*ExpulseHeightMultiplier;
        float z = Random.Range(-1f, 1f);
        Rb.AddForce(new Vector3(x,y,z) * ExpulseForce);
    }
    
    IEnumerator DelayActivate()
    {
        yield return new WaitForSeconds(ActivationTime);
        _canPickup = true;
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(DespawnTime);
        Die(null);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _canPickup)
        {
            var displayAmount = Amount;
            Amount = PlayerInventory.TryAddItem(Item, Amount);
            if (displayAmount != Amount)
            {
                Item.OnPickupItem.Invoke();
                // GameEvents.OnPickupItem.Raise();
            }
            if(Amount <= 0)
                Destroy(this.gameObject); 
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPickup = true;
        }
    }
}
