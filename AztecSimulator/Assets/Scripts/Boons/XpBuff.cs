using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBuff : SacrificeResult {

	private Person.Attribute mProfession;

	public XpBuff(Person.Attribute profession, string name, string description) : base(name, description)
	{
		mProfession = profession;
	}

	public override void DoEffect()
	{
		GameState.AddXpBuff(mProfession, 2);
		GameState.IncreaseLevelCap(mProfession);
	}

}
