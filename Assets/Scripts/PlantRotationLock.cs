using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XAxisRotationOnly : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Vector3 lockedPosition;
    private float currentXRotation;
    
    public bool onlyWhenMature = true;
    public PlantGrowth growth;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        lockedPosition = transform.position;
        currentXRotation = transform.localEulerAngles.x;
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Check maturity
        if (onlyWhenMature && growth != null && !growth.IsMature())
        {
            // Force release if not mature
            grabInteractable.interactionManager.SelectExit(args.interactorObject, grabInteractable);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Optional: do nothing, or lock position again
    }

    void LateUpdate()
    {
        // Always lock position
        transform.position = lockedPosition;

        // Lock Y and Z rotation, only allow X (tilt forward/backward)
        Vector3 currentRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f);
    }
}
