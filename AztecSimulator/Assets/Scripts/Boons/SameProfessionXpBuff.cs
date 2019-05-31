using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameProfessionXpBuff : SacrificeResult {

	public class Factory : SacrificeResultFactory {
		public SacrificeResult Make(int tier, int luck) { 
			return new SameProfessionXpBuff(tier, luck);
		}
	}

	private int mRequirement;
	private int mBonus;

	private SameProfessionXpBuff(int tier, int luck) : base("Group Learning", "", "+XP") {
		mRequirement = Random.Range(3, Mathf.Max(3, 6 - luck));
		mBonus = 1 * (1 + tier + luck);
		mDescription = "As long as you have " + mRequirement + " or more people of the same profession, they get +" + mBonus + "xp per second";
	}

	public override void DoEffect()
	{
		GameState.SetBoon(BoonType.SAME_PROFESSION_XP_BONUS, mBonus);
		GameState.SetBoon(BoonType.SAME_PROFESSION_XP_REQ, mRequirement);
	}
}
