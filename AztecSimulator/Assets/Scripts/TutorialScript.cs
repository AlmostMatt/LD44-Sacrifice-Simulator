using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour, IDialogCallback {

	private enum TutorialStage
	{
		INTRO,
		SELECT_PEOPLE,
		HEALTH_INFO,
		PROFESSION_INFO,
		FARMER_INFO,
		CHANGE_TO_CIVILIAN,
		CHANGE_TO_CIVILIAN_WAIT,
		CIVILIAN_INFO,
		PAUSE_BUTTON,
		PAUSE_BUTTON_WAIT,
		STARVING_INFO
	}

	private TutorialStage mStage;
	private UIManager mUI;

	// Use this for initialization
	void Start() {

		mStage = TutorialStage.INTRO;
		mUI = Utilities.GetUIManager();

		// Utilities.GetTimingManager().SetPaused(true);
		mUI.ShowMessage("This is a game about raising a tribe under a deity that demands human sacrifices.\n", this);
	}
	
	// Update is called once per frame
	void Update() {

		switch(mStage)
		{
		case TutorialStage.SELECT_PEOPLE:
			{
				int selectedTab = mUI.GetSelectedTabIndex();
				if(selectedTab == 2)
				{
					transform.Find("PeopleHighlight").gameObject.SetActive(false);
					mUI.ShowMessage("This is where you can see the people in your tribe.\n" +
					"You can select people from the left list and see their details on the right.", this);
					mStage = TutorialStage.HEALTH_INFO;
				}
				break;
			}
		case TutorialStage.CHANGE_TO_CIVILIAN_WAIT:
			{
				List<Person> people = Utilities.GetPersonManager().People;
				if
				(
						people[0].GetAttribute(PersonAttributeType.PROFESSION) == PersonAttribute.FARMER
					&&  people[1].GetAttribute(PersonAttributeType.PROFESSION) == PersonAttribute.CIVILIAN
				)
				{
					mUI.ShowMessage("Civilians increase the birthrate of your tribe.", this);
					mStage = TutorialStage.CIVILIAN_INFO;
				}
				break;
			}
		case TutorialStage.PAUSE_BUTTON_WAIT:
			{
				if(GameState.GameSpeed != 0)
				{
					mStage = TutorialStage.STARVING_INFO;
				}
			}
			break;
		}
	}

	public void OnDialogClosed()
	{
		AdvanceTutorial();
	}

	private void AdvanceTutorial()
	{
		// TODO: can this be coroutine'd?
		switch(mStage)
		{
		case TutorialStage.INTRO:
			Debug.Log("me: " + transform.name);
			transform.Find("PeopleHighlight").gameObject.SetActive(true);
			mUI.ShowMessage("Click the PEOPLE tab to see the people in your tribe.\n", this);
			mStage = TutorialStage.SELECT_PEOPLE;
			break;
		case TutorialStage.HEALTH_INFO:
			// maybe there's no reason to do this so soon?
			transform.Find("LifeforceHighlight").gameObject.SetActive(true);
			mUI.ShowMessage("This is a person's lifeforce. If it reaches 0, they die.\n", this);
			mStage = TutorialStage.PROFESSION_INFO;
			break;
		case TutorialStage.PROFESSION_INFO:
			transform.Find("LifeforceHighlight").gameObject.SetActive(false);
			transform.Find("ProfessionHighlight").gameObject.SetActive(true);
			mUI.ShowMessage("This is a person's profession. Different professions provide different benefits to your tribe.", this);
			mStage = TutorialStage.FARMER_INFO;
			break;
		case TutorialStage.FARMER_INFO:
			transform.Find("ProfessionHighlight").gameObject.SetActive(false);
			mUI.ShowMessage("Right now both of your people are farmers. Farmers provide food for your tribe. Each person requires 1 food.", this);
			mStage = TutorialStage.CHANGE_TO_CIVILIAN;
			break;
		case TutorialStage.CHANGE_TO_CIVILIAN:
			mUI.ShowMessage("You can change a person's profession. Select the second person and change them to be a civilian.", this);
			Utilities.SetGamePaused(true, true);
			mStage = TutorialStage.CHANGE_TO_CIVILIAN_WAIT;
			break;
		case TutorialStage.CIVILIAN_INFO:
			mUI.ShowMessage("Increasing your population is important. Be sure to always have at least one civilian, or your tribe will stagnate.", this);
			mStage = TutorialStage.PAUSE_BUTTON;
			break;
		case TutorialStage.PAUSE_BUTTON:
			mUI.ShowMessage("You can pause and unpause the game with this button. Right now the game is paused. Click it to unpause.", this);
			mStage = TutorialStage.PAUSE_BUTTON_WAIT;
			break;
		}
	}
}
