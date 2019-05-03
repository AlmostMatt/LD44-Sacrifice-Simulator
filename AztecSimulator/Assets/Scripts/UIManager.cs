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
	public GameObject uiOngoingObject;

	private ToggleGroup mDemandToggleGroup;
	private ToggleGroup mProfessionToggleGroup;
	private ToggleGroup mPeopleToggleGroup;

	private List<GameObject> mUiPeoplePool = new List<GameObject>();
	private List<GameObject> mUiDemandPool = new List<GameObject>();
	private List<GameObject> mUiNotificationPool = new List<GameObject>();
	private List<GameObject> mUiOngoingPool = new List<GameObject>();

	private Dictionary<Person.Attribute, Toggle> mProfessionToggles = new Dictionary<Person.Attribute, Toggle>();

	private God mGod;
	private PersonManager mPersonManager;
	private SpriteManager mSpriteManager;

	private List<string> mEventMessages = new List<string>();
	private List<bool> mNotificationIsGod = new List<bool>();
	private List<string> mNotificationMessages = new List<string>();
	private List<float> mNotificationDurations = new List<float>();

	// For tab colors
	private static Color sAttentionColor = new Color(255f / 255f, 89f / 255f, 111f / 255f);
	private static Color sSelectedColor = new Color(118f/255f, 242f/255f, 172/255f);
	private static Color sNormalColor = new Color(1f, 1f, 1f, 150f/255f);

	// Use this for initialization
	void Awake () {
		mGod = Utilities.GetGod();
		mPersonManager = Utilities.GetPersonManager();
		mSpriteManager = Utilities.GetSpriteManager();
		// create toggle groups for dynaically instantiate toggles.
		mDemandToggleGroup = CreateToggleGroup("demandToggleGroup", true);
		mProfessionToggleGroup = CreateToggleGroup("professionToggleGroup", false);
		mPeopleToggleGroup = CreateToggleGroup("peopleToggleGroup", false);

		Person.Attribute[] professions = Utilities.GetAttrValues(Person.AttributeType.PROFESSION);
		foreach (Person.Attribute profession in professions) {
			GameObject professionObject = Instantiate(uiProfessionObject);
			professionObject.transform.SetParent(transform.Find("Right/Person/Content/Professions/ProfessionList"), false);
			professionObject.transform.Find("Toggle/Icon").GetComponent<Image>().sprite = mSpriteManager.GetSprite(profession); 
			professionObject.transform.Find("Toggle/Name").GetComponent<Text>().text = profession.ToString(); 
			professionObject.transform.Find("Toggle/InfoText").GetComponent<Text>().text = profession.GetDescription(); 
			Toggle toggle = professionObject.GetComponentInChildren<Toggle>();
			toggle.group = mProfessionToggleGroup;
			mProfessionToggles.Add(profession, toggle);
		}
		mProfessionToggles[Person.Attribute.FARMER].isOn = true;

		OnTabChanged(true); // set active tab color initially
	}

	void Update () {
		List<Person> selectedPeople = getSelectedPeople();
		God.GodDemand selectedDemand = getSelectedDemand();
		// Only allow sacrificing if either no demand is selected or all of the required conditions are met
		bool canSacrifice = selectedPeople.Count > 0 && (selectedDemand == null || selectedDemand.mDemand.CheckSatisfaction(selectedPeople));
		transform.Find("Right/People/SacrificeButton").GetComponent<Button>().interactable = canSacrifice;

		// TODO: define a renderable interface, and generalize each of these loops to updateRenderable
		// Ongoing is an example of a renderable

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
				string[] descriptionStrings = people[i].GetUIDescription(selectedDemand != null ? selectedDemand.mDemand : null);
				uiPerson.transform.Find("Toggle/TextTL").GetComponent<Text>().text = descriptionStrings[0];
				uiPerson.transform.Find("Toggle/TextTR").GetComponent<Text>().text = descriptionStrings[1];
				uiPerson.transform.Find("Toggle/BLGroup/Text").GetComponent<Text>().text = descriptionStrings[2];
				Person.Attribute profession = people[i].GetAttribute(Person.AttributeType.PROFESSION);
				uiPerson.transform.Find("Toggle/BLGroup/Icons/Icon1").gameObject.SetActive(selectedDemand != null && selectedDemand.mDemand.IsRelevantAttribute(profession));
				uiPerson.transform.Find("Toggle/BLGroup/Icons/Icon2").GetComponent<Image>().sprite = mSpriteManager.GetSprite(profession);
				uiPerson.transform.Find("Toggle/BRGroup/Text").GetComponent<Text>().text = descriptionStrings[3];
			}
		}

		// Update the single-person info view
		if (selectedPeople.Count > 0) {
			Person selectedPerson = selectedPeople[0];
			transform.Find("Right/Person/Content/PersonInfo/Text").GetComponent<Text>().text = selectedPerson.GetLongUIDescription();
		}

		// Update the god and demands
		if(mGod != null) {
			string fleetingDemandsTabName = "Fleeting\r\nDemands";
			if (mGod.FleetingDemands.Count > 0) {
				fleetingDemandsTabName += " (" + mGod.FleetingDemands.Count + ")"; 
			}
			if(GetSelectedTabIndex() != 0)
			{
				Transform tab1 = transform.Find("Left/TabGroup/Tab1");
				tab1.GetComponentInChildren<Image>().color = mGod.FleetingDemands.Count > 0 ?  sAttentionColor : sNormalColor;
			}

			transform.Find("Left/TabGroup/Tab1/Text").GetComponent<Text>().text = fleetingDemandsTabName;
			transform.Find("Left/Demands/Name").GetComponent<Text>().text = mGod.Name;
			// todo: separate short term and long term demands
			List<God.GodDemand> demands = GetSelectedTabIndex() == 0 ? mGod.FleetingDemands : mGod.PermanentDemands;
			Transform demandContainer = transform.Find("Left/Demands/DemandList/Viewport/Content");
			int renewCount = 0;
			int nonRenewCount = 0;
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
				// Update visibility
				uiDemand.transform.gameObject.SetActive(i < demands.Count);
				// Update text and store id in name
				if (i < demands.Count) {
					
					// Update position
					RectTransform rt = uiDemand.GetComponent<RectTransform>();
					if(demands[i].mTimeLeft >= 0)
					{
						// Relying on GridLayout for now
						//rt.anchoredPosition = new Vector2(0f, 90f * (i - (demands.Count - 1) / 2f));	
					}
					else
					{
						float left = -120f;
						float right = 120f;
						float x;
						float y;
						if(demands[i].mIsRenewable) {
							y = 90f * renewCount++;
							x = left;
						} else {
							y = 90f * nonRenewCount++;
							x = right;
						}
						// Relying on GridLayout for now
						//rt.anchoredPosition = new Vector2(x, y - 150f);
					}

					uiDemand.name = demands[i].mId.ToString();
					uiDemand.GetComponent<HoverInfo>().SetText(demands[i].GetResultDescription());
					string[] demandStrings = demands[i].GetUIDescriptionStrings(selectedPeople);
					string[] demandIconNames = demands[i].GetUIDescriptionIconNames();
					int numRows = Mathf.Min(demandStrings.Length / 2, demandIconNames.Length);
					Transform uiDemandVGroup = uiDemand.transform.Find("VGroup");
					for (int rowI = 0; rowI < Mathf.Max(numRows, uiDemandVGroup.childCount); rowI++) {
						Transform row;
						if (rowI >= uiDemandVGroup.childCount) {
							row = Instantiate(uiDemandVGroup.GetChild(0));
							row.SetParent(uiDemandVGroup, false);
						} else {
							row = uiDemandVGroup.GetChild(rowI);
						}
						row.gameObject.SetActive(rowI < numRows);
						if (rowI < numRows) {
							row.GetChild(0).GetComponent<Text>().text = demandStrings[2 * rowI];
							row.GetChild(1).GetComponent<Image>().sprite = mSpriteManager.GetSprite(demandIconNames[rowI]);
							row.GetChild(1).gameObject.SetActive(demandIconNames[rowI] != "" && demandIconNames[rowI] != "NONE");
							row.GetChild(2).GetComponent<Text>().text = demandStrings[2 * rowI + 1];
						}
					}
				}
			}
		}

		// Update notification objects
		// UPDATE: this now only shows and updates the timer of the first (oldest) notification
		Transform notificationContainer = transform.Find("BRCorner");
		Transform godContainer = transform.Find("BLCorner");
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

			if(i < mNotificationMessages.Count)
			{
				if(mNotificationIsGod[i])
				{
					uiNotification.transform.SetParent(godContainer);
					uiNotification.GetComponent<Image>().color = Color.gray;
				}
				else
				{
					uiNotification.transform.SetParent(notificationContainer);
					uiNotification.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.8f);
				}
			}

			// Update position
			RectTransform rt = uiNotification.GetComponent<RectTransform>();
			// TODO: slide down if others have faded
			rt.anchoredPosition = new Vector2(0f,100f*i);
			// Update visibility
			uiNotification.transform.gameObject.SetActive(i ==0 && i < mNotificationMessages.Count);
			// Update text and alpha
			if (i < mNotificationMessages.Count) {
				uiNotification.transform.Find("Text").GetComponent<Text>().text = mNotificationMessages[i]; 
				// Fade out over the last 0.25s
				uiNotification.GetComponent<CanvasGroup>().alpha = Mathf.Min(1f, mNotificationDurations[i] * 4f);
			}
		}
		if (mNotificationMessages.Count > 0) {
			mNotificationDurations[0] -= Time.deltaTime;
			if (mNotificationDurations[0] <= 0f) {
				mNotificationIsGod.RemoveAt(0);
				mNotificationMessages.RemoveAt(0);
				mNotificationDurations.RemoveAt(0);
			}
		}

		// Update ongoing objects
		Transform ongoingContainer = transform.Find("TRCorner/OngoingGroup");
		// TODO: maintain a list of Ongoing structs
		//List<Ongoing> ongoings = new List<Ongoing>();
		//ongoings.Add(new Ongoing("WARRIOR", "Ongoing!", "4 Enemy Warriors are approaching!", 92.1f, true));
		List<Ongoing> ongoings = GameState.Ongoings;
		for(int i = 0; i < Mathf.Max(ongoings.Count, mUiOngoingPool.Count); i++)
		{
			GameObject uiOngoing;
			// Spawn new UI ongoing
			if (i >= mUiOngoingPool.Count) {
				uiOngoing = Instantiate(uiOngoingObject);
				mUiOngoingPool.Add(uiOngoing);
				uiOngoing.transform.SetParent(ongoingContainer);
			} else {
				uiOngoing = mUiOngoingPool[i];
			}
			// Update visibility
			uiOngoing.transform.gameObject.SetActive(i < ongoings.Count);
			// Update text and alpha
			if (i < ongoings.Count) {
				ongoings[i].RenderTo(uiOngoing);
			}
		}

		// Update the top UI bar
		string foodString = Utilities.ColorString("Food: " + GameState.FoodSupply + "/" + people.Count, "red", people.Count > GameState.FoodSupply);
		transform.Find("Top/Left/Item1/Text").GetComponent<Text>().text = foodString;
		string armyString = GameState.InvaderSize == 0
			? "Army: " + GameState.ArmyStrength
			: Utilities.ColorString("Army: " + GameState.ArmyStrength + "/" + GameState.InvaderSize, "red", GameState.InvaderSize > GameState.ArmyStrength);
		transform.Find("Top/Left/Item2/Text").GetComponent<Text>().text = armyString;
		string birthString1 = float.IsInfinity(GameState.TimeBetweenBirths) ? "Birth rate is 0" : string.Format("Birth every {0:0.0}s", GameState.TimeBetweenBirths);
		string birthString2 = mPersonManager.People.Count == PersonManager.MAX_POPULATION
			? "(max population)"
			: (float.IsInfinity(GameState.TimeUntilBirth)
				? "(need civilians)"
				: string.Format("Next in {0:0.0}s", GameState.TimeUntilBirth));
		transform.Find("Top/Left/Item3/Text").GetComponent<Text>().text = birthString1 + "\r\n" + birthString2;
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

	private God.GodDemand getSelectedDemand() {
		int demandId = getSelectedDemandId();
		if(mGod != null && demandId != 0) {
			foreach (God.GodDemand demand in mGod.Demands) {
				if (demand.mId == demandId) {
					return demand;
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
			LogEvent("You sacrifice " + sacrificedNames + " to " + mGod.Name, 1f);
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
			person.ChangeProfession(selectedProfession);
		}
	}

	public void LogEvent(string message, float duration=2f, bool isGod=false) {
		message = message.Trim();
		mEventMessages.Add(message);
		string newLogText = "";
		for (int i = 0; i < mEventMessages.Count; i++) {
			// Bullet point
			newLogText += "\u2022 " + mEventMessages[i] + "\r\n";
		}
		transform.Find("Left/Log/Scroll View/Viewport/LogText").GetComponent<Text>().text = newLogText.Trim();

		// Only notify for events that have a non-zero duration.
		if (duration > 0f) {
			mNotificationIsGod.Add(isGod);
			mNotificationMessages.Add(isGod ? "<b>" + message + "</b>" : message);
			mNotificationDurations.Add(duration);
		}
	}

	private int GetSelectedTabIndex() {
		Transform tabGroup = transform.Find("Left/TabGroup");
		int tabIdx = 0;
		foreach(Transform t in tabGroup)
		{
			if (t.GetComponentInChildren<Toggle>().isOn) {
				return tabIdx;
			}
			tabIdx++;
		}
		Debug.Log("WARNING: No tab selected");
		return 0;
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
		int selectedTabIndex = GetSelectedTabIndex();
		Transform tabGroup = transform.Find("Left/TabGroup");
		int tabIdx = 0;
		foreach(Transform t in tabGroup)
		{
			t.GetComponentInChildren<Image>().color = tabIdx == selectedTabIndex ?  sSelectedColor : sNormalColor;
			tabIdx++;
		}
	}
}
