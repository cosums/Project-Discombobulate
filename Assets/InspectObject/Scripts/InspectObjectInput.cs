using UnityEngine;
using UnityEngine.EventSystems;

public class InspectObjectInput : MonoBehaviour, IDragHandler
{
    [SerializeField] private Camera previewCamera;
    [SerializeField] private float rotateSpeed = 0.3f;

    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target == null || eventData.button != PointerEventData.InputButton.Left) return;

        Vector2 delta = eventData.delta * rotateSpeed;
        target.Rotate(previewCamera.transform.up, -delta.x, Space.World);
        target.Rotate(previewCamera.transform.right, delta.y, Space.World);
    }
}