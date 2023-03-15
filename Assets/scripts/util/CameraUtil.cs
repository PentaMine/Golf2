using UnityEngine;

public class CameraUtil
{
    // returns true if the mouse is pointing to a collider and set the hitpoint refrence to the position
    public static bool GetCursorInWorldPos(out Vector3 hitPoint, LayerMask layer)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool didRayHit = Physics.Raycast(ray, out RaycastHit hit, 1000f, layer);
        hitPoint = hit.point;
        return didRayHit;
    }
}
