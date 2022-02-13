using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour
{
    [SerializeField] private int numKeysLeft;
    [SerializeField] private bool permanentActivation;
    [SerializeField] private bool activatedOnStart;
    private bool activated;
    public bool Activated
    {
        get { return activated; }
    }
    
    [SerializeField] private Animator anim;

    private void Start()
    {
        if (activatedOnStart)
        {
            ActivatedKey();
        }
    }

    public void ActivatedKey()
    {
        numKeysLeft--;

        if (numKeysLeft <= 0)
        {
            Debug.Log("Open door");
            activated = true;
            anim.SetBool("activated", true);
        }
    }

    public void DeactivatedKey()
    {
        if (!(permanentActivation && activated)) //if not already activated and it permanently activated
        {
            Debug.Log("Open door");
            numKeysLeft++;
            activated = false;
            anim.SetBool("activated", false);
        }
    }
}