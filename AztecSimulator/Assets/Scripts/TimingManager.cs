using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingManager : MonoBehaviour {

	private bool mPaused = false;

	// MUST be the first script to run each frame, so that the proper gameDeltaTime is set for all other systems
	void Update () {

		// temp just to allow pausing / unpausing

		if(Input.GetButtonUp("Jump"))
		{
			mPaused = !mPaused;
			if(mPaused)
				GameState.GameSpeed = 0;
			else
				GameState.GameSpeed = 1;
		}

		GameState.GameDeltaTime = Time.deltaTime * GameState.GameSpeed;
	}
}
