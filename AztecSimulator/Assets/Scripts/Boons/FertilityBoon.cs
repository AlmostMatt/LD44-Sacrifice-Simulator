using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilityBoon : SacrificeResult {

	private static string sName = "Fertility Blessing";
	private static string sDesc = "God blesses our people with newborns.";
	private static string sAltDesc = "It's a baby shower";

	private class FertilityEvent : RandomEventSystem.RandomEvent
	{
		private int mBoost;
		private float mDuration;

		public FertilityEvent(int boost, float duration) {
			mBoost = boost;
			mDuration = duration;
			mOngoing = new Ongoing("BirthRateBoost", sName, sDesc, mDuration, true); 
		}

		public override float Start()
		{
			GameState.SetBoon(BoonType.BONUS_FERTILITY, mBoost);
			GameState.Ongoings.Add(mOngoing);
			return(mDuration);
		}

		public override void Removed()
		{
			GameState.SetBoon(BoonType.BONUS_FERTILITY, 0);
			GameState.Ongoings.Remove(mOngoing);
		}
	}

	public FertilityBoon() : base(sName, "Temporary birth rate increase") {
	}

	public override void DoEffect()
	{
		Utilities.LogEvent(sDesc, 1f);
		Utilities.GetEventSystem().ScheduleEvent(new FertilityEvent(100, 10), 0);
	}
}
