using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BouncingLaser : MonoBehaviour
{
    public Transform rayOrigin;
    [Range(1, 10)]
    public int NumberOfBounces;

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 laser = rayOrigin.position;       // Ray Origin
        Vector3 laserDirection = rayOrigin.right; // Ray Direction

        // Recursive function for Reflection
        ShootRay(laser, laserDirection, NumberOfBounces);
    }

    void ShootRay(Vector3 origin, Vector3 direction, int iterations)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit))
        {
            iterations--;
            Vector3 hitPosition = hit.point;
            Vector3 surfaceNormal = hit.normal;

            // Once we know the collision point on the surface, 
            // we can calculatethe normal component of the Ray Direction
            float rayNormalComponent = Vector3.Dot(direction, surfaceNormal);

            // Calculate Reflection

            // 1. Using Unity Built-in function
            //Vector3 hitBounceDirection = Vector3.Reflect(direction, surfaceNormal);

            // 2. Manually
            // The direction normal component is the only one affected by the reflection
            // To get the reflected direction we need to reverse only the normal component
            // and to achieve this we subtract the normal component twice
            Vector3 hitBounceDirection = direction - (2 * rayNormalComponent * surfaceNormal);

            // Draw the ray
            Handles.color = Color.green;
            Handles.DrawAAPolyLine(origin, hitPosition);

            if (iterations > 0)
            {
                //recursive call
                ShootRay(hitPosition, hitBounceDirection, iterations);
            }
        }
        else // Ray misses
        {
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(origin, (origin + direction));
        }
    }
    #endif
}

