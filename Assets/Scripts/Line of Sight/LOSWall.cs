using UnityEngine;
using System.Collections;

public class LOSWall : LOSEntity
{
    /// <summary>
    /// Base width.
    /// </summary>
    public float BaseWidth = 0;

    /// <summary>
    /// Base height.
    /// </summary>
    public float BaseHeight = 0;

    /// <summary>
    /// Wall bounds.
    /// </summary>
    public override Rect Bounds
    {
        get
        {

            return new Rect(
                transform.position.x - BaseWidth / 2,
                transform.position.z - BaseHeight / 2,
                BaseWidth, BaseHeight);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0, 0, 0.5F);
        Gizmos.DrawCube(transform.position, new Vector3(BaseWidth, Height, BaseHeight));
    }
}
