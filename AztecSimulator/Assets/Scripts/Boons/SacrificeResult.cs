using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SacrificeResult {

    public string mName;
    public string mDescription;
    public string mIcon;

    public SacrificeResult(string name, string description, string icon="")
	{
		mName = name;
		mDescription = description;
        mIcon = icon;
	}

	public void DebugPrint()
	{
		Debug.Log(mName + " -- " + mDescription);
	}

	public abstract void DoEffect();
}
