using UnityEditor;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Space]
    [Header("Clock Parameters")]
    [Space]
    [Range(1f, 10f)]
    public float radius = 5f;
    public bool smooth = false;

    private void OnDrawGizmos()
    {
        Vector3 clockPos = transform.position;
        Vector3 clockUp = transform.up;
        Vector3 clockNormal = -transform.forward; // left hand rule for rotation

        // Get current time
        System.DateTime currentTime = System.DateTime.Now;
        float currentHour = currentTime.Hour;
        float currentMin = currentTime.Minute;
        float currentSec = currentTime.Second;
        float currentMS = currentTime.Millisecond;

        // from time to angles
        // Seconds
        float currentSecAngle = currentSec * 360 / 60;
        if (smooth)
            currentSecAngle += currentMS * 6 / 1000;
        // Minutes
        float currentMinAngle = currentMin * 360 / 60;
        currentMinAngle += currentSec * 6 / 60;
        // Hour
        float currentHourAngle = currentHour * 360 / 12;
        currentHourAngle += currentMin * 30 / 60;

        // Drawings

        // Clock Shape
        Handles.color = Color.white;
        Handles.DrawWireDisc(clockPos, clockNormal, radius);

        // Clock hands
        Vector3 hourHandDir = Quaternion.AngleAxis(currentHourAngle, clockNormal) * clockUp;
        Vector3 minHandDir = Quaternion.AngleAxis(currentMinAngle, clockNormal) * clockUp;
        Vector3 secHandDir = Quaternion.AngleAxis(currentSecAngle, clockNormal) * clockUp;
        float hourHandLenght = radius * 5 / 10;
        float minHandLenght = radius * 7 / 10;
        float secHandLenght = radius * 8 / 10;
        Vector3 hourHand = hourHandDir * hourHandLenght;
        Vector3 minHand = minHandDir * minHandLenght;
        Vector3 secHand = secHandDir * secHandLenght;
        Handles.color = Color.white;    // Hours
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 4, clockPos, clockPos + hourHand);
        Handles.color = Color.white;    // Minutes
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 4, clockPos, clockPos + minHand);
        Handles.color = Color.red;      // Seconds
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 2, clockPos, clockPos + secHand);

        // Tickmarks
        float bigTickOffset = radius * 1 / 10;
        float bigTickThickness = 3;
        float smallTickOffset = radius * 1 /20;
        float smallTickThickness = 2;

        Handles.color = Color.grey;
        for (int i = 0; i < 60; i++)
        {
            Vector3 tickDir = Quaternion.AngleAxis(i*6, clockNormal) * clockUp;
            Vector3 tickStart;
            Vector3 tickEnd;

            if ((i % 5) == 0) // multiple of 5
            {
                tickStart = clockPos + (tickDir * (radius - bigTickOffset));
                tickEnd = clockPos + (tickDir * (radius + bigTickOffset));
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, bigTickThickness, tickStart, tickEnd);
            }
            else
            {
                tickStart = clockPos + (tickDir * (radius - smallTickOffset));
                tickEnd = clockPos + (tickDir * (radius + smallTickOffset));
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, smallTickThickness, tickStart, tickEnd);
            }
        }
    }
}
