using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Paper : FoldingArea
{

    [Header("Paper")]

    [SerializeField] private Color paperColor = new Color32(0xF3, 0xEE, 0xEB, 0xFF); //#F3EEEB
    [SerializeField] protected ColorPreset preset = ColorPreset.Red;

    [SerializeField] private Vector2 size;
    public Vector2 Size { get { return size; } }
    [SerializeField] private float sizeOverflow = 2f;

    private FoldingArea[] areas;

    [SerializeField] private LayerMask foldPointLayer;

    [SerializeField] private EdgeCollider2D selectEdge;
    public EdgeCollider2D SelectEdge { get { return selectEdge; } }

    public bool isFolded;

    private void Start() {
        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        areas = transform.Find("Objects").GetComponentsInChildren<FoldingArea>();
        
        UpdateSelectEdge();
        UpdateObject();
        UpdateColor(preset);
    }

    private void Update() {
        if (Application.isPlaying) { 
            return; 
        } else {
            UpdateColor(preset);
        }

        points = pointsParent.GetComponentsInChildren<FoldPoint>().ToList();
        areas = transform.Find("Objects").GetComponentsInChildren<FoldingArea>();

        size = CalcSize();

        UpdateSelectEdge();
        UpdateObject();
    }

    public void UpdateColor(ColorPreset preset) {
        if (preset == ColorPreset.None) {
            return;
        }

        base.UpdateColor(paperColor, preset.Paper());
        foreach (FoldingArea foldingArea in areas) {
            foldingArea.UpdateColor(preset.Platform(), preset.Platform());
        }
    }

    private Vector2 CalcSize() {
        Vector2 maxDiff = Vector2.zero;
        for (int i = 0; i < points.Count; i++) {
            for (int j = 1; j < points.Count; j++) {
                Vector2 diff = points[i].transform.localPosition - points[j].transform.localPosition;
                maxDiff = Vector2.Max(maxDiff, diff);
            }
        }

        return maxDiff + Vector2.one * sizeOverflow;
    }

    public void Fold(Vector2 midpoint, Vector2 foldDir, Vector2 dragDir) {
        // Debug.Log("------Fold------");
        AddAllIntersections();

        float checkAngle = Vector2.SignedAngle(transform.right, foldDir); //find angle between fold direction and transform right direction
        float magnitude = size.magnitude; //find magnitude of size

        //find position of check box by doing the midpoint minus direction of the drag * magnitude of the size divided by 2
        Vector2 pos = midpoint - (2 * magnitude * dragDir.normalized);
        
        //do a check with overlap square with sides of size.magnitude and position determined above, with angle of check angle to the horizontal
        Collider2D[] pointsToReflect = Physics2D.OverlapBoxAll(pos, 4 * magnitude * Vector2.one, checkAngle, foldPointLayer); //use a layermask to only get vertices
        // Debug.Log("------Reflect------");

        foreach (Collider2D pointCol in pointsToReflect) { //loop through vertices found with OverlapBoxAll
            if (pointCol.transform.parent.parent == transform || pointCol.transform.parent.parent.parent.parent == transform) { //if point belongs to this paper
                FoldPoint foldPoint = pointCol.GetComponent<FoldPoint>(); //get FoldPoint from the vertex

                if (foldPoint != null && !foldPoint.isIntersection) { //if foldPoint is not null and is not an intersection
                    foldPoint.Reflect(); //reflect that point
                }
            }
        }

        UpdateAllObjects();
    }

    private void AddAllIntersections() {
        foreach (FoldingArea area in areas) { //loop through areas
            area.AddIntersections(); //add intersections on area
        }

        AddIntersections();
    }

    public void UpdateAllObjects() {
        // Debug.Log("------Update all------");

        foreach (FoldingArea area in areas) { //loop through areas
            area.UpdateObject(); //update it
        }
        
        UpdateObject();
    }

    private void UpdateSelectEdge() {
        Vector2[] edgePoints = new Vector2[points.Count + 1];

        for (int i = 0; i < points.Count; i++) {
            edgePoints[i] = points[i].transform.localPosition;
        }

        edgePoints[edgePoints.Length - 1] = points[0].transform.localPosition;

        selectEdge.points = edgePoints;
    }

    public void ResetPaper() {
        // Debug.Log("------Reset all----------");

        foreach (FoldingArea area in areas) {
            area.ResetPoints();
        }

        ResetPoints();
    }

    private void OnDrawGizmosSelected() {
        Vector2 center = Vector2.zero;
        foreach (FoldPoint pointPos in Points) {
            center += (Vector2) pointPos.transform.position;
        }

        center /= Points.Count;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
}
