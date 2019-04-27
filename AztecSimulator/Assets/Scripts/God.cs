using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour {

	public class SacrificeDemand
	{
		List<Person.Attribute> mDemandedAttributes;

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

	public List<SacrificeResult> MakeSacrifice(List<Person> people) {

		List<SacrificeResult> results = new List<SacrificeResult>();

		SacrificeResult sr = new GoodCropBoon();
		results.Add(sr);

		return(results);
	}
}
