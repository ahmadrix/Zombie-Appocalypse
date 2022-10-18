using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject button;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            MainMenu();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);      //Loading game scene
    }
    
    public void MainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);    //Loading Main menu scene
    }

    public void HowToPlay()
    {
        button.SetActive(true);
    }

    public void MainMenuFromHowToPlay()
    {
        button.SetActive(false);
    }
    
    public void QuitGame () 
    {
        Application. Quit ();
        Debug. Log("Game is exiting");
        //Just to make sure its working.
    }
}
