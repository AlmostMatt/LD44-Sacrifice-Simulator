using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	private bool mPaused = false;

	public GameObject bgm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleMusic()
	{
		mPaused = !mPaused;
		if(mPaused)
			bgm.GetComponent<AudioSource>().Pause();
		else
			bgm.GetComponent<AudioSource>().UnPause();
	}
}
