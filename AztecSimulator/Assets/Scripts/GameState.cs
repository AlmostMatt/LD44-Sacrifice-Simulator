﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

	// dumping ground for persistent state

	private static GameState sGameState = new GameState();

	private float mGameDeltaTime;
	public static float GameDeltaTime
	{
		get { return(sGameState.mGameDeltaTime); }
		set { sGameState.mGameDeltaTime = value; } // ONLY TimingManager is allowed to edit this
	}

	private float mGameSpeed = 1;
	public static float GameSpeed
	{
		get { return(sGameState.mGameSpeed); }
		set { sGameState.mGameSpeed = value; }
	}

	private int mFoodSupply = 0;
	public static int FoodSupply // wow I didn't think this would actually work
	{
		get { return(sGameState.mFoodSupply); }
		set { sGameState.mFoodSupply = value; }
	}

	private int mArmySize = 0;
	public static int ArmySize // wow I didn't think this would actually work
	{
		get { return(sGameState.mArmySize); }
		set { sGameState.mArmySize = value; }
	}

	private int mInvaderSize = 0;
	public static int InvaderSize // wow I didn't think this would actually work
	{
		get { return(sGameState.mInvaderSize); }
		set { sGameState.mInvaderSize = value; }
	}

	private bool mDrought = false;
	public static bool Drought 
	{
		get { return(sGameState.mDrought); }
		set { sGameState.mDrought = value; }
	}

	private bool mImprovedLifespan1 = false;
	public static bool ImprovedLifespan1
	{
		get { return(sGameState.mImprovedLifespan1); }
		set { sGameState.mImprovedLifespan1 = value; }
	}

	private Dictionary<Person.Attribute, List<int>> mXpBuffs = new Dictionary<Person.Attribute, List<int>>();
	public static void AddXpBuff(Person.Attribute profession, int multiplier)
	{
		if(!sGameState.mXpBuffs.ContainsKey(profession))
			sGameState.mXpBuffs.Add(profession, new List<int>());
		
		List<int> buffs = sGameState.mXpBuffs[profession];
		buffs.Add(multiplier);
	}
	public static int GetBuffedXp(Person.Attribute profession, int baseValue)
	{
		if(!sGameState.mXpBuffs.ContainsKey(profession))
			return(baseValue);
		
		foreach(int mult in sGameState.mXpBuffs[profession])
		{
			baseValue *= mult;
		}
		return(baseValue);
	}

	private int mFavour = 0;
	public static int Favour
	{
		get { return(sGameState.mFavour); }
		set { sGameState.mFavour = value; }
	}

	private float mFertilityBonus = 0;
	public static float FertilityBonus
	{
		get { return(sGameState.mFertilityBonus); }
		set { sGameState.mFertilityBonus = value; }
	}



}
