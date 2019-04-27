using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // for First()

// Creates and manages UI objects.
public class UIManager : MonoBehaviour {

	public GameObject uiPersonObject;
	public GameObject uiDemandObject;

	private God mGod;
	private PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool = new List<GameObject>();
	private List<GameObject> mUiDemandPool = new List<GameObject>();
	private int mMaxEventMessages = 8;
	private List<string> mEventMessages = new List<string>();
	private ToggleGroup mDemandToggleGroup;

	// Use this for initialization
	void Start () {
		mGod = Utilities.GetGod();
		mPersonManager = Utilities.GetPersonManager();
		// create an object for the demands toggle group.
		GameObject emptyObj = new GameObject("demandToggleGroup");
		emptyObj.transform.parent = transform;
		mDemandToggleGroup = emptyObj.AddComponent<ToggleGroup>();
		mDemandToggleGroup.allowSwitchOff = true;
	}

	void Update () {
		// Update people
		// TODO: use people-changed-listener instead of update
		List<Person> people = mPersonManager.People;
		Transform peopleContainer = transform.Find("Right/People/PeopleList");
		for(int i = 0; i < Mathf.Max(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson;
			// Spawn new UI person
			if (i >= mUiPeoplePool.Count) {
				uiPerson = Instantiate(uiPersonObject);
				mUiPeoplePool.Add(uiPerson);
				uiPerson.transform.SetParent(peopleContainer);
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			// Update position
			RectTransform rt = uiPerson.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0f,35f*(i-(people.Count-1)/2f));
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i < people.Count);
			// Update text
			if (i < people.Count) {
				string[] descriptionStrings = people[i].GetUIDescription();
				uiPerson.transform.Find("Toggle/TextTL").GetComponent<Text>().text = descriptionStrings[0];
				uiPerson.transform.Find("Toggle/TextTR").GetComponent<Text>().text = descriptionStrings[1];
				uiPerson.transform.Find("Toggle/TextBL").GetComponent<Text>().text = descriptionStrings[2];
				uiPerson.transform.Find("Toggle/TextBR").GetComponent<Text>().text = descriptionStrings[3];
			}
		}

		// Update the god and demands
		if(mGod != null) {
			transform.Find("Left/God/Name").GetComponent<Text>().text = mGod.Name;
			List<God.SacrificeDemand> demands = mGod.Demands;
			Transform demandContainer = transform.Find("Left/God/DemandList");
			for(int i = 0; i < Mathf.Max(demands.Count, mUiDemandPool.Count); i++)
			{
				GameObject uiDemand;
				// Spawn new UI demand
				if (i >= mUiDemandPool.Count) {
					uiDemand = Instantiate(uiDemandObject);
					mUiDemandPool.Add(uiDemand);
					uiDemand.transform.SetParent(demandContainer);
					uiDemand.GetComponent<Toggle>().group = mDemandToggleGroup;
				} else {
					uiDemand = mUiDemandPool[i];
				}
				// Update position
				RectTransform rt = uiDemand.GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(0f,60f*(i-(demands.Count-1)/2f));
				// Update visibility
				uiDemand.transform.gameObject.SetActive(i < demands.Count);
				// Update text and store id in name
				if (i < demands.Count) {
					uiDemand.name = demands[i].mId.ToString();
					uiDemand.transform.Find("Text").GetComponent<Text>().text = demands[i].GetString(); 
				}
			}
		}

		// Update the top UI bar
		transform.Find("Top/PopulationText").GetComponent<Text>().text = "Population: " + people.Count;
		transform.Find("Top/ResourceText").GetComponent<Text>().text = "Food: " + GameState.FoodSupply;
	}

	private List<Person> getSelectedPeople() {
		List<Person> people = mPersonManager.People;
		List<Person> result = new List<Person>();
		for(int i = 0; i < Mathf.Min(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson = mUiPeoplePool[i];
			Toggle selectedToggle = uiPerson.transform.GetComponentInChildren<Toggle>();
			if (selectedToggle.isOn) {
				result.Add(people[i]);
			}
		}
		return result;
	}

	private int getSelectedDemandId() {
		if (!mDemandToggleGroup.AnyTogglesOn()) {
			return 0;
		}
		Toggle activeDemand = mDemandToggleGroup.ActiveToggles().First();
		int demandId;
		System.Int32.TryParse(activeDemand.gameObject.name, out demandId);
		return demandId;
	}

	public void OnSacrifice() {
		List<Person> selectedPeople = getSelectedPeople();
		Debug.Log("Sacrificing " + selectedPeople.Count + " people.");
		if (selectedPeople.Count == 0) { return; }

        
		if(mGod != null) {
			string sacrificedNames = Utilities.ConcatStrings(selectedPeople.ConvertAll(person => person.Name));
			LogEvent("You sacrifice " + sacrificedNames + " to " + mGod.Name);
			mGod.MakeSacrifice(getSelectedDemandId(), selectedPeople);
			mDemandToggleGroup.SetAllTogglesOff();
        }

		for(int i = 0; i < mUiPeoplePool.Count; i++)
		{
			GameObject uiPerson = mUiPeoplePool[i];
			Toggle selectedToggle = uiPerson.transform.GetComponentInChildren<Toggle>();
			selectedToggle.isOn = false;
		}
	}

	public void LogEvent(string message) {
		mEventMessages.Add(message);
		string newLogText = "";
		// Concatenate the last K messages
		for (int i = Mathf.Max(0, mEventMessages.Count-mMaxEventMessages); i < mEventMessages.Count; i++) {
			newLogText += mEventMessages[i] + "\n";
		}
		transform.Find("Left/Log/LogText").GetComponent<Text>().text = newLogText;
	}
}
