using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ItemBehavior : MonoBehaviour
{
    [Header("Bottom Layer Settings")]
    public LayerMask bottomLayerMask = 1 << 8; // Default to layer 8; adjust in the Inspector as needed

    [Header("Physics Settings")]
    public bool freezePositionOnContact = true; // Whether to freeze position upon contact
    public bool freezeRotationOnContact = true; // Whether to freeze rotation upon contact
    public float contactThreshold = 0.01f; // Contact threshold that prevents minor movement

    [Header("Tag Detection")]
    public string targetTag = "Item"; // Tag to check against
    private Rigidbody rb;
    private Collider itemCollider;
    private bool isOnBottom = false;
    private Vector3 relativeOffset;
    private Quaternion lastRotation;
    private float lastContactTime;
    private Collider bottomCollider;
    private Transform bottomTransform;
    private Transform originalParent; // Original parent transform
    private bool hasCounted = false; // Tracks whether counting already occurred

    // Events
    public System.Action<Collider> OnBottomContact;
    public System.Action OnBottomExit;
    public System.Action<GameObject> OnBottomDestroyed; // Invoked when the bottom object is destroyed
    public System.Action<GameObject, string> OnTaggedItemDestroyed; // Invoked when an item with the specified tag is destroyed

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
        originalParent = transform.parent; // Cache the initial parent transform

        if (rb == null)
        {
            Debug.LogError($"ItemBehavior: {gameObject.name} is missing a Rigidbody component!");
            enabled = false;
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogError($"ItemBehavior: {gameObject.name} is missing a Collider component!");
            enabled = false;
            return;
        }

        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (isOnBottom && bottomTransform != null)
        {
            if (freezePositionOnContact)
            {
                MaintainRelativePosition();
            }

            if (freezeRotationOnContact)
            {
                FreezeRotation();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bottomLayerMask == (bottomLayerMask | (1 << collision.gameObject.layer)))
        {
            if (!isOnBottom)
            {
                isOnBottom = true;
                bottomCollider = collision.collider;
                bottomTransform = collision.transform;
                lastContactTime = Time.time;

                relativeOffset = transform.position - bottomTransform.position;
                lastRotation = transform.rotation;

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // Bind hierarchy relationship
                transform.SetParent(bottomTransform, true);

                // Count immediately upon contact
                if (!hasCounted)
                {
                    CheckTagAndCount();
                    hasCounted = true; // Mark as counted
                }

                OnBottomContact?.Invoke(bottomCollider);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (bottomLayerMask == (bottomLayerMask | (1 << collision.gameObject.layer)))
        {
            if (bottomCollider == collision.collider)
            {
                StartCoroutine(DelayedBottomCheck());
            }
        }
    }

    IEnumerator DelayedBottomCheck()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        bool stillOnBottom = false;

        Vector3 checkCenter = transform.position;
        Vector3 checkSize = itemCollider.bounds.size * 1.1f;

        Collider[] overlappingColliders = Physics.OverlapBox(checkCenter, checkSize * 0.5f, transform.rotation, bottomLayerMask);

        foreach (Collider col in overlappingColliders)
        {
            if (col != null && col != itemCollider)
            {
                float distance = Vector3.Distance(transform.position, col.ClosestPoint(transform.position));
                if (distance < itemCollider.bounds.extents.magnitude * 0.5f)
                {
                    stillOnBottom = true;
                    bottomCollider = col;
                    bottomTransform = col.transform;
                    relativeOffset = transform.position - bottomTransform.position;
                    break;
                }
            }
        }

        if (!stillOnBottom)
        {
            isOnBottom = false;
            bottomCollider = null;
            bottomTransform = null;

            // Reset the count flag so the next contact can be counted
            hasCounted = false;

            // Invoke exit event
            OnBottomExit?.Invoke();
        }
    }

    void MaintainRelativePosition()
    {
        if (!isOnBottom || bottomTransform == null) return;

        Vector3 targetPosition = bottomTransform.position + relativeOffset;

        if (Vector3.Distance(transform.position, targetPosition) > contactThreshold)
        {
            transform.position = targetPosition;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    void FreezeRotation()
    {
        if (Quaternion.Angle(transform.rotation, lastRotation) > contactThreshold)
        {
            transform.rotation = lastRotation;

            if (rb != null && !rb.isKinematic)
            {
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    // Check whether the bottom object was destroyed
    void LateUpdate()
    {
        if (isOnBottom && bottomTransform == null)
        {
            // Bottom object destroyed; invoke the event
            OnBottomDestroyed?.Invoke(gameObject);

            // Reset state
            isOnBottom = false;
            bottomCollider = null;
            bottomTransform = null;
            hasCounted = false; // Reset the count marker
        }
    }

    // Check the tag and increment counts
    private void CheckTagAndCount()
    {
        // Verify the object carries the specified tag
        if (gameObject.CompareTag(targetTag))
        {
            // Increment the counter via CountManager
            CountManager.IncrementTagCount(targetTag);

            // Invoke callback
            OnTaggedItemDestroyed?.Invoke(gameObject, targetTag);
        }
    }

    public void SetBottomContact(bool onBottom, Collider bottom = null)
    {
        isOnBottom = onBottom;

        if (onBottom)
        {
            bottomCollider = bottom;
            bottomTransform = bottom?.transform;
            if (bottomTransform != null)
            {
                relativeOffset = transform.position - bottomTransform.position;
                transform.SetParent(bottomTransform, true);
            }
            lastRotation = transform.rotation;
            lastContactTime = Time.time;

            // Also count when the contact is applied manually
            if (!hasCounted)
            {
                CheckTagAndCount();
                hasCounted = true;
            }

            OnBottomContact?.Invoke(bottomCollider);
        }
        else
        {
            bottomCollider = null;
            bottomTransform = null;
            hasCounted = false; // Reset the count marker
            OnBottomExit?.Invoke();
        }
    }

    public bool IsOnBottom() => isOnBottom;
    public Collider GetBottomCollider() => bottomCollider;
    public Vector3 GetRelativeOffset() => relativeOffset;
    public float GetLastContactTime() => lastContactTime;

    public void ResetFreezeState()
    {
        if (bottomTransform != null)
        {
            relativeOffset = transform.position - bottomTransform.position;
        }
        lastRotation = transform.rotation;
    }

    public void ForceExitBottom()
    {
        isOnBottom = false;
        bottomCollider = null;
        bottomTransform = null;
        hasCounted = false; // Reset the count marker
        OnBottomExit?.Invoke();
    }

    // Manually reset count state so it can be counted again
    public void ResetCountState()
    {
        hasCounted = false;
    }
}