using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SacrificeDemand
{
	private static int sId = 0;

	public int mId;
	public SacrificeResult mSatisfiedResult;
	public SacrificeResult mIgnoredResult;

	public SacrificeDemand(SacrificeResult satisfiedResult, SacrificeResult ignoredResult) {

		mId = ++sId;

		mSatisfiedResult = satisfiedResult;
		mIgnoredResult = ignoredResult;
	}

	public abstract bool CheckSatisfaction(List<Person> people);
	public abstract string GetString();

	public void DebugPrint()
	{
		Debug.Log(GetString());
	}
}
