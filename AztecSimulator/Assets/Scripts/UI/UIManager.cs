using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // for First()
using UnityEngine.SceneManagement;

// Creates and manages UI objects.
public class UIManager : MonoBehaviour {
    private static Color GOD_MSG_COLOR = new Color(1f, .9f, .9f, 0.9f);
    private static Color NOTIF_COLOR = new Color(.9f, .9f, 1f, 0.9f);

    public GameObject uiNotificationObject;
	public GameObject uiPersonObject;
	public GameObject uiDemandObject;
	public GameObject uiProfessionObject;
	public GameObject uiOngoingObject;
    public GameObject uiDividerObject;
    public GameObject uiProfessionAreaObject;

    private List<GameObject> mUiPeoplePool = new List<GameObject>();
    private BidirectionalMap<Person, GameObject> mUiPeopleMap = new BidirectionalMap<Person, GameObject>();
    private List<GameObject> mUiDemandPool = new List<GameObject>();
    private BidirectionalMap<GodDemand, GameObject> mUiDemandMap = new BidirectionalMap<GodDemand, GameObject>();
    private List<GameObject> mUiNotificationPool = new List<GameObject>();
	private List<GameObject> mUiOngoingPool = new List<GameObject>();
    private BidirectionalMap<Ongoing, GameObject> mUiOngoingMap = new BidirectionalMap<Ongoing, GameObject>();

    private God mGod;
	private PersonManager mPersonManager;
	private SpriteManager mSpriteManager;

	private List<string> mEventMessages = new List<string>();
	private List<bool> mNotificationIsGod = new List<bool>();
	private List<string> mNotificationMessages = new List<string>();
	private List<float> mNotificationDurations = new List<float>();

	private IDialogCallback mDialogCallback;
	private struct DialogMessage
	{
		public string msg;
		public IDialogCallback c;
		public DialogMessage(string message, IDialogCallback callback)
		{
			msg = message;
			c = callback;
		}
	};
	private List<DialogMessage> mPendingDialogMessages = new List<DialogMessage>();
	private bool mDialogOpen = false;

	// For tab colors
	private static Color sAttentionColor = new Color(255f / 255f, 89f / 255f, 111f / 255f);
	private static Color sSelectedColor = new Color(118f/255f, 242f/255f, 172/255f);
	private static Color sNormalColor = new Color(1f, 1f, 1f, 150f/255f);

	// Use this for initialization
	void Awake () {
		mGod = Utilities.GetGod();
		mPersonManager = Utilities.GetPersonManager();
		mSpriteManager = Utilities.GetSpriteManager();

        Transform professionAreaHolder = transform.Find("Center (H)/ProfessionGroups/V");
        bool isFirstArea = true;
        foreach (PersonAttribute profession in GameState.Scenario.AvailableProfessions)
        {
            if (!isFirstArea)
            {
                Instantiate(uiDividerObject, professionAreaHolder);
            }
            GameObject professionArea = Instantiate(uiProfessionAreaObject, professionAreaHolder);
            professionArea.GetComponent<ProfessionArea>().SetProfession(profession);
            isFirstArea = false;
        }
    }

    void UpdateRenderables<T>(
        List<T> renderables,
        GameObject newObject,
        List<GameObject> objectPool,
        BidirectionalMap<T, GameObject> renderableObjectMap,
        Transform uiContainer,
        System.Action<GameObject> onCreateCallback = null)
		where T : IRenderable
    {
        // Put all GameObjects in a set, and remove the objects that are still in used
        HashSet<GameObject> unusedObjects = new HashSet<GameObject>(renderableObjectMap.Values);
        foreach (T renderable in renderables)
		{
			GameObject uiObject;
			// Spawn new UI person
			if (!renderableObjectMap.ContainsKey(renderable)) {
                if (objectPool.Count > 0)
                {
                    uiObject = objectPool[objectPool.Count - 1];
                    objectPool.RemoveAt(objectPool.Count - 1);
                    uiObject.SetActive(true);
                } else
                {
                    uiObject = Instantiate(newObject);
                }
                uiObject.transform.SetParent(uiContainer, false);
                renderableObjectMap.Add(renderable, uiObject);
                if (onCreateCallback != null)
                {
                    onCreateCallback(uiObject);
                }
			} else {
				uiObject = renderableObjectMap.GetValue(renderable);
                unusedObjects.Remove(uiObject);
            }
            renderable.RenderTo(uiObject);
		}
        foreach (GameObject uiObject in unusedObjects)
        {
            renderableObjectMap.RemoveValue(uiObject);
            objectPool.Add(uiObject);
            uiObject.SetActive(false);
            // Undo any scaling of this object before it is reused
            uiObject.transform.localScale = new Vector3(1f, 1f, 1f);
            uiObject.transform.SetParent(null, false);
        }
	}

