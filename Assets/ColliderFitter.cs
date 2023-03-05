using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFitter : MonoBehaviour
{

    PolygonCollider2D polygonCollider2D;
    Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {
        polygonCollider2D = this.gameObject.GetComponent<PolygonCollider2D>();
        sprite = this.gameObject.GetComponent<Sprite>();

        UpdatePolygonCollider2D(0.001f);
    }

    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> simplifiedPoints = new List<Vector2>();
    public void UpdatePolygonCollider2D(float tolerance = 0.05f)
    {
        polygonCollider2D.pathCount = sprite.GetPhysicsShapeCount();
        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            sprite.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, tolerance, simplifiedPoints);
            polygonCollider2D.SetPath(i, simplifiedPoints);
        }
    }
}
