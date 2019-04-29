using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlagueCurse : SacrificeResult {

	public PlagueCurse() : base("Plague", "SICKNESS")
	{

	}

	public override void DoEffect()
	{
		Utilities.LogEvent("A terrible plague befalls your people", 1f);
		foreach(Person p in Utilities.GetPersonManager().People)
		{
			if(Random.value < 0.3)
			{
				p.Health -= Mathf.Min(p.Health, 20);
			}
		}
	}
}
