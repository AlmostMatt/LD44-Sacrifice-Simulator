using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBoon : SacrificeResult {

	private int mAmount;

	public HealingBoon() : base("Healing Blessing", "Restores some lifeforce to your people") {
		mAmount = 20;
	}

	public override void DoEffect() {
		Utilities.LogEvent("God soothes the pains of your people.", 1f);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			p.Heal(20);
		}
	}
}
