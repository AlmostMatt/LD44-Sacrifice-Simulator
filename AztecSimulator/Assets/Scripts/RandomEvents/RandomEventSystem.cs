﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem : MonoBehaviour {

	public abstract class RandomEvent
	{
		public abstract float Warn(); // return 0 if no warning
		public abstract void Start();
		public abstract bool Update();
	}

	public float initialEventDelay;
	public float minEventInterval = 45;
	public float maxEventInterval = 75;

	private float mEventTimer;
	private RandomEvent mEvent;

	private enum EventSystemState
	{
		COOLDOWN,
		WARNING,
		ACTIVE
	};

	private EventSystemState mState;
	private bool mEventActive;

	// Use this for initialization
	void Start () {
		ScheduleEvent();

		mEventTimer += initialEventDelay;
	}
	
	// Update is called once per frame
	void Update () {

		mEventTimer -= Time.deltaTime;

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
		mEventActive = false;
		mEvent = new InvaderAttack();
		mEventTimer = Random.Range(maxEventInterval, maxEventInterval);
	}
}