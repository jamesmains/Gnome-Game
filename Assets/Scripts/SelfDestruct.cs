using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    [SerializeField] private float selfDestructTime = 20f;
    private IKillable killable;

    private void OnEnable() {
        killable = GetComponent<IKillable>();
        StopAllCoroutines();
        StartCoroutine(DestructSelf());
    }

    private IEnumerator DestructSelf() {
        yield return new WaitForSeconds(selfDestructTime);
        if (killable != null)
            killable.Die(null);
        else Destroy(gameObject);
    }
}