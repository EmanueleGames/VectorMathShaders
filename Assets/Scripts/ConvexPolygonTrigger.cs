using UnityEngine;

public class ConvexPolygonTrigger : MonoBehaviour
{
    public Transform player;
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;
    public Transform pointE;

    private void OnDrawGizmos()
    {
        Vector2 pointAPos = pointA.position;
        Vector2 pointBPos = pointB.position;
        Vector2 pointCPos = pointC.position;
        Vector2 pointDPos = pointD.position;
        Vector2 pointEPos = pointE.position;
        Vector2 playerPos = player.position;

        bool isInside = polygonContain(pointAPos, pointBPos, pointCPos, pointDPos, pointEPos, playerPos);

        // Draw the Poligon
        Gizmos.color = isInside ? Color.green : Color.red;
        Gizmos.DrawLine(pointAPos, pointBPos);
        Gizmos.DrawLine(pointBPos, pointCPos);
        Gizmos.DrawLine(pointCPos, pointDPos);
        Gizmos.DrawLine(pointDPos, pointEPos);
        Gizmos.DrawLine(pointEPos, pointAPos);
    }

    bool polygonContain(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Vector2 e, Vector2 player)
    {
        float WedgeProduct(Vector2 v1, Vector2 v2) => v1.x * v2.y - v1.y * v2.x;

        float wedgeAB = WedgeProduct(b - a, player - a);
        float wedgeBC = WedgeProduct(c - b, player - b);
        float wedgeCD = WedgeProduct(d - c, player - c);
        float wedgeDE = WedgeProduct(e - d, player - d);
        float wedgeEA = WedgeProduct(a - e, player - e);

        if( ( wedgeAB > 0 && wedgeBC > 0 && wedgeCD > 0 && wedgeDE > 0 && wedgeEA > 0 ) ||
            ( wedgeAB < 0 && wedgeBC < 0 && wedgeCD < 0 && wedgeDE < 0 && wedgeEA < 0 ) )
            return true;

        return false;
    }
}