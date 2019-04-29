using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueCurse : SacrificeResult {

	private int mAmount;

	public PlagueCurse() : base("Plague", "Illness will drain the lifeforce of your people")
	{
		mAmount = -20;
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("A terrible plague befalls your people", 1f);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			if(Random.value < 0.3)
			{
				p.Heal(mAmount);
			}
		}
	}
}
