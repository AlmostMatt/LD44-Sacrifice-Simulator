using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem : MonoBehaviour {

	public abstract class RandomEvent
	{
		public abstract float Warn(); // return 0 if no warning
		public abstract void Start();
		public abstract bool Update();
	}

	public int debugAttackIdx = -1;
	public float initialEventDelay = 3;
	public float minEventInterval = 45;
	public float maxEventInterval = 75;

	private float mEventTimer;
	private RandomEvent mEvent;

	private RandomEvent[] mPossibleEvents = {
		new InvaderAttack(),
		new Drought()
	};

	private enum EventSystemState
	{
		COOLDOWN,
		WARNING,
		ACTIVE
	};

	private EventSystemState mState;

	// Use this for initialization
	void Start () {
		ScheduleEvent();
		// todo: have randomization of event intervals affect initial event delay
		mEventTimer = initialEventDelay;
	}
	
	// Update is called once per frame
	void Update () {

		mEventTimer -= GameState.GameDeltaTime;

		switch(mState) {
		case EventSystemState.COOLDOWN:
			if(mEventTimer <= 0)
			{
				float warningTime = mEvent.Warn();
				if(warningTime > 0)
				{
					mState = EventSystemState.WARNING;
					mEventTimer = warningTime;
				}
				else
				{
					mState = EventSystemState.ACTIVE;
					mEvent.Start();	
				}
			}
			break;
		case EventSystemState.WARNING:
			if(mEventTimer <= 0)
			{
				mState = EventSystemState.ACTIVE;
				mEvent.Start();
			}
			break;
		case EventSystemState.ACTIVE:
			if(mEvent.Update())
			{
				mState = EventSystemState.COOLDOWN;
				ScheduleEvent();
			}
			break;
		}

		// need a hook for when the event ends?
	}

	private void ScheduleEvent()
	{
		if(debugAttackIdx >= 0) mEvent = mPossibleEvents[debugAttackIdx];
		else mEvent = mPossibleEvents[Random.Range(0, mPossibleEvents.Length)];
		mEventTimer = Random.Range(maxEventInterval, maxEventInterval);
	}
}
