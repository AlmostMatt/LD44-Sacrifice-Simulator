using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SacrificeResult {

	public string mName;
	public string mDescription;

	public SacrificeResult(string name, string description)
	{
		mName = name;
		mDescription = description;
	}

	public void DebugPrint()
	{
		Debug.Log(mName + " -- " + mDescription);
	}

	public abstract void DoEffect();
}
