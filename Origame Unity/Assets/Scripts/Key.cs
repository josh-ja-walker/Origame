using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Activatable[] activateWhenUsed;
    [SerializeField] private Activatable[] deactivateWhenUsed;
    private bool active = false;

    [SerializeField] private Animator anim;
    [SerializeField] private LineRenderer[] lines;
    [SerializeField] private Color onColour;
    [SerializeField] private Color offColour;

    public void Activate()
    {
        if (!active)
        {
            if (anim != null)
            {
                anim.SetBool("activated", true);
            }

            foreach (LineRenderer line in lines)
            {
                line.startColor = onColour;
                line.endColor = onColour;
            }

            foreach (Activatable item in activateWhenUsed)
            {
                if (item != null)
                {
                    item.ActivatedKey();
                }
            }

            foreach (Activatable item in deactivateWhenUsed)
            {
                if (item != null)
                {
                    item.DeactivatedKey();
                }
            }

            active = true;
        }
    }

    public void Deactivate()
    {
        if (active)
        {
            if (anim != null)
            {
                anim.SetBool("activated", false);
            }

            foreach (LineRenderer line in lines)
            {
                line.startColor = offColour;
                line.endColor = offColour;
            }

            foreach (Activatable item in activateWhenUsed)
            {
                if (item != null)
                {
                    item.DeactivatedKey();
                }
            }
        
            foreach (Activatable item in deactivateWhenUsed)
            {
                if (item != null)
                {
                    item.ActivatedKey();
                }
            }

            active = false;
        }
    }
}
