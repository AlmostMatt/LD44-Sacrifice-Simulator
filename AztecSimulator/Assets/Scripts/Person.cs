using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour, IRenderable {
	private static Dictionary<Person.Attribute, float> EFFICIENCY_BASE = new Dictionary<Person.Attribute, float> {
		{Person.Attribute.FARMER, 1.5f},
		{Person.Attribute.WARRIOR, 1f},
		{Person.Attribute.CIVILIAN, 0.5f},
		{Person.Attribute.NONE, 0f}
	};
	private static Dictionary<Person.Attribute, float> EFFICIENCY_PER_LEVEL = new Dictionary<Person.Attribute, float> {
		{Person.Attribute.FARMER, 0.75f},
		{Person.Attribute.WARRIOR, 0.5f},
		{Person.Attribute.CIVILIAN, 0.25f},
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
	private Dictionary<AttributeType, Attribute> mAttributes = new Dictionary<AttributeType, Attribute>();
	private int mLevel;
	private float mAge;
	private float mXp = 0f;

	private float mHealth;
	private float mMaxHealth;
	private float mBaseHealthDecayRate;
	// info about recent health gain/loss in order to enable fancy UI
	private float mRecentHealthChange;
	private float mTimeSinceHealthChanged;

	public string Name {
		get { return(mName); } 
	}
    public Attribute Profession
    {
        get {
            return GetAttribute(AttributeType.PROFESSION);
        }
    }
	public List<Attribute> NonProfessionAttributes
    {
        get
        {
            List<Attribute> attributes = new List<Attribute>();
            foreach (KeyValuePair<AttributeType, Attribute> kvp in mAttributes)
            {
                if (kvp.Key != AttributeType.PROFESSION)
                {
                    attributes.Add(kvp.Value);
                }
            }
            return attributes;
        }
    }
	public float Health
	{
		get { return(mHealth); }
	}
	public float MaxHealth
	{
		get { return(mMaxHealth); }
	}
	public float RecentHealthChange
	{
		get { return(mRecentHealthChange); }
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
			EFFICIENCY_BASE[Profession]
			+ (EFFICIENCY_PER_LEVEL[Profession]) * (mLevel - 1));
		}
	}

	public void ChangeProfession(Attribute newProfession)
	{
		if (Profession == newProfession)
        {
            return;
        }
        // note: this could rely on the fact that profession happens to be the last attribute
        mAttributes[AttributeType.PROFESSION] = newProfession;
		int xpRetention = GameState.GetBoonValue(BoonType.PROFESSION_CHANGE_RETAIN_XP);
		if(xpRetention > 0)
			mXp = (xpRetention / 100f) * mXp;
		else
			mXp = 0;
		mLevel = GetLevelForXp(mXp);
	}

	// This should be called after the person has already been created and Awake completed.
	public void OverrideRandomValues(Attribute newProfession = Attribute.NONE, int level = -1) {
		if (newProfession != Attribute.NONE)
        {
            mAttributes[AttributeType.PROFESSION] = newProfession;
		}
		if (level != -1) {
			mLevel = level;
			mXp = GetTotalXpForLevel(level);
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
        Attribute attr;
        return mAttributes.TryGetValue(attrType, out attr) ? attr : Attribute.NONE;
	}

	public void AddXp(float amount)
	{
		// maybe this wants to be smarter about caps, idk
		mXp += amount;
	}

	public void Heal(float amount)
	{
		float newHealth = Mathf.Clamp(mHealth + amount, 0, mMaxHealth);
		// Handle the over-heal while at max HP case, in which timeSinceHealthChange should not update
		if (newHealth != mHealth) {
			mTimeSinceHealthChanged = 0f;
			mRecentHealthChange += newHealth - mHealth;
			mHealth = newHealth;
		}
	}

	public void Damage(float amount)
	{
		Heal(-amount);
	}

	// Use this for initialization
	//void Start () {
	void Awake() {
		mName = NAMES[UnityEngine.Random.Range(0, NAMES.Length)];
		mLevel = 1;
		mAge = 0;

        int numRandomAttributes = 2; //  Random.Range(1,3);
        Attribute[] randomAttrs = RandomAttributes(numRandomAttributes, false);
        foreach (Attribute attr in randomAttrs)
        {
            mAttributes[attr.GetAttrType()] = attr;
        }
        mAttributes[AttributeType.PROFESSION] = Attribute.NONE; // Start without a profession

        mIsHungry = false;
		mMaxHealth = mHealth = startingHealth;
		//mBaseHealthDecayRate = Random.Range(0.45f, 0.55f);
		mBaseHealthDecayRate = 0;
		if(GameState.ImprovedLifespan1) mBaseHealthDecayRate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		mAge += 0.2f * GameState.GameDeltaTime;

		Attribute profession = Profession;
		if(mLevel < GameState.GetLevelCap(profession))
		{
			if (profession != Attribute.NONE) {
				int xpGain = 1;
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

				mXp += xpGain * 0.25f * GameState.GameDeltaTime;
			}

			mLevel = GetLevelForXp(mXp);
		}

		float healthDecayRate = mBaseHealthDecayRate; // 0.
		if(mIsHungry)
		{
			// healthDecayRate *= 5;
			healthDecayRate = 3f;
		}
		else if(GameState.HasBoon(BoonType.SURPLUS_FOOD_TO_HEALING))
		{
			float healAmount = GameState.GetBoonValue(BoonType.SURPLUS_FOOD_TO_HEALING) / 100f;
			healthDecayRate -=  healAmount * GameState.FoodSurplus;
		}
		Damage(healthDecayRate * GameState.GameDeltaTime);
		// Time since health changed is strictly for UI purposes, so it does not use game-time.
		mTimeSinceHealthChanged += Time.deltaTime;
		if (mTimeSinceHealthChanged >= 1f) {
			mRecentHealthChange = 0f;
		}
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
        List<Attribute> nonProfAttributes = NonProfessionAttributes;
        string attrString = "";
		for(int i = 0; i < nonProfAttributes.Count; ++i) {
			string attr = System.Enum.GetName(typeof(Attribute), (int)nonProfAttributes[i]);
			bool isRelevant = selectedDemand != null && selectedDemand.IsRelevantAttribute(nonProfAttributes[i]);
			attrString += Utilities.ColorString(attr, "green", isRelevant) + ", ";
		}
		bool isLevelRelevant = selectedDemand != null && selectedDemand.IsRelevantLevel(mLevel);
		string levelString = "Lvl " + Utilities.ColorString(GetLevelString(), "green", isLevelRelevant);
		string lifeString = (mIsHungry ? "<size=10>STARVING!</size> " : "") + " " + Utilities.ColorString(Utilities.ColorString(Mathf.Ceil(mHealth).ToString(), "red", mRecentHealthChange < 0f), "green", mRecentHealthChange > 0f);
		string ageString = Utilities.ColorString(mName + " (Age " + Age + ")", "green", selectedDemand != null && selectedDemand.IsRelevantAge(Age));
		return new [] {ageString, attrString, levelString, lifeString};
	}

	private int GetLevelUpProgressPercent()
	{
		float requiredXpForCurrentLevel = GetTotalXpForLevel(mLevel);
		float requiredXpForNextLevel = GetTotalXpForLevel(mLevel+1);
		return (int)Mathf.Floor(100f * (mXp - requiredXpForCurrentLevel)  / (requiredXpForNextLevel - requiredXpForCurrentLevel));
	}

	private float GetXpToNextLevel()
	{
		int nextLevel = Mathf.Min(mLevel + 1, GameState.GetLevelCap(Profession));
		float requiredXpForLevel = GetTotalXpForLevel(nextLevel);
		return(requiredXpForLevel - mXp);
	}

	private float GetTotalXpForLevel(int level)
	{
		return level * (level - 1) * 5; // based on required xp to level L = L * 10
	}

	private string GetLevelString()
	{
		//if (mLevel == GameState.GetLevelCap(Profession)) {
		//	return mLevel + " (MAX)";
		//} else {
		return mLevel + "/" + GameState.GetLevelCap(Profession).ToString() + " ";
		//}
	}

	public void DebugPrint() {
        List<Attribute> nonProfAttributes = NonProfessionAttributes;
        Debug.Log(mName + " has: ");
		for(int i = 0; i < nonProfAttributes.Count; ++i) {
			Debug.Log(System.Enum.GetName(typeof(Attribute), (int)nonProfAttributes[i]));
		}
	}

	public static Attribute[] RandomAttributes(int howMany, bool alsoRandomProfession = true)
	{
		AttributeType[] attrTypes = {
			AttributeType.PERSONALITY,
			AttributeType.HEIGHT,
			AttributeType.STRENGTH,
			// AttributeType.EYE_COLOR,
		};

		howMany = Mathf.Min(attrTypes.Length, howMany);
		AttributeType[] randomAttributes = Utilities.RandomSubset(attrTypes, howMany);

		Attribute[] attributes = new Attribute[howMany + (alsoRandomProfession ? 1 : 0)];
        if (alsoRandomProfession)
        {
            attributes[howMany] = Utilities.GetRandomAttr(AttributeType.PROFESSION);
        }
		for(int i = 0; i < howMany; ++i)
		{
			attributes[i] = Utilities.GetRandomAttr(randomAttributes[i]);
		}

		return(attributes);
	}

	public void RenderTo(GameObject uiPrefab) {
        // TODO: use hovered or partially completed demand to control the person's highlight
        // GodDemand selectedDemand = Utilities.GetSelectedDemand();
        // selectedDemand.mDemand.IsRelevantAttribute(profession)

        // Level txt
        uiPrefab.transform.Find("H/Level").GetComponent<Text>().text = mLevel.ToString();
        // Profession
        Sprite professionSprite = Utilities.GetSpriteManager().GetSprite(Profession == Attribute.NONE ? "NO_PROFESSION" : Profession.ToString());
        uiPrefab.transform.Find("H/Profession/Image").GetComponent<Image>().sprite = professionSprite;
        // Attributes
        List<Attribute> attributes = NonProfessionAttributes;
        if (attributes.Count > 2)
        {
            Debug.Log("WARNING: a person has >2 attributes!");
        }
        for (int i = 0; i < 2; i++)
        {
            Image attrImage = uiPrefab.transform.Find("H/V/Attribute" + (i+1).ToString()).GetComponent<Image>();
            attrImage.enabled = (i < attributes.Count);
            if (i < attributes.Count)
            {
                attrImage.sprite = Utilities.GetSpriteManager().GetSprite(attributes[i]);
            }
        }
        // HP / XP bars
        uiPrefab.transform.Find("H/HP/Bar").localScale = new Vector3(1f, mHealth / MaxHealth, 1f);
        if (mLevel == GameState.GetLevelCap(Profession))
        {
            uiPrefab.transform.Find("H/XP/Bar").localScale = new Vector3(1f, 1f, 1f);
            uiPrefab.transform.Find("H/XP/Bar").GetComponent<Image>().color = new Color(.7f, .7f, .7f);
        }
        else
        {
            uiPrefab.transform.Find("H/XP/Bar").GetComponent<Image>().color = new Color(1f, 1f, 0f);
            uiPrefab.transform.Find("H/XP/Bar").localScale = new Vector3(1f, GetLevelUpProgressPercent()/100f, 1f);
        }
    }
}

