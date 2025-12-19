using UnityEngine;

[ExecuteInEditMode]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Movement speed
    [Tooltip("Input dead zone threshold - ignore input values below this to prevent drift")]
    public float inputDeadZone = 0.1f; // Dead zone to prevent unwanted movement

    [Header("Boundary Settings")]
    [Tooltip("Enable movement boundaries to prevent player from going outside the play area")]
    public bool useBoundaries = true;
    [Tooltip("Minimum allowed position (X, Y, Z)")]
    public Vector3 minBoundary = new Vector3(-10f, -10f, -10f);
    [Tooltip("Maximum allowed position (X, Y, Z)")]
    public Vector3 maxBoundary = new Vector3(10f, 10f, 10f);
    [Tooltip("Show boundary gizmos in Scene view for debugging")]
    public bool showBoundaryGizmos = true;

    private bool isInitialized = false;
    private Rigidbody rb;

    void Start()
    {
        InitializeController();
    }

    void InitializeController()
    {
        Debug.Log("=== Starting Player movement controller initialization ===");
        
        // Get or add Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.LogWarning("PlayerMovementController: No Rigidbody found, added one automatically.");
        }
        
        // Ensure gravity is enabled
        rb.useGravity = true;
        
        // Freeze rotation on X and Z axes to prevent player from tipping over
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        Debug.Log("Player movement controller initialization complete");
        Debug.Log("=== Initialization finished ===");
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
        {
            InitializeController();
            return;
        }
    }

    void FixedUpdate()
    {
        if (!isInitialized || rb == null) return;

        HandleMovement();
    }

    void HandleMovement()
    {
        // Read player input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Apply dead zone to prevent unwanted movement from input drift
        if (Mathf.Abs(h) < inputDeadZone) h = 0f;
        if (Mathf.Abs(v) < inputDeadZone) v = 0f;

        // Only move if there's actual input
        if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
        {
            return; // No input, don't move
        }

        // Calculate movement direction
        Vector3 moveDirection = transform.right * h + transform.forward * v;
        moveDirection.Normalize(); // Normalize to avoid faster diagonal movement

        // Calculate target position
        Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        // Apply boundary constraints if enabled
        if (useBoundaries)
        {
            targetPosition = ClampToBoundaries(targetPosition);
        }

        // Use Rigidbody.MovePosition in FixedUpdate to respect physics (gravity will work)
        // This moves the rigidbody while keeping physics interactions
        rb.MovePosition(targetPosition);
    }

    Vector3 ClampToBoundaries(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minBoundary.x, maxBoundary.x),
            Mathf.Clamp(position.y, minBoundary.y, maxBoundary.y),
            Mathf.Clamp(position.z, minBoundary.z, maxBoundary.z)
        );
    }

    void OnDrawGizmos()
    {
        DrawBoundaryGizmos();
    }

    void OnDrawGizmosSelected()
    {
        DrawBoundaryGizmos();
    }

    void DrawBoundaryGizmos()
    {
        if (!showBoundaryGizmos || !useBoundaries) return;

        // Validate boundary values
        if (minBoundary.x >= maxBoundary.x || 
            minBoundary.y >= maxBoundary.y || 
            minBoundary.z >= maxBoundary.z)
        {
            // Invalid boundaries, draw a warning
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
            return;
        }

        // Draw boundary box in Scene view
        Gizmos.color = Color.yellow;
        Vector3 center = (minBoundary + maxBoundary) * 0.5f;
        Vector3 size = maxBoundary - minBoundary;
        
        // Draw wireframe cube
        Gizmos.DrawWireCube(center, size);
        
        // Also draw corners for better visibility
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawCube(center, size);
        
        // Draw corner markers
        Gizmos.color = Color.yellow;
        float cornerSize = 0.2f;
        Gizmos.DrawWireSphere(new Vector3(minBoundary.x, minBoundary.y, minBoundary.z), cornerSize);
        Gizmos.DrawWireSphere(new Vector3(maxBoundary.x, maxBoundary.y, maxBoundary.z), cornerSize);
    }

    // Public API: change movement speed
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        Debug.Log($"Movement speed set to: {moveSpeed}");
    }

    // Public API: get current speed
    public float GetCurrentSpeed()
    {
        return moveSpeed;
    }
}