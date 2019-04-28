using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonLibrary {

	private static SacrificeResult[] sTemporaryBoons = {
		new GoodCropBoon(),
		new FertilityBoon()
		//new VitalityBoon(),
		//new StudiousBoon()
	};

	private static SacrificeResult[] sTierOneBoons = {
		new FarmerXpBuff(),
		new ImprovedLifespan(),
		new WarriorXpBuff()
	};

	public static SacrificeResult RandomTierOneBoon()
	{
		return(sTierOneBoons[Random.Range(0, sTierOneBoons.Length)]);
	}

	public static SacrificeResult[] RandomTierOneBoons(int howMany)
	{
		SacrificeResult[] boons = Utilities.RandomSubset<SacrificeResult>(sTierOneBoons, howMany);
		return(boons);
	}

	public static SacrificeResult RandomTemporaryBoon()
	{
		return(sTemporaryBoons[Random.Range(0, sTemporaryBoons.Length)]);
	}

}
