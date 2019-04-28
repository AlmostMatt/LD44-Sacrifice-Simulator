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
		// When adding new professions, update the Utilities maps from attr to type and prof to description. Update the UIManager sprites.
		FARMER = 0,
		WARRIOR,
		CIVILIAN,
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
	public void ResetLevel() {
		mLevel = 1;
		mXp = 0f;
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

		int numRandomAttributes = Random.Range(1,4);
		mAttributes = RandomAttributes(numRandomAttributes);

		// for now: always start as farmer
		mAttributes[mAttributes.Length-1] = Attribute.FARMER;

		mIsHungry = false;
		mHealth = startingHealth;
		//mBaseHealthDecayRate = Random.Range(0.45f, 0.55f);
		mBaseHealthDecayRate = 0;
		if(GameState.ImprovedLifespan1) mBaseHealthDecayRate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		mAge += 0.15f * GameState.GameDeltaTime;

		float healthDecayRate = mBaseHealthDecayRate;
		if(mIsHungry)
		{
			// healthDecayRate *= 5;
			healthDecayRate = 1;
		}
		mHealth -= healthDecayRate * GameState.GameDeltaTime;
		Attribute profession = GetAttribute(AttributeType.PROFESSION);
		if (profession != Attribute.NONE) {
			int xpGain = 1; // 1 xp per second;
			xpGain = GameState.GetBuffedXp(profession, xpGain);
			mXp += xpGain * GameState.GameDeltaTime;
		}

		if(mHealth <= 0)
		{
			string deathMsg = mName + " has died at the age of " + Age + ". Their lifeforce returns to the earth.";
			Debug.Log(deathMsg);
			Utilities.LogEvent(deathMsg);
			Utilities.GetPersonManager().RemovePerson(this);
		}
		float xpToLevelUp = mLevel * 10;
		if (mXp>= xpToLevelUp) {
			mXp -= xpToLevelUp;
			mLevel += 1;
		}
	}

	// Returns a list of strings to be used in top-left, top-right, bottom-left, bottom-right order in a list of people
	// Icons (profession and health) are added by the UI Manager
	public string[] GetUIDescription(SacrificeDemand selectedDemand)
	{
		string attrString = "";
		for(int i = 0; i < mAttributes.Length; ++i) {
			if (mAttributes[i].GetAttrType() != AttributeType.PROFESSION) {
				string attr = System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]);
				bool isRelevant = selectedDemand != null && selectedDemand.IsRelevantAttribute(mAttributes[i]);
				attrString += Utilities.ColorString(attr, "green", isRelevant) + ", ";
			}
		}
		if(mIsHungry) { attrString += "  HUNGRY!"; }
		bool isLevelRelevant = selectedDemand != null && selectedDemand.IsRelevantLevel(mLevel);
		string levelString = "Lvl " + Utilities.ColorString(mLevel.ToString(), "green", isLevelRelevant) + " ";
		string lifeString = " " + Mathf.Ceil(mHealth);
		return new [] {mName + " (Age " + Age + ")", attrString, levelString, lifeString};
	}

	// Returns a single multiline string
	public string GetLongUIDescription()
	{
		string result = "Name: " + mName + "\r\n";
		result += "Level: " + mLevel + "\r\n";
		result += "Profession: " + GetAttribute(AttributeType.PROFESSION).ToString() + "\r\n";
		result += "Age: " + Age + "\r\n";
		result += "Lifeforce: " + Mathf.Ceil(mHealth);
		// todo: attributes
		return result;
	}

	public void DebugPrint() {
		
		Debug.Log(mName + " has: ");
		for(int i = 0; i < mAttributes.Length; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)mAttributes[i]));
		}
	}

	public static Attribute[] RandomAttributes(int howMany)
	{
		AttributeType[] attrTypes = {
			AttributeType.PERSONALITY,
			AttributeType.HEIGHT,
			AttributeType.STRENGTH,
			AttributeType.EYE_COLOR,
		};

		AttributeType[] randomAttributes = Utilities.RandomSubset(attrTypes, howMany);

		Attribute[] attributes = new Attribute[howMany + 1];
		attributes[howMany] = Utilities.GetRandomAttr(AttributeType.PROFESSION);
		for(int i = 0; i < howMany; ++i)
		{
			attributes[i] = Utilities.GetRandomAttr(randomAttributes[i]);
		}

		return(attributes);
	}
}

