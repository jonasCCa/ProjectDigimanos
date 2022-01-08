using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Usable : Item
{
    // REMEMBER TO ADD EFFECTS, IF ANY
    [Header("Attributes")]
    public int HP;
    public int MP;
    public int HPBonus;
    public int MPBonus;
    public int Str;
    public int Wis;
    public int Con;
    public int Min;
    public int duration;

    [Header("Quantity")]
    public int quantity;
    [SerializeField] private int maxQuantity;

    public Usable Clone(Usable original) {
        Usable aux = new Usable();

        aux.ID = original.ID;
        aux.value = original.value;

        aux.itemObject = original.itemObject;

        aux.HP = original.HP;
        aux.MP = original.MP;
        aux.Str = original.Str;
        aux.Wis = original.Wis;
        aux.Con = original.Con;
        aux.Min = original.Min;
        aux.duration = original.duration;

        aux.SetMax(original.GetMax());
        aux.quantity = original.GetQuantity();

        return aux;
    }
    
    // Returns a list of strings representing the item effects
    // in the following format, ignoring the '<' and '>' symbols:
    //
    // (<B> or <N>)?<Attribute Abreviation>;<Attribute Value>;<Effect Duration>
    //
    // IMPORTANT: Nerfs are returned in absolute value
    //
    // If the attribute should be Buffed, it's formated as B<Attribute Abreviation>
    // If the attribute should be Nerfed, it's formated as N<Attribute Abreviation>
    // HP and MP recovery don't have a duration value
    public List<string> Use() {
        List<string> aux = new List<string>();

        if(HP > 0) {
            aux.Add("HP;" + HP);
        }
        if(MP > 0) {
            aux.Add("MP;" + MP);
        }
        if(HPBonus > 0) {
            aux.Add("BHP;" + HPBonus + ";" + duration);
        } else if(HPBonus < 0) {
            aux.Add("NHP;" + (HPBonus*-1) + ";" + duration);
        }
        if(MPBonus > 0) {
            aux.Add("BMP;" + MPBonus + ";" + duration);
        } else if(MPBonus < 0) {
            aux.Add("NMP;" + (MPBonus*-1) + ";" + duration);
        }
        if(Str > 0) {
            aux.Add("BStr;" + Str + ";" + duration);
        } else if(Str < 0) {
            aux.Add("NStr;" + (Str*-1) + ";" + duration);
        }
        if(Wis > 0) {
            aux.Add("BWis;" + Wis + ";" + duration);
        } else if(Wis < 0) {
            aux.Add("NWis;" + (Wis*-1) + ";" + duration);
        }
        if(Con > 0) {
            aux.Add("BCon;" + Con + ";" + duration);
        } else if(Con < 0) {
            aux.Add("NCon;" + (Con*-1) + ";" + duration);
        }
        if(Min > 0) {
            aux.Add("BMin;" + Min + ";" + duration);
        } else if(Min < 0) {
            aux.Add("NMin;" + (Min*-1) + ";" + duration);
        }

        return aux;
    }

    public int GetQuantity() {
        return quantity;
    }

    public int GetMax() {
        return maxQuantity;
    }

    public void SetMax(int m) {
        this.maxQuantity = m;
    }

    // Adds amount to quantity
    // Returns how many leftovers there were
    public int AddQuantity(int amount) {
        // If Quantity is already at Maximum, return everything
        if(quantity >= maxQuantity) {
            return amount;
        }

        // If adding will get larger than Maximum, adds till Maximum and return leftovers
        if(quantity+amount > maxQuantity) {
            int leftovers = quantity+amount - maxQuantity;

            quantity = maxQuantity;

            return leftovers;
        }
        
        // If adding won't get larger than Maximum, add everything and return 0 leftovers
        quantity += amount;
        return 0;
    }

    // Removes amount from quantity
    // Returns true if successful, false if amount is invalid
    public bool RemoveQuantity(int amount) {
        if(quantity-amount >= 0) {
            quantity -= amount;
            return true;
        }
        return false;
    }
}
