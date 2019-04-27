using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public enum Attribute {
		FARMER = 0,
		WARRIOR,
		TALL,
		SHORT,
		BLUE_EYES,
		GREEN_EYES,
		BROWN_EYES,
		MAX_VALUE
	}

	private static string[] NAMES = {
		"Nathan",
		"Matt",
		"Brendan",
		"Joe",
		"Bob"
	};

	private string mName;
	private Attribute[] mAttributes;

	public string Name {
		get { return(mName); } 
	}
	public Attribute[] Attributes {
		get { return(mAttributes); }
	}

	// Use this for initialization
	void Start () {
		mName = NAMES[UnityEngine.Random.Range(0, NAMES.Length)];

		int numAttributes = Random.Range(1,3);
		mAttributes = new Attribute[numAttributes];

		int[] attributeSelection = RandomList((int)Attribute.MAX_VALUE, numAttributes);
		for(int i = 0; i < mAttributes.Length; ++i)
		{
			mAttributes[i] = (Attribute)attributeSelection[i];
		}

		DebugPrint();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	public void DebugPrint() {
		
		Debug.Log(mName + " has: ");
		for(int i = 0; i < mAttributes.Length; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]));
		}
	}

	private int[] RandomList(int totalPossibilities, int numChoices) {
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

