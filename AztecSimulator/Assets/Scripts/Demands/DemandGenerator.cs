using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemandGenerator {
	
	public static SacrificeDemand SimpleDemand(SacrificeResult satisfiedResult = null, SacrificeResult ignoredResult = null)
	{
		SacrificeDemand d = new SacrificeDemand(satisfiedResult, ignoredResult);

		int numAttributes = Random.Range(1,3);
		List<Person.Attribute> attributes = GenerateSatisfiableDemands(numAttributes);

		// add them as separate criteria so that they're easier to satisfy
		foreach(Person.Attribute attr in attributes)
		{
			Criterion c = new Criterion();
			c.mAttributes.Add(attr);
			d.mCriteria.Add(c);
		}

		return(d);
	}

	public static SacrificeDemand TierOneDemand()
	{
		SacrificeDemand d = new SacrificeDemand();

		Criterion c = new Criterion();

		c.mMinLevel = Random.Range(0,11);
		c.mMinAge = Random.Range(0, 21);
		c.mCount = Random.Range(1, 6);
		c.mAttributes = GenerateRandomDemands(1);

		d.mCriteria.Add(c);

		return(d);
	}

	public static SacrificeDemand TierTwoDemand()
	{
		SacrificeDemand d = new SacrificeDemand();

		Criterion c = new Criterion();

		if(Random.value < 0.5)
		{
			c.mMinLevel = 10;
		}
		else
		{
			c.mMinAge = 30;
		}

		c.mAttributes = GenerateRandomDemands(2);

		d.mCriteria.Add(c);

		return(d);
	}

	public static SacrificeDemand VictoryDemand()
	{
		// appropriately tuned demands to win the game
		SacrificeDemand warriorDemand = new SacrificeDemand();
		Criterion c = new Criterion();
		c.mMinLevel = 20;
		c.mAttributes.Add(Person.Attribute.WARRIOR);
		c.mCount = 5;
		warriorDemand.mCriteria.Add(c);
		return(warriorDemand);
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

	public static List<Person.Attribute> GenerateRandomDemands(int tier)
	{
		List<Person.Attribute> attrs = new List<Person.Attribute>();
		Person.Attribute[] attributeAndProfession = Person.RandomAttributes(tier);
		if(tier == 1)
		{
			attrs.Add(attributeAndProfession[Random.Range(0, attributeAndProfession.Length)]);
		}
		else if(tier == 2)
		{
			// restrict to profession + one other attribute...
			// asking for two random attributes that you have no control over just feels bad
			attrs.Add(attributeAndProfession[Random.Range(0, attributeAndProfession.Length -1)]);
			attrs.Add(attributeAndProfession[attributeAndProfession.Length-1]);
		}
		return(attrs);
		/*
		List<Person.Attribute> demands = new List<Person.Attribute>();
		var numAttrs = System.Enum.GetValues(typeof(Person.Attribute)).Length-1;
		int[] attributes = Utilities.RandomList(numAttrs, preferredNum);
		for(int i = 0; i < preferredNum; ++i)
		{
			demands.Add((Person.Attribute)attributes[i]);
		}
		return(demands);
		*/
	}

}
