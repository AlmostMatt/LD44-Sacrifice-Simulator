using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCropBoon : SacrificeResult {

	public GoodCropBoon() : base("Good Crops", "RIPE WHEAT FOR ALL")
	{

	}

	public override void DoEffect()
	{
		Utilities.LogEvent("This season's quinoa harvest was blessed. +1 Food Supply");
		GameState.FoodSupply += 1;
	}
}
