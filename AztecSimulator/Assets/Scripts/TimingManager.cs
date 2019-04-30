using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour {

	private bool mPaused = false;

	// MUST be the first script to run each frame, so that the proper gameDeltaTime is set for all other systems
	void Update () {

		// temp just to allow pausing / unpausing

		if(Input.GetButtonUp("Jump"))
		{
			// assume that this is attached to a toggleable Play/Pause button
			transform.GetComponent<Toggle>().isOn = !transform.GetComponent<Toggle>().isOn; 
		}

		GameState.GameDeltaTime = Mathf.Min(1, Time.deltaTime) * GameState.GameSpeed;  // avoid huge frame time spikes
		GameState.GameTimeElapsed += GameState.GameDeltaTime;
	}

	public void TogglePause() {
		mPaused = !mPaused;
		if(mPaused)
			GameState.GameSpeed = 0;
		else
			GameState.GameSpeed = 1;
	}
}
