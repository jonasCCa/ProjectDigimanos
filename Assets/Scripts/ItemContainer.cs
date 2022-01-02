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
}
