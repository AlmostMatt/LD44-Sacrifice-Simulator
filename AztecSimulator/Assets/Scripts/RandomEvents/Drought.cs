using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drought : RandomEventSystem.RandomEvent {

	private float mDuration;
	private bool mIntervened = false;
	private int mDemandId;

	private Sprite mPrevBackgroundSprite;

	private class GodIntervention : SacrificeResult {

		private Drought mDrought;

		public GodIntervention(Drought drought) : base("Rain", "Stops the drought") {
			mDrought = drought;
		}

		public override void DoEffect() {
			mDrought.mIntervened = true;
		}
	}

	public override float Start () {
        GameState.SetBoon(BoonType.BONUS_FOOD_PERCENT, GameState.GetBoonValue(BoonType.BONUS_FOOD_PERCENT) - 50);
        // ~15s at minute 0, ~20s at minute 4, ~25s at minute 9, ~30s at minute 16
        float diffIncrease = Mathf.Sqrt(GameState.GameTimeElapsed / 60f);
		mDuration = (Random.Range(13, 18) + Mathf.FloorToInt(5 * diffIncrease));
		mIntervened = false;

		Image background = Utilities.GetBackground();
		mPrevBackgroundSprite = background.sprite;
		background.sprite = Utilities.GetSpriteManager().GetSprite("DroughtBackground");

        string description = "-50% food production for " + mDuration + " seconds";
        mOngoing = new Ongoing("DROUGHT", "Drought!", description, mDuration, false);
		GameState.Ongoings.Add(mOngoing);
		Utilities.LogEvent("A drought has befallen your farmland", 1f);
		mDemandId = Utilities.GetGod().AddFleetingDemand(
			(int)diffIncrease,
			new GodIntervention(this), 
			null, 
			mDuration, 
			"God offers RAIN in exchange for "
		);

		return(mDuration);
	}

	public override bool Update () {
		return(mIntervened);
	}

	public override void Removed() {

		if(mIntervened)
			Utilities.LogEvent("God blessed the rains down in your city. The drought has ended and food production can return to normal.", 1.5f);
		else
			Utilities.LogEvent("The drought has ended.", 1f);

		if(mDemandId > 0) Utilities.GetGod().RemoveDemand(mDemandId);

		Utilities.GetBackground().sprite = mPrevBackgroundSprite;

        GameState.SetBoon(BoonType.BONUS_FOOD_PERCENT, GameState.GetBoonValue(BoonType.BONUS_FOOD_PERCENT) + 50);
		GameState.Ongoings.Remove(mOngoing);
		mOngoing = null;
		Utilities.GetEventSystem().ScheduleEvent(new Drought(), Random.Range(45, 91));
	}
}
