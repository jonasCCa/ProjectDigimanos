using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public enum ItemType {
        Usable,
        Equipable
    }
    public ItemType itemType;

    public Usable uItem;
    public Equipable eItem;

    public void InstantiateItem() {
        if(itemType==ItemType.Usable) {
            Instantiate(uItem.itemObject,transform.position,Quaternion.identity,transform);
            return;
        }
        if(itemType==ItemType.Equipable) {
            Instantiate(eItem.itemObject,transform.position,Quaternion.identity,transform);
            return;
        }
    }
}
