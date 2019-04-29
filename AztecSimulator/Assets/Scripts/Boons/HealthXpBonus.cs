using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthXpBonus : SacrificeResult {

	private bool mHealthy;
	private int mThreshold;
	private int mBonus;

	public HealthXpBonus() : base("", "") {
		mHealthy = Random.value < 0.7;
		mBonus = 1;
		if(mHealthy)
		{
			mThreshold = Random.Range(9, 11) * 10;
			mName = "Healthy Body, Healthy Mind";
			mDescription = "People with " + mThreshold + " lifeforce or more get +" + mBonus + "xp per second";
		}
		else
		{
			mThreshold = 20;
			mName = "Desperate Learning";
			mDescription = "People with " + mThreshold + " lifeforce or less get +" + mBonus + "xp per second";
		}
	}

	public override void DoEffect()
	{
		if(mHealthy)
		{
			GameState.SetBoon(BoonType.HEALTHY_BONUS_XP_THRESHOLD, mThreshold);
			GameState.SetBoon(BoonType.HEALTHY_BONUS_XP, mBonus);
		}
		else
		{
			GameState.SetBoon(BoonType.UNHEALTHY_BONUS_XP_THRESHOLD, mThreshold);
			GameState.SetBoon(BoonType.UNHEALTHY_BONUS_XP, mBonus);
		}
	}

}
