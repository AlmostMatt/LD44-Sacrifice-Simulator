using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCropBoon : SacrificeResult {

	public GoodCropBoon() : base("Good Crops", "RIPE WHEAT FOR ALL")
	{

	}

	public override void DoEffect()
	{
		Utilities.LogEvent("Your crops have been blessed by god. Your people will have mini-wheats for days.");
	}
}
