using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem : MonoBehaviour {

	public abstract class RandomEvent
	{
		public Ongoing mOngoing; // UI notification

		public virtual float Warn() { return(0); } // return 0 if no warning
		public abstract float Start();
		public virtual bool Update() { return(false); } // return true to bail early
		public virtual void Removed() {}
	}
		
	public enum EventSystemState
	{
		PENDING,
		WARNING,
		ACTIVE,
		COOLDOWN
	};

	private class TimedEvent {

		public RandomEvent mEvent;
		public float mTimer;
		public EventSystemState mState;

		public TimedEvent(RandomEvent ev, float timer)
		{
			mEvent = ev;
			mTimer = timer;
			mState = EventSystemState.PENDING;
		}

		public void Update()
		{
			
			mTimer -= GameState.GameDeltaTime;

			switch(mState) 
			{
			case EventSystemState.PENDING:
				if(mTimer <= 0)
				{
					float warningTime = mEvent.Warn();
					if(warningTime > 0)
					{
						mState = EventSystemState.WARNING;
						mTimer = warningTime;
					}
					else
					{
						mState = EventSystemState.ACTIVE;
						mTimer = mEvent.Start();	
					}
				}
				break;
			case EventSystemState.WARNING:
				if(mTimer <= 0)
				{
					mState = EventSystemState.ACTIVE;
					mTimer = mEvent.Start();
				}
				break;
			case EventSystemState.ACTIVE:
				if(mEvent.Update() || mTimer <= 0)
				{
					mState = EventSystemState.COOLDOWN;
				}
				break;
			}
		}
	}

	public int debugEventIdx = -1;
	public float initialEventDelay = 3;

	private List<TimedEvent> mEvents;

	private EventSystemState mState;

	// Use this for initialization
	void Start () {
		mEvents = new List<TimedEvent>();

		// hack in events for now
		ScheduleEvent(new InvaderAttack(), initialEventDelay);
		ScheduleEvent(new Drought(), initialEventDelay + Random.Range(30, 60));
	}
	
	// Update is called once per frame
	void Update () {

		foreach(TimedEvent te in mEvents)
		{
			te.Update();
		}

		for(int i = mEvents.Count - 1; i >= 0; --i)
		{
			if(mEvents[i].mState == EventSystemState.COOLDOWN)
			{
				// do we need a hook for when the event ends?
				mEvents[i].mEvent.Removed();
				mEvents.RemoveAt(i);
			}
		}

		List<Ongoing> ongoings = GameState.Ongoings;
		for(int i = ongoings.Count - 1; i >= 0; --i)
		{
			ongoings[i].mDuration -= GameState.GameDeltaTime;
			if(ongoings[i].mDuration <= 0)
			{
				ongoings.RemoveAt(i);
			}
		}
	}

	public void ScheduleEvent(RandomEvent ev, float delay)
	{
		mEvents.Add(new TimedEvent(ev, delay));
	}
}
