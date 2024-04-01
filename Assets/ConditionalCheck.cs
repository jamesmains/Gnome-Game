using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalCheck : SerializedMonoBehaviour
{
    [SerializeField] private bool continousCheck;
    [SerializeField] private float tickRate;
    private float timer;
    [FoldoutGroup("Events")][SerializeField] private UnityEvent onMeetsRequirements;
    [FoldoutGroup("Events")][SerializeField] private UnityEvent onFailsRequirements;
    public List<Condition> Conditions = new();
    private bool cachedState;

    private void Awake()
    {
        CheckStatus(true);
    }

    private void Update()
    {
        if (!continousCheck) return;
        if (timer <= 0)
        {
            CheckStatus();
            timer = tickRate;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    public void CheckStatus(bool resetCache = false)
    {
        bool canDo = IsConditionMet();
        if (resetCache)
        {
            cachedState = !canDo;
        }

        if (canDo && (!cachedState))
        {
            onMeetsRequirements.Invoke();    
        }
        else if(!canDo && (cachedState))
        {
            onFailsRequirements.Invoke();
        }
        cachedState = canDo;
    }
    
    private bool IsConditionMet()
    {
        return Conditions.TrueForAll(c => c.IsConditionMet());
    }
}