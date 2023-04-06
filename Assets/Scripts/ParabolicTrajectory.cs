using UnityEngine;

public class ParabolicTrajectory : MonoBehaviour
{
    [Space]
    [Header("Settings")]
    [Space]
    public Transform OriginTf;
    public Transform TerrainTf;

    [Range(0f, 50f)]
    public float startingSpeed = 25f;

    private void OnDrawGizmos()
    {
        Vector3 initialPos = OriginTf.position;
        Vector3 startingDir = OriginTf.forward;
        Vector3 startingVel = startingDir * startingSpeed;
        Vector3 acceleration = Physics.gravity;

        Vector3 terrainPos = TerrainTf.position;

        // Draw the trajectory
        const int SAMPLES = 100;
        Vector3 newPoint = Vector3.zero;
        Vector3 lastPoint = Vector3.zero;

        Vector3 GetPoint(float t) => initialPos + startingVel * t + .5f * acceleration * t * t;

        for (int i = 0; i < SAMPLES; i++)
        {
            float t = i / (SAMPLES - 1f);
            float sampleTime = t * 5f; //draw duration
            lastPoint = newPoint;
            newPoint = GetPoint(sampleTime);

            if (i != 0)
                Gizmos.DrawLine(lastPoint, newPoint);
        }

        // If we know there is a terrain with a specific height
        // then we can determine the hit point of the trajectory

        // To get the instants when the trajectory intersect our plane, we only need the gravity motion
        Vector3 startingVel_Y = Vector3.up * (Vector3.Dot(startingVel, Vector3.up));

        // Then we solve the second degree equation to get the time of intersection t1 and t2
        double A = -0.5 * acceleration.magnitude;
        double B = startingVel_Y.magnitude;
        double C = initialPos.y - terrainPos.y;
        int hitPoints = SolveQuadratic(A, B, C, out double t1, out double t2);

        //Draw the hit points
        if (hitPoints >= 1)
        {
            Vector3 hitPos;
            if (t1 > 0) // draw the first hit
            {
                hitPos = GetPoint((float)t1);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(hitPos - Vector3.right, hitPos + Vector3.right);
                Gizmos.DrawLine(hitPos - Vector3.forward, hitPos + Vector3.forward);
            }
            if (t2 > 0) // draw the second hit
            {
                hitPos = GetPoint((float)t2);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(hitPos - Vector3.right, hitPos + Vector3.right);
                Gizmos.DrawLine(hitPos - Vector3.forward, hitPos + Vector3.forward);
            }
        }
    }
    public static int SolveQuadratic(double a, double b, double c, out double r0, out double r1)
    {
        r0 = double.NaN;
        r1 = double.NaN;

        // Classic form: Ax^2 + Bx + C = 0

        double delta = (b * b) - (4 * a * c);

        if (delta == 0)         // 1 root
        {
            r0 = (-b)/(2*a);
            return 1;
        }
        else if (delta < 0)     // 0 real root
        {
            return 0;
        }
        else                    // 2 real roots
        {
            double sqrt_delta = System.Math.Sqrt(delta);
            r0 = (-b - sqrt_delta) / (2 * a);
            r1 = (-b + sqrt_delta) / (2 * a);
            return 2;
        }
    }
}
