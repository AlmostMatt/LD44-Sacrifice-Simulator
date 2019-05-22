using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCropBoon : SacrificeResult {

	private static string sDesc = "God blessed us with extra food that will last for a short while.";

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) {
			return new GoodCropBoon(tier, luck);
		}
	}

	private class GoodCropEvent : RandomEventSystem.RandomEvent
	{
		private int mBoost;
		private float mDuration;

		public GoodCropEvent(int boost, float duration) {
			mBoost = boost;
			mDuration = duration;
			mOngoing = new Ongoing("FoodBoost", "Blessed Crops", sDesc, mDuration, true); 
		}

		public override float Start()
		{
			GameState.SetBoon(BoonType.BONUS_FOOD_PERCENT, GameState.GetBoonValue(BoonType.BONUS_FOOD_PERCENT) + mBoost);
			GameState.Ongoings.Add(mOngoing);
			return(mDuration);
		}

		public override void Removed()
		{
            GameState.SetBoon(BoonType.BONUS_FOOD_PERCENT, GameState.GetBoonValue(BoonType.BONUS_FOOD_PERCENT) - mBoost);
			GameState.Ongoings.Remove(mOngoing);
		}
	}

	private int mBoost;
	private float mDuration;
	private GoodCropBoon(int tier, int luck) : base("Blessed Crops", "")
	{
		mBoost = 25 + (25 * (tier + luck));
		mDuration = 30f;
		mDescription = "+" + mBoost + "% food for " + mDuration + " seconds";
	}

	public override void DoEffect()
	{
		Utilities.LogEvent(sDesc);
		Utilities.GetEventSystem().ScheduleEvent(new GoodCropEvent(mBoost, mDuration), 0);
	}
}
