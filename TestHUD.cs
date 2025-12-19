using UnityEngine;

namespace ConveyorSystem
{
    /// <summary>
    /// Perfect VR HUD — stays inside FOV (ScreenSpaceOverlay),
    /// rotates with head direction but never moves in world space.
    /// This avoids clipping, drifting, and floating screen issues.
    /// </summary>
    public class VRFixedHUD : MonoBehaviour
    {
        [Tooltip("VR camera (auto-detected if empty). Usually Main Camera inside XR Origin.")]
        public Transform vrCamera;

        [Header("Options")]
        [Tooltip("UI rotates horizontally to follow head direction.")]
        public bool rotateWithHead = true;

        [Tooltip("Ignore head pitch (up/down tilt) so HUD never leaves screen.")]
        public bool ignoreVerticalPitch = true;

        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("VRFixedHUD requires a Canvas component!");
                enabled = false;
                return;
            }

            // ⭐ KEY: Must be Overlay mode for perfect VR HUD (never clipped)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        private void Start()
        {
            // Auto-detect camera if not assigned
            if (vrCamera == null)
            {
                if (Camera.main != null)
                    vrCamera = Camera.main.transform;
                else
                {
                    var cam = FindFirstObjectByType<Camera>();
                    if (cam != null)
                        vrCamera = cam.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (!rotateWithHead || vrCamera == null)
                return;

            // Copy head rotation
            Vector3 euler = vrCamera.rotation.eulerAngles;

            // Prevent HUD from tilting up/down (no pitch)
            if (ignoreVerticalPitch)
            {
                euler.x = 0f;  // remove pitch
                euler.z = 0f;  // remove roll
            }

            // Apply rotation to Canvas
            transform.rotation = Quaternion.Euler(euler);
        }
    }
}
