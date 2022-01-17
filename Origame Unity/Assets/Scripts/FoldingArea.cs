using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

[ExecuteAlways]
public class FoldingArea : MonoBehaviour
{
    protected List<FoldPoint> points = new List<FoldPoint>(); //list of points
    public List<FoldPoint> Points //encapsulation for points - public get only
    {
        get { return points; }
    }

    [Header("Folding Area")]
    [SerializeField] protected GameObject pointsParent;

    private int checkIntersectIndex = 0; //for looping through points for checking intersections
    private List<FoldPoint> intersectPoints = new List<FoldPoint>(); //list of intersection points
    
    [SerializeField] private LayerMask foldLineLayer; //line that the fold line is on

    [SerializeField] protected SpriteShapeController unfoldedSprite; //the sprite shape
    [SerializeField] protected SpriteShapeController foldedSprite;

    [SerializeField] private float spritePointsLimit = 0.032f;

    protected List<FoldPoint> unfoldedPoints = new List<FoldPoint>();
    protected List<FoldPoint> foldedPoints = new List<FoldPoint>();

    [SerializeField] private LineRenderer unfoldedOutline;
    [SerializeField] private LineRenderer foldedOutline;
    [SerializeField] private bool renderOutline;

    private void Start()
    {
        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        UpdateObject();
    }

    private void Update()
    {
        if (Application.isPlaying) { return; }
        
        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        UpdateObject();
    }

    public void UpdateObject()
    {
        foldedPoints.Clear();
        unfoldedPoints.Clear();

        foreach (FoldPoint point in points)
        {
            if (point.isIntersection)
            {
                foldedPoints.Add(point);
                unfoldedPoints.Add(point);
            }
            else if (point.isReflected)
            {
                foldedPoints.Add(point);
            }
            else
            {
                unfoldedPoints.Add(point);
            }
        }

        if (CheckSpriteCloseness())
        {
            SetShape(unfoldedSprite, unfoldedPoints);

            if (Application.isPlaying)
            {
                SetShape(foldedSprite, foldedPoints);
            }
            else
            {
                SetShape(foldedSprite, unfoldedPoints);
            }
        }

        if (renderOutline)
        {
            SetOutline();
        }
    }

    public void ResetPoints()
    {
        for (int pointIndex = 0; pointIndex < points.Count; pointIndex++) //loop through points
        {
            if (points[pointIndex].isIntersection) //if is an intersection with foldLine
            {
                Debug.Log("Destroy " + points[pointIndex].name);

                Destroy(points[pointIndex].gameObject); //delete

                points.RemoveAt(pointIndex); //remove in points list
                pointIndex--;
            }
            else if (points[pointIndex].isReflected) //if is a reflected vertex
            {
                points[pointIndex].ResetFoldPoint(); //unfold it
            }
        }

        //reset lists, etc.
        checkIntersectIndex = 0;
        intersectPoints.Clear();
    }

    public void AddIntersections() 
    {
        while (checkIntersectIndex < points.Count - 1) //loop through points
        {
            CheckAndAdd(checkIntersectIndex, checkIntersectIndex + 1); //call CheckAndAdd() to find an intersection of two vertices
        }

        CheckAndAdd(points.Count - 1, 0);
    }

    private void CheckAndAdd(int point1Index, int point2Index)
    {
        //linecast from one vertex to the next to find any intersection with fold line
        RaycastHit2D hit = Physics2D.Linecast(points[point1Index].transform.position, points[point2Index].transform.position, foldLineLayer);

        if (hit) //if linecast hit
        {
            //instantiate a new FoldPoint by copying the point and placing it at the hit.point with the parent of this transform
            FoldPoint newPoint = Instantiate(points[point1Index], hit.point, Quaternion.identity, pointsParent.transform);
            newPoint.isIntersection = true; //is an intersection is true
            
            Debug.Log("Add " + newPoint.name);
            points.Insert(point1Index + 1, newPoint); //insert this new point into the points list
            intersectPoints.Add(newPoint); //add point to intersectPoints list
            
            checkIntersectIndex++; //increment pointer to account for inserting a new point
        }

        checkIntersectIndex++; //increment pointer as normal
    }

    protected void SetShape(SpriteShapeController sprite, List<FoldPoint> _points)
    {
        Debug.Log("Set " + sprite.name);

        sprite.spline.Clear();

        if (_points.Count > 0)
        {
            sprite.gameObject.SetActive(true);

            for (int pointIndex = 0; pointIndex < _points.Count; pointIndex++)
            {
                try
                {
                    sprite.spline.InsertPointAt(pointIndex, _points[pointIndex].transform.position);
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException || ex is IndexOutOfRangeException)
                    {
                        Debug.Log("set shape exception");
                        continue;
                    }

                    throw;
                }
            }
        }
        else if (Application.isPlaying)
        {
            sprite.gameObject.SetActive(false);
        }

        sprite.RefreshSpriteShape();
    }

    private bool CheckSpriteCloseness()
    {
        for (int currentPointIndex = 0; currentPointIndex < points.Count - 1; currentPointIndex++)
        {
            if ((points[currentPointIndex].transform.position - points[currentPointIndex + 1].transform.position).sqrMagnitude 
                < (spritePointsLimit * spritePointsLimit))
            {
                return false;
            }
        }

        if ((points[points.Count - 1].transform.position - points[0].transform.position).sqrMagnitude
                < (spritePointsLimit * spritePointsLimit))
        {
            return false;
        }

        return true;
    }

    private void SetOutline()
    {
        unfoldedOutline.positionCount = 0;
        foldedOutline.positionCount = 0;

        foreach (FoldPoint unfoldedPoint in unfoldedPoints)
        {
            unfoldedOutline.positionCount++;
            unfoldedOutline.SetPosition(unfoldedOutline.positionCount - 1, unfoldedPoint.transform.position);
        }

        foreach (FoldPoint foldedPoint in foldedPoints)
        {
            foldedOutline.positionCount++;
            foldedOutline.SetPosition(foldedOutline.positionCount - 1, foldedPoint.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        foreach (FoldPoint point in points)
        {
            Gizmos.DrawSphere(point.transform.position, 0.1f);
        }
    }
}