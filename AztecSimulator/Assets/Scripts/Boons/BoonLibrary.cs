using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonLibrary {

	private static SacrificeResult[] sTemporaryBoons = {
		new GoodCropBoon(),
		new FertilityBoon(),
		new HealingBoon(),
		new Favour()
		//new StudiousBoon()
	};

	private static SacrificeResult[] sTemporaryCurses = {
		new PlagueCurse()
	};

	private static SacrificeResult[] sTierOneBoons = {
		new FarmerXpBuff(),
		new ImprovedLifespan(),
		new WarriorXpBuff(),
		new Favour(),
		new SurplusFoodUse(),
		new SacrificeBonus(),
		new SameProfessionXpBuff(),
		new CombatVictoryReward()
	};

	private static SacrificeResult[] sTierTwoBoons = {
		new FarmerXpBuff(),
		new ImprovedLifespan(),
		new WarriorXpBuff(),
		new Favour(),
		new WarriorChildProtect(),
		new ChangeProfessionRetainXp(),
		new HealthXpBonus()
	};

	public static SacrificeResult RandomTemporaryBoon() {
		return(Utilities.RandomSelection<SacrificeResult>(sTemporaryBoons));
	}

	public static SacrificeResult RandomTemporaryCurse() {
		return(Utilities.RandomSelection<SacrificeResult>(sTemporaryCurses));
	}

	public static SacrificeResult RandomTierOneBoon() {
		return(Utilities.RandomSelection<SacrificeResult>(sTierOneBoons));
	}

	public static SacrificeResult[] RandomTierOneBoons(int howMany)
	{
		return(Utilities.RandomSubset<SacrificeResult>(sTierOneBoons, howMany));
	}

	public static SacrificeResult[] RandomTierTwoBoons(int howMany)
	{
		return(Utilities.RandomSubset<SacrificeResult>(sTierTwoBoons, howMany));
	}
}
