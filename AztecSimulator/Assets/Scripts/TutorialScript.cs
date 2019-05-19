using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour, IDialogCallback {

	private int mStage;

	// Use this for initialization
	void Start () {

		mStage = 0;

		/// Utilities.GetTimingManager().SetPaused(true);
		Utilities.GetUIManager().ShowMessage("This is a game about sacrificing people", this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnDialogClosed()
	{
		++mStage;
		if(mStage == 1)
		{
			Utilities.GetUIManager().ShowMessage("Start by trying to do something cool", this);
		}
	}
}
