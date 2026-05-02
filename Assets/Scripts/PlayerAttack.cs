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

    [Header("Scene Feedback")]
    [SerializeField] private GameObject swingCue;
    [SerializeField] private float swingCueDuration = 0.12f;

    private float _nextAttackTime;
    private float _hideSwingCueAt;

    private void Awake()
    {
        if (swingCue)
            swingCue.SetActive(false);
    }

    private void Update()
    {
        UpdateSwingCue();

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

        ShowSwingCue(origin, ray.direction);

        if (!TryGetAttackHit(ray, out RaycastHit hit))
            return;

        Debug.Log($"Attack hit {hit.collider.name}");

        if (hit.rigidbody)
            hit.rigidbody.AddForceAtPosition(ray.direction * knockbackImpulse, hit.point, ForceMode.Impulse);

        Damageable damageable = hit.collider.GetComponent<Damageable>() ?? hit.collider.GetComponentInParent<Damageable>();
        if (damageable)
            damageable.TakeDamage(damage);
    }

    private bool TryGetAttackHit(Ray ray, out RaycastHit closestHit)
    {
        closestHit = default;
        bool hasHit = false;
        float closestDistance = float.MaxValue;

        RaycastHit[] hits = Physics.SphereCastAll(ray, attackRadius, attackRange, hitLayers, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider || hit.collider.transform.root == transform.root)
                continue;

            if (hit.distance >= closestDistance)
                continue;

            closestHit = hit;
            closestDistance = hit.distance;
            hasHit = true;
        }

        return hasHit;
    }

    private void ShowSwingCue(Vector3 origin, Vector3 direction)
    {
        if (!swingCue)
            return;

        swingCue.transform.position = origin + direction.normalized * 1.25f;
        swingCue.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, 0f, 25f);
        swingCue.SetActive(true);
        _hideSwingCueAt = Time.time + swingCueDuration;
    }

    private void UpdateSwingCue()
    {
        if (!swingCue || !swingCue.activeSelf || Time.time < _hideSwingCueAt)
            return;

        swingCue.SetActive(false);
    }
}
