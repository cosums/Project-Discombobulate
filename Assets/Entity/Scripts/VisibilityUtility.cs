using UnityEngine;

public static class VisibilityUtility
{
    public static bool FOVCheck(Transform nodeTransform, Camera playerCamera)
    {
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(nodeTransform.position);
        if (viewportPoint.z <= 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            return false;
        }

        return true;
    }

    public static bool FOVCheck(Vector3 position, Camera playerCamera)
    {
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(position);
        if (viewportPoint.z <= 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            return false;
        }

        return true;
    }
}