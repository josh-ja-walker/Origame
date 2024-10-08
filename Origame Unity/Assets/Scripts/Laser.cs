using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class Laser : Activator
{
    private List<Vector2> points = new List<Vector2>();

    [Header("Laser")]
    [SerializeField] private LayerMask hitLayers;

    [SerializeField] private GameObject hitSprite;

    [SerializeField] private float maxReflections;
    private float reflections;

    [SerializeField] private float maxDistance = 50f;

    private LineRenderer laserLine;
    [SerializeField] private EdgeCollider2D edge;


    private void Start() {
        laserLine = GetComponent<LineRenderer>();
        laserLine.enabled = true;

        //remove the offset of the edgeCollider so that the points are accurate
        edge.transform.position = Vector3.zero;
        edge.transform.localEulerAngles = -transform.eulerAngles;
    }


    void Update() {
        points.Clear(); //clear existing points
        points.Add(transform.position); //add the start position

        DoRaycasts(transform.position, -transform.up, maxDistance); //start the recursive function with a raycast from start, downwards

        edge.SetPoints(points); //set the edge to the points set in DoRaycasts()

        laserLine.positionCount = points.Count; //set length of LineRenderer.positions
        laserLine.SetPositions(points.ToArray().Select(x => (Vector3) x).ToArray()); //set the positions to the points, converted to an array of Vector3s by ConvertArray()

        reflections = 0; //set reflections to 0
    }

    private void DoRaycasts(Vector2 startPos, Vector2 direction, float dist) { //recursive function that raycasts repeatedly to find points for the list
        if (dist <= 0) {
            return;
        }
    
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100f, hitLayers); //fire a ray in direction from pos, with distance 100 and a layerMask

        if (!hit) {
            points.Add(startPos + (direction * dist));
            if (Application.isPlaying) {
                Deactivate();
            }
        } else { 
            hitSprite.SetActive(false); //turn off the hit sprite

            float hitDist = (hit.point - startPos).magnitude;
            if (hitDist <= 0) {
                return;
            }
            
            points.Add(hit.point); //add the point to the list of points
            
            if (hit.collider.CompareTag("Laser Panel")) { //if hit a laser panel
                if (Application.isPlaying) {
                    Deactivate();
                }
            } else if (hit.collider.CompareTag("Reflective") && (reflections <= maxReflections)) { //hit a reflective surface
                reflections++; //increment number of reflections
                    
                //reflect the direction of the raycast in the normal
                //do a raycast from this point (move backwards slightly to ensure the laser doesn't get stuck in the same place)
                DoRaycasts(hit.point - 0.01f * direction, Vector2.Reflect(direction, hit.normal), dist - hitDist);
                
            } else { //hit normal ground
                if (Application.isPlaying) {
                    Deactivate();
                }   

                hitSprite.SetActive(true); //turn on hit sprite
                
                //set up hit sprite's transform
                hitSprite.transform.position = hit.point; //set position
                hitSprite.transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(hit.normal, Vector2.up)); //set angle in z axis to correct angle
            }
        }
    }

}