using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemandGenerator {

	public static SacrificeDemand ScaledDemand(int tier)
	{
		tier = Mathf.Max(0, tier + (Random.Range(0, 100) / 70));
		if(tier == 0)
		{
			// one attribute that you have
			return(EasiestDemand());
		}

		if(tier == 1)
		{
			// 1 attribute that you may not have
			if(Random.value < 0.5)
				return(BuildDemand(0,1,1));

			// 2 on the same person
			return(BuildDemand(1,1,2));
		}
	
		if(tier == 2)
		{
			if(Random.value < 0.5)
			{
				// 2 on the same person
				return(BuildDemand(1,1,2));
			}

			// 1 attribute that you may not have, plus extra constraint
			SacrificeDemand d = BuildDemand(0,1,1);
			d.mCriteria[0].mMinLevel = Random.Range(2, 4);
			return(d);
		}

		// tier >= 3
		SacrificeDemand demand;
		if(Random.value < 0.5)
		{
			// 2 attributes you have
			demand = BuildDemand(2, 0, 1);
		}
		else if(Random.value < 0.5)
		{
			// one of each
			demand = BuildDemand(1, 1, 1);
		}
		else
		{
			// 2 attributes that you may not have
			demand = BuildDemand(0,2,1);
		}

		if(tier >= 4)
		{
			foreach(Criterion c in demand.mCriteria)
			{
				if(Random.value < 0.3)
				{
					c.mMinAge = Random.Range(10, 11 + (tier - 3) * 5);
				}
				else if(Random.value < 0.3)
				{
					c.mMinLevel = tier + 2;
				}

				if(Random.value < 0.1 + (tier / 10))
				{
					c.mCount = (tier / 2) + 1;
				}
			}
		}

		return(demand);
	}

	public static SacrificeDemand EasiestDemand()
	{
		// guaranteed to be satisfiable now
		SacrificeDemand d = new SacrificeDemand();
		List<Person.Attribute> attributes = GenerateSatisfiableDemands(1);
		foreach(Person.Attribute attr in attributes)
		{
			Criterion c = new Criterion();
			c.mAttributes.Add(attr);
			d.mCriteria.Add(c);
		}

		return(d);
	}

	public static SacrificeDemand BuildDemand(int numGuaranteed, int numAny, int numPerCriterion)
	{
		SacrificeDemand d = new SacrificeDemand();

		List<Person.Attribute> attributes = GenerateSatisfiableDemands(numGuaranteed);
		List<Person.Attribute> anyAttrs = GenerateRandomDemandsDumb(numAny);

		List<Person.Attribute> allAttrs = new List<Person.Attribute>();
		for(int i = 0; i < anyAttrs.Count; ++i)
		{
			allAttrs.Add(anyAttrs[i]);
		}
		for(int i = 0; i < attributes.Count; ++i)
		{
			Person.Attribute attr = attributes[i];
			if(!allAttrs.Contains(attr))
			{
				allAttrs.Add(attr);
			}
		}
			
		int idx = 0;
		while(idx < allAttrs.Count)
		{
			Criterion c = new Criterion();
			for(int i = 0; i < numPerCriterion && idx < allAttrs.Count; ++i)
			{
				c.mAttributes.Add(allAttrs[idx]);
				++idx;
			}
			d.mCriteria.Add(c);
		}

		return(d);
	}

	public static SacrificeDemand SimpleDemand(int tier)
	{
		SacrificeDemand d = new SacrificeDemand();

		int minAttr = Mathf.Min(2, 1+tier);
		int maxAttr = Mathf.Min(3, 2+tier);
		int numAttributes = Random.Range(minAttr,maxAttr + 1);
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
		c.mCount = Random.Range(1, 3);
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
		// (maybe) appropriately tuned demands to win the game
		if(Random.value < 0.5)
		{
			SacrificeDemand professionDemand = new SacrificeDemand();
			Criterion profC = new Criterion();
			profC.mMinLevel = 7;
			Person.Attribute[] attr = Person.RandomAttributes(1);
			profC.mAttributes.Add(attr[attr.Length-1]);
			profC.mCount = 5;
			professionDemand.mCriteria.Add(profC);
			return(professionDemand);
		}

		SacrificeDemand experienceDemand = new SacrificeDemand();
		Criterion expC = new Criterion();
		expC.mMinLevel = 6;
		expC.mCount = 3;
		experienceDemand.mCriteria.Add(expC);
		return(experienceDemand);
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

	public static List<Person.Attribute> GenerateRandomDemandsDumb(int howMany)
	{
		Person.Attribute[] attributesAndProfession = Person.RandomAttributes(howMany); // gives howMany + 1, because it tacks profession onto the end
		return new List<Person.Attribute>(Utilities.RandomSubset(attributesAndProfession, howMany));
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
