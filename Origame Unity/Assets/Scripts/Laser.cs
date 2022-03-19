using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private EdgeCollider2D edge;
    [SerializeField] private Transform startPos;
    [SerializeField] private GameObject hitSprite;

    [SerializeField] private LayerMask hitLayers;
    
    [SerializeField] private float maxReflections;
    private float reflections;

    private List<Vector2> points = new List<Vector2>();

    private Key laserPanel;

    private void Start()
    {
        //remove the offset of the edgeCollider so that the points are accurate
        edge.transform.position = Vector3.zero;
        edge.transform.localEulerAngles = -transform.eulerAngles;
    }

    void FixedUpdate()
    {
        points.Clear(); //clear existing points
        points.Add(startPos.position); //add the start position

        DoRaycasts(startPos.position, -transform.up); //start the recursive function with a raycast from start, downwards

        edge.SetPoints(points); //set the edge to the points set in DoRaycasts()

        laserLine.positionCount = points.Count; //set length of LineRenderer.positions
        laserLine.SetPositions(ConvertArray(points.ToArray())); //set the positions to the points, converted to an array of Vector3s by ConvertArray()

        reflections = 0; //set reflections to 0
    }

    private void DoRaycasts(Vector2 startPos, Vector2 direction) //recursive function that raycasts repeatedly to find points for the list
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100f, hitLayers); //fire a ray in direction from pos, with distance 100 and a layerMask

        if (hit) //if hit something
        {
            points.Add(hit.point); //add the point to the list of points

            if (hit.collider.CompareTag("Laser Panel")) //if hit a laser panel
            {
                hitSprite.gameObject.SetActive(false); //turn off the hit sprite

                if (laserPanel == null) //if hasn't already got the laserPanel
                {
                    laserPanel = hit.collider.GetComponent<Key>(); //get key component on laserPanel

                    if (laserPanel != null) 
                    {
                        laserPanel.Activate(); //activate it
                    }
                }
            }
            else //did not hit laser panel
            {
                if (hit.collider.CompareTag("Reflective") && (reflections < maxReflections + 1)) //hit a reflective surface
                {
                    reflections++; //increment number of reflections
                    
                    //reflect the direction of the raycast in the normal
                    //do a raycast from this point (move backwards slightly to ensure the laser doesn't get stuck in the same place)
                    DoRaycasts(hit.point - 0.01f * direction, Vector2.Reflect(direction, hit.normal));
                }
                else //hit normal ground
                {
                    if (laserPanel != null) //if laserPanel was active
                    {
                        //deactivate it
                        laserPanel.Deactivate();
                        laserPanel = null;
                    }

                    hitSprite.gameObject.SetActive(true); //turn on hit sprite
                    
                    //set up hit sprite's transform
                    hitSprite.transform.position = hit.point; //set position
                    hitSprite.transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(hit.normal, Vector2.up)); //set angle in z axis to correct angle
                }
            }
        }
        else if (laserPanel != null) //if hit nothing and laserPanel was active
        {
            //deactivate it
            laserPanel.Deactivate();
            laserPanel = null;
        }

    }

    private Vector3[] ConvertArray(Vector2[] array) //convert an array of Vector2s to Vector3s for LineRenderer
    {
        Vector3[] vector3 = new Vector3[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            vector3[i] = array[i];
        }

        return vector3;
    }
}