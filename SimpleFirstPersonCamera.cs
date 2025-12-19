using UnityEngine;
#if ENABLE_VR && ENABLE_XR_MODULE
using UnityEngine.XR;
#endif

public class SimpleEyeCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform; // Assign the Main Camera here
    public Transform playerTransform; // Assign the Player here
    public Vector3 eyeOffset = new Vector3(0, 1.6f, 0); // Eye offset from the player origin

    [Header("Look Settings")]
    public float lookSpeed = 2f; // Mouse look sensitivity
    public float maxLookAngle = 80f; // Maximum vertical look angle
    public bool invertY = false; // Invert vertical mouse movement
    public bool enableMouseLook = true; // Enable mouse look controls

    [Header("VR Settings")]
    [Tooltip("在 VR 模式下自动禁用此脚本（VR 相机的 Transform 由 XR 系统控制）")]
    public bool autoDisableInVR = true;

    private float rotationX = 0f;
    private bool isInitialized = false;
    private bool isVREnabled = false;

    void Start()
    {
        // 检查是否在 VR 模式下
        CheckVRMode();
        
        if (isVREnabled && autoDisableInVR)
        {
            Debug.Log("SimpleEyeCamera: 检测到 VR 模式，自动禁用此脚本（VR 相机的 Transform 由 XR 系统控制）");
            enabled = false;
            return;
        }
        
        InitializeCamera();
    }
    
    /// <summary>
    /// 检查是否在 VR 模式下
    /// </summary>
    private void CheckVRMode()
    {
#if ENABLE_VR && ENABLE_XR_MODULE
        if (XRSettings.enabled && XRSettings.isDeviceActive)
        {
            string deviceName = XRSettings.loadedDeviceName;
            if (!string.IsNullOrEmpty(deviceName) && 
                deviceName != "None" && 
                deviceName != "MockHMD" && 
                deviceName != "OpenVR Display")
            {
                isVREnabled = true;
                return;
            }
        }
#endif
        isVREnabled = false;
    }

    void InitializeCamera()
    {
        Debug.Log("=== Starting Simple Eye Camera initialization ===");

        // Auto-detect the Main Camera
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                Debug.Log("Automatically bound to the main camera");
            }
            else
            {
                Debug.LogError("Main camera not found!");
                return;
            }
        }

        // Auto-detect the Player
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log($"Automatically bound to Player: {player.name}");
            }
            else
            {
                Debug.LogError("Player object not found!");
                return;
            }
        }

        // Position the camera at eye level
        if (cameraTransform != null && playerTransform != null)
        {
            cameraTransform.position = playerTransform.position + eyeOffset;
            Debug.Log("Camera positioned at eye level");
        }

        // Lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isInitialized = true;
        Debug.Log("Simple eye camera initialization complete");
        Debug.Log("=== Initialization finished ===");
    }

    void Update()
    {
        // 如果启用了 VR 自动禁用，且检测到 VR，则直接返回
        if (autoDisableInVR)
        {
            CheckVRMode();
            if (isVREnabled)
            {
                return;
            }
        }
        
        if (!isInitialized)
        {
            InitializeCamera();
            return;
        }

        HandleMouseLook();
        HandleCameraPosition();
    }

    void HandleMouseLook()
    {
        if (!enableMouseLook || cameraTransform == null || playerTransform == null) return;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Horizontal look (rotate the player body)
        playerTransform.Rotate(0, mouseX, 0);

        // Vertical look (camera only)
        if (invertY)
            mouseY = -mouseY;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);
    }

    void HandleCameraPosition()
    {
        if (cameraTransform == null || playerTransform == null) return;

        // Move the camera to the eye position
        cameraTransform.position = playerTransform.position + eyeOffset;

        // Align the camera rotation
        cameraTransform.rotation = Quaternion.Euler(rotationX, playerTransform.eulerAngles.y, 0);
    }

    // Public API: set eye offset
    public void SetEyeOffset(Vector3 offset)
    {
        eyeOffset = offset;
        Debug.Log($"Eye offset set to: {eyeOffset}");
    }

    // Public API: set look sensitivity
    public void SetLookSpeed(float speed)
    {
        lookSpeed = speed;
        Debug.Log($"Look speed set to: {lookSpeed}");
    }

    // Public API: toggle mouse look
    public void ToggleMouseLook()
    {
        enableMouseLook = !enableMouseLook;
        Debug.Log($"Mouse look: {(enableMouseLook ? "Enabled" : "Disabled")}");
    }

    // Public API: force-reset camera position
    public void ForceResetCamera()
    {
        if (cameraTransform != null && playerTransform != null)
        {
            cameraTransform.position = playerTransform.position + eyeOffset;
            cameraTransform.rotation = Quaternion.Euler(rotationX, playerTransform.eulerAngles.y, 0);
            Debug.Log("Camera position force-reset");
        }
    }

    // Unlock the cursor when the script is disabled
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Re-lock the cursor when the script is enabled
    void OnEnable()
    {
        if (isInitialized)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Gizmo helper
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            // Draw the eye position
            Vector3 eyePos = playerTransform.position + eyeOffset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(eyePos, 0.1f);

            // Draw helper lines
            if (cameraTransform != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(playerTransform.position, eyePos);
                Gizmos.DrawLine(eyePos, cameraTransform.position);
            }
        }
    }
}