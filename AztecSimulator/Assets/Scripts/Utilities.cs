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

	public static string GetDescription(this Person.Attribute attr) {
		switch(attr){
		case Person.Attribute.FARMER:
			return "Capable of feeding a number of people equal to level.";
		case Person.Attribute.WARRIOR:
			return "Capable of fighting off a number of invaders equal to level.";
		default:
			return "<profession description>";
		}
	}

	public static Person.Attribute[] GetAttrValues(Person.AttributeType attrType) {
		return System.Array.FindAll<Person.Attribute>((Person.Attribute[])System.Enum.GetValues(typeof(Person.Attribute)), attr => GetAttrType(attr) == attrType);
	}

	public static Person.Attribute GetRandomAttr(Person.AttributeType attrType) {
		Person.Attribute[] possibleValues = GetAttrValues(attrType);
		return possibleValues[Random.Range(0, possibleValues.Length)];
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

	public static T RandomSelection<T>(T[] possibleValues) {
		return(possibleValues[Random.Range(0, possibleValues.Length)]);
	}

	public static T[] RandomSubset<T>(T[] possibleValues, int numToChoose) {
		int[] listOfIndexes = new int[possibleValues.Length];
		for(int i= 0; i< possibleValues.Length; ++i) {
			listOfIndexes[i] = i;
		}
		for(int numChosen = 0; numChosen < numToChoose; ++numChosen) {
			// choose a random index (excluding those that have been moved to the front of the list)
			int randomIndex = Random.Range(numChosen, possibleValues.Length);
			// swap the randomly selected index with an index at the front of the list
			var tmp = listOfIndexes[randomIndex];
			listOfIndexes[randomIndex] = listOfIndexes[numChosen];
			listOfIndexes[numChosen] = tmp;
		}
		T[] result = new T[numToChoose];
		for(int i = 0; i < numToChoose; ++i) {
			result[i] = possibleValues[listOfIndexes[i]];
		}
		return result;
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

	// Conditionally color a string. "color" is a hex stirng like "#00ffffff" or a name like "yellow"
	// https://docs.unity3d.com/Manual/StyledText.html
	public static string ColorString(string text, string color, bool shouldApply) {
		return shouldApply ? "<color=" + color + ">" + text + "</color>" : text;
	}
}
