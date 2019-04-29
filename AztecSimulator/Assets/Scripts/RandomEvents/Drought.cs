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

		public GodIntervention(Drought drought) : base("Divine Intervention - Rain", "Stops the drought") {
			mDrought = drought;
		}

		public override void DoEffect() {
			mDrought.mIntervened = true;
		}
	}

	public override float Start () {
		GameState.Drought = true;
		int diffIncrease = Mathf.FloorToInt(GameState.GameTimeElapsed / 60);
		mDuration = (Random.Range(1, 4) + diffIncrease) * 10;
		mIntervened = false;

		Image background = Utilities.GetBackground();
		mPrevBackgroundSprite = background.sprite;
		background.sprite = Utilities.GetSpriteManager().GetSprite("DroughtBackground");

		mOngoing = new Ongoing("DROUGHT", "Drought!", "A drought is reducing your crop yield.", mDuration, false);
		GameState.Ongoings.Add(mOngoing);
		Utilities.LogEvent("A drought has befallen your farmland", 1f);
		mDemandId = Utilities.GetGod().AddFleetingDemand(
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

		GameState.Drought = false;

		GameState.Ongoings.Remove(mOngoing);
		mOngoing = null;
		Utilities.GetEventSystem().ScheduleEvent(new Drought(), Random.Range(45, 91));
	}
}
