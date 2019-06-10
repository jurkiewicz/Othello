using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void PlayerMinMax()
    {
        SceneManager.LoadScene("Player");
    }

    public void PlayerMCTS()
    {
        SceneManager.LoadScene("PlayerMCTS");
    }

    public void MinMaxRandom()
    {
        SceneManager.LoadScene("AI");
    }

    public void MCTSMinMax()
    {
        SceneManager.LoadScene("Othello");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }

}
