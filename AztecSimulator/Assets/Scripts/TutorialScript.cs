using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour, IDialogCallback {

	// Tutorial 1: teaching profession changing and sacrificing

	// would a more natural flow be to point out the sacrifice first and then 
	// show how the player can change a person's profession in order to satisfy it?
	// maybe even 2 sacrifices? First one is a farmer, no problem, then 2nd one is a warrior?

	// stages, IN ORDER
	private enum TutorialStage
	{
		START,
		INTRO,
		FARMER_INFO,
		CHANGE_TO_WARRIOR,
		CHANGE_TO_WARRIOR_WAIT,
		FOOD_INFO,
		FOOD_INFO_2,
		SACRIFICE_INFO,
		SACRIFICE_INFO_2,
		SACRIFICE_WAIT,
		TUTORIAL_END,
		HEALTH_INFO,
		PROFESSION_INFO,
		CIVILIAN_INFO,
		PAUSE_BUTTON,
		PAUSE_BUTTON_WAIT,
		STARVING_INFO
	}

	private TutorialStage mStage;
	private UIManager mUI;

	// Use this for initialization
	void Start() {

		mStage = TutorialStage.START;
		mUI = Utilities.GetUIManager();

		// Utilities.GetTimingManager().SetPaused(true);
		mUI.ShowMessage("This is a game about raising a tribe under a deity that demands human sacrifices.\n", this);
	}
	
	// Update is called once per frame
	void Update() {

		switch(mStage)
		{
		case TutorialStage.CHANGE_TO_WARRIOR_WAIT:
			{
				List<Person> people = Utilities.GetPersonManager().People;
				if (people.FindAll(p => p.Profession == PersonAttribute.WARRIOR).Count > 0)
				{
					AdvanceTutorial();
				}
				break;
			}
		case TutorialStage.SACRIFICE_WAIT:
			break;
		case TutorialStage.PAUSE_BUTTON_WAIT:
			{
				if(GameState.GameSpeed != 0)
				{
					AdvanceTutorial();
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
		mStage = mStage + 1;

		switch(mStage)
		{
		case TutorialStage.INTRO:
			Debug.Log("me: " + transform.name);
			ShowHighlight("PeopleHighlight", true);
			mUI.ShowMessage("The left side of the screen shows the people in your tribe.", this);
			break;
		case TutorialStage.FARMER_INFO:
			ShowHighlight("PeopleHighlight", false);
			ShowHighlight("FoodHighlight", true);
			mUI.ShowMessage("Right now, both of your people are farmers. They are providing food for your tribe.", this);
			break;
		case TutorialStage.CHANGE_TO_WARRIOR:
			mUI.ShowMessage("You can change a person's profession by dragging them to another area on the left.\n" +
				"Change one of them to be a warrior.", this);
			break;
		case TutorialStage.FOOD_INFO:
			mUI.ShowMessage("Now that you only have one farmer, your food production has dropped.", this);
			break;
		case TutorialStage.FOOD_INFO_2:
			mUI.ShowMessage("Each person in your tribe requires 1 food, so you are still producing enough.", this);
			break;
		case TutorialStage.SACRIFICE_INFO:
			ShowHighlight("Demands", true);
			mUI.ShowMessage("You will need to sacrifice people from your tribe to satisfy your god's demands.", this);
			break;
		case TutorialStage.SACRIFICE_INFO_2:
			mUI.ShowMessage("Your god is demanding that you sacrifice a warrior.\n" +
				"Drag your warrior from the left over to the sacrifice on the right.", this);
			break;
		case TutorialStage.HEALTH_INFO:
			// maybe there's no reason to do this so soon?
			transform.Find("LifeforceHighlight").gameObject.SetActive(true);
			mUI.ShowMessage("This is a person's lifeforce. If it reaches 0, they die.\n", this);
			break;
		case TutorialStage.PROFESSION_INFO:
			transform.Find("LifeforceHighlight").gameObject.SetActive(false);
			transform.Find("ProfessionHighlight").gameObject.SetActive(true);
			mUI.ShowMessage("This is a person's profession. Different professions provide different benefits to your tribe.", this);
			break;		
		case TutorialStage.CIVILIAN_INFO:
			mUI.ShowMessage("Increasing your population is important. Be sure to always have at least one civilian, or your tribe will stagnate.", this);
			break;
		case TutorialStage.PAUSE_BUTTON:
			mUI.ShowMessage("You can pause and unpause the game with this button. Right now the game is paused. Click it to unpause.", this);
			break;
		}
	}

	private void ShowHighlight(string name, bool show)
	{
		Transform t = transform.Find(name);
		if(t != null)
		{
			t.gameObject.SetActive(show);
		}
	}
}
