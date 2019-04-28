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
		
	private float startingHealth = 100;

	private string mName;
	private Attribute[] mAttributes;
	private int mLevel;
	private float mAge;
	private float mXp = 0f;

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
	public int Level 
	{
		get { return(mLevel); }
	}
	public int Age
	{
		get { return((int)Mathf.Floor(mAge)); }
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
		mLevel = 1;
		mAge = 0;

		var attrTypes = new [] {
			AttributeType.PERSONALITY,
			AttributeType.HEIGHT,
			AttributeType.STRENGTH,
			AttributeType.EYE_COLOR,
		};
		int numRandomAttributes = Random.Range(1,4);
		AttributeType[] randomAttributes = Utilities.RandomSubset(attrTypes, numRandomAttributes);

		mAttributes = new Attribute[numRandomAttributes + 1];
		mAttributes[numRandomAttributes] = Utilities.GetRandomAttr(AttributeType.PROFESSION);
		for(int i = 0; i < numRandomAttributes; ++i)
		{
			mAttributes[i] = Utilities.GetRandomAttr(randomAttributes[i]);
		}

		mIsHungry = false;
		mHealth = startingHealth;
		mBaseHealthDecayRate = Random.Range(0.45f, 0.55f);
		if(GameState.ImprovedLifespan1) mBaseHealthDecayRate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		mAge += 0.15f * Time.deltaTime;

		float healthDecayRate = mBaseHealthDecayRate;
		if(mIsHungry)
		{
			healthDecayRate *= 5;
		}
		mHealth -= healthDecayRate * Time.deltaTime;
		if (GetAttribute(AttributeType.PROFESSION) != Attribute.NONE) {
			mXp += Time.deltaTime; // 1 xp per second
		}

		if(mHealth <= 0)
		{
			string deathMsg = mName + " has died at the age of " + Age + ". Their lifeforce returns to the earth.";
			Debug.Log(deathMsg);
			Utilities.LogEvent(deathMsg);
			Utilities.GetPersonManager().RemovePerson(this);
		}
		float xpToLevelUp = 3f;
		if (mXp>= xpToLevelUp) {
			mXp -= xpToLevelUp;
			mLevel += 1;
		}
	}

	// Returns a list of strings to be used in top-left, top-right, bottom-left, bottom-right order
	public string[] GetUIDescription()
	{
		string attrString = "";
		for(int i = 0; i < mAttributes.Length; ++i) {
			if (mAttributes[i].GetAttrType() != AttributeType.PROFESSION) {
				attrString += System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]) + ", ";
			}
		}
		if(mIsHungry) { attrString += "  HUNGRY!"; }
		string profString = "Lv " + mLevel + " " + GetAttribute(AttributeType.PROFESSION).ToString();
		string lifeString = "Lifeforce: " + Mathf.Ceil(mHealth);
		return new [] {mName + " (" + Age + ")", attrString, profString, lifeString};
	}

	public void DebugPrint() {
		
		Debug.Log(mName + " has: ");
		for(int i = 0; i < mAttributes.Length; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]));
		}
	}

}

