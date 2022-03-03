using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseThePlatform : StateMachineBehaviour
{
    //if enters deactivated state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>(); //get moving platform
        
        if (movingPlat != null)
        {
            movingPlat.goingForwards = false; //reverse
            movingPlat.ReachPoint(); //turn
        }
    }

    //if exits deactivated state, i.e. turned on
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>();

        if (movingPlat != null)
        {
            movingPlat.goingForwards = true; //go forwards
            movingPlat.ReachPoint(); //turn
        }
    }
}
