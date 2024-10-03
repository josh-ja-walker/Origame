using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Offset
{
    public static Vector2 Apply(Vector2 offset, Transform transform) {
        // return (Vector2) transform.position + new Vector2((transform.eulerAngles.y == 0 ? 1 : -1) * offset.x, offset.y);
        return transform.position + (transform.right * offset.x) + (transform.up * offset.y);
    }

}
