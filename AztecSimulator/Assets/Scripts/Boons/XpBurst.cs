using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBurst : SacrificeResult {

	private int mAmount;

	public XpBurst() : base("Divine Inspiration", "Gives your people a boost of xp") {
		mAmount = 5;
	}

	public override void DoEffect() {
		Utilities.LogEvent("God teaches your people", 1f);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			p.AddXp(mAmount);
		}
	}
}
