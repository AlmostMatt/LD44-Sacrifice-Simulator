using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodCropBoon : SacrificeResult {

	private static string sDesc = "God blessed us with extra food that will last for a short while.";

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
			GameState.SetBoon(BoonType.BONUS_FOOD, mBoost);
			GameState.Ongoings.Add(mOngoing);
			return(mDuration);
		}

		public override void Removed()
		{
			GameState.SetBoon(BoonType.BONUS_FOOD, 0);
			GameState.Ongoings.Remove(mOngoing);
		}
	}

	public GoodCropBoon() : base("Blessed Crops", "Temporary boost to food")
	{
	}

	public override void DoEffect()
	{
		Utilities.LogEvent(sDesc);
		Utilities.GetEventSystem().ScheduleEvent(new GoodCropEvent(10, 20), 0);
	}
}
