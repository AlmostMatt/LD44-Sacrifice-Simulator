using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBoon : SacrificeResult {

	public HealingBoon() : base("Healing Blessing", "Restores some lifeforce to your people") {
	}

	public override void DoEffect() {
		Utilities.LogEvent("God soothes the pains of your people and renews their vigor.", 1f);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			p.Health += 20;
		}
	}
}
