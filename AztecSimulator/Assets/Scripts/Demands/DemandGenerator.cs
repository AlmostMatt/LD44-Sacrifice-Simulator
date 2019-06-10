using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemandGenerator {

	public static SacrificeDemand ScaledDemand(int tier, bool mustBeSatisfiable = false)
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
			if(!mustBeSatisfiable && Random.value < 0.5)
				return(BuildDemand(0,1,1));

			// 2 on the same person
			if(!mustBeSatisfiable && Random.value < 0.7)
				return(BuildDemand(1,1,2));

			return(BuildDemand(1,0,1));
		}
	
		if(tier == 2)
		{
			if(mustBeSatisfiable)
				return(BuildDemand(1, 0, 1));

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
		if(mustBeSatisfiable)
		{
			// demand a specific person
			return(PersonDemand(tier));
		}

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
			int personCount = Mathf.Min((tier / 2) + 1, 3);
			foreach(Criterion c in demand.mCriteria)
			{
				if(Random.value < 0.5)
				{
					c.mMinLevel = tier + 1;
				}
					
				if(personCount > 0 && Random.value < 0.1 + (tier / 10))
				{
					c.mCount = Mathf.Min(Random.Range(1, personCount+1), 3);
					personCount -= c.mCount;
				}
			}
		}

		return(demand);
	}

	public static SacrificeDemand EasiestDemand()
	{
		// guaranteed to be satisfiable now
		SacrificeDemand d = new SacrificeDemand();
		List<PersonAttribute> attributes = GenerateSatisfiableDemands(1);
		foreach(PersonAttribute attr in attributes)
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

		// 11th hour special case hacks
		if(numPerCriterion == 2)
		{
			Criterion doubleCrit = new Criterion();
			PersonAttribute[] doubleAttrs = Person.RandomAttributes(1);
			doubleCrit.mAttributes = new List<PersonAttribute>(doubleAttrs);
			d.mCriteria.Add(doubleCrit);
			return d;
		}

		List<PersonAttribute> attributes = GenerateSatisfiableDemands(numGuaranteed);
		List<PersonAttribute> anyAttrs = GenerateRandomDemandsDumb(numAny);

		List<PersonAttribute> allAttrs = new List<PersonAttribute>();
		for(int i = 0; i < anyAttrs.Count; ++i)
		{
			allAttrs.Add(anyAttrs[i]);
		}
		for(int i = 0; i < attributes.Count; ++i)
		{
			PersonAttribute attr = attributes[i];
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
		List<PersonAttribute> attributes = GenerateSatisfiableDemands(numAttributes);

		// add them as separate criteria so that they're easier to satisfy
		foreach(PersonAttribute attr in attributes)
		{
			Criterion c = new Criterion();
			c.mAttributes.Add(attr);
			d.mCriteria.Add(c);
		}

		return(d);
	}

	public static SacrificeDemand PersonDemand(int tier)
	{
		int numPeople = tier >= 6 ? 2 : 1;

		Person[] people = Utilities.RandomSubset<Person>(Utilities.GetPersonManager().People.ToArray(), numPeople);

		SacrificeDemand d = new SacrificeDemand();
		foreach(Person p in people)
		{
			Criterion c = new Criterion();
			c.mMinLevel = p.Level;
			c.mAttributes.Add(p.Profession);
            List<PersonAttribute> personAttributes = p.NonProfessionAttributes;
            c.mAttributes.Add(personAttributes[Random.Range(0, personAttributes.Count)]);
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

		c.mMinLevel = 6;

		c.mAttributes = GenerateRandomDemands(2);

		d.mCriteria.Add(c);

		return(d);
	}

	public static List<PersonAttribute> GenerateSatisfiableDemands(int preferredNum)
	{
		List<PersonAttribute> demands = new List<PersonAttribute>(); 

		// naive (or clever?): if we just pick a person at random and an attribute at random,
		// then we're guaranted to be able to satisfy it. The bonus is that 
		// some other people might share that attribute, so we get player choice involved basically for free
		List<Person> people = Utilities.GetPersonManager().People;

		while(preferredNum-- > 0)
		{
			Person p = people[Random.Range(0, people.Count)];

            PersonAttribute attribute = p.NonProfessionAttributes[Random.Range(0, p.NonProfessionAttributes.Count)];
			if(!demands.Contains(attribute))
				demands.Add(attribute);
		}

		return(demands);
	}

	public static List<PersonAttribute> GenerateRandomDemandsDumb(int howMany)
	{
		PersonAttribute[] attributesAndProfession = Person.RandomAttributes(howMany); // gives howMany + 1, because it tacks profession onto the end
		return new List<PersonAttribute>(Utilities.RandomSubset(attributesAndProfession, howMany));
	}

	public static List<PersonAttribute> GenerateRandomDemands(int tier)
	{
		List<PersonAttribute> attrs = new List<PersonAttribute>();
		PersonAttribute[] attributeAndProfession = Person.RandomAttributes(tier);
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
		List<PersonAttribute> demands = new List<PersonAttribute>();
		var numAttrs = System.Enum.GetValues(typeof(PersonAttribute)).Length-1;
		int[] attributes = Utilities.RandomList(numAttrs, preferredNum);
		for(int i = 0; i < preferredNum; ++i)
		{
			demands.Add((PersonAttribute)attributes[i]);
		}
		return(demands);
		*/
	}

}
