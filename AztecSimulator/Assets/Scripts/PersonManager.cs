using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour {

	public static int MAX_POPULATION = 10;
	public int numStartingPeople = 8;

	public float civilianBirthRateIncrease = 2;
	public float birthInterval = 20;

	private List<Person> mPeople;

	public List<Person> People {
		get { return(mPeople); }
	}

	private float mRepopulateTimer;

	private List<GameObject> mPeopleChangedListeners;

	// Use this for initialization
	void Start () {

		mPeople = new List<Person>();
		for(int i = 0; i < numStartingPeople; ++i)
		{
			SpawnPerson();
		}

		mPeopleChangedListeners = new List<GameObject>();

		mRepopulateTimer = birthInterval;
	}
	
	// Update is called once per frame
	void Update () {

		// repopulate. todo: figure out what actual logic we want for this,
		// e.g. if it depends on other factors, if there's a hard cap, etc.
		List<Person> civilians = FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.CIVILIAN);
		float birthRate = civilians.Count * civilianBirthRateIncrease;
		if(mRepopulateTimer > 0)
		{
			mRepopulateTimer -= GameState.GameDeltaTime * birthRate;
		}
		else
		{
			if (mPeople.Count < MAX_POPULATION) {
				Person p = SpawnPerson();
				Debug.Log(p.Name + " was born!");
				Utilities.LogEvent(p.Name + " was born!");
			}
			mRepopulateTimer = birthInterval;
		}

		// Update ArmySize
		int armySize = 0;
		List<Person> warriors = FindPeople(Person.AttributeType.PROFESSION, Person.Attribute.WARRIOR);
		foreach(Person p in warriors) {
			armySize += p.Level;
		}

		if(GameState.HasBoon(BoonType.WARRIOR_CHILD_PROTECT))
		{
			List<Person> children = mPeople.FindAll(x => x.Age <= 10);
			if(children != null)
			{
				armySize += children.Count;
			}
		}

		GameState.ArmySize = armySize;
	}

	private Person SpawnPerson()
	{
		GameObject go = new GameObject("Person");
		go.AddComponent<Person>();
		Person p = go.GetComponent<Person>();
		mPeople.Add(p);
		return(p);
	}

	public void AddPeopleChangedListener(GameObject go)
	{
		mPeopleChangedListeners.Add(go);
	}

	public void RemovePerson(Person p)
	{
		mPeople.Remove(p);
		GameObject.Destroy(p.gameObject);
		PeopleChanged();
	}

	public void RemovePeople(List<Person> people)
	{
		foreach(Person p in people)
		{
			GameObject.Destroy(p.gameObject);
			mPeople.Remove(p);
		}

		PeopleChanged();
	}

	public List<Person> FindPeople(Person.AttributeType attrType, Person.Attribute attrValue)
	{
		List<Person> results = new List<Person>();
		foreach(Person p in mPeople) {
			if(p.GetAttribute(attrType) == attrValue) {
				results.Add(p);
			}
		}
		return(results);
	}

	private void PeopleChanged()
	{
		// not sure if these might dangle... careful about adding listeners that could be destroyed!
		foreach(GameObject go in mPeopleChangedListeners)
		{
			if(go != null)
			{
				go.BroadcastMessage("OnPeopleChanged");
			}
		}
	}
}
