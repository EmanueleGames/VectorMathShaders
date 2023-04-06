using UnityEditor;
using UnityEngine;

public class SphericalSectorTrigger : MonoBehaviour
{

    public Transform objectToDetect;

    [Space]
    [Header("Trigger Parameters")]
    [Space]
    [Range(0f, 20f)]
    public float radius = 0.8f;
    public bool showSphere = false;
    [Range(0f, 180f)]
    public float thresholdAngleDeg = 60;

    [Space]
    public bool insideFOV;
    public bool insideRadius;

    [Space]
    public bool isInside;

    private void OnDrawGizmos()
    {
        Vector3 objectPos = objectToDetect.position;
        Vector3 triggerPos = Vector3.zero;
        Vector3 triggerDir = Vector3.forward;

        float thresholdAngleRad = Mathf.Deg2Rad * thresholdAngleDeg; // FoV in Radians

        // Global to Local
        Vector3 triggerToObjectWorld = (objectPos - triggerPos);
        Vector3 triggerToObjectLocal = transform.InverseTransformVector(triggerToObjectWorld);
        Vector3 triggerToObjectDirLocal = triggerToObjectLocal.normalized;

        // Condition 1 - FoV check
        float dotProduct = Vector3.Dot(triggerDir, triggerToObjectDirLocal);
        float triggerToObjectAngleRad = Mathf.Acos(dotProduct);
        float triggerToObjectAngleDeg = Mathf.Rad2Deg * triggerToObjectAngleRad;
        insideFOV = triggerToObjectAngleDeg <= (thresholdAngleDeg / 2);

        // Condition 2 - Distance check (squared)
        float triggerToObjectDistSq = ((triggerToObjectLocal.x) * (triggerToObjectLocal.x) +
                                       (triggerToObjectLocal.y) * (triggerToObjectLocal.y) +
                                       (triggerToObjectLocal.z) * (triggerToObjectLocal.z));
        insideRadius = triggerToObjectDistSq <= radius*radius;

        // Final Condition
        isInside = insideFOV && insideRadius;


        // We're gonna draw in local space
        Gizmos.matrix = transform.localToWorldMatrix;
        Handles.matrix = transform.localToWorldMatrix;

        // I'm drawing the sphere using many WireDiscs
        if (showSphere)
        {
            Handles.color = new Color(1, 1, 1, .2f);
            for (int i = 0; i < 18; i++)
            {
                Vector3 normal_1 = Quaternion.AngleAxis(10 * i, Vector3.right) * Vector3.up;
                Vector3 normal_2 = Quaternion.AngleAxis(10 * i, Vector3.forward) * Vector3.up;
                Vector3 normal_3 = Quaternion.AngleAxis(10 * i, Vector3.up) * Vector3.right;
                Handles.DrawWireDisc(Vector3.zero, normal_1, radius);
                Handles.DrawWireDisc(Vector3.zero, normal_2, radius);
                Handles.DrawWireDisc(Vector3.zero, normal_3, radius);
            }

        }

        // Resolving the triangle to get FoV sector points
        Vector3 sectorCenter = triggerDir * (radius * Mathf.Cos(thresholdAngleRad / 2));
        float centerToSphereDist = (radius * Mathf.Sin(thresholdAngleRad / 2));
        // Draw the Cone of View
        Handles.color = new Color(1, .92f, .016f, .5f);
        Handles.DrawWireDisc(sectorCenter, triggerDir, centerToSphereDist);
        for (int i = 0; i < 18; i++)
        {
            Vector3 rotatingVectorDir = Quaternion.AngleAxis(i * 20, Vector3.forward) * Vector3.up;
            Vector3 rotatingVector = rotatingVectorDir * centerToSphereDist;
            Handles.DrawLine(Vector3.zero, sectorCenter + rotatingVector);
            Vector3 rotatingNormal = Quaternion.AngleAxis((i * 20) - 90, Vector3.forward) * Vector3.up;
            Handles.DrawWireArc(Vector3.zero, rotatingNormal, sectorCenter + rotatingVector, thresholdAngleDeg, radius);
        }

        if (isInside)
        {
            Handles.color = Color.green;
            Handles.DrawLine(Vector3.zero, triggerToObjectLocal);
        }
    }
}
