﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Favour : SacrificeResult {

	// generally improved things from god, e.g.
	// increased time between demands
	// event aid is cheaper / more effective
	// random generation skews towards player-positive things

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new Favour();
		}
	}

	private Favour() : base(
        "Favour",
        "God takes favour on your people.\r\n(Random demands will be better for you)",
        "STAR") {}

	public override void DoEffect() {
		Utilities.LogEvent("Your people find favour with god", Utilities.SHORT_LOG_DURATION);
		GameState.Favour++;
	}

}
