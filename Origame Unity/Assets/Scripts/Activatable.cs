using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {

    [SerializeField] private int activatorsRequired;
    private int counter;

    private bool active;
    public bool IsActive() { return active; }

    [SerializeField] private bool permanentActivation;
    [SerializeField] private bool startActivation;
    
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();

        if (startActivation) {
            active = true;
            UpdateAnimator();
        }
    }

    public void Updated(Activator activator) {
        if (activator.IsActive()) {
            counter++;
        } else {
            counter--;
        }
        
        if (counter >= activatorsRequired) {
            active = true;
        } else if (!permanentActivation) { 
            /* If not permanently activated, deactivate */
            active = false;
        }

        UpdateAnimator();
    }

    private void UpdateAnimator() {
        if (anim != null) {
            anim.SetBool("activated", active);
        }
    }

}