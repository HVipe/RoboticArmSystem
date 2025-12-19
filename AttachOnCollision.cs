using UnityEngine;
using System.Collections.Generic;

public class AttachOnCollision : MonoBehaviour
{
    [Tooltip("List of tags allowed to attach on contact")]
    public List<string> allowedTags = new List<string>();

    private void OnCollisionEnter(Collision collision)
    {
        if (IsAllowedTag(collision.gameObject.tag))
        {
            collision.transform.SetParent(transform, true);
            Debug.Log($"{collision.gameObject.name} is now parented to {gameObject.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsAllowedTag(other.tag))
        {
            other.transform.SetParent(transform, true);
            Debug.Log($"{other.gameObject.name} is now parented to {gameObject.name}");
        }
    }

    private bool IsAllowedTag(string tagToCheck)
    {
        foreach (string tag in allowedTags)
        {
            if (tagToCheck == tag)
                return true;
        }
        return false;
    }
}
