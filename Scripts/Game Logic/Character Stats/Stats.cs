using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField]
    private int baseValue;
    
    private List<int> modifiers = new List<int>(); // list of the currently equipments
    public int GetValue()
    {
        int finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        return finalValue;
    }
    public void SetDamage(int damage)
    {
        baseValue = damage;
        //modifiers.Add(damage);
    }

    public void IncreaseDamage(int damage)
    {
        baseValue += damage;
    }

    public void AddModifier(int modifier)
    {
        if(modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier(int modifier)
    {
        if(modifier != 0)
        {
            modifiers.Remove(modifier);
        }
    }
}
