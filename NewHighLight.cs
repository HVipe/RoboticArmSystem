using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ConveyorSystem
{
    public class VRHighlightState : MonoBehaviour
    {
        [Header("Highlight")]
        public bool enableHighlight = true;

        [Header("Shader Settings")]
        [Tooltip("Highlight parameter name in shader")]
        public string highlightProperty = "_Highlight";

        [Header("Collider")]
        public Collider targetCollider;

        private XRRayInteractor[] rays;
        private Renderer[] renderers;
        private MaterialPropertyBlock mpb;
        private bool isHighlighted;

        void Awake()
        {
            if (targetCollider == null)
                targetCollider = GetComponentInChildren<Collider>();

            renderers = GetComponentsInChildren<Renderer>();
            mpb = new MaterialPropertyBlock();

            rays = FindObjectsByType<XRRayInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }

        void Update()
        {
            if (!enableHighlight || targetCollider == null)
                return;

            bool hit = false;

            foreach (var ray in rays)
            {
                if (ray == null || !ray.enabled) continue;

                if (ray.TryGetCurrent3DRaycastHit(out RaycastHit hitInfo))
                {
                    if (hitInfo.collider == targetCollider ||
                        hitInfo.collider.transform.IsChildOf(transform))
                    {
                        hit = true;
                        break;
                    }
                }
            }

            if (hit != isHighlighted)
            {
                isHighlighted = hit;
                ApplyHighlight(isHighlighted);
            }
        }

        void ApplyHighlight(bool on)
        {
            foreach (var r in renderers)
            {
                if (r == null) continue;

                r.GetPropertyBlock(mpb);
                mpb.SetFloat(highlightProperty, on ? 1f : 0f);
                r.SetPropertyBlock(mpb);
            }
        }

        void OnDisable()
        {
            ApplyHighlight(false);
        }
    }
}
