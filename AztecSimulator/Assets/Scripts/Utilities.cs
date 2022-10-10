using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities {

    public const float SHORT_LOG_DURATION = 1.4f;
    public const float MEDIUM_LOG_DURATION = 2f;
    public const float LONG_LOG_DURATION = 2.8f;

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

    public static Transform GetCanvas()
    {
        return GameObject.FindGameObjectWithTag("UIManager").transform;
    }

    public static UIManager GetUIManager()
	{
		// TODO: optimize this function for frequent calls by storing uiManager in a variable
		// TODO: handle no-ui-manager edge case in all functions that call this
		GameObject uiManager = GameObject.FindGameObjectWithTag("UIManager");
		return uiManager.GetComponent<UIManager>();
	}

	public static TimingManager GetTimingManager()
	{
		return GameObject.FindGameObjectWithTag("SystemsAndManagers").GetComponent<TimingManager>();
	}

	public static void SetGamePaused(bool paused, bool disablePauseButton = false)
	{
		GetTimingManager().SetPaused(paused);
		if(disablePauseButton)
		{
			GetUIManager().SetPauseButtonEnabled(false);
		}
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

    public static int GetIndexOfClosestChild(Transform t, Vector2 position)
    {
        int closestIndex = 0; // or t.childCount - 1;
        float minDD = Mathf.Infinity;
        // Ideally this would just look at t.GetChild(i).position, but with layouts position can be incorrect for just-added elements
        // So I assume a grid layout and do math based on the number of children.
        // Assumptions: children are centered, top left first, and flexible num columns
        GridLayoutGroup g = t.GetComponent<GridLayoutGroup>();
        float cellW = g.cellSize.x + g.spacing.x;
        float cellH = g.cellSize.y + g.spacing.y;
        // Unity centers the row when it is the only row, and left aligns the row when there are multiple rows.
        int numPerRow = Mathf.Min(t.childCount, Mathf.FloorToInt(t.GetComponent<RectTransform>().rect.width / cellW));
        int numRows = Mathf.FloorToInt((t.childCount + numPerRow - 1) / numPerRow);
        for (int i = 0; i < t.childCount; i++)
        {
            int row = Mathf.FloorToInt(i / numPerRow);
            // 0 of 4 -> -1.5;  1 of 4 -> -0.5;  2 of 4 -> 0.5;  3 or 4 -> 1.5
            float xOffset = cellW * ((i%numPerRow) - ((numPerRow - 1)/2f));
            float yOffset = - cellH * (row - ((numRows - 1) / 2f));
            Vector2 childPos = (Vector2)t.position + new Vector2(xOffset, yOffset);
            float DD = (position - childPos).sqrMagnitude;
            if (DD < minDD)
            {
                minDD = DD;
                closestIndex = i;
            }
        }
        return closestIndex;
    }
}
