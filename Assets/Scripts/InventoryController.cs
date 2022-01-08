using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [SerializeField] StatsController playerStats;
    [SerializeField] UIPlayerController uIPlayerController;
    [SerializeField] List<Item> itemList;
    [SerializeField] int capacity;

    [Header("UI")]
    public GameObject listParentUI;
    [SerializeField] GameObject itemPrefab;

    [Header(" ")]
    [SerializeField] GameObject itemContainerPrefab;

    void Start() {
        //playerStats = GetComponent<StatsController>();
    }

    // Adds an Item to the inventory
    // Returns how many leftovers there were
    //      Kinda sus implementation; if the item isn't Usable, it's straight forward and fast
    //      but if the item IS Usable, adding it becomes O(n), being 'n' the inventory used
    public int AddItem(Item item) {
        GameObject itemAux = null;
        // If item is not Usable, it doesn't stack
        if(!(item is Usable)) {
            // If inventory is full, return 1 leftover
            if(itemList.Count >= capacity) {
                return 1;
            }
            // If there are free spaces, adds the item and return 0 leftovers
            if(item is Equipable) {
                itemList.Add(((Equipable)item).Clone(item as Equipable));
                // Adds item to the inventory UI
                itemAux = Instantiate(itemPrefab,itemPrefab.transform.position,Quaternion.identity,listParentUI.transform);
                itemAux.GetComponent<ListItemController>().UpdateItem(item);
                // Updates list size on UI
                uIPlayerController.SetMaxIndex(itemList.Count-1);
                uIPlayerController.UpdateSelected(false);
            }

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
            // Updates item quantity on the inventory UI
            listParentUI.transform.GetChild(itemIndex).GetComponent<ListItemController>().UpdateItemQuantity(((Usable)itemList[itemIndex]).GetQuantity());
            return leftovers;
        }
        // If it doesn't,
        // If the inventory is already full, return all leftovers
        if(itemList.Count >= capacity) {
            return (item as Usable).GetQuantity();
        }
        // If there are free spaces, adds the item and return 0 leftovers
        itemList.Add(((Usable)item).Clone(item as Usable));
        // Adds item to the inventory UI
        itemAux = Instantiate(itemPrefab,itemPrefab.transform.position,Quaternion.identity,listParentUI.transform);
        itemAux.GetComponent<ListItemController>().UpdateItem(item);
        // Updates list size on UI
        uIPlayerController.SetMaxIndex(itemList.Count-1);
        uIPlayerController.UpdateSelected(false);

        return 0;
    }

    // Uses Item by referencing it's index
    public void UseItemByIndex(int index) {
        // If index is valid 
        if(index < itemList.Count) {
            // Can only use Usable Items
            if(itemList[index] is Usable) {
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
                        case "MP":
                            used = playerStats.RestoreMP(int.Parse(aux[1]));
                            break;
                        case "BHP":
                            used = playerStats.RecieveBuff(AttType.HP,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "BMP":
                            used = playerStats.RecieveBuff(AttType.MP,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "BStr":
                            used = playerStats.RecieveBuff(AttType.STR,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "BWis":
                            used = playerStats.RecieveBuff(AttType.WIS,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "BCon":
                            used = playerStats.RecieveBuff(AttType.CON,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "BMin":
                            used = playerStats.RecieveBuff(AttType.MIN,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NHP":
                            used = playerStats.RecieveNerf(AttType.HP,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NMP":
                            used = playerStats.RecieveNerf(AttType.MP,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NStr":
                            used = playerStats.RecieveNerf(AttType.STR,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NWis":
                            used = playerStats.RecieveNerf(AttType.WIS,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NCon":
                            used = playerStats.RecieveNerf(AttType.CON,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                        case "NMin":
                            used = playerStats.RecieveNerf(AttType.MIN,int.Parse(aux[1]),int.Parse(aux[2]));
                            break;
                    }
                }

                // If item was used, remove 1. If there are no items left, remove it
                if(used) {
                    Debug.Log("Used" + item.ID + ": " + item.quantity + "(" + item.GetMax() + ")" + " -> " + (item.quantity-1));
                    
                    item.RemoveQuantity(1);

                    if(item.GetQuantity() == 0) {
                        itemList.RemoveAt(index);
                        // Destroys item on inventory UI
                        Destroy(listParentUI.transform.GetChild(index).gameObject);
                        // Updates list size on UI
                        uIPlayerController.SetMaxIndex(itemList.Count-1);
                        uIPlayerController.UpdateSelected(true);
                    } else {
                        // Updates item quantity on inventory UI
                        listParentUI.transform.GetChild(index).GetComponent<ListItemController>().UpdateItemQuantity(item.GetQuantity());
                    }
                }
            }
        }
    }

    // Drops the item from the inventory to the ground
    public bool DropItemByIndex(int index) {
        // If index is valid 
        if(index < itemList.Count) {
            // Decide wether to create an Usable or Equipable
            if(itemList[index] is Usable) {
                Usable item = (Usable)itemList[index];

                GameObject gO = Instantiate(itemContainerPrefab, transform.position - new Vector3(0,1,0), Quaternion.identity);
                gO.GetComponent<ItemContainer>().itemType = ItemContainer.ItemType.Usable;
                gO.GetComponent<ItemContainer>().uItem = item.Clone(item);
                gO.GetComponent<ItemContainer>().InstantiateItem();

                itemList.RemoveAt(index);
                // Destroys item on inventory UI
                Destroy(listParentUI.transform.GetChild(index).gameObject);
                // Updates list size on UI
                uIPlayerController.SetMaxIndex(itemList.Count-1);
                uIPlayerController.UpdateSelected(true);

                Debug.Log("Drped " + item.ID);

                return true;
            } else if(itemList[index] is Equipable) {
                Equipable item = (Equipable)itemList[index];

                GameObject gO = Instantiate(itemContainerPrefab, transform.position - new Vector3(0,1,0), Quaternion.identity);
                gO.GetComponent<ItemContainer>().itemType = ItemContainer.ItemType.Equipable;
                gO.GetComponent<ItemContainer>().eItem = item.Clone(item);
                gO.GetComponent<ItemContainer>().InstantiateItem();

                itemList.RemoveAt(index);
                // Destroys item on inventory UI
                Destroy(listParentUI.transform.GetChild(index).gameObject);
                // Updates list size on UI
                uIPlayerController.SetMaxIndex(itemList.Count-1);
                uIPlayerController.UpdateSelected(true);

                Debug.Log("Drped " + item.ID);

                return true;
            }
        }
        // If can't drop item, return false
        return false;
    }
}
