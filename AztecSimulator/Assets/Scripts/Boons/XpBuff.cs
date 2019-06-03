using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBuff : SacrificeResult {

	private PersonAttribute mProfession;

	public XpBuff(PersonAttribute profession) : base("", "")
	{
		mProfession = profession;
        string professionString = profession.CapitalizedString();
        mName = string.Format("Level {0} {1}s", GameState.GetLevelCap(mProfession)+1, professionString);
        mDescription = professionString + "s gain xp faster. +1 max level.";
        mIcon = profession.ToString();
    }

	public override void DoEffect()
	{
		GameState.AddXpBuff(mProfession, 1);
		GameState.IncreaseLevelCap(mProfession);
	}

}
