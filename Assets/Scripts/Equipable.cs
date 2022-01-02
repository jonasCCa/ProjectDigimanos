using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Equipable : Item
{
    [SerializeField] private bool equipped;

    public Equipable Clone(Equipable original) {
        Equipable aux = new Equipable();

        aux.ID = original.ID;
        aux.value = original.value;

        if(original.IsEquipped())
            aux.Equip();
        else
            aux.UnEquip();

        return aux;
    }
    
    public void Equip() {
        equipped = true;
    }

    public void UnEquip() {
        equipped = false;
    }

    public bool IsEquipped() {
        return equipped;
    }
}
