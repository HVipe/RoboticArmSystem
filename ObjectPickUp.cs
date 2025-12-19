using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public float interactionDistance = 2f;
    public Transform player;
    public string targetTag = "basketball"; // Tag of objects that can be picked up
    public KeyCode pickupKey = KeyCode.F; // Key used to pick up
    public KeyCode dropKey = KeyCode.Q; // Key used to drop

    private int objectCount = 0;
    private List<GameObject> objectsInScene = new List<GameObject>(); // All target objects in the scene
    private GameObject heldObject = null; // Currently held object

    void Start()
    {
        // Find all tagged objects at startup
        FindAllObjects();
    }

    void FindAllObjects()
    {
        // Gather every object with the target tag
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(targetTag);
        objectsInScene.Clear();

        foreach (GameObject obj in allObjects)
        {
            objectsInScene.Add(obj);
        }

        Debug.Log($"Found {objectsInScene.Count} {targetTag} objects");
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Pick up
        if (Input.GetKeyDown(pickupKey) && heldObject == null)
        {
            TryPickupObject();
        }

        // Drop
        if (Input.GetKeyDown(dropKey) && heldObject != null)
        {
            DropObject();
        }
    }

    void TryPickupObject()
    {
        GameObject nearestObject = FindNearestObject();

        if (nearestObject != null)
        {
            PickupObject(nearestObject);
        }
    }

    GameObject FindNearestObject()
    {
        GameObject nearestObject = null;
        float minDistance = float.MaxValue;

        foreach (GameObject obj in objectsInScene)
        {
            if (obj != null && obj.activeSelf)
            {
                float distanceToObject = Vector3.Distance(player.position, obj.transform.position);
                if (distanceToObject < minDistance && distanceToObject <= interactionDistance)
                {
                    minDistance = distanceToObject;
                    nearestObject = obj;
                }
            }
        }

        return nearestObject;
    }

    void PickupObject(GameObject obj)
    {
        heldObject = obj;
        obj.SetActive(false);
        objectCount++;

        Debug.Log($"Picked up object: {obj.name}. Total held count: {objectCount}");
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            // Place object in front of the player
            Vector3 dropPosition = player.position + player.forward * 2f;
            heldObject.transform.position = dropPosition;
            heldObject.SetActive(true);

            Debug.Log($"Dropped object: {heldObject.name}");

            heldObject = null;
        }
    }

    // Public API: add a dynamically spawned object to the list
    public void AddObject(GameObject obj)
    {
        if (obj != null && obj.CompareTag(targetTag) && !objectsInScene.Contains(obj))
        {
            objectsInScene.Add(obj);
        }
    }

    // Public API: remove an object from the tracking list
    public void RemoveObject(GameObject obj)
    {
        if (objectsInScene.Contains(obj))
        {
            objectsInScene.Remove(obj);
        }
    }

    // Public API: refresh list of tracked objects (for dynamic spawns)
    public void RefreshObjectList()
    {
        FindAllObjects();
    }

    // Public API: retrieve current count of picked objects
    public int GetObjectCount()
    {
        return objectCount;
    }

    // Public API: set the current object count
    public void SetObjectCount(int count)
    {
        objectCount = count;
    }

    // Public API: change the target tag
    public void SetTargetTag(string newTag)
    {
        targetTag = newTag;
        RefreshObjectList();
    }

    // Public API: check if an object is currently held
    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    // Public API: get the currently held object reference
    public GameObject GetHeldObject()
    {
        return heldObject;
    }
}