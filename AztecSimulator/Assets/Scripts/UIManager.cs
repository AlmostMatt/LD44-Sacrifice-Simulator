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

	private List<GameObject> mUiPeoplePool = new List<GameObject>();
    private List<GameObject> mUiDemandPool = new List<GameObject>();
    private List<GameObject> mUiDemandPool2 = new List<GameObject>();
    private List<GameObject> mUiNotificationPool = new List<GameObject>();
	private List<GameObject> mUiOngoingPool = new List<GameObject>();

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
    }

    void UpdateRenderables<T>(List<T> renderables, GameObject newObject, List<GameObject> objectPool, Transform uiContainer, System.Action<GameObject> onCreateCallback = null)
		where T : IRenderable {
		for(int i = 0; i < Mathf.Max(renderables.Count, objectPool.Count); i++)
		{
			GameObject uiObject;
			// Spawn new UI person
			if (i >= objectPool.Count) {
				uiObject = Instantiate(newObject);
				objectPool.Add(uiObject);
                if (onCreateCallback != null)
                {
                    onCreateCallback(uiObject);
                }
			} else {
				uiObject = objectPool[i];
			}
			uiObject.transform.SetParent(uiContainer);
			// Update visibility
			uiObject.transform.gameObject.SetActive(i < renderables.Count);
			// Update text
			if (i < renderables.Count) {
				renderables[i].RenderTo(uiObject);
			}
		}
	}

    void Update () {
		List<Person> people = mPersonManager.People;
		Transform peopleContainer = GetPeopleContainer(Person.Attribute.FARMER);
		UpdateRenderables(people, uiPersonObject, mUiPeoplePool, peopleContainer);

		// Update the god and demands
		if(mGod != null) {
            // TODO: show the god name somewhere			
            //transform.Find("Left/Demands/Name").GetComponent<Text>().text = mGod.Name;
            Transform fleetingDemandContainer = transform.Find("Center (H)/DemandGroups/V/Fleeting/G");
            Transform permanentDemandContainer = transform.Find("Center (H)/DemandGroups/V/Permanent/G");
            UpdateRenderables(mGod.FleetingDemands, uiDemandObject, mUiDemandPool, fleetingDemandContainer);
            UpdateRenderables(mGod.PermanentDemands, uiDemandObject, mUiDemandPool2, permanentDemandContainer);
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
		UpdateRenderables(GameState.Ongoings, uiOngoingObject, mUiOngoingPool, ongoingContainer);

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

	// called when the sacrifice button is clicked
	public void OnSacrifice() {
        // TODO: sacrifice relevant people
        List<Person> selectedPeople = new List<Person>();//getSelectedPeople();
		Debug.Log("Sacrificing " + selectedPeople.Count + " people.");
		if (selectedPeople.Count == 0) { return; }

		if(mGod != null) {
			string sacrificedNames = Utilities.ConcatStrings(selectedPeople.ConvertAll(person => person.Name));
			LogEvent("You sacrifice " + sacrificedNames + " to " + mGod.Name, 1f);
			//mGod.MakeSacrifice(relevantDemand, selectedPeople);
		}
	}

	// Called when a person is dropped on a profession area
	public void OnChangeProfession(Person person, Person.Attribute newProfession) {
        person.ChangeProfession(newProfession);
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

	private Transform GetPeopleContainer(Person.Attribute profession) {
        return transform.Find("Center (H)/ProfessionGroups/V/" + profession.ToString() + "/G");
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
