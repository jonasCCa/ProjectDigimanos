using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItemController : MonoBehaviour
{
    public Image slotImage;
    //public Item item;

    public void UpdateItem(Item input) {
        transform.GetChild(0).GetComponent<Text>().text = input.ID;
        if(input is Usable)
            transform.GetChild(1).GetComponent<Text>().text = "x" + ((Usable)input).quantity;

        // Insert stats here
    }

    public void UpdateItemQuantity(int quantity) {
        transform.GetChild(1).GetComponent<Text>().text = "x" + quantity;
    }
    
    public void SetSelected() {
        // Place-holder for future LeanTween implementation (maybe?)
        slotImage.color = new Color32(59,65,185,208);
    }

    public void SetUnselected() {
        // Place-holder for future LeanTween implementation (maybe?)
        slotImage.color = new Color32(99,130,154,179);
    }
}
