using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPlayerController : MonoBehaviour
{
    public enum MenuType {
        INV, SUB_INV
    }
    public PlayerController playerController;

    [Header("References")]
    public MenuType curType;
    public RectTransform scrollTransform;
    public RectTransform invSubMenuTransform;

    [Header("Inventory")]
    public int index;
    public int maxIndex;
    public int visibleItems;
    public float elementHeight = 35.2483f;

    [Header("Inventory - SubMenu")]
    public int subIndex;
    public float curY;
    public bool upsideDownY;

    [Header("Controls")]
    public bool wasPressed;


    // Start is called before the first frame update
    void Start()
    {
        curType = MenuType.INV;

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
        switch(curType) {
            // Inside Inventory
            case MenuType.INV:
                int lastIndex = index;
                bool option5 = false;

                if(maxIndex > -1) {
                    if(movement < 0) {
                        if (index < maxIndex) {
                            //Debug.Log("1");
                            index++;
                            if (index > 1 && index < maxIndex) {
                                //Debug.Log("2");
                                scrollTransform.offsetMax -= new Vector2(0, -elementHeight);
                            }
                        } else {
                            //Debug.Log("3");
                            index = 0;
                            scrollTransform.offsetMax = Vector2.zero;
                        }
                    } else if(movement > 0) {
                        if (index > 0) {
                            //Debug.Log("4");
                            index--;
                            if(index < maxIndex - 1 && index > 0) {
                                //Debug.Log("5");
                                option5 = true;
                                scrollTransform.offsetMax -= new Vector2(0, elementHeight);
                            }
                        } else {
                            //Debug.Log("6");
                            index = maxIndex;
                            scrollTransform.offsetMax = new Vector2(0,(maxIndex-(visibleItems-1))*elementHeight);
                        }
                    }

                    scrollTransform.GetChild(lastIndex).GetComponent<ListItemController>().SetUnselected();
                    scrollTransform.GetChild(index).GetComponent<ListItemController>().SetSelected();

                    if(!option5)
                        invSubMenuTransform.position = new Vector3(invSubMenuTransform.position.x, scrollTransform.GetChild(index).transform.position.y + elementHeight*1.5f, invSubMenuTransform.position.z);
                    else
                        invSubMenuTransform.position = new Vector3(invSubMenuTransform.position.x, scrollTransform.GetChild(index).transform.position.y + elementHeight*2.5f, invSubMenuTransform.position.z);
                }
                break;
            // Inside Inventory's SubMenu
            case MenuType.SUB_INV:
                //int lastSubIndex = subIndex;

                // Possible Place-holder for more options
                if(subIndex==0) {
                    subIndex = 1;
                    // Use is inactive
                    invSubMenuTransform.GetChild(0).GetComponent<Image>().color = new Color32(99,130,154,255);
                    // Drop is active
                    invSubMenuTransform.GetChild(1).GetComponent<Image>().color = new Color32(73,105,173,255);
                } else {
                    subIndex = 0;

                    // Use is active
                    invSubMenuTransform.GetChild(0).GetComponent<Image>().color = new Color32(73,105,173,255);
                    // Drop is inactive
                    invSubMenuTransform.GetChild(1).GetComponent<Image>().color = new Color32(99,130,154,255);
                }
                break;
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

                    switch(curType) {
                        // If in Inventory, open context sub menu
                        case MenuType.INV:
                            if(index <= maxIndex && maxIndex > -1) {
                                curType = MenuType.SUB_INV;
                                // Open Sub Menu
                                invSubMenuTransform.gameObject.SetActive(true);
                            }
                            break;
                        // If in Inventory's Sub Menu, action is determined by index
                        case MenuType.SUB_INV:
                            switch(subIndex) {
                                // Use Index
                                case 0:
                                    if(GetComponent<InventoryController>().UseItemByIndex(index)){
                                        curType = MenuType.INV;
                                        invSubMenuTransform.gameObject.SetActive(false);
                                    }
                                    break;
                                // Drop Index
                                case 1:
                                    if(GetComponent<InventoryController>().DropItemByIndex(index)) {
                                        curType = MenuType.INV;
                                        invSubMenuTransform.gameObject.SetActive(false);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            } else {
                wasPressed = false;
            }
        }
    }

    public void OnCancel(InputAction.CallbackContext value) {
        if(scrollTransform != null && this.isActiveAndEnabled) {
            if(value.ReadValueAsButton()==true && playerController.stats.isAlive) {
                if(!wasPressed) {
                    wasPressed = true;

                    // If SubMenu is open, close SubMenu
                    if(curType==MenuType.SUB_INV) {
                        subIndex = 0;
                        // Update UI
                        invSubMenuTransform.GetChild(0).GetComponent<Image>().color = new Color32(73,105,173,255);
                        invSubMenuTransform.GetChild(1).GetComponent<Image>().color = new Color32(99,130,154,255);

                        curType = MenuType.INV;
                        invSubMenuTransform.gameObject.SetActive(false);
                    }
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
                    // Force close SubMenu
                    subIndex = 0;
                    curType = MenuType.INV;
                    invSubMenuTransform.gameObject.SetActive(false);
                    // Update UI
                    invSubMenuTransform.GetChild(0).GetComponent<Image>().color = new Color32(73,105,173,255);
                    invSubMenuTransform.GetChild(1).GetComponent<Image>().color = new Color32(99,130,154,255);
                    // Close Menu
                    playerController.playerMenu.SetActive(false);
                    // Switch to gameplay
                    playerController.playerInput.SwitchCurrentActionMap("Gameplay");
                }
            }
        }
    }
}
