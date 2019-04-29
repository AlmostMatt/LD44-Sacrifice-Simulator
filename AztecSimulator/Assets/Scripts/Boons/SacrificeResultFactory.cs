using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SacrificeResultFactory {
	
	SacrificeResult Make(int tier, int luck);

}
