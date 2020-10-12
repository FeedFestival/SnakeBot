using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public Vector2Int Pos;
    public void PlaceApple(Vector2Int pos)
    {
        Pos = pos;
        transform.localPosition = new Vector3(Pos.x, 0, Pos.y);
    }

    public void Consume()
    {
        // 1. Add energy to snake
        Destroy(this.gameObject);
    }
}
