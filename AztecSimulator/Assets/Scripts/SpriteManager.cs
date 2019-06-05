using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {
    [System.Serializable]
    public struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }
    // These fields are functionally equivalent, but they logically group icons
    public NamedSprite[] namedAttributes;
    public NamedSprite[] namedIcons;
    public NamedSprite[] namedOther;

	private Dictionary<string, Sprite> stringToSpriteMap = new Dictionary<string, Sprite>();

	void Awake() {
		if (stringToSpriteMap.Count == 0) {
			Setup();
		}
	}

	private void Setup() {
        foreach (NamedSprite namedSprite in namedAttributes)
        {
            stringToSpriteMap.Add(namedSprite.name, namedSprite.sprite);
        }
        foreach (NamedSprite namedSprite in namedIcons)
        {
            stringToSpriteMap.Add(namedSprite.name, namedSprite.sprite);
        }
        foreach (NamedSprite namedSprite in namedOther)
        {
            stringToSpriteMap.Add(namedSprite.name, namedSprite.sprite);
        }
    }

    // TODO: add an option to choose between 16px and 32px variants
    public Sprite GetSprite(PersonAttribute attribute) {
		if (attribute == PersonAttribute.NONE) {
			return null;
		}
		return GetSprite(attribute.ToString());
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
