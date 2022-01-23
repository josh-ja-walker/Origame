using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private bool isOn = false;
    [SerializeField] private bool canTurnOff = false;
    [SerializeField] private Activatable activatable;
    [SerializeField] private Animator anim;

    public void Interact()
    {
        if (isOn)
        {
            if (canTurnOff)
            {
                SwitchOff();
            }
        }
        else
        {
            SwitchOn();
        }
    }

    private void SwitchOn()
    {
        activatable.ActivatedKey();

        anim.SetBool("isOn", true);
        isOn = true;
    }

    private void SwitchOff()
    {
        activatable.DeactivatedKey();

        anim.SetBool("isOn", false);
        isOn = false;
    }
}
