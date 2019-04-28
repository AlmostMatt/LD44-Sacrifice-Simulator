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
	public GameObject uiProfessionObject;

	public Sprite warriorSprite;
	public Sprite farmerSprite;
	public Sprite civilianSprite;
	private Dictionary<Person.Attribute, Sprite> mProfessionSprites;

	private ToggleGroup mDemandToggleGroup;
	private ToggleGroup mProfessionToggleGroup;
	private ToggleGroup mPeopleToggleGroup;

	private List<GameObject> mUiPeoplePool = new List<GameObject>();
	private List<GameObject> mUiDemandPool = new List<GameObject>();
	private List<GameObject> mUiNotificationPool = new List<GameObject>();

	private Dictionary<Person.Attribute, Toggle> mProfessionToggles = new Dictionary<Person.Attribute, Toggle>();

	private God mGod;
	private PersonManager mPersonManager;

	private List<string> mEventMessages = new List<string>();
	private List<string> mNotificationMessages = new List<string>();
	private List<float> mNotificationDurations = new List<float>();

	// Use this for initialization
	void Awake () {
		mGod = Utilities.GetGod();
		mPersonManager = Utilities.GetPersonManager();
		// create toggle groups for dynaically instantiate toggles.
		mDemandToggleGroup = CreateToggleGroup("demandToggleGroup", true);
		mProfessionToggleGroup = CreateToggleGroup("professionToggleGroup", false);
		mPeopleToggleGroup = CreateToggleGroup("peopleToggleGroup", false);

		mProfessionSprites = new Dictionary<Person.Attribute, Sprite> {
			{Person.Attribute.WARRIOR, warriorSprite},
			{Person.Attribute.FARMER, farmerSprite},
			{Person.Attribute.CIVILIAN, civilianSprite}
		};
		Person.Attribute[] professions = Utilities.GetAttrValues(Person.AttributeType.PROFESSION);
		foreach (Person.Attribute profession in professions) {
			GameObject professionObject = Instantiate(uiProfessionObject);
			professionObject.transform.SetParent(transform.Find("Right/Person/Professions/ProfessionList"), false);
			professionObject.transform.Find("Toggle/Icon").GetComponent<Image>().sprite = mProfessionSprites[profession]; 
			professionObject.transform.Find("Toggle/Name").GetComponent<Text>().text = profession.ToString(); 
			professionObject.transform.Find("Toggle/InfoText").GetComponent<Text>().text = profession.GetDescription(); 
			Toggle toggle = professionObject.GetComponentInChildren<Toggle>();
			toggle.group = mProfessionToggleGroup;
			mProfessionToggles.Add(profession, toggle);
		}
		mProfessionToggles[Person.Attribute.FARMER].isOn = true;
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
				if (GetSelectedTabIndex() == 2) {
					uiPerson.GetComponentInChildren<Toggle>().group = mPeopleToggleGroup;
				}
			} else {
				uiPerson = mUiPeoplePool[i];
			}
			uiPerson.transform.SetParent(peopleContainer);
			// Update visibility
			uiPerson.transform.gameObject.SetActive(i < people.Count);
			// Update text
			if (i < people.Count) {
				string[] descriptionStrings = people[i].GetUIDescription(selectedDemand);
				uiPerson.transform.Find("Toggle/TextTL").GetComponent<Text>().text = descriptionStrings[0];
				uiPerson.transform.Find("Toggle/TextTR").GetComponent<Text>().text = descriptionStrings[1];
				uiPerson.transform.Find("Toggle/BLGroup/Text").GetComponent<Text>().text = descriptionStrings[2];
				Person.Attribute profession = people[i].GetAttribute(Person.AttributeType.PROFESSION);
				uiPerson.transform.Find("Toggle/BLGroup/Icon").GetComponent<Image>().sprite = mProfessionSprites[profession];
				uiPerson.transform.Find("Toggle/BRGroup/Text").GetComponent<Text>().text = descriptionStrings[3];
			}
		}

		// Update the single-person info view
		if (selectedPeople.Count > 0) {
			Person selectedPerson = selectedPeople[0];
			transform.Find("Right/Person/PersonInfo/Text").GetComponent<Text>().text = selectedPerson.GetLongUIDescription();
		}

		// Update the god and demands
		if(mGod != null) {
			string fleetingDemandsTabName = "Fleeting\r\nDemands";
			if (mGod.FleetingDemands.Count > 0) {fleetingDemandsTabName += " (" + mGod.FleetingDemands.Count + ")"; }
			transform.Find("Left/TabGroup/Tab1/Text").GetComponent<Text>().text = fleetingDemandsTabName;
			transform.Find("Left/Demands/Name").GetComponent<Text>().text = mGod.Name;
			// todo: separate short term and long term demands
			List<SacrificeDemand> demands = GetSelectedTabIndex() == 0 ? mGod.FleetingDemands.ConvertAll(fD => fD.mDemand) : mGod.Demands;
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
		string foodString = Utilities.ColorString("Food: " + GameState.FoodSupply + "/" + people.Count, "red", people.Count > GameState.FoodSupply);
		transform.Find("Top/Left/Item1/Text").GetComponent<Text>().text = foodString;
		string armyString = GameState.InvaderSize == 0
			? "Army: " + GameState.ArmySize
			: Utilities.ColorString("Army: " + GameState.ArmySize + "/" + GameState.InvaderSize, "red", GameState.InvaderSize > GameState.ArmySize);
		transform.Find("Top/Left/Item2/Text").GetComponent<Text>().text = armyString;
		transform.Find("Top/Left/Item3/Text").GetComponent<Text>().text = "<placeholder>";
		// Population
		transform.Find("Top/Right/Item1/Text").GetComponent<Text>().text = "Population: " + people.Count + "/" + PersonManager.MAX_POPULATION;
	}

	private void clearSelectedPeople() {
		for(int i = 0; i < mUiPeoplePool.Count; i++)
		{
			GameObject uiPerson = mUiPeoplePool[i];
			Toggle selectedToggle = uiPerson.transform.GetComponentInChildren<Toggle>();
			selectedToggle.isOn = false;
		}
	}

	private void clearSelectedDemands() {
		for(int i = 0; i < mUiDemandPool.Count; i++)
		{
			GameObject uiDemand = mUiDemandPool[i];
			Toggle selectedToggle = uiDemand.transform.GetComponentInChildren<Toggle>();
			selectedToggle.isOn = false;
		}
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
			// todo: have a single method for all demands
			foreach (SacrificeDemand demand in mGod.Demands) {
				if (demand.mId == demandId) {
					return demand;
				}
			}
			foreach (God.FleetingDemand fDemand in mGod.FleetingDemands) {
				if (fDemand.mDemand.mId == demandId) {
					return fDemand.mDemand;
				}
			}
		}
		return null;
	}

	private Person.Attribute getSelectedProfession() {
		foreach (KeyValuePair<Person.Attribute, Toggle> entry in mProfessionToggles) {
			if (entry.Value.isOn) {
				return entry.Key;
			}
		}
		return Person.Attribute.NONE;
	}

	// called when the sacrifice button is clicked
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
		clearSelectedPeople();
	}

	// called when the change profession button is clicked
	public void OnChangeProfession() {
		List<Person> selectedPeople = getSelectedPeople();
		Person.Attribute selectedProfession = getSelectedProfession();
		if (selectedPeople.Count == 0) { return; }
		foreach (Person person in selectedPeople) {
			// note: this could rely on the fact that profession happens to be the last attribute
			for (int i =0; i < person.Attributes.Length; i++) {
				if (person.Attributes[i].GetAttrType() == Person.AttributeType.PROFESSION) {
					person.Attributes[i] = selectedProfession;
				}
			}
			person.ResetLevel();
		}
	}

	public void LogEvent(string message, float duration=4f) {
		message = message.Trim();
		mEventMessages.Add(message);
		string newLogText = "";
		for (int i = 0; i < mEventMessages.Count; i++) {
			newLogText += mEventMessages[i] + "\n";
		}
		transform.Find("Left/Log/Scroll View/Viewport/LogText").GetComponent<Text>().text = newLogText;

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
		var leftPeopleList = transform.Find("Left/People/PeopleList/Viewport/Content");
		var rightPeopleList = transform.Find("Right/People/PeopleList/Viewport/Content");
		return tabIndex == 2 ? leftPeopleList : rightPeopleList;
	}

	private ToggleGroup CreateToggleGroup(string name, bool canUnselect) {
		GameObject emptyObj = new GameObject(name);
		emptyObj.transform.parent = transform;
		ToggleGroup group = emptyObj.AddComponent<ToggleGroup>();
		group.allowSwitchOff = canUnselect;
		return group;
	}

	public void OnTabChanged(bool isTheActiveTab) {
		// only do this logic once.
		if (!isTheActiveTab) { return; }
		clearSelectedPeople();
		clearSelectedDemands();
		// todo: clear selected demand
		int selectedTab = GetSelectedTabIndex();
		if (selectedTab == 2 && mUiPeoplePool.Count > 0) {
			// select the first person
			mUiPeoplePool[0].GetComponentInChildren<Toggle>().isOn = true;
		}
		foreach (GameObject uiPerson in mUiPeoplePool) {
			Toggle toggle = uiPerson.GetComponentInChildren<Toggle>();
			// use a toggle group for the people tab.
			toggle.group = selectedTab == 2 ? mPeopleToggleGroup : null;
		}

		// todo: update selected tab (demand list) and people container connections here
	}
}
