using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonLibrary {

	private static SacrificeResult[] sTierOneBoons = {
		new FarmerXpBuff(),
		new ImprovedLifespan(),
		new WarriorXpBuff()
	};

	public static SacrificeResult[] RandomTierOneBoons(int howMany)
	{
		SacrificeResult[] boons = Utilities.RandomSubset<SacrificeResult>(sTierOneBoons, howMany);
		return(boons);
	}

}
