using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Item
{
    public string ID;
    public int value;
    public GameObject itemObject;
    

    public Item Clone(Item original) {
        Item aux = new Item();
        aux.ID = original.ID;
        aux.value = original.value;
        aux.itemObject = original.itemObject;

        return aux;
    }

    public Item Pickup() {
        return this;
    }
}
