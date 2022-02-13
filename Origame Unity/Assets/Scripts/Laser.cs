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
        edge.transform.position = Vector3.zero;
        edge.transform.localEulerAngles = -transform.eulerAngles;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            points.Clear();
            points.Add(startPos.position);

            DoRaycasts(startPos.position, -transform.up);

            edge.SetPoints(points);

            laserLine.positionCount = points.Count;
            laserLine.SetPositions(ConvertArray(points.ToArray()));

            reflections = 0;
        }
    }

    void FixedUpdate()
    {
        points.Clear();
        points.Add(startPos.position);

        DoRaycasts(startPos.position, -transform.up);

        edge.SetPoints(points);

        laserLine.positionCount = points.Count;
        laserLine.SetPositions(ConvertArray(points.ToArray()));

        reflections = 0;
    }

    private void DoRaycasts(Vector2 startPos, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, 100f, hitLayers); //fire a ray in transform.down direction

        hitSprite.SetActive(hit); //turn sprite on if hit is true

        if (hit)
        {
            points.Add(hit.point);

            if (hit.collider.CompareTag("Laser Panel"))
            {
                hitSprite.gameObject.SetActive(false);

                if (laserPanel == null)
                {
                    laserPanel = hit.collider.GetComponent<Key>();

                    if (laserPanel != null)
                    {
                        laserPanel.Activate();
                    }
                }
            }
            else
            {
                if (hit.collider.CompareTag("Reflective") && (reflections < maxReflections + 1))
                {
                    reflections++;
                    DoRaycasts(hit.point - 0.01f * direction, Vector2.Reflect(direction, hit.normal)); //reflect and keep laser going recursively
                }
                else
                {
                    if (laserPanel != null)
                    {
                        Debug.Log("Deactivate");

                        laserPanel.Deactivate();
                        laserPanel = null;
                    }

                    //set up hit transform
                    hitSprite.gameObject.SetActive(true);
                    hitSprite.transform.position = hit.point/* - (direction * hitOffset)*/;
                    hitSprite.transform.eulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(hit.normal, Vector2.up));
                }
            }
        }
        else if (laserPanel != null)
        {
            Debug.Log("Deactivate");

            laserPanel.Deactivate();
            laserPanel = null;
        }

    }

    private Vector3[] ConvertArray(Vector2[] array)
    {
        Vector3[] vector3 = new Vector3[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            vector3[i] = array[i];
        }

        return vector3;
    }
}