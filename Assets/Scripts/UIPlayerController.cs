using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayerController : MonoBehaviour
{
    public PlayerController playerController;
    public RectTransform scrollTransform;
    public int index;
    public int maxIndex;
    public int visibleItems;

    public bool wasPressed;

    public float elementHeight = 35.2483f;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        wasPressed = false;
        maxIndex = scrollTransform.childCount - 1;

        if(maxIndex >= index)
            scrollTransform.GetChild(index).GetComponent<ListItemController>().SetSelected();
    }

    public void SetMaxIndex(int newMax) {
        maxIndex = newMax;
    }

    // Moves "cursor" on the UI menu selection
    void MoveSelector(int movement) {
        int lastIndex = index;

        if(maxIndex > -1) {
            if(movement < 0) {
                if (index < maxIndex) {
                    index++;
                    if (index > 1 && index < maxIndex)
                        scrollTransform.offsetMax -= new Vector2(0, -elementHeight);
                } else {
                    index = 0;
                    scrollTransform.offsetMax = Vector2.zero;
                }
            } else if(movement > 0) {
                if (index > 0) {
                    index--;
                    if(index < maxIndex - 1 && index > 0)
                        scrollTransform.offsetMax -= new Vector2(0, elementHeight);
                } else {
                    index = maxIndex;
                    scrollTransform.offsetMax = new Vector2(0,(maxIndex-(visibleItems-1))*elementHeight);
                }
            }

            scrollTransform.GetChild(lastIndex).GetComponent<ListItemController>().SetUnselected();
            scrollTransform.GetChild(index).GetComponent<ListItemController>().SetSelected();
        }
    }

    // Updates what element is selected after a change in the inventory
    public void UpdateSelected(bool removed) {
        //if(index > maxIndex) {
        //    if(maxIndex >= 0)
        //        index = maxIndex;
        //    else
        //        index = 0;
        //}
        if(maxIndex>=0) {
            if(removed) {
                if(index < maxIndex+1) {
                    scrollTransform.GetChild(index+1).GetComponent<ListItemController>().SetSelected();
                } else {
                    scrollTransform.GetChild(index-1).GetComponent<ListItemController>().SetSelected();
                    index = maxIndex;
                }
            } else {
                if(maxIndex == 0) {
                    index = 0;
                    scrollTransform.GetChild(index).GetComponent<ListItemController>().SetSelected();
                }
            }
        }
    }

    public void OnNavigateUp(InputAction.CallbackContext value) {
        //Debug.Log("onnavUp");
        if(scrollTransform != null && this.isActiveAndEnabled) {
            if(value.ReadValueAsButton()==true) {
                if(!wasPressed) {
                    wasPressed = true;
                    MoveSelector(1);
                }
            } else {
                wasPressed = false;
            }
        }
    }

    public void OnNavigateDown(InputAction.CallbackContext value) {
        //Debug.Log("onnavDown");
        if(scrollTransform != null && this.isActiveAndEnabled) {
            if(value.ReadValueAsButton()==true) {
                if(!wasPressed) {
                    wasPressed = true;
                    MoveSelector(-1);
                }
            } else {
                wasPressed = false;
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext value) {
        if(scrollTransform != null && this.isActiveAndEnabled) {
            if(value.ReadValueAsButton()==true && playerController.stats.isAlive) {
                if(!wasPressed) {
                    wasPressed = true;
                    // Place-holder for sub menu, for now it uses the item
                    GetComponent<InventoryController>().UseItemByIndex(index);

                    // Place-holder for sub menu, for now it drops the item
                    //GetComponent<InventoryController>().DropItemByIndex(index);
                }
            } else {
                wasPressed = false;
            }
        }
    }


    public void OnCloseMenu(InputAction.CallbackContext value) {
        if(playerController != null) {
            if(playerController.playerInput != null && playerController.playerMenu != null) {
                if(value.ReadValueAsButton()==true && value.performed) {
                    playerController.playerMenu.SetActive(false);
                    playerController.playerInput.SwitchCurrentActionMap("Gameplay");
                }
            }
        }
    }
}
