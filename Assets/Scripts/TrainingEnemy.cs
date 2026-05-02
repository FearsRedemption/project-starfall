using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Damageable))]
public class TrainingEnemy : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float detectRange = 8f;
    [SerializeField] private float chaseSpeed = 3.2f;
    [SerializeField] private float stopDistance = 1.4f;
    [SerializeField] private float turnSpeed = 540f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.1f;

    private Rigidbody _rb;
    private float _nextAttackTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        if (!target)
        {
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
            if (player)
                target = player.transform;
        }
    }

    private void FixedUpdate()
    {
        if (!target)
            return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        float distance = toTarget.magnitude;
        if (distance > detectRange)
        {
            SetHorizontalVelocity(Vector3.zero);
            return;
        }

        Vector3 direction = toTarget.normalized;
        if (distance <= stopDistance)
        {
            SetHorizontalVelocity(Vector3.zero);
            TryAttack();
            return;
        }

        SetHorizontalVelocity(direction * chaseSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime || !target)
            return;

        _nextAttackTime = Time.time + attackCooldown;

        EnemyAttackCue cue = GetComponent<EnemyAttackCue>();
        if (cue)
            cue.Flash();

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth)
            playerHealth.TakeDamage(attackDamage);
    }

    private void SetHorizontalVelocity(Vector3 horizontalVelocity)
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
        _rb.linearVelocity = velocity;
    }
}
