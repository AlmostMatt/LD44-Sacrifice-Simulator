using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeBonus : SacrificeResult {
	
	private int mAmount = 0;
	private int mWhichBonus;
	private BoonType mBoonType;

	public SacrificeBonus() : base("", "") {
		mWhichBonus = Random.Range(0, 2);
		switch(mWhichBonus)
		{
		case 0:
			mBoonType = BoonType.SACRIFICE_BONUS_XP;
			mAmount = 10;
			mName = "Knowledge Inheritance";
			mDescription = "Each sacrifice gives " + mAmount + " xp to a random person";
			break;
		case 1:
			mBoonType = BoonType.SACRIFICE_BONUS_HEALING;
			mAmount = 10;
			mName = "Knowledge Inheritance";
			mDescription = "Each sacrifice heals a random person for " + mAmount;
			break;
		}
	}

	public override void DoEffect()
	{
		GameState.SetBoon(mBoonType, mAmount);
	}
}
