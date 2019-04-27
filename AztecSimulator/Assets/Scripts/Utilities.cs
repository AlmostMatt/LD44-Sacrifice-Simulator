using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

	public static PersonManager GetPersonManager()
	{
		return((PersonManager)GameObject.FindGameObjectWithTag("PersonManager").GetComponent<PersonManager>());
	}

	public static int[] RandomList(int totalPossibilities, int numChoices) {
		int[] availableChoices = new int[totalPossibilities];
		for(int i = 0; i < totalPossibilities; ++i) {
			availableChoices[i] = i;
		}

		int[] chosenIndices = new int[numChoices];
		for(int i = 0; i < numChoices; ++i) {
			int pick = Random.Range(i, totalPossibilities);
			chosenIndices[i] = availableChoices[pick];
			availableChoices[pick] = availableChoices[--totalPossibilities];
		}
		return(chosenIndices);
	}
}
