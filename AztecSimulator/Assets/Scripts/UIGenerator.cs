using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates UIPerson objects. Can be merged into PersonManager later
public class UIGenerator : MonoBehaviour {

	public GameObject uiPersonObject;

	private PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool;

	// Use this for initialization
	void Start () {
		mPersonManager = Utilities.GetPersonManager();
		mUiPeoplePool = new List<GameObject>();
	}

	
	void Update () {
		List<Person> people = mPersonManager.People;
		for(int i = 0; i < Mathf.Max(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson;
			// Spawn new UI person
			if (i >= mUiPeoplePool.Count) {
				uiPerson = Instantiate(uiPersonObject);
				mUiPeoplePool.Add(uiPerson);
				uiPerson.transform.SetParent(transform);
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			// Update position
			RectTransform rt = uiPerson.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0f,0f+30f*(i-people.Count/2));
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i <= people.Count);
		}
	}

	void OnSacrifice() {
		// sacrifice the selected people
	}
}
