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
			mPeople.Add(new Person());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
