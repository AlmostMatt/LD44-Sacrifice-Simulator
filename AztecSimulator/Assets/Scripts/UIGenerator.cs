using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Generates UIPerson objects. Can be merged into PersonManager later
public class UIGenerator : MonoBehaviour {

	public GameObject uiPersonObject;

	public PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool;

	// Use this for initialization
	void Start () {
		mUiPeoplePool = new List<GameObject>();
		// FindWithTag   mPersonManager = (PersonManager)FindObjectOfType(typeof(PersonManager));
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
			Text uiText = uiPerson.GetComponentInChildren<Text>();
			uiText.text = people[i].GetUIDescription();
		}
	}
}
