using UnityEngine;

public enum PersonAttribute
{
    /**
     * When adding a new attribute
     *  - update the attrType map
     *  - update the SpriteManager prefab
     * When adding a new profession:
     *  - update the Efficiency maps in Person.cs
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
                return "Scribe"; // Scribes give XP to other units
            default:
                return attr.ToString() + " <GetDescription>";
        }
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
