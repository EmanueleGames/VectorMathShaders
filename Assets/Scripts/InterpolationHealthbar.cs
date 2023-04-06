using UnityEditor;
using UnityEngine;

public class InterpolationHealthbar : MonoBehaviour
{
    [Space]
    [Header("Helthbar")]
    [Space]
    public Transform hpLowPos;
    public Transform hpHighPos;
    public Color hpLowColor;
    public Color hpMidColor;
    public Color hpHighColor;
    [Range(0f, 100f)]
    public float hpPercentage = 100f;

    private void OnDrawGizmos()
    {
        Vector2 hpStart = hpLowPos.position;
        Vector2 hpEnd = hpHighPos.position;
        float normalizedValue = hpPercentage / 100;

        // Value interpolation
        Vector2 hpActualPos = Vector2.Lerp(hpStart, hpEnd, normalizedValue);

        // Color interpolation
        Color hpColor;
        if (hpPercentage >= 50)
            hpColor = Color.Lerp(hpMidColor, hpHighColor, (normalizedValue * 2) - 1);
        else
            hpColor = Color.Lerp(hpLowColor, hpMidColor, normalizedValue * 2);

        Handles.color = Color.grey;
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 25, hpStart, hpEnd);
        Handles.color = hpColor;
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 25, hpStart, hpActualPos);

    }
}