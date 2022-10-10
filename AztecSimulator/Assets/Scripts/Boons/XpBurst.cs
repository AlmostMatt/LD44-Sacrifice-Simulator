using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBurst : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new XpBurst(tier, luck);
		}
	}

	private int mAmount;

	private XpBurst(int tier, int luck) : base("Divine Inspiration", "", "+XP") {
		mAmount = 5 * (1 + tier + luck);
		mDescription = "Instantly gives your people +" + mAmount + "xp";
	}

	public override void DoEffect() {
		Utilities.LogEvent("God teaches your people", Utilities.SHORT_LOG_DURATION);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			p.AddXp(mAmount);
		}
	}
}
