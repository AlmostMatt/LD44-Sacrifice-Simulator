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
		new CivilianXpBuff(),
	};

	private static SacrificeResultFactory[] sRandomizedBoons = {
		new Favour.Factory(),
		new SurplusFoodUse.Factory(),
		new SacrificeBonus.Factory(),
		new SameProfessionXpBuff.Factory(),
		new CombatVictoryReward.Factory(),
		new WarriorChildProtect.Factory(),
		new ChangeProfessionRetainXp.Factory(),
		new HealthXpBonus.Factory()
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

	public static SacrificeResultFactory[] RandomBoonFactories(int howMany) {
		return Utilities.RandomSubset<SacrificeResultFactory>(sRandomizedBoons, howMany);
	}

	public static SacrificeResult[] RandomBoons(int howMany, int tier, int luck)
	{
		SacrificeResultFactory[] factories = Utilities.RandomSubset<SacrificeResultFactory>(sRandomizedBoons, howMany);
		SacrificeResult[] results = new SacrificeResult[factories.Length];
		for(int i = 0; i < factories.Length; ++i)
		{
			results[i] = factories[i].Make(tier, luck);
		}
		return results;
	}

}
