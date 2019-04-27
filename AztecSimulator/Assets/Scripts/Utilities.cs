using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

	// Extenstion method. called with attr.GetAttrType()
	public static Person.AttributeType GetAttrType(this Person.Attribute attr)
	{
		switch(attr){
		case Person.Attribute.FARMER:
		case Person.Attribute.WARRIOR:
			return Person.AttributeType.PROFESSION;
		case Person.Attribute.TALL:
		case Person.Attribute.SHORT:
			return Person.AttributeType.HEIGHT;
		case Person.Attribute.BLUE_EYES:
		case Person.Attribute.GREEN_EYES:
		case Person.Attribute.BROWN_EYES:
			return Person.AttributeType.EYE_COLOR;
		case Person.Attribute.STRONG:
		case Person.Attribute.WEAK:
			return Person.AttributeType.STRENGTH;
		case Person.Attribute.SMART:
		case Person.Attribute.CARING:
		case Person.Attribute.STUPID:
			return Person.AttributeType.PERSONALITY;
		default:
			return Person.AttributeType.NONE;
		}
	}

	public static PersonManager GetPersonManager()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<PersonManager>();
	}

	public static God GetGod()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<God>();
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
