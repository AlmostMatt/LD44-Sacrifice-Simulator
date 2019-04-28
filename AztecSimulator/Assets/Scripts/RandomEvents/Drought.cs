﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drought : RandomEventSystem.RandomEvent {

	private float mDuration;
	private bool mIntervened = false;
	private int mDemandId;

	private class GodIntervention : SacrificeResult {

		private Drought mDrought;

		public GodIntervention(Drought drought) : base("Divine Intervention", "Rain") {
			mDrought = drought;
		}

		public override void DoEffect() {
			mDrought.mIntervened = true;
		}
	}

	public override float Warn() {
		return(0);
	}

	public override void Start () {
		GameState.Drought = true;
		mDuration = 60;
		mIntervened = false;

		Utilities.LogEvent("A drought has befallen your farmland");
		mDemandId = Utilities.GetGod().AddDemand(new GodIntervention(this), null, "God offers RAIN in exchange for ");
	}

	public override bool Update () {
		if(mIntervened)
		{
			Utilities.LogEvent("God blessed the rains down in your city. The drought has ended and food production can return to normal.");
			GameState.Drought = false;
			return(true);
		}

		mDuration -= Time.deltaTime;
		if(mDuration <= 0)
		{
			//int cropsLost = Mathf.Min(GameState.FoodSupply, Random.Range(1, 6));
			//Utilities.LogEvent("Your people lost crops to the drought. -" + cropsLost + " food supply");
			//GameState.FoodSupply -= cropsLost;

			Utilities.LogEvent("The drought has ended.");

			if(mDemandId > 0) Utilities.GetGod().RemoveDemand(mDemandId);

			GameState.Drought = false;
			return(true);
		}

		return(false);
	}
}