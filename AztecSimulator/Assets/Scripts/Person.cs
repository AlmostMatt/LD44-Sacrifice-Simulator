using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {
	// Utility functions to go from attr > type are in Utilities
	public enum AttributeType {
		PROFESSION = 0,
		HEIGHT,
		EYE_COLOR,
		STRENGTH,
		PERSONALITY,
		NONE
	}
	public enum Attribute {
		FARMER = 0,
		WARRIOR,
		TALL,
		SHORT,
		BLUE_EYES,
		GREEN_EYES,
		BROWN_EYES,
		STRONG,
		WEAK,
		SMART,
		CARING,
		STUPID,
		// When picking a random attr, subtract max value by 1 to not select the following
		NONE
	}
	// TODO: group names by gender
	private static string[] NAMES = {
		"Nathan",
		"Matt",
		"Brendan",
		"Coyotl",
		"Coatl",
		"Mazatl",
		"Tototl",
		"Zolin",
		"Nochtli",
		"Aapo",
		"Sachihiro",
		"Cipactli",
		"Itzel",
		"Akna",
		"Nina",
		"Xoc",
		"Kusi",
		"Koya",
		"Kayara",
		"Manko",
		"Atik"
	};

	// todo: for this to be data-driven, we'll need an actual Person prefab with this as the component...
	public float startingHealth = 100;

	private string mName;
	private Attribute[] mAttributes;

	private float mHealth;
	private float mBaseHealthDecayRate;

	public string Name {
		get { return(mName); } 
	}
	public Attribute[] Attributes {
		get { return(mAttributes); }
	}
	public float Health
	{
		get { return(mHealth); }
		set { mHealth = value; }
	}


	private bool mIsHungry;
	public bool Hungry
	{
		get { return(mIsHungry); }
		set { mIsHungry = value; }
	}

	public Attribute GetAttribute(Person.AttributeType attrType) {
		foreach (Attribute attr in mAttributes) {
			if (attr.GetAttrType() == attrType) {
				return attr;
			}
		}
		return Attribute.NONE;
	}

	// Use this for initialization
	//void Start () {
	void Awake() {
		mName = NAMES[UnityEngine.Random.Range(0, NAMES.Length)];

		int numAttributes = Random.Range(1,4);
		mAttributes = new Attribute[numAttributes];

		var numAttrs = System.Enum.GetValues(typeof(Attribute)).Length-1;
		int[] attributeSelection = Utilities.RandomList(numAttrs, numAttributes);
		for(int i = 0; i < mAttributes.Length; ++i)
		{
			mAttributes[i] = (Attribute)attributeSelection[i];
		}

		mIsHungry = false;
		mHealth = startingHealth;
		mBaseHealthDecayRate = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {

		float healthDecayRate = mBaseHealthDecayRate;
		if(mIsHungry)
		{
			healthDecayRate *= 5;
		}
		mHealth -= healthDecayRate * Time.deltaTime;

		if(mHealth <= 0)
		{
			string deathMsg = mName + " has passed. Their lifeforce returns to the earth.";
			Debug.Log(deathMsg);
			Utilities.LogEvent(deathMsg);
			Utilities.GetPersonManager().RemovePerson(this);
		}
	}

	public string GetUIDescription()
	{
		string desc = mName + " - ";
		for(int i = 0; i < mAttributes.Length; ++i) {
			desc += System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]) + ", ";
		}
		desc += "\r\nLifeforce: " + Mathf.Ceil(mHealth);
		if(mIsHungry) desc += "  HUNGRY!";
		return(desc);
	}

	public void DebugPrint() {
		
		Debug.Log(mName + " has: ");
		for(int i = 0; i < mAttributes.Length; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]));
		}
	}

}

