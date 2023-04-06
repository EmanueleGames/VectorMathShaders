using UnityEditor;
using UnityEngine;

public class SliceTrigger : MonoBehaviour
{
    public Transform detectorOrigin;
    public Transform objectToDetect;
    public Transform faceToRotate;

    [Space]
    [Header("Detection Range Configuration")]
    [Space]
    [Range(.1f, 1f)]
    public float angleThreshold = 0.8f;
    [Range(.1f, 2f)]
    public float height = 1;
    [Range(.1f, 5f)]
    public float range = 3;

    [Space]
    [Header("Conditions Check")]
    [Space]
    public bool isInsideAngle;
    public bool isInsideHeight;
    public bool isInsideRange;
    [Space]
    public bool isInside;

    private void OnDrawGizmos()
    {
        Vector3 detectorPos = detectorOrigin.position; 
        Vector3 detectorDir = detectorOrigin.forward;
        Vector3 objectPos = objectToDetect.position;

        // Setting Local -> Global matrix
        Quaternion localSpaceRotation = Quaternion.LookRotation(detectorOrigin.forward, detectorOrigin.up);
        Matrix4x4 localToGlobal = Matrix4x4.TRS(detectorPos, localSpaceRotation, Vector3.one);

        // Direction from Detector to Object
        Vector3 detectorToObject    = (objectPos - detectorPos);
        Vector3 detectorToObjectDir = (objectPos - detectorPos).normalized;

        // We need 3 CONDITIONS to check if the object is inside our FoV

        // 1 - The Angle (Dot Product)
        // When checking if the Object is inside the angle, we need to ignore the Detector LOCAL UP-COMPONENT
        // in the dot product evaluation. To accomplish this, we are not gonna use the vector from the
        // Detector to the Object, but its flattened version on the plane identified by the up-component instead.
        Vector3 detectorToObjectDirFlat = Vector3.ProjectOnPlane(detectorToObjectDir, detectorOrigin.up).normalized;
        float dotProduct = Vector3.Dot(detectorDir, detectorToObjectDirFlat); // Dot Product
        isInsideAngle = dotProduct >= angleThreshold;
        // Uncommenting the following lines of code shows in red the vectors used for the dot product
        /*
        Gizmos.color = Color.white;
        Gizmos.DrawLine(detectorPos, detectorPos + detectorToObjectDir);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(detectorPos, detectorPos + detectorDir);
        Gizmos.DrawLine(detectorPos, detectorPos + detectorToObjectDirFlat);
        */

        // 2 - The Height
        // For the Height we just need to compare the distance (along the Detector LOCAL UP-COMPONENT) between
        // the the Object and the plane identified by the local up-component with the height (its half)
        Vector3 detectorToObjectFlat = Vector3.ProjectOnPlane(detectorToObject, detectorOrigin.up);
        isInsideHeight = (detectorToObject - detectorToObjectFlat).magnitude < (height / 2);
        // Uncommenting the following lines of code shows in red the distance considered
        /*
        Gizmos.color = Color.white;
        Gizmos.DrawLine(detectorPos, detectorPos + detectorToObject);
        Gizmos.DrawLine(detectorPos, detectorPos + detectorToObjectFlat);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(detectorPos + detectorToObject, detectorPos + detectorToObjectFlat);
        */

        // 3 - The Range 
        // Here we just need to compare the magnitude of the vector from the Detector to the position
        // of the Object flattened on the plane identified by the Detector LOCAL UP-COMPONENT
        isInsideRange = detectorToObjectFlat.magnitude <= range;
        // Uncommenting the following lines of code shows the magnitude considered
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawLine(detectorPos, detectorPos + detectorToObjectFlat);
        */

        // Putting all three condition together
        isInside = isInsideAngle && isInsideHeight && isInsideRange;
        
        if (isInside)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(detectorPos, objectPos);
            // The face follows the object
            Quaternion lookAtObjectRotation = Quaternion.LookRotation(detectorToObject, detectorOrigin.up);
            faceToRotate.rotation = Quaternion.Slerp(faceToRotate.rotation, lookAtObjectRotation, Time.deltaTime * 5f);

            // Set color for the drawings
            Gizmos.color = Color.green;
            Handles.color = Color.green;
        }
        else // Object not detected
        {
            // Reset face rotation
            faceToRotate.rotation = Quaternion.Slerp(faceToRotate.rotation, detectorOrigin.rotation, Time.deltaTime * 5f);

            // Set color for the drawings
            Gizmos.color = Color.red;
            Handles.color = Color.red;
        }
        

        // Draw Detection Range

        // Here I'm just using angles to draw the boundaries for the FoV of the detector
        // This code is not used to check any condition, just to give a better image of the range
        float thresholdAngleRad = Mathf.Acos(angleThreshold);
        float thresholdAngleDeg = thresholdAngleRad * 180 / Mathf.PI;
        
        // Position and direction needed for the drawing
        Vector3 halfHeightOffset = (Vector3.up * height / 2);
        Vector3 upperStartingPoint = halfHeightOffset;
        Vector3 lowerStartingPoint = - halfHeightOffset;
        Vector3 thresholdLeftDir = Quaternion.AngleAxis(-thresholdAngleDeg, Vector3.up) * Vector3.forward;
        Vector3 thresholdRightDir = Quaternion.AngleAxis(thresholdAngleDeg, Vector3.up) * Vector3.forward;
        
        Gizmos.matrix = localToGlobal;
        Handles.matrix = localToGlobal;
        
        // Radii (radiuses)
        Gizmos.DrawLine(upperStartingPoint, upperStartingPoint + thresholdLeftDir * range);
        Gizmos.DrawLine(lowerStartingPoint, lowerStartingPoint + thresholdLeftDir * range);
        Gizmos.DrawLine(upperStartingPoint, upperStartingPoint + thresholdRightDir * range);
        Gizmos.DrawLine(lowerStartingPoint, lowerStartingPoint + thresholdRightDir * range);
        // Vertical lines
        Gizmos.DrawLine(lowerStartingPoint, upperStartingPoint);
        Gizmos.DrawLine(upperStartingPoint + thresholdLeftDir * range, lowerStartingPoint + thresholdLeftDir * range);
        Gizmos.DrawLine(upperStartingPoint + thresholdRightDir * range, lowerStartingPoint + thresholdRightDir * range);
        // Circumference arcs
        Handles.DrawWireArc(upperStartingPoint, Vector3.up, thresholdLeftDir, thresholdAngleDeg * 2, range);
        Handles.DrawWireArc(lowerStartingPoint, Vector3.up, thresholdLeftDir, thresholdAngleDeg * 2, range);
        
        Gizmos.matrix = Matrix4x4.identity;
        Handles.matrix = Matrix4x4.identity;
    }
}
