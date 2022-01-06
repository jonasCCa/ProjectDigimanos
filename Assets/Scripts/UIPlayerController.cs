using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPlayerController : MonoBehaviour
{
    public PlayerController playerController;
    public RectTransform scrollTransform;
    //public GameObject itemList;
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

    public void UpdateSelected() {
        if(maxIndex >= index)
            scrollTransform.GetChild(index).GetComponent<ListItemController>().SetSelected();
        
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
            if(value.ReadValueAsButton()==true) {
                if(!wasPressed) {
                    wasPressed = true;

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
