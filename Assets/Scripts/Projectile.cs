using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")] private float InitialVelocity;
    
    private Vector3 moveDirection;
    private Entity parentEntity;
    private float velocity;
    
    private void FixedUpdate() {
        ApplyMovement();
    }

    private void OnEnable() {
        Reset();
    }

    public void Reset() {
        velocity = InitialVelocity;
    }

    // public override void Die() {
    //     gameObject.SetActive(false);
    //     
    //     var obj = Pooler.Instance.SpawnObject(DeathVfx, transform.position);
    //     if (obj == null) return;
    //     var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
    //     obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    // }

    private void ApplyMovement() {
        var _transform = transform;
        var pos = _transform.position;
        pos += moveDirection * (velocity * Time.deltaTime);
        _transform.position = pos;
        transform.rotation = Quaternion.LookRotation (moveDirection);
    }

    public void SetDirection(Vector3 newDirection) {
        moveDirection = newDirection;
        // var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + -90;
        // transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
}