using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnableEffect {
    public List<GameObject> VfxList;
    public List<AudioClip> SfxList;

    public void PlayEffect(Vector3 location) {
        foreach (var vfx in VfxList) {
            Pooler.Instance.SpawnObject(vfx, location);
        }

        foreach (var sfx in SfxList) {
            AudioManager.OnPlayClip.Invoke(sfx);
        }
    }
}