using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeBonus : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new SacrificeBonus(tier, luck);
		}
	}

	private int mAmount = 0;
	private int mWhichBonus;
	private BoonType mBoonType;

	private SacrificeBonus(int tier, int luck) : base("", "") {
		mWhichBonus = Random.Range(0, 2);
		switch(mWhichBonus)
		{
		case 0:
			mBoonType = BoonType.SACRIFICE_BONUS_XP;
			mAmount = 5 * (1 + tier + luck);
			mName = "Knowledge Inheritance";
			mDescription = "Each sacrifice gives " + mAmount + " xp to a random person";
			break;
		case 1:
			mBoonType = BoonType.SACRIFICE_BONUS_HEALING;
			mAmount = 10 * (1 + tier + luck);
			mName = "Lifeforce Inheritance";
			mDescription = "Each sacrifice restores " + mAmount + " lifeforce to a random person";
			break;
		}
	}

	public override void DoEffect()
	{
		GameState.SetBoon(mBoonType, mAmount);
	}
}
