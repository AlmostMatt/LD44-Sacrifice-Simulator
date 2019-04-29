using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerXpBuff : XpBuff {

	public FarmerXpBuff() : base(Person.Attribute.FARMER, "Better Farmers", "Farmers gain xp faster. +1 max level.") {
	}

}
