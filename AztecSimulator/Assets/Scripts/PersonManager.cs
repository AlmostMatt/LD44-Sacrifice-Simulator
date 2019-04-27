using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonManager : MonoBehaviour {

	public int numStartingPeople = 10;

	private List<Person> mPeople;

	public List<Person> People {
		get { return(mPeople); }
	}

	// Use this for initialization
	void Start () {

		mPeople = new List<Person>();

		for(int i = 0; i < numStartingPeople; ++i)
		{
			// not sure this needs to be a gameobject but whatever
			// actually it could be fine if people update in real time from sickness or to elapse training time, etc.
			GameObject go = new GameObject("Person");
			go.AddComponent<Person>();
			mPeople.Add(go.GetComponent<Person>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
