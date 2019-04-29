using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianXpBuff : XpBuff {

	public CivilianXpBuff() : base(Person.Attribute.CIVILIAN, "Better Civilians", "Civilians gain xp faster. Level cap +1") {
	}

}
