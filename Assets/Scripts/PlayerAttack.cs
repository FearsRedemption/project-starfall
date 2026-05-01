using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform aimSource;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackRadius = 0.35f;
    [SerializeField] private float attackHeight = 1.2f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockbackImpulse = 4f;
    [SerializeField] private LayerMask hitLayers = ~0;

    private float _nextAttackTime;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || Time.time < _nextAttackTime)
            return;

        _nextAttackTime = Time.time + attackCooldown;
        Attack();
    }

    private void Attack()
    {
        Transform aim = aimSource ? aimSource : transform;
        Vector3 origin = transform.position + Vector3.up * attackHeight;
        Ray ray = new Ray(origin, aim.forward);

        if (!Physics.SphereCast(ray, attackRadius, out RaycastHit hit, attackRange, hitLayers, QueryTriggerInteraction.Ignore))
            return;

        if (hit.rigidbody)
            hit.rigidbody.AddForceAtPosition(ray.direction * knockbackImpulse, hit.point, ForceMode.Impulse);

        if (hit.collider.TryGetComponent(out Damageable damageable))
            damageable.TakeDamage(damage);
    }
}
