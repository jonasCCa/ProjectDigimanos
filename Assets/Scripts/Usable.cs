using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Usable : Item
{
    // REMEMBER TO COPY EVERY SINGLE STAT
    // REMEMBER TO ADD EFFECTS, IF ANY
    public int HP;
    public int quantity;
    [SerializeField] private int maxQuantity;

    public Usable Clone(Usable original) {
        Usable aux = new Usable();

        aux.ID = original.ID;
        aux.value = original.value;

        aux.SetMax(original.GetMax());
        aux.quantity = original.GetQuantity();
        aux.HP = original.HP;

        return aux;
    }
    
    public List<string> Use() {
        List<string> aux = new List<string>();

        if(HP > 0) {
            aux.Add("HP;" + HP);
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
