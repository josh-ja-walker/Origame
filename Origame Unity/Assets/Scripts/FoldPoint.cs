using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldPoint : MonoBehaviour
{
    private  Vector2 startPos;
    public Vector2 StartPos 
    { 
        get { return startPos; } 
    }

    public bool isIntersection;
    public bool isReflected;

    private void Start() {
        startPos = transform.position;
    }

    public void Reflect() {
        // Debug.Log("Reflect " + name);
        
        Vector2 closestPoint = PlayerFold.instance.FoldLine.ClosestPoint(transform.position);
        Vector2 toFold = closestPoint - (Vector2) transform.position;

        transform.position += (Vector3) toFold * 2f;

        isReflected = true;
    }

    public void ResetFoldPoint() {
        // Debug.Log("Unfold " + name);

        transform.position = startPos;
        isReflected = false;
    }
}
