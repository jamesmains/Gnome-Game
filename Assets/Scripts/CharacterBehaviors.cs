using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CharacterBehavior {
    public virtual bool IsUnique() {
        return false;
    }

    public virtual void BehaviorFixedUpdate(Character c) {
    }

    public virtual void BehaviorUpdate(Character c) {
    }

    public virtual void OnAddBehavior(Character c) {
    }

    public virtual void OnRemoveBehavior(Character c) {
    }
}