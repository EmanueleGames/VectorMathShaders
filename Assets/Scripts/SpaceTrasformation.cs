using UnityEngine;

public class SpaceTrasformation : MonoBehaviour
{
    public Transform globalSpacePoint; // point defined with global coordinates
    public Transform localSpaceOrigin; // origin of the local coordinate system
    public Transform localSpacePoint;  // point defined with local coordinates

    [Space]
    [Header("Local point in Global Coordinates")]
    [Space]
    // Local Point coordinates relative to the global space
    public float LocalPointXGlobal;
    public float LocalPointYGlobal;
    [Space]
    [Header("Global point in Local Coordinates")]
    [Space]
    // Global Point coordinates relative to the local space
    public float GlobalPointXLocal;
    public float GlobalPointYLocal;


    private void OnDrawGizmos()
    {
        // Getting GameObjects infos
        Vector2 globalPoint = globalSpacePoint.position;            // Global Point (global coordinates)
        Vector2 localSpace = localSpaceOrigin.position;             // Local Space - Origin
        Vector2 localSpaceAxisX = localSpaceOrigin.transform.right; // Local Space - X axis direction
        Vector2 localSpaceAxisY = localSpaceOrigin.transform.up;    // Local Space - Y axis direction
        Vector2 localPoint = localSpacePoint.localPosition;         // Local Point (local coordinates)

        // 1) CONVERSION: Local Point -> Global Coordinates
        Vector2 LocalPointInGlobalCoord = localSpace + Vector2.Dot(localPoint, Vector2.right) * localSpaceAxisX
                                                     + Vector2.Dot(localPoint, Vector2.up) * localSpaceAxisY;
        LocalPointXGlobal = LocalPointInGlobalCoord.x;
        LocalPointYGlobal = LocalPointInGlobalCoord.y;

        // 2) CONVERSION: Global Point -> Local Coordinates
        GlobalPointXLocal = Vector2.Dot((globalPoint - localSpace), localSpaceAxisX);
        GlobalPointYLocal = Vector2.Dot((globalPoint - localSpace), localSpaceAxisY);


        // DRAWINGS

        // Draw axis and intervals (0.5) to keep track position and rotation of the coordinate systems
        Gizmos.color = Color.red;
        // Global - X axis
        Gizmos.DrawLine(-transform.right * 5, transform.right * 5);
        for (int i = -10; i < 11; i++)
            Gizmos.DrawSphere((transform.right * i / 2), 0.05f);
        // Local - X axis
        Gizmos.DrawLine(localSpace - localSpaceAxisX * 5, localSpace + localSpaceAxisX * 5);
        for (int i = -10; i < 11; i++)
            Gizmos.DrawSphere(localSpace + (localSpaceAxisX * i / 2), 0.05f);

        Gizmos.color = Color.green;
        // Global - Y axis
        Gizmos.DrawLine(-transform.up * 5, transform.up * 5);
        for (int i = -10; i < 11; i++)
            Gizmos.DrawSphere((transform.up * i / 2), 0.05f);
        // Local - Y axis
        Gizmos.DrawLine(localSpace - localSpaceAxisY * 5, localSpace + localSpaceAxisY * 5);
        for (int i = -10; i < 11; i++)
            Gizmos.DrawSphere(localSpace + (localSpaceAxisY * i / 2), 0.05f);

        // Draw a line from Global Space origin to Global Point
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(Vector2.zero, globalPoint);
        // Draw a line from Local Space origin to Local Point
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(localSpace, LocalPointInGlobalCoord);
    }
}