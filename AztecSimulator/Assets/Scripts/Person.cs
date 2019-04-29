using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {
	private static Dictionary<Person.Attribute, float> EFFICIENCY_BASE = new Dictionary<Person.Attribute, float> {
		{Person.Attribute.FARMER, 1f},
		{Person.Attribute.WARRIOR, 1f},
		{Person.Attribute.CIVILIAN, 1.5f},
		{Person.Attribute.NONE, 0f}
	};
	private static Dictionary<Person.Attribute, float> EFFICIENCY_PER_LEVEL = new Dictionary<Person.Attribute, float> {
		{Person.Attribute.FARMER, 0.5f},
		{Person.Attribute.WARRIOR, 0.5f},
		{Person.Attribute.CIVILIAN, 0.5f},
		{Person.Attribute.NONE, 0f}
	};

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
	private float mMaxHealth;
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
	public float MaxHealth
	{
		get { return(mMaxHealth); }
	}
	public int Level 
	{
		get { return(mLevel); }
	}
	public void ResetLevel() {
		mLevel = 1;
		mXp = 0f;
	}
	public float Efficiency {
		get { return(
			EFFICIENCY_BASE[GetAttribute(AttributeType.PROFESSION)]
			+ (EFFICIENCY_PER_LEVEL[GetAttribute(AttributeType.PROFESSION)]) * (mLevel - 1));
		}
	}

	public void ChangeProfession(Attribute newProfession)
	{
		// note: this could rely on the fact that profession happens to be the last attribute
		for (int i =0; i < mAttributes.Length; i++) {
			if (mAttributes[i].GetAttrType() == Person.AttributeType.PROFESSION) {
				mAttributes[i] = newProfession;
			}
		}

		int xpRetention = GameState.GetBoonValue(BoonType.PROFESSION_CHANGE_RETAIN_XP);
		if(xpRetention > 0)
			mXp = (xpRetention / 100f) * mXp;
		else
			mXp = 0;
		
		mLevel = GetLevelForXp(mXp);
	}

	// This should be called after the person has already been created and Awake completed.
	public void OverrideRandomValues(Attribute profession = Attribute.NONE, int level = -1) {
		if (profession != Attribute.NONE) {
			for (int i=0; i< mAttributes.Length; i++) {
				if (mAttributes[i].GetAttrType() == AttributeType.PROFESSION) {
					mAttributes[i] = profession;
				}
			}
		}
		if (level != -1) {
			mLevel = level;
			mXp = level * (level - 1) * 5; // this is the current formula for total xp, based on required xp to level L = L * 10
		}
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

	public void AddXp(float amount)
	{
		// maybe this wants to be smarter about caps, idk
		mXp += amount;
	}

	public void Heal(float amount)
	{
		mHealth += amount;
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
		mMaxHealth = mHealth = startingHealth;
		//mBaseHealthDecayRate = Random.Range(0.45f, 0.55f);
		mBaseHealthDecayRate = 0;
		if(GameState.ImprovedLifespan1) mBaseHealthDecayRate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		mAge += 0.15f * GameState.GameDeltaTime;

		Attribute profession = GetAttribute(AttributeType.PROFESSION);
		if(mLevel < GameState.GetLevelCap(profession))
		{
			if (profession != Attribute.NONE) {
				int xpGain = 1; // 1 xp per second;
				xpGain = GameState.GetBuffedXp(profession, xpGain);

				if(GameState.HasBoon(BoonType.SAME_PROFESSION_XP_BONUS))
				{
					List<Person> sameProfession = Utilities.GetPersonManager().FindPeople(AttributeType.PROFESSION, profession);
					if(sameProfession.Count >= GameState.GetBoonValue(BoonType.SAME_PROFESSION_XP_REQ))
					{
						xpGain += GameState.GetBoonValue(BoonType.SAME_PROFESSION_XP_BONUS);
					}
				}

				int bonusXpHealthThreshold = GameState.GetBoonValue(BoonType.HEALTHY_BONUS_XP_THRESHOLD);
				if(bonusXpHealthThreshold > 0 && mHealth >= bonusXpHealthThreshold)
				{
					xpGain += GameState.GetBoonValue(BoonType.HEALTHY_BONUS_XP);
				}

				int bonusXpUnhealthyThreshold = GameState.GetBoonValue(BoonType.UNHEALTHY_BONUS_XP_THRESHOLD);
				if(bonusXpUnhealthyThreshold > 0 && mHealth <= bonusXpUnhealthyThreshold)
				{
					xpGain += GameState.GetBoonValue(BoonType.UNHEALTHY_BONUS_XP);
				}

				mXp += xpGain * GameState.GameDeltaTime;
			}

			mLevel = GetLevelForXp(mXp);
		}

		float healthDecayRate = mBaseHealthDecayRate;
		if(mIsHungry)
		{
			// healthDecayRate *= 5;
			healthDecayRate = 1;
		}
		else if(GameState.HasBoon(BoonType.SURPLUS_FOOD_TO_HEALING))
		{
			float healAmount = GameState.GetBoonValue(BoonType.SURPLUS_FOOD_TO_HEALING) / 100f;
			healthDecayRate -=  healAmount * GameState.FoodSurplus;
		}
		mHealth -= healthDecayRate * GameState.GameDeltaTime;

		mHealth = Mathf.Clamp(mHealth, 0, mMaxHealth);

		if(mHealth <= 0)
		{
			string deathMsg = mName + " has died at the age of " + Age + ". Their lifeforce returns to the earth.";
			Debug.Log(deathMsg);
			Utilities.LogEvent(deathMsg, 1f);
			Utilities.GetPersonManager().RemovePerson(this);
		}
	}

	private int GetLevelForXp(float xp)
	{
		int level = 0;
		while(xp >= 0)
		{
			++level;
			float xpToLevelUp = level * 10;
			xp -= xpToLevelUp;
		}
		return(level);
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

