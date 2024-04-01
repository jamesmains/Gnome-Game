using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I302.Manu;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;


    [CreateAssetMenu(fileName = "Inventory", menuName = "Inventories and Items/Inventory")]
    public class Inventory : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int InventorySlotLimit;
        [field: SerializeField] public bool CanCreateImageStringMessages;
        [field: SerializeField] public ItemLookupTable lookupTable;
        [SerializeReference] public InventoryItemData[] InventoryItems;
        
        private void OnEnable()
        {
            LoadInventory();
        }
        
        // This function is used but only from unityevents in the inspector
        public void TryAddItem(Item incomingItem) // Need this signature to call it from UnityEvent so ignore the warning on the other function
        {
            TryAddItem(incomingItem, 1); 
           
            // if(CanCreateImageStringMessages)
            //     GameEvents.OnCreateImageStringMessage.Raise(incomingItem.Sprite,"+1");
        }

        public int TryAddItem(Item incomingItem, int value = 1)
        {
            int remaining = value;

            foreach (var existingItemData in InventoryItems)
            {
                if (remaining <= 0) break;
                if (incomingItem != existingItemData.Item ||
                    existingItemData.Item == null)continue;

                if (existingItemData.Quantity == existingItemData.Item.StackLimit) continue;
                
                for (int r = remaining; r > 0; r--)
                {
                    if (remaining <= 0 || existingItemData.Quantity >= existingItemData.Item.StackLimit) continue;
                    existingItemData.Quantity++;
                    remaining--;
                }
            }
            
            foreach (var potentialEmptySlot in InventoryItems)
            {
                if (remaining <= 0) break;
                if (potentialEmptySlot.Item != null) continue;
                potentialEmptySlot.Item = incomingItem;
                potentialEmptySlot.Quantity = 0;
                for (int r = remaining; r > 0; r--)
                {
                    if (remaining <= 0 || potentialEmptySlot.Quantity >= potentialEmptySlot.Item.StackLimit) continue;
                    potentialEmptySlot.Quantity++;
                    remaining--;
                }
            }
            SaveInventory();
            return remaining;
        }

        public void TrySwapItem(int i1, int i2, Inventory otherInventory)
        {
            var itemData1 = InventoryItems[i1];
            var itemData2 = otherInventory.InventoryItems[i2];
            InventoryItems[i1] = itemData2;
            otherInventory.InventoryItems[i2] = itemData1;
            SaveInventory();
        }

        public int TryAddItemAt(int targetIndex, int value = 1, Item incomingItem = null)
        {
            int remaining = value;
            if (remaining == 0) return 0;

            if (InventoryItems[targetIndex].Item == null && incomingItem != null)
            {
                InventoryItems[targetIndex].Item = incomingItem;
                InventoryItems[targetIndex].Quantity = 0;
            }
            
            for (int r = remaining; r > 0; r--)
            {
                if (remaining <= 0 || InventoryItems[targetIndex].Quantity >= InventoryItems[targetIndex].Item.StackLimit) break;
                InventoryItems[targetIndex].Quantity++;
                remaining--;
            }
            SaveInventory();
            return remaining;
        }

        public void TryRemoveItem(Item incomingItem)
        {
            TryRemoveItem(incomingItem, 1);
        }
        
        public int TryRemoveItem(Item incomingItem, int value = 1)
        {
            int remaining = value;

            foreach (var existingItemData in InventoryItems)
            {
                if (remaining <= 0) break;
                if (incomingItem != existingItemData.Item ||
                    existingItemData.Item == null)continue;

                if (existingItemData.Quantity <= 0) continue;
                
                for (int r = remaining; r > 0; r--)
                {
                    if (remaining <= 0 || existingItemData.Quantity <= 0) continue;
                    existingItemData.Quantity--;
                    remaining--;
                }
            }
            SaveInventory();
            return remaining;
        }

        public void TryRemoveItemAt(int ItemIndex, int amount = 1)
        {
            InventoryItems[ItemIndex].Quantity -= amount;
            if (InventoryItems[ItemIndex].Quantity <= 0)
            {
                InventoryItems[ItemIndex].Item = null;
                InventoryItems[ItemIndex].Quantity = 0;
            }
            SaveInventory();
        }
        
        public int TryUseItem(Item neededItem, int amount = 1)
        {
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                if (neededItem != InventoryItems[i].Item) continue;
                if (amount > InventoryItems[i].Quantity)
                {
                    int used = InventoryItems[i].Quantity;
                    InventoryItems[i].Quantity = 0;
                    amount -= used;
                }
                else
                {
                    InventoryItems[i].Quantity -= amount;
                    amount = 0;
                }
                if (InventoryItems[i].Quantity <= 0)
                    InventoryItems[i].Item = null;
                if (amount == 0) break;
            }

            SaveInventory();
            return amount;
        }

        public bool HasItem(Item itemQuery, int quantity = 1)
        {
            int availableQuantity = 0;
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                if (itemQuery != InventoryItems[i].Item) continue;
                availableQuantity += InventoryItems[i].Quantity;
            }

            return availableQuantity >= quantity;
        }

        public int HasItemAt(Item itemQuery)
        {
            int v = -1;
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                if (InventoryItems[i].Item == itemQuery)
                    v = i;
            }
            return v;
        }
        
        [Button]
        public void SortIventoryByEmptySlots()
        {
            for (int i = 0; i < InventorySlotLimit; i++)
            {
                var targetItem = InventoryItems[i].Item; 
                if (targetItem == null) continue;
                for (int j = 0; j < InventorySlotLimit; j++)
                {
                    if (InventoryItems[j].Item != null) continue;
                    TrySwapItem(i,j,this);
                }
            }
            SaveInventory();
        }

        [FoldoutGroup("Saving and Loading")]
        [Button]
        private void VerifyInventory()
        {
            foreach (var t in InventoryItems)
            {
                if (t.Quantity > 0) continue;
                t.Item = null;
                t.Quantity = 0;
            }
        }
        
        [FoldoutGroup("Saving and Loading")][Button]
        private void SaveInventory()
        {
            VerifyInventory();
            // GameEvents.OnMoveOrAddItem.Raise();
            // SaveLoad.SaveInventory(new InventorySaveData(this));
        }

        [FoldoutGroup("Saving and Loading")][Button]
        private void LoadInventory()
        {
            InventoryItems = new InventoryItemData[InventorySlotLimit];
            InventorySaveData saveData = null; // Todo: Fix
        
            if (saveData is null)
            {
                return;
            }
        
            for (int i = 0; i < saveData.InventorySaveDataItems.Length; i++)
            {
                Item loadedItem = lookupTable.GetItem(saveData.InventorySaveDataItems[i]);
                if (loadedItem == null)
                {
                    InventoryItems[i] = new InventoryItemData(null, 0,i);
                    continue;
                }
                int loadedQuantity = saveData.InventorySaveDataQuantities[i];
                InventoryItems[i] = new InventoryItemData(loadedItem,loadedQuantity,i);
            }
            VerifyInventory();
        }

        [FoldoutGroup("Saving and Loading")][Button]
        private void ClearInventory()
        {
            InventoryItems = new InventoryItemData[InventorySlotLimit];
            for (int i = 0; i < InventoryItems.Length; i++)
            {
                InventoryItems[i] = new InventoryItemData(null, 0,i);
            }
        }
    }

[Serializable]
public class InventoryItemData
{
    public InventoryItemData(Item item, int quantity,int index)
    {
        Item = item;
        Quantity = quantity;
        Index = index;
    }
#if UNITY_EDITOR
    [Button]
    public void ClearItemData()
    {
        Item = null;
        Quantity = 0;
        Index = -1;
    }
#endif  
    public Item Item;
    public int Quantity;
    [HideInInspector]public int Index;
}

[Serializable]
public class InventorySaveData
{
    public string Name { get; private set; }   
    [SerializeReference] public string[] InventorySaveDataItems;
    [SerializeReference] public int[] InventorySaveDataQuantities;
        
    public InventorySaveData(Inventory inventory)
    {
        Name = inventory.Name;
        int arrayLength = inventory.InventorySlotLimit;
        InventorySaveDataItems = new string[arrayLength];
        InventorySaveDataQuantities = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            if(inventory.InventoryItems[i].Item == null)
            {
                InventorySaveDataItems[i] = String.Empty;
                InventorySaveDataQuantities[i] = 0;
                continue;
            }
            InventorySaveDataItems[i] = inventory.InventoryItems[i].Item.Name;
            InventorySaveDataQuantities[i] = inventory.InventoryItems[i].Quantity;
        }
    }
}
