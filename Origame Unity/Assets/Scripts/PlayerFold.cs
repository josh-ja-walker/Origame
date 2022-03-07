using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class PlayerFold : MonoBehaviour
{
    private Vector2 startPos; //position of mouse when click
    private Vector2 endPos; //position of mouse when release
    private Vector2 midpoint;
    
    private Vector2 dragDir;
    [SerializeField] private float snapAngle;
    private Vector2 foldLineDir;
    
    private Vector2 point1;
    private Vector2 point2;

    private bool dragging;

    [SerializeField] private float selectEdgeRadius; //the radius of the fold edge when selecting
    public float SelectEdgeRadius
    {
        get { return selectEdgeRadius; }
    }

    private Stack<Paper> papersToReset = new Stack<Paper>();

    [SerializeField] private EdgeCollider2D foldLine; //the line of the fold as an edge collider
    public EdgeCollider2D FoldLine
    {
        get { return foldLine; }
    }
    [SerializeField] private float extra;

    [SerializeField] private LayerMask paperLayer; //layer that the paper is on

    private Controls controls;

    [SerializeField] private AudioSource click;

    private void Awake()
    {
        controls = new Controls();

        controls.Player.Click.performed += _ => Click(); //when LMB pressed down, call Click method
        controls.Player.Click.canceled += _ => Release(); //when LMB released, call Release method

        try
        {
            controls.Player.UndoFold.performed += _ => ResetFold(); //when button for undoing a fold is pressed (R or Z) call Reset() 
        }
        catch (System.IndexOutOfRangeException) { }
    }

    private void OnEnable() //enable controls
    {
        controls.Player.Click.Enable();
        controls.Player.UndoFold.Enable();
    }

    private void OnDisable() //disable controls
    {
        controls.Player.Click.Disable();
        controls.Player.UndoFold.Disable();
    }

    IEnumerator DoFold()
    {
        while (dragging)
        {
            papersToReset.Peek().ResetPaper();
            
            yield return null;

            CalcValues(false);

            if (dragDir.sqrMagnitude > 0.1f)
            {
                SetFoldLine(); //SetFoldLine() with drag dir
            }
        }

        while (true)
        {
            papersToReset.Peek().ResetPaper();

            yield return null;

            CalcValues(true);

            if (dragDir.sqrMagnitude > 0.1f)
            {
                SetFoldLine(); //SetFoldLine() with drag dir
            }

            break;
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
                Paper selectedPaper = hit.collider.GetComponent<Paper>(); //get Paper component and set it as the Game Manager's selected paper

                if (selectedPaper != null)
                {
                    papersToReset.Push(selectedPaper);
                    papersToReset.Peek().SelectEdge.edgeRadius = 0; //set edge radius to 0 for adding intersections

                    //slow down time
                    Time.timeScale = 0f;

                    dragging = true;

                    click.Play();

                    StartCoroutine(DoFold());
                }
            }
        }
    }

    private void Release() //clicked when mouse released
    {
        if (dragging)
        {
            click.Play();

            papersToReset.Peek().isFolded = true;
            papersToReset.Peek().SelectEdge.edgeRadius = selectEdgeRadius; //set edge radius as selectEdgeRadius

            dragging = false;
        }

        //reset time
        Time.timeScale = 1f;
    }

    private void CalcValues(bool snap)
    {
        Debug.Log("------Calculate values------");

        endPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); //get release position of mouse
        dragDir = endPos - startPos; //find drag dir

        if (snap)
        {
            SnapDragDir();
        }

        midpoint = startPos + (0.5f * dragDir); //find midpoint of the drag
        foldLineDir = Vector2.Perpendicular(dragDir); //fold perpendicular direction to dragDir for foldLineDir
    }

    private void SnapDragDir()
    {
        float angle = -Vector2.SignedAngle(dragDir, Vector2.up); //find angle to vertical

        float multAngles = Mathf.Round(angle / snapAngle); //find closest multiples of division angle 

        dragDir = Quaternion.Euler(0, 0, multAngles * snapAngle) * Vector2.up * dragDir.magnitude;
    }

    private void SetFoldLine() //set the fold line edge collider
    {
        Debug.Log("------Set fold line------");

        if (FindFoldPoints())
        {
            //set points on fold edge as points that hit plus allowance to ensure ClosestPoint works properly in Paper.Fold() method
            foldLine.SetPoints(new List<Vector2> { point1 + (foldLineDir.normalized * extra), point2 - (foldLineDir.normalized * extra) });

            //call Fold() method on selectedPaper passing midpoint, foldLineDir and dragDir
            papersToReset.Peek().Fold(midpoint, foldLineDir, dragDir);
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
            hitAlongBool = (hitAlongFold.collider.transform == papersToReset.Peek().transform);
        }

        if (hitAwayFold)
        {
            hitAwayBool = (hitAwayFold.collider.transform == papersToReset.Peek().transform);
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

    public void CancelFold() //do fx for player to show they bad
    {
        Debug.Log("Cancelled fold");
        Release();
        ResetFold();
    }

    private void ResetFold()
    {
        if (!dragging)
        {
            Debug.Log("Reset everything");

            click.Play();

            point1 = Vector2.zero;
            point2 = Vector2.zero;

            try
            {
                papersToReset.Peek().ResetPaper();
                papersToReset.Peek().UpdateAllObjects();

                papersToReset.Peek().isFolded = false;

                papersToReset.Pop();
            }
            catch (System.Exception) { }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawLine(startPos, startPos + dragDir);
        Gizmos.DrawLine(midpoint - foldLineDir, midpoint + foldLineDir);
        
        Gizmos.DrawSphere(startPos, .1f);
        Gizmos.DrawSphere(endPos, .1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(point1, .1f);
        Gizmos.DrawSphere(point2, .1f);

        try
        {
            Gizmos.DrawWireCube(midpoint - (dragDir.normalized * papersToReset.Peek().Size.magnitude), 2 * Vector2.one * papersToReset.Peek().Size.magnitude);
        }
        catch (System.Exception) { }
    }
}

