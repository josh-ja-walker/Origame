using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class PlayerFold : MonoBehaviour
{
    [SerializeField] private float bulletTime;

    private Vector2 startPos; //position of mouse when click
    private Vector2 endPos; //position of mouse when release
    private Vector2 midpoint;
    private Vector2 dragDir;
    private Vector2 foldLineDir;
    private Vector2 point1;
    private Vector2 point2;
    private bool dragging;

    [SerializeField] private float selectEdgeRadius; //the radius of the fold edge when selecting
    public float SelectEdgeRadius
    {
        get { return selectEdgeRadius; }
    }

    private List<Paper> papersToReset = new List<Paper>();
    private Paper selectedPaper; //the paper that is currently selected to fold

    [SerializeField] private EdgeCollider2D foldLine; //the line of the fold as an edge collider
    public EdgeCollider2D FoldLine
    {
        get { return foldLine; }
    }

    [SerializeField] private SpriteShapeController foldLineSprite;
    [SerializeField] private float lineHeight;

    [SerializeField] private LayerMask paperLayer; //layer that the paper is on

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();

        controls.Player.Click.performed += _ => Click(); //when LMB pressed down, call Click method
        controls.Player.Release.performed += _ => Release(); //when LMB released, call Release method

        try
        {
            controls.Player.UndoFold.performed += _ => ResetFold(); //when button for undoing a fold is pressed (R or Z) call Reset() 
        }
        catch (System.IndexOutOfRangeException) { }
    }

    private void OnEnable() //enable controls
    {
        controls.Player.Click.Enable();
        controls.Player.Release.Enable();
        controls.Player.UndoFold.Enable();
    }

    private void OnDisable() //disable controls
    {
        controls.Player.Click.Disable();
        controls.Player.Release.Disable();
        controls.Player.UndoFold.Disable();
    }

    IEnumerator DoFold()
    {
        while (dragging)
        {
            selectedPaper.ResetPaper();
            
            yield return null;

            CalcValues();

            if (dragDir.sqrMagnitude > 0.1f)
            {
                SetFoldLine(); //SetFoldLine() with drag dir
            }
        }
    }

    private void Click() //called when mouse clicked
    {
        //get start position using ScreenToWorldPoint() on the current mouse position
        startPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); 

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.zero, 5f, paperLayer); //raycast into screen from startPos and hit paper layer
    
        if (hit) //if did hit paper
        {
            if (hit.collider.isTrigger)
            {
                selectedPaper = hit.collider.GetComponent<Paper>(); //get Paper component and set it as the Game Manager's selected paper

                if (selectedPaper != null)
                {
                    selectedPaper.SelectEdge.edgeRadius = 0; //set edge radius to 0 for adding intersections
                    papersToReset.Add(selectedPaper);

                    //slow down time
                    Time.timeScale = 0f;

                    dragging = true;

                    StartCoroutine(DoFold());
                }
            }
        }
    }

    private void Release() //clicked when mouse released
    {
        if (dragging)
        {
            selectedPaper.isFolded = true;
            selectedPaper.SelectEdge.edgeRadius = selectEdgeRadius; //set edge radius as selectEdgeRadius

            dragging = false;
        }

        //reset time
        Time.timeScale = 1f; 
    }

    private void CalcValues()
    {
        Debug.Log("------Calculate values------");

        endPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); //get release position of mouse
        dragDir = endPos - startPos; //find drag dir
        
        midpoint = startPos + (0.5f * dragDir); //find midpoint of the drag
        foldLineDir = Vector2.Perpendicular(dragDir); //fold perpendicular direction to dragDir for foldLineDir
    }

    private void SetFoldLine() //set the fold line edge collider
    {
        Debug.Log("------Set fold line------");

        if (FindFoldPoints())
        {
            //set points on fold edge as points that hit plus allowance to ensure ClosestPoint works properly in Paper.Fold() method
            foldLine.SetPoints(new List<Vector2> { point1 + (foldLineDir.normalized * 10f), point2 - (foldLineDir.normalized * 10f) });

            //call Fold() method on selectedPaper passing midpoint, foldLineDir and dragDir
            selectedPaper.Fold(midpoint, foldLineDir, dragDir);
        }
        else
        {
            CancelFold();
        }
    }

    private bool FindFoldPoints()
    {
        RaycastHit2D hitAlongFold = Physics2D.Raycast(midpoint, foldLineDir, 25f, paperLayer); //raycast along the fold
        RaycastHit2D hitAwayFold = Physics2D.Raycast(midpoint, -foldLineDir, 25f, paperLayer); //raycast along opposite fold direction
        
        bool hitAlongBool = false;
        bool hitAwayBool = false;

        if (hitAlongFold)
        {
            hitAlongBool = (hitAlongFold.collider.transform == selectedPaper.transform);
        }

        if (hitAwayFold)
        {
            hitAwayBool = (hitAwayFold.collider.transform == selectedPaper.transform);
        }
        
        if (hitAlongBool || hitAwayBool)
        {
            if (hitAlongBool && hitAwayBool) //if both hit allowed
            {
                point1 = hitAlongFold.point;
                point2 = hitAwayFold.point;
            }
            else if (!hitAwayBool && hitAlongBool) //if awayFold not hit but alongFold did
            {
                point1 = hitAlongFold.point;
                point2 = Physics2D.Raycast(hitAlongFold.point, foldLineDir, 25f, paperLayer).point;
            }
            else if (!hitAlongBool && hitAwayBool) //if alongFold not hit but awayFold did and is allowed by function
            {
                point1 = hitAwayFold.point;
                point2 = Physics2D.Raycast(hitAwayFold.point, -foldLineDir, 25f, paperLayer).point;
            }

            return true;
        }

        return false;
    }

    private void SetFoldLineSprite()
    {
        try
        {
            foldLineSprite.spline.Clear();

            foldLineSprite.spline.InsertPointAt(0, point1);
            foldLineSprite.spline.InsertPointAt(1, point2);

            foldLineSprite.spline.SetHeight(0, lineHeight);
            foldLineSprite.spline.SetHeight(1, lineHeight);

            foldLineSprite.gameObject.SetActive(true);
        }
        catch (System.ArgumentException)
        {
            Debug.Log("Argument exception SetFoldLineSprite()");
            //foldLineSprite.gameObject.SetActive(false);
        }
    }

    public void CancelFold() //do fx for player to show they bad
    {
        Debug.Log("Cancelled fold");

        ResetFold();
    }

    private void ResetFold()
    {
        Release();
        
        Debug.Log("Reset everything when press reset button");

        point1 = Vector2.zero;
        point2 = Vector2.zero;

        foldLineSprite.gameObject.SetActive(false);

        try
        {
            Paper paperToReset = papersToReset[papersToReset.Count - 1];

            paperToReset.ResetPaper();
            paperToReset.UpdateAllObjects();

            paperToReset.isFolded = false;

            papersToReset.Remove(paperToReset);
        }
        catch (System.ArgumentOutOfRangeException) { }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawLine(midpoint - foldLineDir, midpoint + foldLineDir);
        
        Gizmos.DrawSphere(startPos, .1f);
        Gizmos.DrawSphere(endPos, .1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point1, .1f);
        Gizmos.DrawSphere(point2, .1f);

        try
        {
            Gizmos.DrawWireCube(midpoint - (dragDir.normalized * selectedPaper.Size.magnitude), 2 * Vector2.one * selectedPaper.Size.magnitude);
        }
        catch (System.NullReferenceException) { }
    }
}

