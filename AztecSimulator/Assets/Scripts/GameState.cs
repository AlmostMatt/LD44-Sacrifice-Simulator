using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

	// dumping ground for persistent state

	private static GameState sGameState = new GameState();

	private bool mDrought = false;
	private int mFoodSupply = 10;
	private bool mImprovedLifespan1 = false;

	public static int FoodSupply // wow I didn't think this would actually work
	{
		get { return(sGameState.mFoodSupply); }
		set { sGameState.mFoodSupply = value; }
	}

	public static bool Drought 
	{
		get { return(sGameState.mDrought); }
		set { sGameState.mDrought = value; }
	}

	public static bool ImprovedLifespan1
	{
		get { return(sGameState.mImprovedLifespan1); }
		set { sGameState.mImprovedLifespan1 = value; }
	}

}
