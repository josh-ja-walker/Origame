using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) //ends credits
    {
        //delete saved positions so player spawns at start
        PlayerPrefs.DeleteKey("SavedPosX"); 
        PlayerPrefs.DeleteKey("SavedPosY");

        //reload level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
