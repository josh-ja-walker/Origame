using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private Activatable[] observers; 

    private bool active = false;
    public bool IsActive() { return active; }
    

    private Animator anim; //key animator

    [Header("Line Colours")]
    [SerializeField] private Color onColour; //colour of lines when key is active
    [SerializeField] private Color offColour; //colour of lines when key is inactive
    private LineRenderer[] lines; //change colour of liness

    [Header("Audio")]
    [SerializeField] private AudioSource onAudioSource; //audio to play when activated
    [SerializeField] private AudioSource offAudioSource; //audio to play when activated
    [SerializeField] private bool initialNoiseDisabled; //dont make a noise the first time if active


    void Start() {
        anim = GetComponent<Animator>();
        lines = GetComponentsInChildren<LineRenderer>()
            .Where(line => line.gameObject != gameObject)
            .ToArray();
    }

    /* Activate */
    public void Activate() { 
        /* If already active, ignore call */
        if (!active) { 
            SetActive(true);
        }
    }

    /* Deactivate */
    public void Deactivate() { 
        /* If already inactive, ignore call */
        if (active) { 
            SetActive(false);
        }
    }


    private void SetActive(bool toSet) {
        /* If already set, ignore */
        if (active == toSet) {
            return;
        }

        active = toSet;

        if (anim != null) {
            anim.SetBool("activated", active);
        }

        /* Set lines colour */
        foreach (LineRenderer line in lines) { 
            if (line != null) {
                line.startColor = active ? onColour : offColour;
                line.endColor = line.startColor;
            }
        }

        /* Update observers */
        foreach (Activatable item in observers) { 
            item.Updated(this);
        }

        /* Play activate/deactivate audio */
        if (!initialNoiseDisabled) { /* Deny initial play */
            AudioSource audio = active ? onAudioSource : offAudioSource;
            if (audio != null) {
                audio.Play();
            }
        }

        initialNoiseDisabled = false;
    }

}
