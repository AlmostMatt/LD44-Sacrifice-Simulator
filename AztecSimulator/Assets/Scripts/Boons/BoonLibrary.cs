using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonLibrary {

	private static SacrificeResultFactory[] sTemporaryBoons = {
		new GoodCropBoon.Factory(),
		new FertilityBoon.Factory(),
		new HealingBoon.Factory(),
		new Favour.Factory(),
		new XpBurst.Factory()
	};

	private static SacrificeResult[] sTemporaryCurses = {
		new PlagueCurse()
	};

	public static SacrificeResult[] sGuaranteedRenewableBoons = {
		new FarmerXpBuff(),
		new WarriorXpBuff(),
		new CivilianXpBuff()
	};

	private static SacrificeResultFactory[] sRandomizedBoons = {
		new GoodCropBoon.Factory()
	};

	private static SacrificeResult[] sTierOneBoons = {
		//new FarmerXpBuff(),
		// new ImprovedLifespan(),
//		new WarriorXpBuff(),
		//new Favour(),
		new SurplusFoodUse(),
		new SacrificeBonus(),
		new SameProfessionXpBuff(),
		new CombatVictoryReward()
	};

	private static SacrificeResult[] sTierTwoBoons = {
		//new FarmerXpBuff(),
//		new ImprovedLifespan(),
		//new WarriorXpBuff(),
//		new Favour(),
		new WarriorChildProtect(),
		new ChangeProfessionRetainXp(),
		new HealthXpBonus()
	};

	private static SacrificeResult[] sSuperSpecialBoons = {
	};

	public static SacrificeResult RandomTemporaryBoon(int tier, int luck) {
		return(Utilities.RandomSelection<SacrificeResultFactory>(sTemporaryBoons).Make(tier, luck));
	}

	public static SacrificeResult RandomTemporaryCurse() {
		return(Utilities.RandomSelection<SacrificeResult>(sTemporaryCurses));
	}

	public static SacrificeResult RandomBoon(int tier, int luck) {
		return(Utilities.RandomSelection<SacrificeResultFactory>(sRandomizedBoons).Make(tier, luck));
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
