using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert {

	// desert is prone to droughts, but flat land means you can see enemies coming from far away

	// one side of mountain tends to be dry. other side tends to be lush

	// rivers can flood and give better food production

	// jungles have exotic herbs that are better for medicine, but have dangerous wild animals

	// swamps are dirty and risk plagues

	RandomEventSystem.RandomEvent GetEvent()
	{
		return new Drought();	
	}

	int GetFarmland();
	int GetPopulationCap();


}
