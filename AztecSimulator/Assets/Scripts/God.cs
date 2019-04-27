using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public class SacrificeDemand
	{
		public List<Person.Attribute> mDemandedAttributes;

		public SacrificeDemand() {
			mDemandedAttributes = new List<Person.Attribute>();
			int numAttributes = Random.Range(1,3);

			int[] attributes = Utilities.RandomList((int)Person.Attribute.MAX_VALUE, numAttributes);
			for(int i = 0; i < numAttributes; ++i)
			{
				mDemandedAttributes.Add((Person.Attribute)attributes[i]);
			}
		}

		public void DebugPrint()
		{
			foreach(Person.Attribute attr in mDemandedAttributes)
			{
				Debug.Log(System.Enum.GetName(typeof(Person.Attribute), (int)attr));
			}
		}
	}

	private List<SacrificeDemand> mDemands;

	// Use this for initialization
	void Start () {
		mDemands = new List<SacrificeDemand>();

		int numDemands = Random.Range(1, 3);
		for(int i = 0; i < numDemands; ++i)
		{
			mDemands.Add(new SacrificeDemand());
		}

		DebugPrint();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DebugPrint()
	{
		foreach(SacrificeDemand sd in mDemands)
		{
			Debug.Log("YOUR GOD DEMANDS A PERSON WITH:");
			sd.DebugPrint();
		}
	}

	public List<SacrificeResult> MakeSacrifice(int demandIdx, List<Person> people) {

		PersonManager personMgr = Utilities.GetPersonManager();

		SacrificeDemand demand = mDemands[demandIdx];
		List<Person.Attribute> demandsCopy = new List<Person.Attribute>(demand.mDemandedAttributes);
		foreach(Person p in people)
		{
			foreach(Person.Attribute attr in p.Attributes)
			{
				foreach(Person.Attribute demandedAttr in demandsCopy)
				{
					if(attr == demandedAttr)
					{
						demandsCopy.Remove(demandedAttr);
						break;
					}
				}
			}
		}

		foreach(Person p in people)
		{
			Debug.Log("goodbye " + p.Name);
		}
		personMgr.RemovePeople(people);

		List<SacrificeResult> results = new List<SacrificeResult>();

		if(demandsCopy.Count == 0)
		{
			Debug.Log("YES, THIS SACRIFICE PLEASES ME");
			SacrificeResult sr = new GoodCropBoon();
			results.Add(sr);
		}
		else
		{
			Debug.Log("NO WHAT ARE YOU DOING");
			// results.Add(new PlagueCurse());
		}

		return(results);
	}
}
