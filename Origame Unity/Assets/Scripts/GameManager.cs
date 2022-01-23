using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM; //static reference

    public GameObject player; //reference
    public PlayerFold playerFold; //reference


    private void Awake() //makes this a singleton
    {
        if (GM != null)
        {
            Destroy(gameObject); //destroy this if already an instance in scene
        }
        else //otherwise
        { 
            GM = this; //set reference to this
            DontDestroyOnLoad(gameObject); //make persistent across scenes
        }
    }

    public void LoadLevel(int buildIndex)
    {

    }
}
