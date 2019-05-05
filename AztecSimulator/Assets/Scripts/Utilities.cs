using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities {

	// Extenstion method. called with attr.GetAttrType()
	public static Person.AttributeType GetAttrType(this Person.Attribute attr)
	{
		switch(attr){
		case Person.Attribute.FARMER:
		case Person.Attribute.WARRIOR:
		case Person.Attribute.CIVILIAN:
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
			return "Provides enough food to feed 1 person, and 0.5 more per level.";
		case Person.Attribute.WARRIOR:
			return "Increases army strength by 1, and 0.5 more per level.";
		case Person.Attribute.CIVILIAN:
			return "Raises children, increasing the birth rate, and 1/3 more per level.";
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

	public static SpriteManager GetSpriteManager()
	{
		// TODO: optimize this function for frequent calls by storing spriteManager in a variable
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<SpriteManager>();
	}

	public static PersonManager GetPersonManager()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<PersonManager>();
	}

	public static God GetGod()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<God>();
	}

	public static UIManager GetUIManager()
	{
		// TODO: optimize this function for frequent calls by storing uiManager in a variable
		// TODO: handle no-ui-manager edge case in all functions that call this
		GameObject uiManager = GameObject.FindGameObjectWithTag("UIManager");
		return uiManager.GetComponent<UIManager>();
	}

	public static GodDemand GetSelectedDemand()
	{
		// TODO: optimize this function for frequent calls by having UIManager memoize
		return GetUIManager().getSelectedDemand();
	}

	public static List<Person> GetSelectedPeople()
	{
		// TODO: optimize this function for frequent calls by having UIManager memoize
		return GetUIManager().getSelectedPeople();
	}

	public static RandomEventSystem GetEventSystem()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<RandomEventSystem>();
	}

	public static Image GetBackground()
	{
		return GameObject.FindGameObjectWithTag("Background").GetComponent<Image>();
	}

	public static void LogEvent(string message, float duration=2f, bool isGod=false) {
		GetUIManager().LogEvent(message, duration, isGod);
	}

	public static int[] RandomList(int totalPossibilities, int numChoices) {
		numChoices = Mathf.Min(totalPossibilities, numChoices);
		int[] availableChoices = new int[totalPossibilities];
		for(int i = 0; i < totalPossibilities; ++i) {
			availableChoices[i] = i;
		}

		int[] chosenIndices = new int[numChoices];
		for(int i = 0; i < numChoices; ++i) {
			int pick = Random.Range(0, totalPossibilities);
			chosenIndices[i] = availableChoices[pick];
			availableChoices[pick] = availableChoices[--totalPossibilities];
		}
		return(chosenIndices);
	}

	public static T RandomSelection<T>(T[] possibleValues) {
		return(possibleValues[Random.Range(0, possibleValues.Length)]);
	}

	public static T[] RandomSubset<T>(T[] possibleValues, int numToChoose) {
		numToChoose = Mathf.Min(possibleValues.Length, numToChoose);
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
