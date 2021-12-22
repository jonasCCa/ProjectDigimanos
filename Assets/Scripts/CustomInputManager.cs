using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputManager : MonoBehaviour
{
    [SerializeField] List<Dictionary<string,KeyCode>> controlMapList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool getKey(string actionName, int player) {
        if(Input.GetKey(controlMapList[player][actionName])) {
            return true;
        }
        return false;
    }

    bool getKeyDown(string actionName, int player) {
        if(Input.GetKeyDown(controlMapList[player][actionName])) {
            return true;
        }
        return false;
    }

    bool getKeyUp(string actionName, int player) {
        if(Input.GetKeyUp(controlMapList[player][actionName])) {
            return true;
        }
        return false;
    }
}
