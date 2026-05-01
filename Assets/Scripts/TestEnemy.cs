using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Damageable))]
public class TestEnemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float chaseSpeed = 3.2f;
    [SerializeField] private float stopDistance = 1.4f;
    [SerializeField] private float turnSpeed = 540f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (!target)
            return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        if (distance > detectRange || distance <= stopDistance)
        {
            SetHorizontalVelocity(Vector3.zero);
            return;
        }

        Vector3 direction = toTarget.normalized;
        SetHorizontalVelocity(direction * chaseSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void SetHorizontalVelocity(Vector3 horizontalVelocity)
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
        _rb.linearVelocity = velocity;
    }
}
