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

	public float startingHealth = 100; // todo: for this to be data-driven, we'll need an actual Person prefab with this as the component...

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

