using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HealingSystem : MonoBehaviour
{
    void Update()
    {

        List<Person> people = Utilities.GetPersonManager().People;
        List<Person> peopleNotMaxHealth = people.FindAll(p => p.Health < p.MaxHealth);
        // Doctors produce HP and distribute it evenly among all people that are not max health
        float totalHealing = 0f;
        totalHealing += people.FindAll(p => p.Profession == PersonAttribute.WITCH_DOCTOR).Sum(p => p.Efficiency);
        if (GameState.HasBoon(BoonType.SURPLUS_FOOD_TO_HEALING))
        {
            float foodToHealingRatio = GameState.GetBoonValue(BoonType.SURPLUS_FOOD_TO_HEALING) / 100f;
            totalHealing += foodToHealingRatio * GameState.FoodSurplus;
        }
        float healPerPerson = totalHealing / Mathf.Max(1, peopleNotMaxHealth.Count);
        foreach (Person person in peopleNotMaxHealth)
        {
            person.Heal(healPerPerson * GameState.GameDeltaTime);
        }
    }
}
