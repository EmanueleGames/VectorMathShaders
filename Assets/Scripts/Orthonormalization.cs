using UnityEditor;
using UnityEngine;

public class Orthonormalization : MonoBehaviour
{
    public Transform raycastOrigin;

    private void OnDrawGizmos()
    {
        Vector3 raycastPos = raycastOrigin.position; // Raycast Origin
        Vector3 raycastDir = raycastOrigin.forward;  // Raycast Direction

        // ray hits
        if (Physics.Raycast(raycastPos, raycastDir, out RaycastHit hit))
        {
            // Hit position and surface normal
            Vector3 hitPosition = hit.point;
            Vector3 hitUp = hit.normal;

            Vector3 hitRight = Vector3.Cross(hitUp, raycastDir).normalized;
            Vector3 hitForward = Vector3.Cross(hitRight, hitUp).normalized;

            // Draw the ray
            Handles.color = Color.white;
            Handles.DrawAAPolyLine(raycastPos, hitPosition);
            // Draw the new reference system
            Handles.color = Color.red;      // X Axis
            Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 2, hitPosition, hitPosition + hitRight);
            Handles.color = Color.green;    // Y Axis
            Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 2, hitPosition, hitPosition + hitUp);
            Handles.color = Color.cyan;     // Z Axis
            Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 2, hitPosition, hitPosition + hitForward);

            // Setting Local -> Global matrix
            Quaternion localSpaceRotation = Quaternion.LookRotation(hitForward, hitUp);
            Matrix4x4 localToGlobal = Matrix4x4.TRS(hitPosition, localSpaceRotation, Vector3.one);

            // Draw Bounding Box
            DrawBox(localToGlobal);
        }
        // ray misses
        else
        {
            Handles.color = Color.magenta;
            Handles.DrawAAPolyLine(raycastPos, (raycastPos + raycastDir * 2));
        }


        void DrawBox(Matrix4x4 localToGlobal) {

            // Coordinates
            Vector3[] corners = new Vector3[] {
 	        // Bottom 4 positions:
	        new Vector3( 1, 0, 1 ),
            new Vector3( -1, 0, 1 ),
            new Vector3( -1, 0, -1 ),
            new Vector3( 1, 0, -1 ),
	        // Top 4 positions:
	        new Vector3( 1, 2, 1 ),
            new Vector3( -1, 2, 1 ),
            new Vector3( -1, 2, -1 ),
            new Vector3( 1, 2, -1 )
        };

            // Set Gizmo and Handles matrix
            Gizmos.matrix = localToGlobal;
            Handles.matrix = localToGlobal;

            // Draw the box
            Gizmos.color = Color.blue;
            Handles.color = Color.blue;
            // Vertices
            for (int i = 0; i < corners.Length; i++)
                Gizmos.DrawSphere(corners[i], 0.05f);
            // Edges
            for (int i = 0; i < (corners.Length / 2); i++)
            {
                if (i != 3)
                {
                    Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 1, corners[i], corners[i + 1]);
                    Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 1, corners[i + 4], corners[i + 5]);
                }
                else
                {
                    Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 1, corners[i], corners[0]);
                    Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 1, corners[i + 4], corners[4]);
                }
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 1, corners[i], corners[i + 4]);
            }
            // Reset Gizmo and Handles matrix
            Gizmos.matrix = Matrix4x4.identity;
            Handles.matrix = Matrix4x4.identity;
        }
    }
}
