using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");

        GameManager.GM.playerSpawn.StartCoroutine(GameManager.GM.playerSpawn.LoadAfterDeath());
    }
}
