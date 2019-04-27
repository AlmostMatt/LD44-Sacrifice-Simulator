using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates UIPerson objects. Can be merged into PersonManager later
public class UIGenerator : MonoBehaviour {

	public GameObject uiPersonObject;
	public int numStartingPeople = 10;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < numStartingPeople; ++i)
		{
			GameObject person = Instantiate(uiPersonObject);
			person.transform.SetParent(transform);
			RectTransform rt = person.GetComponent<RectTransform>();
			rt.anchoredPosition = new Vector2(0f,0f+30f*(i-numStartingPeople/2));
		}
	}

	
	void Update () {
		
	}
}
