using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedLifespan : SacrificeResult {

	// permanent buff to all newborns
	public ImprovedLifespan() : base("Improved Lifespan", "Live longer") {
	}

	public override void DoEffect()
	{
		GameState.ImprovedLifespan1 = true;
	}
}
