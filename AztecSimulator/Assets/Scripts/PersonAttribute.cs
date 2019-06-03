using UnityEngine;

public enum PersonAttribute
{
    /**
     * When adding a new attribute
     *  - update the attrType map
     *  - update the SpriteManager prefab
     * When adding a new profession:
     *  - update Description / EfficiencyBase / EfficiencyPerLevel / Color extension functions
     *  - add a new ProfessionGroup to the UI canvas prefab and set the associateProfession
     *  
     *  TODO: have the UICanvas instatiate ProfessionGroups dynamically
     *  Note: VictoryDemand isn't generalized to any profession
     **/

    FARMER = 0,
    WARRIOR,
    CIVILIAN,
    SCRIBE,
    TALL,
    SHORT,
    BLUE_EYES,
    GREEN_EYES,
    BROWN_EYES,
    STRONG,
    WEAK,
    SMART,
    CARING,
    STUPID,
    // When picking a random attr, subtract max value by 1 to not select the following
    NONE
}

// Utility functions to go from attr > type are in Utilities
public enum PersonAttributeType
{
    PROFESSION = 0,
    HEIGHT,
    EYE_COLOR,
    STRENGTH,
    PERSONALITY,
    NONE
}

// Extenstion methods can be called with attr.method(). For example, PersonAttribute.CARING.GetAttrType()
public static class PersonAttributeExtensions
{

    public static PersonAttributeType GetAttrType(this PersonAttribute attr)
    {
        switch (attr)
        {
            case PersonAttribute.FARMER:
            case PersonAttribute.WARRIOR:
            case PersonAttribute.CIVILIAN:
            case PersonAttribute.SCRIBE:
                return PersonAttributeType.PROFESSION;
            case PersonAttribute.TALL:
            case PersonAttribute.SHORT:
                return PersonAttributeType.HEIGHT;
            case PersonAttribute.BLUE_EYES:
            case PersonAttribute.GREEN_EYES:
            case PersonAttribute.BROWN_EYES:
                return PersonAttributeType.EYE_COLOR;
            case PersonAttribute.STRONG:
            case PersonAttribute.WEAK:
                return PersonAttributeType.STRENGTH;
            case PersonAttribute.SMART:
            case PersonAttribute.CARING:
            case PersonAttribute.STUPID:
                return PersonAttributeType.PERSONALITY;
            default:
                return PersonAttributeType.NONE;
        }
    }

    public static string GetDescription(this PersonAttribute attr)
    {
        switch (attr)
        {
            case PersonAttribute.FARMER:
                return "Farmers produce food";
            case PersonAttribute.WARRIOR:
                return "Warriors increase army";
            case PersonAttribute.CIVILIAN:
                return "Civilians have children";
            case PersonAttribute.SCRIBE:
            default:
                return attr.CapitalizedString() + " <GetDescription>";
        }
    }

    public static float GetEfficiencyBase(this PersonAttribute attr)
    {
        switch (attr)
        {
            case PersonAttribute.FARMER:
                return 1.5f;
            case PersonAttribute.WARRIOR:
                return 1f;
            case PersonAttribute.CIVILIAN:
                return 0.5f;
            case PersonAttribute.SCRIBE:
            default:
                return 1f;
        }
    }

    public static float GetEfficiencyPerLevel(this PersonAttribute attr)
    {
        // For now all professions get 50% more effective per level.
        return attr.GetEfficiencyBase() / 2f;
    }

    // TODO: use profession.GetColor() to color profession in GodDemand description/cost, and Person
    public static Color32 GetColor(this PersonAttribute attr)
    {
        switch (attr)
        {
            case PersonAttribute.FARMER:
                return new Color32(60, 99, 60, 255);
            case PersonAttribute.WARRIOR:
                return new Color32(225, 0, 0, 255);
            case PersonAttribute.CIVILIAN:
                return new Color32(23, 19, 102, 255);
            case PersonAttribute.SCRIBE:
                return new Color32(166,0,226,255);
            case PersonAttribute.NONE:
                return new Color32(29, 29, 29, 255);
            default:
                Debug.Log("WARNING: " + attr.ToString() + " has no associated color");
                return Color.white;
        }
    }

    public static string CapitalizedString(this PersonAttribute attr)
    {
        string str = attr.ToString();
        return char.ToUpper(str[0]) + str.Substring(1).ToLower();
    }

    public static PersonAttribute[] GetAllValues(this PersonAttributeType attrType)
    {
        return System.Array.FindAll<PersonAttribute>((PersonAttribute[])System.Enum.GetValues(typeof(PersonAttribute)), attr => GetAttrType(attr) == attrType);
    }

    public static PersonAttribute GetRandomValue(this PersonAttributeType attrType)
    {
        PersonAttribute[] possibleValues = attrType.GetAllValues();
        return possibleValues[Random.Range(0, possibleValues.Length)];
    }
}
