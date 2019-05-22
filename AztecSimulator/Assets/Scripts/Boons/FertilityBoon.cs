using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FertilityBoon : SacrificeResult {

	private static string sName = "Fertility Blessing";
	private static string sDesc = "God blesses our people with newborns.";
	private static string sAltDesc = "It's a baby shower";

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new FertilityBoon(tier, luck);
		}
	}

	private class FertilityEvent : RandomEventSystem.RandomEvent
	{
		private int mBoost;
		private float mDuration;

		public FertilityEvent(int boost, float duration) {
			mBoost = boost;
			mDuration = duration;
            string description = "+" + mBoost + "% birth rate";
            mOngoing = new Ongoing("BirthRateBoost", sName, description, mDuration, true); 
		}

		public override float Start()
        {
            GameState.SetBoon(BoonType.BONUS_BIRTHS_PERCENT, GameState.GetBoonValue(BoonType.BONUS_BIRTHS_PERCENT) + mBoost);
			GameState.Ongoings.Add(mOngoing);
			return(mDuration);
		}

		public override void Removed()
        {
            GameState.SetBoon(BoonType.BONUS_BIRTHS_PERCENT, GameState.GetBoonValue(BoonType.BONUS_BIRTHS_PERCENT) - mBoost);
            GameState.Ongoings.Remove(mOngoing);
		}
	}

	private int mBoost;
	private int mDuration;
	private FertilityBoon(int tier, int luck) : base(sName, "") {
		mBoost = 80 + (20 * (tier + luck));
		mDuration = 15;
        mDescription = "+" + mBoost + "% birth rate for " + mDuration + " seconds";
    }

	public override void DoEffect()
	{
		Utilities.LogEvent(sDesc, 1f);
		Utilities.GetEventSystem().ScheduleEvent(new FertilityEvent(mBoost, mDuration), 0);
	}
}
