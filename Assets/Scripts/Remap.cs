using UnityEditor;
using UnityEngine;

public class Remap : MonoBehaviour
{
    public Transform playerTransf;
    public Transform explosionTransf;

    [Space]
    [Header("Explosion Settings")]
    [Space]
    [Range(0f, 20f)]
    public float explosionMaxRange;
    public float minDamage;
    public float maxDamage;

    [Space]
    [Header("Remapping Function Output")]
    [Space]
    public float damageReceived;

    private void OnDrawGizmos()
    {
        Vector2 playerPos = playerTransf.position;
        Vector2 explosionPos = explosionTransf.position;

        // Distance
        float distance = (playerPos - explosionPos).magnitude;

        // Check player in range
        if (distance <= explosionMaxRange)
        {
            // Remap the damage received based on:
            // 1) distance from the center of the explosion
            // 2) min and max damage chosed by design

            float t = Mathf.InverseLerp(0, explosionMaxRange, distance);
            damageReceived = Mathf.Lerp(minDamage, maxDamage, 1-t);
        }
        else
            damageReceived = 0;


        // Draw Explosion Range
        Color hpCloseColor = Color.red;
        Color hpFarColor = Color.yellow;
        const int CIRCLES_NUM = 10;
        for (int i = 0; i < CIRCLES_NUM; i++)
        {
            float step = (float)(i+1) / (CIRCLES_NUM);
            float stepRadius = Mathf.Lerp(0, explosionMaxRange, step);
            Color stepColor;
            stepColor = Color.Lerp(hpCloseColor, hpFarColor, step);
            Handles.color = stepColor;
            Handles.DrawWireDisc(explosionPos, Vector3.forward, stepRadius);
        }
    }
}