using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {
	public string[] spriteNames;
	public Sprite[] sprites;

	private Dictionary<string, Sprite> stringToSpriteMap = new Dictionary<string, Sprite>();

	void Awake() {
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
		if (stringToSpriteMap.ContainsKey(iconName)) {
			return stringToSpriteMap[iconName];
		}
		Debug.Log("WARNING(SpriteManager): No sprite found for name: " + iconName);
		return null;
	}
}
