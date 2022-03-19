using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Activatable[] activateWhenUsed; //activate these activatable objects when the key is used
    [SerializeField] private Activatable[] deactivateWhenUsed;  //deactivate these activatable objects when the key is used
    private bool active = false; //is key active?

    [SerializeField] private Animator anim; //key animator
    [SerializeField] private LineRenderer[] lines; //change colour of lines
    [SerializeField] private Color onColour; //colour of lines when key is active
    [SerializeField] private Color offColour; //colour of lines when key is inactive

    [SerializeField] private AudioSource onAudioSource; //audio to play when activated
    [SerializeField] private AudioSource offAudioSource; //audio to play when activated

    [SerializeField] private bool firstNoiseNoMake; //dont make a noise the first time if active

    public void Activate() //activate the Activatable
    {
        if (!active) //if not active
        {
            if (anim != null)
            {
                anim.SetBool("activated", true);
            }

            foreach (LineRenderer line in lines) //make lines the right colour
            {
                line.startColor = onColour;
                line.endColor = onColour;
            }

            foreach (Activatable item in activateWhenUsed) //activate the activatables
            {
                if (item != null)
                {
                    item.ActivatedKey();
                }
            }

            foreach (Activatable item in deactivateWhenUsed) //deactivate the activatables
            {
                if (item != null)
                {
                    item.DeactivatedKey();
                }
            }

            //make noises
            if (!firstNoiseNoMake) //stop noise happening when the object is actvated the first time
            {
                if (onAudioSource != null)
                {
                    onAudioSource.Play(); //play activate audio
                }
            }

            firstNoiseNoMake = false;

            active = true;
        }
    }

    public void Deactivate() //deactivate (reverse of activate)
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

            if (!firstNoiseNoMake)
            {
                if (offAudioSource != null)
                {
                    offAudioSource.Play(); //play deactivate audio
                }
            }

            firstNoiseNoMake = false;

            active = false;
        }
    }
}
