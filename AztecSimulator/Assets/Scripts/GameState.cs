using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {

	// dumping ground for persistent state

	private static GameState sGameState = new GameState();

	public static void NewGame()
	{
		sGameState = new GameState();
	}

	private float mGameDeltaTime;
	public static float GameDeltaTime
	{
		get { return(sGameState.mGameDeltaTime); }
		set { sGameState.mGameDeltaTime = value; } // ONLY TimingManager is allowed to edit this
	}

	private float mGameTimeElapsed;
	public static float GameTimeElapsed
	{
		get { return(sGameState.mGameTimeElapsed); }
		set { sGameState.mGameTimeElapsed = value; } // ONLY TimingManager is allowed to edit this
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

	private float mFoodSurplus = 0;
	public static float FoodSurplus
	{
		get {return(sGameState.mFoodSurplus); }
		set { sGameState.mFoodSurplus = value; }
	}

	private int mArmyStrength = 0;
	public static int ArmyStrength
	{
		get { return(sGameState.mArmyStrength); }
		set { sGameState.mArmyStrength = value; }
	}

	private int mInvaderSize = 0;
	public static int InvaderSize
	{
		get { return(sGameState.mInvaderSize); }
		set { sGameState.mInvaderSize = value; }
	}

	// Time between births in seconds (ignores population limit, can be infinity)
	private float mTimeBetweenBirths = 0;
	public static float TimeBetweenBirths
	{
		get { return(sGameState.mTimeBetweenBirths); }
		set { sGameState.mTimeBetweenBirths = value; }
	} // 
	// Time until next birth in seconds (ignores population limit, can be infinity)
	private float mTimeUntilBirth = 0;
	public static float TimeUntilBirth
	{
		get { return(sGameState.mTimeUntilBirth); }
		set { sGameState.mTimeUntilBirth = value; }
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

	private Dictionary<Person.Attribute, int> mLevelCapIncreases = new Dictionary<Person.Attribute, int>();
	public static void IncreaseLevelCap(Person.Attribute profession)
	{
		if(!sGameState.mLevelCapIncreases.ContainsKey(profession))
			sGameState.mLevelCapIncreases.Add(profession, 0);

		sGameState.mLevelCapIncreases[profession]++;
	}
	public static int GetLevelCap(Person.Attribute profession)
	{
		int initialLevelCap = 3;
		if(!sGameState.mLevelCapIncreases.ContainsKey(profession))
			return(initialLevelCap);
		return(sGameState.mLevelCapIncreases[profession] + initialLevelCap);
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

	private int[] mBoons = new int[(int)BoonType.MAX_VALUE];
	public static bool HasBoon(BoonType boon) {
		return(sGameState.mBoons[(int)boon] > 0);
	}
	public static void SetBoon(BoonType boon, bool has) {
		sGameState.mBoons[(int)boon] = has ? 1 : 0;
	}
	public static void SetBoon(BoonType boon, int value) {
		sGameState.mBoons[(int)boon] = value;
	}
	public static int GetBoonValue(BoonType boon) {
		return(sGameState.mBoons[(int)boon]);
	}

	private List<Ongoing> mOngoings = new List<Ongoing>();
	public static List<Ongoing> Ongoings
	{
		get { return(sGameState.mOngoings); }
	}

	private IScenario mScenario;
	public static IScenario Scenario
	{
		get { return(sGameState.mScenario); }
		set { sGameState.mScenario = value; }
	}
}
