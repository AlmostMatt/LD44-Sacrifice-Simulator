using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

	public static PersonManager GetPersonManager()
	{
		return GameObject.FindGameObjectWithTag("PersonManager").GetComponent<PersonManager>();
	}

	public static God GetGod()
	{
		return GameObject.FindGameObjectWithTag("God").GetComponent<God>();
	}

	public static void LogEvent(string message)
	{
		GameObject uiManager = GameObject.FindGameObjectWithTag("UIManager");
		if (uiManager != null) { uiManager.GetComponent<UIManager>().LogEvent(message); }
		else { Debug.Log("Logged event: " + message); }
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

	// Joins a list of strings with commas and AND.
	public static string ConcatStrings(List<string> strings, bool useAllCaps = false) {
		string result = "";
		for (int i = 0; i < strings.Count; i++) {
			result += strings[i];
			if (i == strings.Count - 2) { result += useAllCaps ? " AND " : " and "; }
			else if (i < strings.Count - 2) { result += ", "; }
		}
		return result;
	}
}
