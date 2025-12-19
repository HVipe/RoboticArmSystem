using UnityEngine;

public class PickUpDrop : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickUpRange = 3f;          // Maximum distance an object can be picked up
    public Transform holdPoint;             // Transform where the held object stays
    public LayerMask pickUpLayer;           // Layers that contain pickup targets

    [Header("Cone Detection Settings")]
    public float coneAngle = 30f;           // Angle of the detection cone in degrees
    public int maxDetectObjects = 10;       // Maximum number of objects to evaluate each frame
    public bool showDebugCone = true;       // Show the debug gizmo for the detection cone

    private Rigidbody heldObjectRb;         // Rigidbody of the currently held object
    private Collider heldObjectCol;         // Collider of the currently held object

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObjectRb == null)
            {
                TryPickUp();
            }
            else
            {
                Drop();
            }
        }
    }

    void TryPickUp()
    {
        // Find the nearest object within the detection cone
        GameObject nearestObject = GetNearestObjectInCone();

        if (nearestObject != null)
        {
            Rigidbody rb = nearestObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObjectRb = rb;
                heldObjectCol = nearestObject.GetComponent<Collider>();

                // Disable physics while the item is held
                heldObjectRb.isKinematic = true;
                heldObjectCol.enabled = false;

                // Snap to the hold point
                heldObjectRb.transform.SetParent(holdPoint);
                heldObjectRb.transform.localPosition = Vector3.zero;
                heldObjectRb.transform.localRotation = Quaternion.identity;

                Debug.Log($"Picked up {heldObjectRb.name}");
            }
        }
    }

    GameObject GetNearestObjectInCone()
    {
        Vector3 playerPosition = Camera.main.transform.position;
        Vector3 playerForward = Camera.main.transform.forward;

        // Collect colliders within range on the configured layers
        Collider[] colliders = Physics.OverlapSphere(playerPosition, pickUpRange, pickUpLayer);

        GameObject nearestObject = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (col == null) continue;

            // Distance from the player to the object
            float distance = Vector3.Distance(playerPosition, col.transform.position);

            // Ensure it is inside the pickup range
            if (distance <= pickUpRange)
            {
                // Angle between player forward and direction to the object
                Vector3 directionToObject = (col.transform.position - playerPosition).normalized;
                float angle = Vector3.Angle(playerForward, directionToObject);

                // Check if the object lies within the cone
                if (angle <= coneAngle * 0.5f)
                {
                    // Keep the closest object as the selection
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestObject = col.gameObject;
                    }
                }
            }
        }

        return nearestObject;
    }

    void Drop()
    {
        if (heldObjectRb == null) return;

        // Restore physics
        heldObjectRb.isKinematic = false;
        heldObjectCol.enabled = true;

        // Detach from the holder
        heldObjectRb.transform.SetParent(null);

        // Prevent inheriting motion from the holder
        heldObjectRb.linearVelocity = Vector3.zero;
        heldObjectRb.angularVelocity = Vector3.zero;

        Debug.Log($"Dropped {heldObjectRb.name}");

        heldObjectRb = null;
        heldObjectCol = null;
    }


    // Visualise the detection cone while the object is selected
    void OnDrawGizmosSelected()
    {
        if (!showDebugCone) return;

        Vector3 playerPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
        Vector3 playerForward = Camera.main != null ? Camera.main.transform.forward : transform.forward;

        // Draw the detection cone boundary
        Gizmos.color = Color.yellow;

        // Half of the cone angle in radians
        float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;

        // Left cone boundary
        Vector3 leftDirection = Quaternion.AngleAxis(-halfAngle * Mathf.Rad2Deg, Vector3.up) * playerForward;
        Gizmos.DrawRay(playerPosition, leftDirection * pickUpRange);

        // Right cone boundary
        Vector3 rightDirection = Quaternion.AngleAxis(halfAngle * Mathf.Rad2Deg, Vector3.up) * playerForward;
        Gizmos.DrawRay(playerPosition, rightDirection * pickUpRange);

        // Radius outline
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPosition, pickUpRange);

        // Forward direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerPosition, playerForward * pickUpRange);
    }

}