using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign CameraRig -> YawPivot here. Used for camera-relative movement and facing.")]
    [SerializeField] private Transform cameraYaw;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float groundAcceleration = 45f;
    [Tooltip("How quickly the player turns to match the camera yaw (degrees/sec).")]
    [SerializeField] private float turnSpeed = 720f;
    
    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("Jump")]
    [SerializeField] private float jumpImpulse = 6f;
    [SerializeField] private float airAcceleration = 12f;
    [SerializeField] private float maxAirSpeed = 8.5f;
    [Tooltip("Extra distance beyond the capsule bottom used for ground detection.")]
    [SerializeField] private float groundCheckExtraDistance = 0.05f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Dodge")]
    [SerializeField] private float dodgeSpeed = 13f;
    [SerializeField] private float dodgeDuration = 0.18f;
    [SerializeField] private float dodgeCooldown = 0.45f;
    [SerializeField] private float doubleTapWindow = 0.25f;

    private Rigidbody _rb;
    private CapsuleCollider _capsule;

    private Vector2 _moveInput;
    private bool _jumpRequested;
    private bool _dodgeRequested;

    private float _dodgeTimeRemaining;
    private float _nextDodgeTime;
    private Vector3 _dodgeVelocity;

    private float _lastWPress = -1f;
    private float _lastAPress = -1f;
    private float _lastSPress = -1f;
    private float _lastDPress = -1f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (_moveInput.sqrMagnitude > 1f) _moveInput.Normalize();

        if (Input.GetButtonDown("Jump"))
            _jumpRequested = true;

        if (Input.GetKeyDown(KeyCode.LeftAlt) || WasDoubleTapped(KeyCode.W, ref _lastWPress) || WasDoubleTapped(KeyCode.A, ref _lastAPress) ||
            WasDoubleTapped(KeyCode.S, ref _lastSPress) || WasDoubleTapped(KeyCode.D, ref _lastDPress))
            _dodgeRequested = true;
    }

    private void FixedUpdate()
    {
        Vector3 forward = GetFlattenedForward();
        Vector3 right = GetFlattenedRight();
        Vector3 moveDir = forward * _moveInput.y + right * _moveInput.x;

        bool grounded = IsGrounded();
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && _moveInput.sqrMagnitude > 0.01f;
        float targetSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

        FaceCameraYaw();

        if (_dodgeRequested)
            TryStartDodge(moveDir, forward);

        _dodgeRequested = false;

        if (_dodgeTimeRemaining > 0f)
        {
            _dodgeTimeRemaining -= Time.fixedDeltaTime;
            SetHorizontalVelocity(_dodgeVelocity);
            HandleJump(grounded);
            return;
        }

        if (_jumpRequested && grounded)
        {
            HandleJump(true);
            return;
        }

        if (grounded)
            ApplyGroundMovement(moveDir, targetSpeed);
        else
            ApplyAirMovement(moveDir, targetSpeed);

        HandleJump(grounded);
    }

    private void ApplyGroundMovement(Vector3 moveDir, float targetSpeed)
    {
        Vector3 targetHorizontal = moveDir * targetSpeed;
        Vector3 horizontal = Vector3.MoveTowards(GetHorizontalVelocity(), targetHorizontal, groundAcceleration * Time.fixedDeltaTime);
        SetHorizontalVelocity(horizontal);
    }

    private void ApplyAirMovement(Vector3 moveDir, float targetSpeed)
    {
        if (moveDir.sqrMagnitude < 0.0001f)
            return;

        float airTargetSpeed = Mathf.Min(targetSpeed, maxAirSpeed);
        Vector3 targetHorizontal = moveDir * airTargetSpeed;
        Vector3 horizontal = Vector3.MoveTowards(GetHorizontalVelocity(), targetHorizontal, airAcceleration * Time.fixedDeltaTime);
        SetHorizontalVelocity(Vector3.ClampMagnitude(horizontal, maxAirSpeed));
    }

    private void HandleJump(bool grounded)
    {
        if (!_jumpRequested)
            return;

        _jumpRequested = false;

        if (!grounded)
            return;

        Vector3 velocity = _rb.linearVelocity;
        velocity.y = jumpImpulse;
        _rb.linearVelocity = velocity;
    }

    private void TryStartDodge(Vector3 moveDir, Vector3 forward)
    {
        if (Time.time < _nextDodgeTime)
            return;

        Vector3 direction = moveDir.sqrMagnitude > 0.0001f ? moveDir.normalized : forward;
        _dodgeVelocity = direction * dodgeSpeed;
        _dodgeTimeRemaining = dodgeDuration;
        _nextDodgeTime = Time.time + dodgeCooldown;
    }

    private bool WasDoubleTapped(KeyCode key, ref float lastPressTime)
    {
        if (!Input.GetKeyDown(key))
            return false;

        bool wasDoubleTap = Time.time - lastPressTime <= doubleTapWindow;
        lastPressTime = Time.time;
        return wasDoubleTap;
    }

    private Vector3 GetHorizontalVelocity()
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.y = 0f;
        return velocity;
    }

    private void SetHorizontalVelocity(Vector3 horizontalVelocity)
    {
        Vector3 velocity = _rb.linearVelocity;
        velocity.x = horizontalVelocity.x;
        velocity.z = horizontalVelocity.z;
        _rb.linearVelocity = velocity;
    }

    private Vector3 GetFlattenedForward()
    {
        Vector3 forward = cameraYaw ? cameraYaw.forward : Vector3.forward;
        forward.y = 0f;
        return forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.forward;
    }

    private Vector3 GetFlattenedRight()
    {
        Vector3 right = cameraYaw ? cameraYaw.right : Vector3.right;
        right.y = 0f;
        return right.sqrMagnitude > 0.0001f ? right.normalized : Vector3.right;
    }

    private void FaceCameraYaw()
    {
        if (!cameraYaw)
            return;

        float yaw = cameraYaw.eulerAngles.y;
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        _rb.MoveRotation(Quaternion.RotateTowards(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
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
