using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

	private static GameState sGameState = new GameState();

	private int mFoodSupply = 10;

	public static int FoodSupply // wow I didn't think this would actually work
	{
		get { return(sGameState.mFoodSupply); }
		set { sGameState.mFoodSupply = value; }
	}


}
