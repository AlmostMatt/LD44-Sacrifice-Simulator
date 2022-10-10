using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour, IRenderable
{
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

    // At level N, XP to next level is 40 * N 
    private const int xpPerLevelFactor = 40;
	private float startingHealth = 100;

	private string mName;
	private Dictionary<PersonAttributeType, PersonAttribute> mAttributes = new Dictionary<PersonAttributeType, PersonAttribute>();
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
    public PersonAttribute Profession
    {
        get {
            return GetAttribute(PersonAttributeType.PROFESSION);
        }
    }
	public List<PersonAttribute> NonProfessionAttributes
    {
        get
        {
            List<PersonAttribute> attributes = new List<PersonAttribute>();
            foreach (KeyValuePair<PersonAttributeType, PersonAttribute> kvp in mAttributes)
            {
                if (kvp.Key != PersonAttributeType.PROFESSION)
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
			Profession.GetEfficiencyBase()
			+ (Profession.GetEfficiencyPerLevel() * (mLevel - 1)));
		}
	}

	public void ChangeProfession(PersonAttribute newProfession)
	{
		if (Profession == newProfession)
        {
            return;
        }
        // note: this could rely on the fact that profession happens to be the last attribute
        mAttributes[PersonAttributeType.PROFESSION] = newProfession;
		int xpRetention = GameState.GetBoonValue(BoonType.PROFESSION_CHANGE_RETAIN_XP);
		if(xpRetention > 0)
			mXp = (xpRetention / 100f) * mXp;
		else
			mXp = 0;
		mLevel = GetLevelForXp(mXp);
	}

	// This should be called after the person has already been created and Awake completed.
	public void OverrideRandomValues(PersonAttribute newProfession = PersonAttribute.NONE, int level = -1) {
		if (newProfession != PersonAttribute.NONE)
        {
            mAttributes[PersonAttributeType.PROFESSION] = newProfession;
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

    private float mHungryBuffer=0f;
	private bool mIsHungry;
	public bool Hungry
	{
		get { return(mIsHungry); }
		set { mIsHungry = value; }
	}

	public PersonAttribute GetAttribute(PersonAttributeType attrType) {
        PersonAttribute attr;
        return mAttributes.TryGetValue(attrType, out attr) ? attr : PersonAttribute.NONE;
	}

	public void AddXp(float amount)
	{
		// maybe this wants to be smarter about caps, idk
		mXp += amount;
        mLevel = GetLevelForXp(mXp);
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
        PersonAttribute[] randomAttrs = RandomAttributes(numRandomAttributes, false);
        foreach (PersonAttribute attr in randomAttrs)
        {
            mAttributes[attr.GetAttrType()] = attr;
        }
        mAttributes[PersonAttributeType.PROFESSION] = PersonAttribute.FARMER; // Default to a food-producing profession

        mIsHungry = false;
		mMaxHealth = mHealth = startingHealth;
		//mBaseHealthDecayRate = Random.Range(0.45f, 0.55f);
		mBaseHealthDecayRate = 0;
		if(GameState.ImprovedLifespan1) mBaseHealthDecayRate *= 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		mAge += 0.2f * GameState.GameDeltaTime;

		float healthDecayRate = mBaseHealthDecayRate; // 0.
		if(mIsHungry)
		{
            if (mHungryBuffer > 0f)
            {
                mHungryBuffer -= GameState.GameDeltaTime;
            }
            else
            {
                healthDecayRate = 3f;
            }
        }
        else
        {
            // Allow 2 seconds before starvation will cause damage.
            mHungryBuffer = 2f;
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
			float xpToLevelUp = level * xpPerLevelFactor;
			xp -= xpToLevelUp;
		}
		return(level);
	}

	// Returns a list of strings to be used in top-left, top-right, bottom-left, bottom-right order in a list of people
	// Icons (profession and health) are added by the UI Manager
	public string[] GetUIDescription(SacrificeDemand selectedDemand)
	{
        List<PersonAttribute> nonProfAttributes = NonProfessionAttributes;
        string attrString = "";
		for(int i = 0; i < nonProfAttributes.Count; ++i) {
			string attr = System.Enum.GetName(typeof(PersonAttribute), (int)nonProfAttributes[i]);
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
		return xpPerLevelFactor * level * (level - 1)  / 2; // based on required xp to level L = L * xpPerLevelFactor
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
        List<PersonAttribute> nonProfAttributes = NonProfessionAttributes;
        Debug.Log(mName + " has: ");
		for(int i = 0; i < nonProfAttributes.Count; ++i) {
			Debug.Log(System.Enum.GetName(typeof(PersonAttribute), (int)nonProfAttributes[i]));
		}
	}

	public static PersonAttribute[] RandomAttributes(int howMany, bool alsoRandomProfession = true)
	{
        PersonAttributeType[] attrTypes = {
            PersonAttributeType.PERSONALITY,
            PersonAttributeType.HEIGHT,
            PersonAttributeType.STRENGTH,
			// PersonAttributeType.EYE_COLOR,
		};

		howMany = Mathf.Min(attrTypes.Length, howMany);
        PersonAttributeType[] randomAttributes = Utilities.RandomSubset(attrTypes, howMany);

        PersonAttribute[] attributes = new PersonAttribute[howMany + (alsoRandomProfession ? 1 : 0)];
        if (alsoRandomProfession)
        {
            attributes[howMany] = PersonAttributeType.PROFESSION.GetRandomValue();
        }
		for(int i = 0; i < howMany; ++i)
		{
			attributes[i] = randomAttributes[i].GetRandomValue();
		}

		return(attributes);
    }

    private bool IsHighlighted()
    {
        UIManager uiManager = Utilities.GetUIManager();
        // If this is being dragged, highlight it
        if (uiManager.GetPeopleBeingDragged().Contains(this))
        {
            return true;
        }

        foreach (GodDemand demand in uiManager.GetPartiallyCompletedDemands())
        {
            // If this is attached to a demand slot, highlight it
            if (demand.GetAssociatedPeople().Contains(uiManager.GetUiPerson(this)))
            {
                return true;
            }
            // If this could fill one of the remaining slots of a partially completed demand, highlight it
            if (demand.GetRelevantSlots(this).Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void RenderTo(GameObject uiPrefab) {
        // Level txt
        uiPrefab.transform.Find("H/Level").GetComponent<Text>().text = mLevel.ToString();
        // Profession
        Sprite professionSprite = Utilities.GetSpriteManager().GetSprite(Profession == PersonAttribute.NONE ? "NO_PROFESSION" : Profession.ToString());
        uiPrefab.transform.Find("H/Profession/Image").GetComponent<Image>().sprite = professionSprite;
        // Attributes
        List<PersonAttribute> attributes = NonProfessionAttributes;
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
        // Highlight outline or flashing starving outline
        Outline outline = uiPrefab.transform.Find("Background").GetComponent<Outline>();
        bool isHighlighted = IsHighlighted();
        bool isStarving = mIsHungry;
        outline.enabled = isHighlighted || isStarving;
        if (isStarving)
        {
            outline.effectColor = new Color32(0xff, 0x00, 0x00, 0x64); // x64 is alpha of 100/255
        } else if (isHighlighted)
        {
            // TODO - maybe make this flash to be more obvious
            outline.effectColor = new Color32(0x00, 0xff, 0x00, 0x80);
        }
    }
}

