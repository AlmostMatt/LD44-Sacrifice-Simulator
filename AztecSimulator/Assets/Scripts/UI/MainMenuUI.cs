using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

	public string gameScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void NewGame()
	{
		Debug.Log("New game!");
		SceneManager.LoadScene(gameScene);
	}

	public void Tutorial()
	{
		Debug.Log("Tutorial!");
	}
}
