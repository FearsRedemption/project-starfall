using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 6f;
    [SerializeField] private float groundCheckExtraDistance = 0.05f;
    [SerializeField] private LayerMask groundLayers = ~0;

    private Rigidbody _rb;
    private CapsuleCollider _capsule;

    private bool _jumpRequested;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        // Read input here (never miss button-down)
        if (Input.GetButtonDown("Jump"))
            _jumpRequested = true;
    }

    private void FixedUpdate()
    {
        // Movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(x, 0f, z).normalized;
        Vector3 newPosition = _rb.position + move * (moveSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);

        // Jump
        if (_jumpRequested)
        {
            _jumpRequested = false;

            if (IsGrounded())
            {
                _rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
            }
        }
    }

    private bool IsGrounded()
    {
        // SphereCast just below the capsule to detect ground reliably.
        float radius = Mathf.Max(0.01f, _capsule.radius * 0.95f);

        // Bottom of capsule in world space
        Vector3 center = transform.position + _capsule.center;
        float halfHeight = Mathf.Max(_capsule.height * 0.5f - _capsule.radius, 0f);
        Vector3 bottom = center + Vector3.down * halfHeight;

        // Start slightly above the bottom so we don't start inside the ground
        Vector3 origin = bottom + Vector3.up * 0.02f;

        float castDistance = 0.02f + groundCheckExtraDistance;

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            castDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore
        );
    }
}
