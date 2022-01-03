using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] StatsController playerStats;
    [SerializeField] List<Item> itemList;
    [SerializeField] int capacity;

    void Start() {
        //playerStats = GetComponent<StatsController>();
    }

    // Adds an Item to the inventory
    // Returns how many leftovers there were
    //      Kinda sus implementation; if the item isn't Usable, it's straight forward and fast
    //      but if the item IS Usable, adding it becomes O(n), being 'n' the inventory used
    public int AddItem(Item item) {
        // If item is not Usable, it doesn't stack
        if(!(item is Usable)) {
            // If inventory is full, return 1 leftover
            if(itemList.Count >= capacity) {
                return 1;
            }
            // If there are free spaces, adds the item and return 0 leftovers
            if(item is Equipable)
                itemList.Add(((Equipable)item).Clone(item as Equipable));
            else
                itemList.Add(item.Clone(item));

            return 0;
        }

        // Checks if the item already exists in the inventory
        int itemIndex = -1;
        for(int i=0; i<itemList.Count; i++) {
            if(itemList[i].ID == item.ID) {
                //Debug.Log(itemList[i].ID + " == " + item.ID);
                itemIndex = i;
                break; 
            }
        }

        // If it does, adds the quantity and return the leftovers
        if(itemIndex != -1) {
            int leftovers = ((Usable)itemList[itemIndex]).AddQuantity((item as Usable).GetQuantity());
            return leftovers;
        }
        // If it doesn't,
        // If the inventory is already full, return all leftovers
        if(itemList.Count >= capacity) {
            return (item as Usable).GetQuantity();
        }
        // If there are free spaces, adds the item and return 0 leftovers
        itemList.Add(((Usable)item).Clone(item as Usable));
        return 0;
    }

    // Uses Item by referencing it's index
    public void UseItemByIndex(int index) {
        // If index is valid 
        if(index < itemList.Count) {
            Usable item = (Usable)itemList[index];

            List<string> itemEffects = item.Use();

            bool used = false;

            // For each effect, apply it and flags item as "used" appropriately
            foreach(string usage in itemEffects) {
                string[] aux = usage.Split(';');

                switch(aux[0]) {
                    case "HP":
                        used = playerStats.Heal(int.Parse(aux[1]));
                        break;
                }
            }

            // If item was used, remove 1. If there are no items left, remove it
            if(used) {

                Debug.Log("Used" + item.ID + ": " + item.quantity + "(" + item.GetMax() + ")" + " -> " + (item.quantity-1));
                
                item.RemoveQuantity(1);
                if(item.GetQuantity() == 0) {
                    itemList.RemoveAt(index);
                }
            }
        }
    }

    // Uses Item by referencing the item itself
    public void UseItem(Usable item) {
        List<string> itemEffects = item.Use();

        bool used = false;

        // For each effect, apply it and flags item as "used" appropriately
        foreach(string usage in itemEffects) {
            string[] aux = usage.Split(';');

            switch(aux[0]) {
                case "HP":
                    used = playerStats.Heal(int.Parse(aux[1]));
                    break;
            }
        }

        // If item was used, remove 1. If there are no items left, remove it
        if(used) {
            item.RemoveQuantity(1);
            if(item.GetQuantity() == 0) {
                itemList.Remove(item);
            }
        }
    }
}