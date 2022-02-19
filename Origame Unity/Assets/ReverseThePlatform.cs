using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseThePlatform : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>();
        
        if (movingPlat != null)
        {
            movingPlat.goingForwards = false;
            movingPlat.ReachPoint();
        }
    }
        
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MovingPlatform movingPlat = animator.transform.GetChild(0).GetComponent<MovingPlatform>();

        if (movingPlat != null)
        {
            movingPlat.goingForwards = true;
            movingPlat.ReachPoint();
        }
    }
}
