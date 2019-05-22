using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBoon : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) {
			return new HealingBoon(tier, luck);
		}
	}

	private int mAmount;

	private HealingBoon(int tier, int luck) : base("Healing Blessing", "") {
		mAmount = 20 * (1 + tier + luck);
		mDescription = "Restores " + mAmount + " lifeforce to your people";
	}

	public override void DoEffect() {
		Utilities.LogEvent("God soothes the pains of your people.", 1f);
		foreach(Person p in Utilities.GetPersonManager().People.FindAll(p => p.Health < p.MaxHealth))
		{
			p.Heal(mAmount);
			Utilities.LogEvent(p.Name + "was healed (+" + mAmount + ").", 0f);
		}
	}
}
