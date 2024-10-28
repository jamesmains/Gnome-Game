using System;
using System.Linq;
using JetBrains.Annotations;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IPointerDownHandler,IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField, FoldoutGroup("Looks")] private Color normalColor;
    [field: SerializeField, FoldoutGroup("Looks")] private Color highlightedColor;

    [field: SerializeField,FoldoutGroup("Hooks")] private Vector3 playerPositionVariable; // Replace with current Player Location
    [field: SerializeField,FoldoutGroup("Hooks")] private GameObject pickupItemObject;
    [field: SerializeField,FoldoutGroup("Hooks")] private Image frameDisplay;
    [field: SerializeField,FoldoutGroup("Hooks")] private Image iconDisplay;
    [field: SerializeField,FoldoutGroup("Hooks")] private TextMeshProUGUI countDisplayText;
    
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private InventoryWindow parentInventoryWindow;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private static InventorySlot movingItem;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private bool mouseDown;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private bool waitingForDrag;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private bool splitting;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private bool tryingToSplit;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private bool tryingToConsume;
    [field: SerializeField,FoldoutGroup("Debug"),ReadOnly] private Inventory thisInventory;

    [field: SerializeField] public InventoryItemData storedItemData = new(null,0,-1);
    private static InventorySlot highlightedInventorySlot;

    private void OnEnable()
    {
        // GameEvents.OnMouseScroll += TryMoveItemToOtherOpenWindow;
        // GameEvents.OnAltMouseButtonUp += delegate
        // {
            // if (Global.PlayerLock > 0) return;
        //     if (splitting)
        //     {
        //         tryingToSplit = false;
        //         splitting = false;
        //         return;
        //     }
        //     tryingToSplit = false;
        //     splitting = false;
        //     
        //         TryConsumeItem();
        // };
        // GameEvents.OnAltMouseButtonDown += delegate
        // {
        //     if (highlightedInventorySlot != this) return;
        //     tryingToSplit = true;
        //     tryingToConsume = true;
        // };
    }

    private void OnDisable()
    {
        // movingItem = null;
        // GameEvents.OnItemRelease.Raise();
        // GameEvents.OnMouseScroll -= TryMoveItemToOtherOpenWindow;
        // GameEvents.OnAltMouseButtonUp -= delegate
        // {
        //     if (Global.PlayerLock > 0) return;
        //     if (splitting)
        //     {
        //         tryingToSplit = false;
        //         splitting = false;
        //         return;
        //     }
        //     tryingToSplit = false;
        //     splitting = false;
        //     TryConsumeItem();
        // };
        // GameEvents.OnAltMouseButtonDown -= delegate
        // {
        //     if (highlightedInventorySlot != this) return;
        //     tryingToSplit = true;
        //     tryingToConsume = true;
        // };
    }

    private void Update()
    {
        if (parentInventoryWindow == null || parentInventoryWindow.IsLocked) return;
        if (waitingForDrag && mouseDown && Vector2.Distance(Input.mousePosition, transform.position) > 85f )
        {
            waitingForDrag = false;
            if (movingItem == null)
            {
                Drag();
            }
        }
        else if (!mouseDown) waitingForDrag = false;
    }

    
    public void AssignItemData(InventoryItemData inventoryItemData, int index, InventoryWindow origin, string overrideText = "")
    {
        // if (inventoryItemData.Item is Gear item && inventoryItemData != storedItemData && origin.RestrictByItemType)
        // {
        //     if ( !HasAllowedItemType(origin,inventoryItemData.Item))
        //     {
        //         foreach (var effect in item.GearEffects)
        //         {
        //             effect.OnEquip();
        //         }
        //     }
        // }
        
        storedItemData = inventoryItemData;
        storedItemData.Index = index;
        parentInventoryWindow = origin;
        thisInventory = origin.Inventory;

        if(origin.Inventory.InventoryItems[index].Item == null || storedItemData.Item == null) {Disable(); return; }
        
        iconDisplay.enabled = true;
        iconDisplay.sprite = origin.Inventory.InventoryItems[storedItemData.Index].Item.Icon;
        
        countDisplayText.enabled = true;
        countDisplayText.text = origin.Inventory.InventoryItems[storedItemData.Index].Quantity.ToString();
        
        if (origin.Inventory.InventoryItems[storedItemData.Index].Quantity == 1)
        {
            countDisplayText.enabled = false;
        }
        
        if (string.IsNullOrEmpty(overrideText)) return;
        countDisplayText.enabled = true;
        countDisplayText.text = overrideText;
    }

    public void Disable()
    {
        iconDisplay.enabled = false;
        countDisplayText.enabled = false;
    }

    private void Drag()
    {
        if (parentInventoryWindow.Inventory.InventoryItems[storedItemData.Index].Quantity <= 0 || storedItemData.Item == null) return;
        if (tryingToSplit) splitting = true;
        tryingToConsume = false;
        movingItem = this;
        //GameEvents.OnItemMove.Raise(parentInventoryWindow.Inventory.InventoryItems[storedItemData.Index].Item);
        iconDisplay.color = new Color(1, 1, 1, 0.5f);
    }
    
    private void DropItem(int q = -1)
    {
        var itemData = parentInventoryWindow.Inventory.InventoryItems[storedItemData.Index];
        int quantity = q < 0 ? itemData.Quantity : q;
        quantity = tryingToSplit ? quantity / 2 : quantity;
        if (quantity == 0) return;
        
        Vector3 spawnPos = playerPositionVariable;
        // var pickup = Instantiate(pickupItemObject, spawnPos,Quaternion.identity).GetComponent<Pickup>();
        //
        // pickup.Setup(itemData.Item,quantity);
        // this.thisInventory.TryRemoveItemAt(storedItemData.Index,quantity);
        // if (this.thisInventory.InventoryItems[storedItemData.Index].Quantity <= 0)
        //     storedItemData = null;
    }

    private void TryReleaseItem()
    {
        if (movingItem == null) return;
        if (this != highlightedInventorySlot && highlightedInventorySlot is not null)
        {
            TryMoveItemToSlot();
        }
        else if (InventoryWindow.HighlightedInventoryWindow != parentInventoryWindow && InventoryWindow.HighlightedInventoryWindow != null)
        {
            int quantity = tryingToSplit ? storedItemData.Quantity / 2 : storedItemData.Quantity; 
            TryMoveItemToInventoryWindow(InventoryWindow.HighlightedInventoryWindow,quantity);
        }
        else if(!MouseOverUserInterfaceUtil.IsMouseOverUI)
        {
            DropItem();
        }

        movingItem = null;
        iconDisplay.color = new Color(1, 1, 1, 1);
        // GameEvents.OnItemRelease.Raise();
        // GameEvents.OnMoveOrAddItem.Raise();
    }

    private void TryConsumeItem()
    {
        if (highlightedInventorySlot != this || storedItemData.Item == null || !tryingToConsume) return;
        if (storedItemData.Item.IsConsumable)
        {
            foreach (var itemEffect in storedItemData.Item.ItemEffects)
            {
                itemEffect.OnConsume();   
            }
            thisInventory.TryRemoveItemAt(storedItemData.Index,1);
        }
    }

    private void TryMoveItemToSlot()
    {
        InventorySlot his = highlightedInventorySlot;
        InventoryWindow hisWindow = his.parentInventoryWindow;

        if (hisWindow.RemoveOnly) return;
        if(hisWindow.RestrictByItemType && !HasAllowedItemType(hisWindow))
        {
            return;
        }
        if (storedItemData.Item == his.storedItemData.Item || his.storedItemData.Item == null)
            AddItemToExistingStack(hisWindow,his.storedItemData.Index);
        else
            SwapItemStacks(hisWindow,his.storedItemData.Index);  
    }

    private void AddItemToExistingStack(InventoryWindow hisWindow, int hisIndex)
    {
        var thisItemData = thisInventory.InventoryItems[storedItemData.Index];
        int quantity = thisItemData.Quantity;
        quantity = tryingToSplit ? quantity / 2 : quantity;
        var overflow = hisWindow.Inventory.TryAddItemAt(hisIndex, quantity, storedItemData.Item);
        thisInventory.TryRemoveItemAt(storedItemData.Index,quantity - overflow);
    }

    private void SwapItemStacks(InventoryWindow hisWindow, int hisIndex)
    {
        if (hisWindow.RestrictByItemType && !HasAllowedItemType(hisWindow))
        {
            return;
        }
        thisInventory.TrySwapItem(storedItemData.Index,hisIndex,hisWindow.Inventory); 
    }

    private void TryMoveItemToOtherOpenWindow(float movement)
    {
        if (movement == 0 || storedItemData.Item == null || movingItem != null) return;
        if (InventoryWindow.OpenInventoryWindows.Count <= 1 || highlightedInventorySlot != this) return;
        
        int index = InventoryWindow.OpenInventoryWindows.IndexOf(this.parentInventoryWindow);
        index += (int)Mathf.Abs(movement) * movement > 0 ? 1 : -1;
        if (index >= InventoryWindow.OpenInventoryWindows.Count) index = 0;
        else if (index < 0) index = InventoryWindow.OpenInventoryWindows.Count - 1; 
        
        if(movement > 0)
            TryMoveItemToInventoryWindow(InventoryWindow.OpenInventoryWindows[index], 1);
        else if(movement < 1)
            TryGetItemFromInventoryWindow(InventoryWindow.OpenInventoryWindows[index], 1);
    }
    
    private void TryMoveItemToInventoryWindow(InventoryWindow targetWindow, int amount)
    {
        if ((targetWindow.RestrictByItemType && !HasAllowedItemType(targetWindow)) || targetWindow.RemoveOnly)
        {
            return;
        }
        var i = parentInventoryWindow.Inventory.InventoryItems[storedItemData.Index];
        var overflow = targetWindow.Inventory.TryAddItem(i.Item,amount);
        parentInventoryWindow.Inventory.TryRemoveItemAt(storedItemData.Index,amount - overflow);
    }

    private void TryGetItemFromInventoryWindow(InventoryWindow targetWindow, int amount)
    {
        if (targetWindow.RestrictByItemType && !HasAllowedItemType(targetWindow))
        {
            return;
        }
        var i = targetWindow.Inventory.HasItemAt(storedItemData.Item);
        if (i < 0) return;
        var overflow = parentInventoryWindow.Inventory.TryAddItemAt(storedItemData.Index,amount,storedItemData.Item);
        targetWindow.Inventory.TryRemoveItemAt(i,amount - overflow);
    }

    private bool HasAllowedItemType(InventoryWindow targetWindow)
    {
        if (storedItemData.Item == null) return false;
        return targetWindow.AllowedTypes.Any(o => o.Type == storedItemData.Item.ItemType);
    }

    private bool HasAllowedItemType(InventoryWindow targetWindow, Item targetItem)
    {
        if (targetItem == null) return false;
        return targetWindow.AllowedTypes.Any(o => o.Type == targetItem.ItemType);
    }

    private void ToggleHighlight(bool state)
    {
        frameDisplay.color = state ? highlightedColor : normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDown = true;
        waitingForDrag = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseDown = false;
        TryReleaseItem();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ((highlightedInventorySlot is null || highlightedInventorySlot != this) && !parentInventoryWindow.IsLocked)
        {
            if (highlightedInventorySlot is not null) highlightedInventorySlot.ToggleHighlight(false);
            highlightedInventorySlot = this;
            ToggleHighlight(true);
        }

        if (storedItemData.Item != null)
        {
            string n = storedItemData.Item.Name;
            string t = storedItemData.Item.ItemType.ToString();
            string d = storedItemData.Item.Description;
            string tooltipMessage = $"{n}\n{t}\n{d}";
            // GameEvents.OnShowTooltip.Raise(tooltipMessage);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightedInventorySlot != null)
        {
            highlightedInventorySlot.ToggleHighlight(false);
            highlightedInventorySlot = null;   
        }
        // GameEvents.OnMouseExitInventorySlot.Raise();
        // GameEvents.OnHideTooltip.Raise();
    }
}
