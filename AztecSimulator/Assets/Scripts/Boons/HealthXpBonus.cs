using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthXpBonus : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new HealthXpBonus(tier, luck);
		}
	}

	private bool mHealthy;
	private int mThreshold;
	private int mBonus;

	private HealthXpBonus(int tier, int luck) : base("", "") {
		mHealthy = Random.value < 0.7;
		mBonus = 2 * (1 + tier);
		if(mHealthy)
		{
			mThreshold = 10 * Random.Range(Mathf.Max(9 - luck, 7), 11);
			mName = "Healthy Body, Healthy Mind";
			mDescription = "People with " + mThreshold + " lifeforce or more get +" + mBonus + "xp per second";
		}
		else
		{
			mThreshold = 10 * Mathf.Min(3, Random.Range(1, 2 + luck));
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
