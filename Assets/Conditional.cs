using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I302.Manu;
using Sirenix.OdinInspector;
using UnityEngine;


#region Generic

[Serializable]
public abstract class Condition
{
    public abstract bool IsConditionMet();
    public abstract void Use();
}

public class AndCondition : Condition
{
    [field: SerializeField]
    public List<Condition> Conditions { get; private set; } = new();
    
    public override bool IsConditionMet()
    {
        return Conditions.TrueForAll(c => c.IsConditionMet());
    }
    public override void Use()
    {
        Conditions.ForEach(c => c.Use());
    }
}

public class OrCondition : Condition
{
    [field: SerializeField]
    public List<Condition> Conditions { get; private set; } = new();

    public override bool IsConditionMet()
    {
        return Conditions.Exists(c => c.IsConditionMet());
    }
    
    public override void Use()
    {
        Conditions.ForEach(c => c.Use());
    }
}

public class NotCondition : Condition
{
    [field: SerializeField]
    public List<Condition> Conditions { get; private set; } = new();

    public override bool IsConditionMet()
    {
        return !Conditions.Exists(c => c.IsConditionMet());
    }
    
    public override void Use()
    {
        Conditions.ForEach(c => c.Use());
    }
}

#endregion

#region Calendar

public abstract class CalendarCondition : Condition
{
    public abstract bool IsConditionMet(int day, int month);
    public override bool IsConditionMet()
    {
        throw new NotImplementedException();
    }

    public override void Use()
    {
        throw new NotImplementedException();
    }
}

public class CalendarAndCondition : CalendarCondition
{
    [field: SerializeField]
    public List<CalendarCondition> Conditions { get; private set; } = new List<CalendarCondition>();
    
    public override bool IsConditionMet(int day, int month)
    {
        return Conditions.TrueForAll(c => c.IsConditionMet(day,month));
    }
}

public class CalendarOrCondition : CalendarCondition
{
    [field: SerializeField]
    public List<CalendarCondition> Conditions { get; private set; } = new List<CalendarCondition>();

    public override bool IsConditionMet(int day, int month)
    {
        return Conditions.Exists(c => c.IsConditionMet(day,month));
    }
}

public class CalendarNotCondition : CalendarCondition
{
    [field: SerializeField]
    public List<CalendarCondition> Conditions { get; private set; } = new List<CalendarCondition>();

    public override bool IsConditionMet(int day, int month)
    {
        return !Conditions.Exists(c => c.IsConditionMet(day,month));
    }
}

#endregion

public class ItemConditional : Condition
{
    [field: SerializeField]
    public Item Item { get; private set; }
    [field: SerializeField]
    public int RequiredAmount { get; private set; }
    [field: SerializeField, Tooltip("This will use any item in the first slot")]
    public bool IsGift { get; private set; }
    [field: SerializeField]
    public bool ConsumeOnUse { get; private set; }
    [field: SerializeField]
    public Inventory Inventory { get; private set; }
    
    public override bool IsConditionMet()
    {
        return Inventory.HasItem(Item, RequiredAmount) || (IsGift && Inventory.InventoryItems[0].Item != null);
    }

    public override void Use()
    {
        if(ConsumeOnUse) Inventory.TryUseItem(Item,RequiredAmount);
    }
}




