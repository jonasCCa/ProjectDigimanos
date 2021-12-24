using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages multiple player intances
public class MultiplayerManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private List<GameObject> playerList;
    [SerializeField] private CameraController cController;


    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        cController = GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Calculates centroid between all players
    public Vector3 calculateTargetPosition() {
        Vector3 newTarget = new Vector3();

        foreach(GameObject player in playerList) {
            newTarget += player.transform.position;
        }
        if(playerList.Count > 0)
            newTarget = newTarget / playerList.Count;

        //Debug.Log(newTarget.ToString());
        return newTarget;
    }

    // Calculates the longest distance between players
    //   -> Complexity O(n² - n), gotta test performance
    public float calculatePlayerDistance() {
        float longestDistance = 0;

        for(int i=0; i<playerList.Count; i++) {
            for(int j=0; j<playerList.Count; j++) {
                if(j!=i) {
                    float aux = Vector3.Distance(playerList[i].transform.position, playerList[j].transform.position);
                    if(aux > longestDistance) {
                        longestDistance = aux;
                    }
                }
            }
        }

        return longestDistance;
    }

    // Adds Player to list when joined, set SinglePlayer flag on CameraController
    public void OnPlayerJoined(PlayerInput pI) {
        playerList.Add(pI.gameObject);

        Debug.Log("Player Joined; ID: " + pI.playerIndex);

        if(playerList.Count == 1)
            cController.setSinglePlayerFlag(true);
        else
            cController.setSinglePlayerFlag(false);

    }

    // Removes Player to list when left
    public void OnPlayerLeft(PlayerInput pI) {
        for(int i=0; i<playerList.Count; i++) {
            if(playerList[i].GetComponent<PlayerInput>().playerIndex == pI.playerIndex) {
                playerList.RemoveAt(i);
            }
        }

        Debug.Log("Player Left; ID: " + pI.playerIndex);

        if(playerList.Count == 1)
            cController.setSinglePlayerFlag(true);
        else
            cController.setSinglePlayerFlag(false);
    }



    public List<GameObject> GetPlayerList() {
        return playerList;
    }
}
