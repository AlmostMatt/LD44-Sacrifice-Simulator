﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueCurse : SacrificeResult {

	private int mAmount;

	public PlagueCurse() : base("Plague", "Illness will drain the lifeforce of your people")
	{
		mAmount = Random.Range(2,6) * 10;
	}

	public override void DoEffect()
	{
		Utilities.LogEvent("A terrible plague befalls your people", Utilities.MEDIUM_LOG_DURATION);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			if(Random.value < 0.5)
			{
				p.Damage(mAmount);
				Utilities.LogEvent(p.Name + " was wounded by the plague (-" + mAmount + ").", 0f);
			}
		}
	}
}
