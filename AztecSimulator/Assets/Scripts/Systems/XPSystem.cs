using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class XPSystem : MonoBehaviour
{
    void Update()
    {

        List<Person> people = Utilities.GetPersonManager().People;
        List<Person> peopleNotMaxLevel = people.FindAll(p => p.Level < GameState.GetLevelCap(p.Profession));
        // Scribes produce XP and distribute it evenly among all people that are not max level
        float scribeBonus = people.FindAll(p => p.Profession == PersonAttribute.SCRIBE).Sum(p => p.Efficiency) / Mathf.Max(1, peopleNotMaxLevel.Count);
        foreach (Person person in peopleNotMaxLevel)
        {
            PersonAttribute prof = person.Profession;
            float xpGain = 1;
            xpGain = GameState.GetBuffedXp(prof, xpGain);
            if (GameState.HasBoon(BoonType.SAME_PROFESSION_XP_BONUS))
            {
                List<Person> sameProfession = Utilities.GetPersonManager().FindPeople(PersonAttributeType.PROFESSION, prof);
                if (sameProfession.Count >= GameState.GetBoonValue(BoonType.SAME_PROFESSION_XP_REQ))
                {
                    xpGain += GameState.GetBoonValue(BoonType.SAME_PROFESSION_XP_BONUS);
                }
            }
            int bonusXpHealthThreshold = GameState.GetBoonValue(BoonType.HEALTHY_BONUS_XP_THRESHOLD);
            if (bonusXpHealthThreshold > 0 && person.Health >= bonusXpHealthThreshold)
            {
                xpGain += GameState.GetBoonValue(BoonType.HEALTHY_BONUS_XP);
            }
            int bonusXpUnhealthyThreshold = GameState.GetBoonValue(BoonType.UNHEALTHY_BONUS_XP_THRESHOLD);
            if (bonusXpUnhealthyThreshold > 0 && person.Health <= bonusXpUnhealthyThreshold)
            {
                xpGain += GameState.GetBoonValue(BoonType.UNHEALTHY_BONUS_XP);
            }
            xpGain += scribeBonus;
            person.AddXp(xpGain * GameState.GameDeltaTime);
        }
    }
}
