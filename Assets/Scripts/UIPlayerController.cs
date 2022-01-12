using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIPlayerController : MonoBehaviour
{
    public enum MenuType {
        INV, SUB_INV, SUB_INV_DROP
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
    public int dropQuant;

    [Header("Controls")]
    public bool wasPressed;


    // Start is called before the first frame update
    void Start()
    {
        curType = MenuType.INV;

        index = 0;
        wasPressed = false;
        maxIndex = scrollTransform.childCount - 1;

        dropQuant = 1;

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
            // Inside Drop SubMenu
            case MenuType.SUB_INV_DROP:
                // If movement is up and drop quantity is lower than item quantity
                if(movement > 0 && dropQuant < GetComponent<InventoryController>().GetItemQuantityByIndex(index)) {
                    // Increase drop quantity
                    dropQuant++;
                } else {
                    // If movement is down and drop quantity is higher than 0
                    if(movement < 0 && dropQuant > 1) {
                        // Decrease drop quantity
                        dropQuant--;
                    }
                }
                UpdateDropUI();
                break;
        }
    }

    // Updates text and arrows from drop sub menu
    void UpdateDropUI() {
        // Update drop quantity text
        invSubMenuTransform.GetChild(2).GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = dropQuant.ToString();
        // If drop quantity reaches item quantity
        if(dropQuant == GetComponent<InventoryController>().GetItemQuantityByIndex(index)) {
            // Disables Up Arrow
            invSubMenuTransform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(false);
        } else {
            invSubMenuTransform.GetChild(2).GetChild(0).GetChild(0).gameObject.SetActive(true);
        }
        // If drop quantity reaches 1
        if(dropQuant == 1) {
            // Disables Down Arrow
            invSubMenuTransform.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(false);
        } else {
            invSubMenuTransform.GetChild(2).GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
    }

    // Updates what element is selected after a change in the inventory
    public void UpdateSelected(bool removed) {
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
        if(scrollTransform != null && invSubMenuTransform != null && this.isActiveAndEnabled) {
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
        if(scrollTransform != null && invSubMenuTransform != null && this.isActiveAndEnabled) {
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
        if(scrollTransform != null && invSubMenuTransform != null && this.isActiveAndEnabled) {
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

                                float realElementHeight = elementHeight*(Screen.height / scrollTransform.root.GetComponent<CanvasScaler>().referenceResolution.y);

                                invSubMenuTransform.position = new Vector3(invSubMenuTransform.position.x,
                                                                            scrollTransform.GetChild(index).transform.position.y + realElementHeight*1.5f,
                                                                            invSubMenuTransform.position.z);
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
                                    curType = MenuType.SUB_INV_DROP;
                                    dropQuant = 1;
                                    UpdateDropUI();
                                    invSubMenuTransform.GetChild(2).gameObject.SetActive(true);
                                    break;
                            }
                            break;
                        // If in Drop Sub Menu, dropQuant is removed from current item
                        case MenuType.SUB_INV_DROP:
                            if(GetComponent<InventoryController>().DropItemByIndex(index, dropQuant)) {
                                // Go back to inventory
                                curType = MenuType.INV;
                                // Reset drop quantity
                                dropQuant = 1;
                                UpdateDropUI();
                                // Close submenus
                                invSubMenuTransform.GetChild(2).gameObject.SetActive(false);
                                invSubMenuTransform.gameObject.SetActive(false);
                            } else {
                                // Go back to submenu
                                curType = MenuType.SUB_INV;
                                // Reset drop quantity
                                dropQuant = 1;
                                UpdateDropUI();
                                // Close drop submenu
                                invSubMenuTransform.GetChild(2).gameObject.SetActive(false);
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
        if(scrollTransform != null && invSubMenuTransform != null && this.isActiveAndEnabled) {
            if(value.ReadValueAsButton()==true && playerController.stats.isAlive) {
                if(!wasPressed) {
                    wasPressed = true;

                    switch(curType) {
                        case MenuType.SUB_INV:
                            subIndex = 0;
                            // Update UI
                            invSubMenuTransform.GetChild(0).GetComponent<Image>().color = new Color32(73,105,173,255);
                            invSubMenuTransform.GetChild(1).GetComponent<Image>().color = new Color32(99,130,154,255);

                            curType = MenuType.INV;
                            invSubMenuTransform.gameObject.SetActive(false);
                            break;
                        case MenuType.SUB_INV_DROP:
                            curType = MenuType.SUB_INV;
                            dropQuant = 1;
                            UpdateDropUI();
                            invSubMenuTransform.GetChild(2).gameObject.SetActive(false);
                            break;
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
                    // Force close drop submenu
                    dropQuant = 1;
                    UpdateDropUI();
                    invSubMenuTransform.GetChild(2).gameObject.SetActive(false);
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
