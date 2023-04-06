using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RadialTrigger : MonoBehaviour
{
    public Transform triggerCenter;
    public Transform player;
    public float radius;
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Vector2 T = triggerCenter.position;  // center of the trigger
        Vector2 P = player.position;  // player position

        // Draw the trigger circle
        Handles.DrawWireDisc(T, Vector3.forward, radius);

        // Inside/Outside condition using Unity function
        //float distCP = (P - C).magnitude;
        //bool isOuside = distCP > radius;

        // Inside/Outside condition 
        // Optimized way to check the distance, without the square root
        bool isOuside = ((P.x - T.x)*(P.x - T.x) + (P.y - T.y)*(P.y - T.y)) > (radius * radius);

        // The trigger condition changes the color of the circle
        Handles.color = isOuside ? Color.red : Color.green;
        Handles.DrawWireDisc(T, new Vector3(0, 0, 1), radius);
    }
    #endif
}