    void Update () {
		List<Person> people = mPersonManager.People;
		UpdateRenderables(people, uiPersonObject, mUiPeoplePool, mUiPeopleMap, null, InitializeUIPerson);

		// Update the god and demands
		if(mGod != null) {
            // TODO: show the god name somewhere			
            //transform.Find("Left/Demands/Name").GetComponent<Text>().text = mGod.Name;
            UpdateRenderables(mGod.Demands, uiDemandObject, mUiDemandPool, mUiDemandMap, null, InitializeUIDemand);
        }

        // TODO: define a class to represent a notification message, and have it implement IRenderable
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
				uiNotification.transform.SetParent(notificationContainer, false);
			} else {
				uiNotification = mUiNotificationPool[i];
			}

			if(i < mNotificationMessages.Count)
			{
				if(mNotificationIsGod[i])
				{
					uiNotification.transform.SetParent(godContainer, false);
					uiNotification.GetComponent<Image>().color = GOD_MSG_COLOR;
				}
				else
				{
					uiNotification.transform.SetParent(notificationContainer, false);
					uiNotification.GetComponent<Image>().color = NOTIF_COLOR;
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
		UpdateRenderables(GameState.Ongoings, uiOngoingObject, mUiOngoingPool, mUiOngoingMap, ongoingContainer);

        // Update the top UI bar
        string foodString = string.Format("Food: {0:0.#} / {1}", GameState.FoodSupply, people.Count);
		transform.Find("Top/Left/Item1/Text").GetComponent<Text>().text = Utilities.ColorString(foodString, "red", people.Count > GameState.FoodSupply);
		string armyString = GameState.InvaderSize == 0
			? string.Format("Army: {0:0.#}", GameState.ArmyStrength)
			: Utilities.ColorString(string.Format("Army: {0:0.#} / {1}", GameState.ArmyStrength, GameState.InvaderSize), "red", GameState.InvaderSize > GameState.ArmyStrength);
		transform.Find("Top/Left/Item2/Text").GetComponent<Text>().text = armyString;
		string birthString1 = float.IsInfinity(GameState.TimeBetweenBirths) ? "Birth rate is 0" : string.Format("Birth every {0:0.#}s", GameState.TimeBetweenBirths);
		string birthString2 = mPersonManager.People.Count == PersonManager.MAX_POPULATION
			? "(max population)"
			: (float.IsInfinity(GameState.TimeUntilBirth)
				? "(need civilians)"
				: string.Format("Next in {0:0.0}s", GameState.TimeUntilBirth));
		transform.Find("Top/Left/Item3/Text").GetComponent<Text>().text = birthString1 + "\r\n" + birthString2;
		// Population
		transform.Find("Top/Right/Item1/Text").GetComponent<Text>().text = "Population: " + people.Count + "/" + PersonManager.MAX_POPULATION;

		if(mPendingDialogMessages.Count > 0 && !mDialogOpen)
		{
			// messages pause the game by default. could make this optional if we need to.
			Utilities.SetGamePaused(true);
			// TODO: disable pause button too

            mDialogOpen = true;
            DialogMessage dm = mPendingDialogMessages[0];
			mDialogCallback = dm.c;

			Transform popDialog = transform.Find("PopupDialog");
			popDialog.gameObject.SetActive(true);
			popDialog.Find("DialogText").GetComponent<Text>().text = dm.msg;

			mPendingDialogMessages.RemoveAt(0);
		}
    }

    public GameObject GetUiDemand(GodDemand demand)
    {
        if (mUiDemandMap.ContainsKey(demand))
        {
            return mUiDemandMap.GetValue(demand);
        }
        return null;
    }

    public GameObject GetUiPerson(Person person)
    {
        if (mUiPeopleMap.ContainsKey(person)) {
            return mUiPeopleMap.GetValue(person);
        }
        return null;
    }

    public List<Person> GetPeopleBeingDragged()
    {
        List<Person> result = new List<Person>();
        // When an object is being dragged the parent is the canvas.
        foreach (GameObject uiPerson in mUiPeopleMap.Values)
        {
            if (uiPerson.transform.parent == Utilities.GetCanvas())
            {
                result.Add(mUiPeopleMap.GetKey(uiPerson));
            }
        }
        return result;
    }

    public List<GodDemand> GetPartiallyCompletedDemands()
    {
        List<GodDemand> result = new List<GodDemand>();
        foreach (GodDemand demand in mGod.Demands)
        {
            if (demand.GetAssociatedPeople().Count > 0)
            {
                result.Add(demand);
            }
        }
        return result;
    }

    // Called when a GameObject is dropped on a profession area
    // Returns false if the dropped object is not a person.
    public bool OnChangeProfession(GameObject uiObject, PersonAttribute newProfession)
    {
        if (!mUiPeopleMap.ContainsValue(uiObject))
        {
            return false;
        }
        mUiPeopleMap.GetKey(uiObject).ChangeProfession(newProfession);
        return true;
    }

    // Called when a GameObject is dropped on a demand slot, whether or not it is a valid drop
    // Returns the list of person slots that are relevant.
    public List<int> OnDropPersonOnDemand(GameObject uiPerson, GameObject uiDemand)
    {
        if (!mUiPeopleMap.ContainsValue(uiPerson))
        {
            return new List<int>();
        }
        return mUiDemandMap.GetKey(uiDemand).GetRelevantSlots(mUiPeopleMap.GetKey(uiPerson));
    }

    // Called immediately after a person is successfully dropped on a demand slot
    public void MaybeSacrifice(GameObject uiDemandObject)
    {
        GodDemand demand = mUiDemandMap.GetKey(uiDemandObject);
        if (demand.IsSatisfied())
        {
            List<Person> peopleToSacrifice = demand.GetAssociatedPeople().ConvertAll(obj => mUiPeopleMap.GetKey(obj));
            Debug.Log("Sacrificing " + peopleToSacrifice.Count + " people.");
            if (mGod != null)
            {
                string sacrificedNames = Utilities.ConcatStrings(peopleToSacrifice.ConvertAll(person => person.Name));
                LogEvent("You sacrifice " + sacrificedNames + " to " + mGod.Name, Utilities.SHORT_LOG_DURATION);
                mGod.MakeSacrifice(demand, peopleToSacrifice);
            }

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
        // TODO: have a log in the scene again
		// transform.Find("Left/Log/Scroll View/Viewport/LogText").GetComponent<Text>().text = newLogText.Trim();

		// Only notify for events that have a non-zero duration.
		if (duration > 0f) {
			mNotificationIsGod.Add(isGod);
			mNotificationMessages.Add(isGod ? "<b>" + message + "</b>" : message);
			mNotificationDurations.Add(duration);
		}
	}

    public void GoBackToMenu()
    {
        // Go back to menu after current event log queue completes
        float totalEventLogQueue = 0f;
        foreach (float f in mNotificationDurations)
        {
            totalEventLogQueue += f;
        }
        Invoke("GoBackToMenuInternal", totalEventLogQueue);
    }

    private void GoBackToMenuInternal()
    {
        GameState.Scenario = null;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

	public int GetSelectedTabIndex() {
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

    private void InitializeUIPerson(GameObject uiPerson)
    {
        Person person = mUiPeopleMap.GetKey(uiPerson);
        string container = "Center (H)/ProfessionGroups/V/" + person.Profession.ToString() + "/Scrollable/Viewport/Content (G)";
        uiPerson.transform.SetParent(transform.Find(container), false);
    }

    private void InitializeUIDemand(GameObject uiDemand)
    {
        GodDemand demand = mUiDemandMap.GetKey(uiDemand);
        string groupName = "Permanent";
        if (demand.GroupId != -1)
        {
            groupName = "Featured";
        }
        else if (demand.IsTemporary)
        {
            groupName = "Fleeting";
        }
        uiDemand.transform.SetParent(transform.Find("Center (H)/DemandGroups/V/" + groupName + "/Viewport/Content (G)"), false);
    }

	public void OnTabChanged(bool isTheActiveTab) {
		// only do this logic once.
		if (!isTheActiveTab) { return; }
		int selectedTab = GetSelectedTabIndex();
	}

	public void ShowMessage(string s, IDialogCallback callback)
	{
		mPendingDialogMessages.Add(new DialogMessage(s, callback));
	}

	public void OnCloseDialog()
	{
		Transform t = transform.Find("PopupDialog");
		t.gameObject.SetActive(false);
        mDialogOpen = false;

        if(mDialogCallback != null)
		{
			mDialogCallback.OnDialogClosed();
			mDialogCallback = null;
		}
	}

	public void SetPauseButtonEnabled(bool enabled)
	{
		// TODO
	}

	public void OnGamePaused(bool paused)
	{
		// TODO: update pause button. Probably don't even want the pause button to be a toggle anymore, since the game can be paused through other ways
		// so it might as well just be a button with an image that changes whenever the game is paused/unpaused
	}
}
