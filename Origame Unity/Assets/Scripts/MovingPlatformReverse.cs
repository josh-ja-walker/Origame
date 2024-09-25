using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformReverse : StateMachineBehaviour
{
    //if enters deactivated state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>(); //get moving platform component
        
        if (movingPlat != null)
        {
            movingPlat.goingForwards = false; //reverse the direction of the platform - move backwards
            movingPlat.ReachPoint(); //cause the moving platform to recalculate its destination
        }
    }

    //if exits deactivated state, i.e. turned on
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>(); //get the component

        if (movingPlat != null)
        {
            movingPlat.goingForwards = true; //go forwards from now on
            movingPlat.ReachPoint(); //recalculate
        }
    }
}
