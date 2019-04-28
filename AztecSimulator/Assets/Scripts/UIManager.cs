using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // for First()

// Creates and manages UI objects.
public class UIManager : MonoBehaviour {

	public GameObject uiNotificationObject;
	public GameObject uiPersonObject;
	public GameObject uiDemandObject;

	private God mGod;
	private PersonManager mPersonManager;
	private List<GameObject> mUiPeoplePool = new List<GameObject>();
	private List<GameObject> mUiDemandPool = new List<GameObject>();
	private int mMaxEventMessages = 8;
	private List<string> mEventMessages = new List<string>();
	private ToggleGroup mDemandToggleGroup;

	private List<GameObject> mUiNotificationPool = new List<GameObject>();
	private List<string> mNotificationMessages = new List<string>();
	private List<float> mNotificationDurations = new List<float>();

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
		List<Person> selectedPeople = getSelectedPeople();
		SacrificeDemand selectedDemand = getSelectedDemand();
		transform.Find("Right/People/SacrificeButton").GetComponent<Button>().interactable = (selectedPeople.Count > 0);

		// Update people
		// TODO: use people-changed-listener instead of update
		List<Person> people = mPersonManager.People;
		Transform peopleContainer = GetPeopleContainer();
		for(int i = 0; i < Mathf.Max(people.Count, mUiPeoplePool.Count); i++)
		{
			GameObject uiPerson;
			// Spawn new UI person
			if (i >= mUiPeoplePool.Count) {
				uiPerson = Instantiate(uiPersonObject);
				mUiPeoplePool.Add(uiPerson);
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			uiPerson.transform.SetParent(peopleContainer);
			// Update position
			RectTransform rt = uiPerson.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0f,35f*(i-(people.Count-1)/2f));
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i < people.Count);
			// Update text
			if (i < people.Count) {
				string[] descriptionStrings = people[i].GetUIDescription(selectedDemand);
				uiPerson.transform.Find("Toggle/TextTL").GetComponent<Text>().text = descriptionStrings[0];
				uiPerson.transform.Find("Toggle/TextTR").GetComponent<Text>().text = descriptionStrings[1];
				uiPerson.transform.Find("Toggle/TextBL").GetComponent<Text>().text = descriptionStrings[2];
				uiPerson.transform.Find("Toggle/TextBR").GetComponent<Text>().text = descriptionStrings[3];
			}
		}

		// Update the god and demands
		if(mGod != null) {
			transform.Find("Left/Demands/Name").GetComponent<Text>().text = mGod.Name;
			List<SacrificeDemand> demands = mGod.Demands;
			Transform demandContainer = transform.Find("Left/Demands/DemandList");
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
					uiDemand.transform.Find("Text").GetComponent<Text>().text = demands[i].GetLongDescription(); 
				}
			}
		}

		// Update notification objects
		Transform notificationContainer = transform.Find("Corner");
		for(int i = 0; i < Mathf.Max(mNotificationMessages.Count, mUiNotificationPool.Count); i++)
		{
			GameObject uiNotification;
			// Spawn new UI demand
			if (i >= mUiNotificationPool.Count) {
				uiNotification = Instantiate(uiNotificationObject);
				mUiNotificationPool.Add(uiNotification);
				uiNotification.transform.SetParent(notificationContainer);
			} else {
				uiNotification = mUiNotificationPool[i];
			}
			// Update position
			RectTransform rt = uiNotification.GetComponent<RectTransform>();
			// TODO: slide down if others have faded
			rt.anchoredPosition = new Vector2(0f,100f*i);
			// Update visibility
			uiNotification.transform.gameObject.SetActive(i < mNotificationMessages.Count);
			// Update text and alpha
			if (i < mNotificationMessages.Count) {
				uiNotification.transform.Find("Text").GetComponent<Text>().text = mNotificationMessages[i]; 
				uiNotification.GetComponent<CanvasGroup>().alpha = Mathf.Min(1f, mNotificationDurations[i] / 1f);
			}
		}
		for (int i = mNotificationMessages.Count-1; i>= 0; i--) {
			mNotificationDurations[i] -= Time.deltaTime;
			if (mNotificationDurations[i] <= 0f) {
				mNotificationMessages.RemoveAt(i);
				mNotificationDurations.RemoveAt(i);
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

	private SacrificeDemand getSelectedDemand() {
		int demandId = getSelectedDemandId();
		if(mGod != null && demandId != 0) {
			foreach (SacrificeDemand demand in mGod.Demands) {
				if (demand.mId == demandId) {
					return demand;
				}
			}
		}
		return null;
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

	public void LogEvent(string message, float duration=4f) {
		mEventMessages.Add(message);
		string newLogText = "";
		// Concatenate the last K messages
		for (int i = Mathf.Max(0, mEventMessages.Count-mMaxEventMessages); i < mEventMessages.Count; i++) {
			newLogText += mEventMessages[i] + "\n";
		}
		transform.Find("Left/Log/LogText").GetComponent<Text>().text = newLogText;

		mNotificationMessages.Add(message);
		mNotificationDurations.Add(duration);
	}

	private int GetSelectedTabIndex() {
		// TODO: use the toggle group to find the tab more easily
		if (transform.Find("Left/TabGroup/Tab1").GetComponent<Toggle>().isOn) { return 0; }
		else if (transform.Find("Left/TabGroup/Tab2").GetComponent<Toggle>().isOn) { return 1; }
		else if (transform.Find("Left/TabGroup/Tab3").GetComponent<Toggle>().isOn) { return 2; }
		else if (transform.Find("Left/TabGroup/Tab4").GetComponent<Toggle>().isOn) { return 3; }
		return -1;
	}

	private Transform GetPeopleContainer() {
		int tabIndex = GetSelectedTabIndex();
		var leftPeopleList = transform.Find("Left/People/PeopleList");
		var rightPeopleList = transform.Find("Right/People/PeopleList");
		return tabIndex == 2 ? leftPeopleList : rightPeopleList;
	}
}
