using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign CameraRig -> YawPivot here. Used for camera-relative movement and facing.")]
    [SerializeField] private Transform cameraYaw;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [Tooltip("How quickly the player turns to match the camera yaw (degrees/sec).")]
    [SerializeField] private float turnSpeed = 720f;
    
    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 6f;
    [Tooltip("Extra distance beyond the capsule bottom used for ground detection.")]
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
        if (Input.GetButtonDown("Jump"))
            _jumpRequested = true;
    }

    private void FixedUpdate()
    {
        // --- Input (W/S forward/back, A/D left/right) ---
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(x, 0f, z);
        if (input.sqrMagnitude > 1f) input.Normalize();

        // --- Camera-relative movement (flattened to ground plane) ---
        Vector3 forward = cameraYaw ? cameraYaw.forward : Vector3.forward;
        Vector3 right = cameraYaw ? cameraYaw.right : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.z + right * input.x;
        
        // Sprint
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && input.sqrMagnitude > 0.01f;
        float currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        // Move
        Vector3 newPosition = _rb.position + moveDir * (currentSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(newPosition);

        // --- Grounded-style facing: face the camera yaw (so camera stays "behind") ---
        if (cameraYaw)
        {
            float yaw = cameraYaw.eulerAngles.y;
            Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
            _rb.MoveRotation(
                Quaternion.RotateTowards(
                    _rb.rotation,
                    targetRot,
                    turnSpeed * Time.fixedDeltaTime
                )
            );
        }

        // Jump
        if (!_jumpRequested) return;
        _jumpRequested = false;

        if (IsGrounded())
            _rb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        float radius = Mathf.Max(0.01f, _capsule.radius * 0.95f);

        Vector3 center = transform.position + _capsule.center;
        float halfHeight = Mathf.Max(_capsule.height * 0.5f - _capsule.radius, 0f);
        Vector3 bottom = center + Vector3.down * halfHeight;

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
