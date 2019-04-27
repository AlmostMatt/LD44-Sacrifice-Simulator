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

	public float startingHealth = 30; // todo: for this to be data-driven, we'll need an actual Person prefab with this as the component...

	private string mName;
	private Attribute[] mAttributes;

	private float mHealth;
	private float mHealthDecayRate;

	public string Name {
		get { return(mName); } 
	}
	public Attribute[] Attributes {
		get { return(mAttributes); }
	}

	// Use this for initialization
	//void Start () {
	void Awake() {
		mName = NAMES[UnityEngine.Random.Range(0, NAMES.Length)];

		int numAttributes = Random.Range(1,3);
		mAttributes = new Attribute[numAttributes];

		int[] attributeSelection = Utilities.RandomList((int)Attribute.MAX_VALUE, numAttributes);
		for(int i = 0; i < mAttributes.Length; ++i)
		{
			mAttributes[i] = (Attribute)attributeSelection[i];
		}

		mHealth = startingHealth;
		mHealthDecayRate = Random.Range(0.5f, 1.5f);

		// DebugPrint();
	}
	
	// Update is called once per frame
	void Update () {

		mHealth -= mHealthDecayRate * Time.deltaTime;
		if(mHealth <= 0)
		{
			Debug.Log(mName + " died of old age!");
			Utilities.GetPersonManager().RemovePerson(this);
		}
	}
		
	public string GetUIDescription()
	{
		string desc = mName + " - ";
		for(int i = 0; i < mAttributes.Length; ++i) {
			desc += System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]) + ", ";
		}
		return(desc);
	}

	public void DebugPrint() {
		
		Debug.Log(mName + " has: ");
		for(int i = 0; i < mAttributes.Length; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]));
		}
	}

}

