using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {
	public string[] spriteNames;
	public Sprite[] sprites;

	private Dictionary<string, Sprite> stringToSpriteMap = new Dictionary<string, Sprite>();

	void Awake() {
		if (stringToSpriteMap.Count == 0) {
			Setup();
		}
	}

	private void Setup() {
		for (int i=0; i<Mathf.Min(spriteNames.Length, sprites.Length); i++) {
			stringToSpriteMap.Add(spriteNames[i], sprites[i]);
		}
	}

	public Sprite GetSprite(Person.Attribute profession) {
		if (profession == Person.Attribute.NONE) {
			return null;
		}
		return GetSprite(profession.ToString());
	}

	public Sprite GetSprite(string iconName) {
		if (iconName == "" || iconName == "NONE") {
			return null;
		}
		// Call Setup here in case awake hasnt yet happened
		if (stringToSpriteMap.Count == 0) {
			Setup();
		}
		if (stringToSpriteMap.ContainsKey(iconName)) {
			return stringToSpriteMap[iconName];
		}
		Debug.Log("WARNING(SpriteManager): No sprite found for name: " + iconName);
		return null;
	}
}
