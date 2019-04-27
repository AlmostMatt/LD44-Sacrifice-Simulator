using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDemand : SacrificeDemand {

	public List<Person.Attribute> mDemandedAttributes;

	public SimpleDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult) : base(satisfiedResult, ignoredResult)
	{
		int numAttributes = Random.Range(1,3);
		mDemandedAttributes = GenerateSatisfiableDemands(numAttributes);		
	}

	public override string GetString()
	{
		string satisfiedString = mSatisfiedResult == null ? "<demanded>" : mSatisfiedResult.mName;
		string costString = GetDemandedAttributes();
		return satisfiedString + "\r\nDEMAND\r\n" + costString;
	}

	public string GetDemandedAttributes()
	{
		return Utilities.ConcatStrings(mDemandedAttributes.ConvertAll(
			attr => System.Enum.GetName(typeof(Person.Attribute), (int)attr)
		), true);
	}

	public override bool CheckSatisfaction(List<Person> people)
	{
		List<Person.Attribute> demandsCopy = new List<Person.Attribute>(mDemandedAttributes);
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

		return(demandsCopy.Count == 0);
	}

	public static List<Person.Attribute> GenerateSatisfiableDemands(int preferredNum)
	{
		List<Person.Attribute> demands = new List<Person.Attribute>(); 

		// naive (or clever?): if we just pick a person at random and an attribute at random,
		// then we're guaranted to be able to satisfy it. The bonus is that 
		// some other people might share that attribute, so we get player choice involved basically for free
		List<Person> people = Utilities.GetPersonManager().People;

		while(preferredNum-- > 0)
		{
			Person p = people[Random.Range(0, people.Count)];
			Person.Attribute attribute = p.Attributes[Random.Range(0, p.Attributes.Length)];
			if(!demands.Contains(attribute))
				demands.Add(attribute);
		}

		return(demands);
	}

	public static List<Person.Attribute> GenerateRandomDemands(int preferredNum)
	{
		List<Person.Attribute> demands = new List<Person.Attribute>();
		var numAttrs = System.Enum.GetValues(typeof(Person.Attribute)).Length-1;
		int[] attributes = Utilities.RandomList(numAttrs, preferredNum);
		for(int i = 0; i < preferredNum; ++i)
		{
			demands.Add((Person.Attribute)attributes[i]);
		}
		return(demands);
	}

}
