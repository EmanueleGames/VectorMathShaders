using UnityEngine;

public class LookAtTrigger : MonoBehaviour
{
    public Transform player;
    public Transform enemy;
    [Range(0f,1f)]
    public float threshold = 0.8f; // sets the FoV width

    // just for visual output
    public float dotProduct;
    public float thresholdAngleRad;
    public float thresholdAngleDeg;

    private void OnDrawGizmos()
    {
        Vector2 playerPos = player.position;
        Vector2 enemyPos  = enemy.position;

        // Getting directions
        // where is the enemy looking
        Vector2 enemyLookDir = enemy.right;
        // from enemy to player
        Vector2 enemyToPlayerDir = (playerPos - enemyPos).normalized;

        // Dot Product
        dotProduct = enemyLookDir.x * enemyToPlayerDir.x +
                     enemyLookDir.y * enemyToPlayerDir.y;

        // Player in Enemy's FoV condition check
        bool isLooking = dotProduct >= threshold;

        // Draws a symbolic vector to represent how the Dot Product
        // changes value accordingly to the positions of enemy and player
        //Gizmos.DrawLine(enemyPos, enemyPos + (enemyLookDir * dotProduct));

        // Here I'm just using angles to draw the boundaries for the FoV of the enemy
        // This code is not used to check any condition, just to give a better image of the threshold
        thresholdAngleRad = Mathf.Acos(threshold);
        thresholdAngleDeg = thresholdAngleRad * 180 / Mathf.PI;
        Vector2 thresholdLimitA = Quaternion.AngleAxis(thresholdAngleDeg, transform.forward) * (enemyLookDir * 4);
        Vector2 thresholdLimitB = Quaternion.AngleAxis(-thresholdAngleDeg, transform.forward) * (enemyLookDir * 4);
        Gizmos.color = isLooking ? Color.green : Color.red; // green if enemy can see the player, red otherwise
        Gizmos.DrawLine(enemyPos, enemyPos + (thresholdLimitA));
        Gizmos.DrawLine(enemyPos, enemyPos + (thresholdLimitB));
    }
}
