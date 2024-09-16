using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Paper : FoldingArea
{
    [Header("Paper")]

    [SerializeField] private Vector2 size;
    public Vector2 Size
    { 
        get { return size; }
    }

    private FoldingArea[] areas;

    [SerializeField] private LayerMask foldPointLayer;

    [SerializeField] private EdgeCollider2D selectEdge;
    public EdgeCollider2D SelectEdge 
    {
        get { return selectEdge; }
    }

    public bool isFolded;

    private void Start()
    {
        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        areas = transform.Find("Objects").GetComponentsInChildren<FoldingArea>();
        
        UpdateSelectEdge();
        UpdateObject();
    }

    private void Update()
    {
        if (Application.isPlaying) { return; }

        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        areas = transform.Find("Objects").GetComponentsInChildren<FoldingArea>();

        UpdateSelectEdge();
        UpdateObject();
    }

    public void Fold(Vector2 midpoint, Vector2 foldDir, Vector2 dragDir)
    {
        Debug.Log("------Fold------");
        AddAllIntersections();

        float checkAngle = Vector2.SignedAngle(transform.right, foldDir); //find angle between fold direction and transform right direction
        float magnitude = size.magnitude; //find magnitude of size

        //find position of check box by doing the midpoint minus direction of the drag * magnitude of the size divided by 2
        Vector2 pos = midpoint - (2 * magnitude * dragDir.normalized);
        
        //do a check with overlap square with sides of size.magnitude and position determined above, with angle of check angle to the horizontal
        Collider2D[] pointsToReflect = Physics2D.OverlapBoxAll(pos, 4 * magnitude * Vector2.one, checkAngle, foldPointLayer); //use a layermask to only get vertices
        Debug.Log("------Reflect------");

        foreach (Collider2D pointCol in pointsToReflect) //loop through vertices found with OverlapBoxAll
        {
            if (pointCol.transform.parent.parent == transform || pointCol.transform.parent.parent.parent.parent == transform) //if point belongs to this paper
            {
                FoldPoint foldPoint = pointCol.GetComponent<FoldPoint>(); //get FoldPoint from the vertex

                if (foldPoint != null && !foldPoint.isIntersection) //if foldPoint is not null and is not an intersection
                {
                    foldPoint.Reflect(); //reflect that point
                }
            }
        }

        UpdateAllObjects();
    }

    private void AddAllIntersections()
    {
        foreach (FoldingArea area in areas) //loop through areas
        {
            area.AddIntersections(); //add intersections on area
        }

        AddIntersections();
    }

    public void UpdateAllObjects()
    {
        Debug.Log("------Update all------");

        foreach (FoldingArea area in areas) //loop through areas
        {
            area.UpdateObject(); //update it
        }
        
        UpdateObject();
    }

    private void UpdateSelectEdge()
    {
        Vector2[] edgePoints = new Vector2[points.Count + 1];

        for (int i = 0; i < points.Count; i++)
        {
            edgePoints[i] = points[i].transform.localPosition;
        }

        edgePoints[edgePoints.Length - 1] = points[0].transform.localPosition;

        selectEdge.points = edgePoints;
    }

    public void ResetPaper()
    {
        Debug.Log("------Reset all----------");

        foreach (FoldingArea area in areas)
        {
            area.ResetPoints();
        }

        ResetPoints();
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 center = Vector2.zero;

        foreach (FoldPoint pointPos in Points)
        {
            center += (Vector2) pointPos.transform.position;
        }

        center /= Points.Count;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
