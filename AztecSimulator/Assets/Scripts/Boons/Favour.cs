using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Favour : SacrificeResult {

	// generally improved things from god, e.g.
	// increased time between demands
	// event aid is cheaper / more effective
	// random generation skews towards player-positive things
	public Favour() : base("Favour", "God takes favour on your people") {}

	public override void DoEffect() {
		Utilities.LogEvent("Your people find favour with god");
		GameState.Favour++;
	}

}
