using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameProfessionXpBuff : SacrificeResult {

	private int mRequirement;
	private int mBonus;

	public SameProfessionXpBuff() : base("Group Learning", "") {
		mRequirement = Random.Range(3, 6);
		mBonus = 1;
		mDescription = "As long as you have " + mRequirement + " or more people of the same profession, they get " + mBonus + " bonus xp";
	}

	public override void DoEffect()
	{
		GameState.SetBoon(BoonType.SAME_PROFESSION_XP_BONUS, mBonus);
		GameState.SetBoon(BoonType.SAME_PROFESSION_XP_REQ, mRequirement);
	}
}
