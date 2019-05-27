using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBuff : SacrificeResult {

	private Person.Attribute mProfession;

	public XpBuff(Person.Attribute profession, string professionString) : base("", "")
	{
		mProfession = profession;
        mName = "Level " + (GameState.GetLevelCap(mProfession)+1) + " " + professionString;
        mDescription = professionString + " gain xp faster. +1 max level.";

    }

	public override void DoEffect()
	{
		GameState.AddXpBuff(mProfession, 1);
		GameState.IncreaseLevelCap(mProfession);
	}

}
