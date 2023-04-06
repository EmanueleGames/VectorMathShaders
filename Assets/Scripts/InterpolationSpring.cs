using UnityEngine;

public class InterpolationSpring : MonoBehaviour
{
    [Space]
    [Header("Spring Parameters")]
    [Space]

    [Range(0, 20)]
    public float height = 5;
    [Range(0, 5)]
    public float coilRadius = 2;
    [Range(0, 10)]
    public int coilNumber = 5;


    private void OnDrawGizmos()
    {
        Vector3 lowerCenter = transform.position;
        Vector3 upperCenter = transform.position + Vector3.up * height;

        Vector3 coilNormal = Vector3.up;
        Vector3 coilStart = Vector3.forward * coilRadius;

        int totalDegree = 360 * coilNumber;

        Vector3 lastPoint = default;
        Vector3 newPoint = default;

        // Draw the spring
        const int DETAIL_SAMPLES = 200;
        for (int i = 0; i < DETAIL_SAMPLES; i++)
        {
            float t = i / (DETAIL_SAMPLES - 1f);

            lastPoint = newPoint;

            // LERP along the height
            Vector3 tHeight = Vector3.Lerp(lowerCenter, upperCenter, t);
            // LERP along the turns
            float tAngle = Mathf.Lerp(0, totalDegree, t);
            Vector3 tCoilDir = Quaternion.Euler(0, tAngle, 0) * coilStart;

            newPoint = tHeight + tCoilDir;

            if (i != 0)
                Gizmos.DrawLine(lastPoint, newPoint);
        }
    }
}