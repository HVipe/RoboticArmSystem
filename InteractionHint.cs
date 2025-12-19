using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ConveyorSystem;

public class InteractionPrompt : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject promptUI;
    public string promptText = "Press F to start";
    public float interactionDistance = 3f;

    [Header("Conveyor Control")]
    public ConveyorBelt conveyorBelt; 

    [Header("Input Settings")]
    public bool useVRInput = false; // Disable VR input by default to avoid conflicts
    public bool useKeyboardInput = true; // Enable keyboard input

    private Transform player;
    private bool isPlayerNearby = false;
    private List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    private void Start()
    {
        // Locate the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }

        // Hide the prompt UI initially
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (conveyorBelt == null)
        {
            Debug.LogError("Please assign the conveyorBelt reference in the Inspector!");
        }

        // Optionally initialize XR devices
        if (useVRInput)
        {
            InitializeXRDevices();
        }

        Debug.Log("InteractionPrompt initialized");
    }

    private void InitializeXRDevices()
    {
        inputDevices.Clear();
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(
            UnityEngine.XR.InputDeviceCharacteristics.Controller | UnityEngine.XR.InputDeviceCharacteristics.Right,
            inputDevices);
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= interactionDistance)
        {
            if (!isPlayerNearby)
            {
                ShowPrompt();
            }
            isPlayerNearby = true;

            // Check input triggers
            CheckInput();
        }
        else
        {
            if (isPlayerNearby)
            {
                HidePrompt();
            }
            isPlayerNearby = false;
        }
    }

    private void CheckInput()
    {
        bool inputDetected = false;

        // Keyboard input
        if (useKeyboardInput && Input.GetKeyDown(KeyCode.F))
        {
            inputDetected = true;
            Debug.Log("Keyboard input: F key detected");
        }

        // VR controller input
        if (useVRInput && CheckVRInput())
        {
            inputDetected = true;
            Debug.Log("VR input detected");
        }

        if (inputDetected)
        {
            ActivateConveyor();
        }
    }

    private bool CheckVRInput()
    {
        foreach (var device in inputDevices)
        {
            if (device.isValid)
            {
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
                {
                    return true;
                }

                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.trigger, out float triggerValue) && triggerValue > 0.5f)
                {
                    return true;
                }

                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool gripButton) && gripButton)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ShowPrompt()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(true);
            Debug.Log("Interaction prompt shown");
        }
    }

    private void HidePrompt()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
            Debug.Log("Interaction prompt hidden");
        }
    }

    private void ActivateConveyor()
    {
        if (conveyorBelt == null)
        {
            Debug.LogError("Conveyor belt reference is null!");
            return;
        }

        if (!isPlayerNearby)
        {
            Debug.LogWarning("Player is outside the interaction range!");
            return;
        }

        Debug.Log("Conveyor belt activated!");
        conveyorBelt.ActivateConveyor();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}