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

    [Header("UI")]
    public List<GameObject> playerIndicators;
    public List<GameObject> playerUI;

    [Header("Tests")]
    public List<Material> playerMaterials;

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
        int playerCount = 0;

        foreach(GameObject player in playerList) {
            if(player.GetComponent<StatsController>().isAlive) {
                newTarget += player.transform.position;
                playerCount++;
            }
        }
        if(playerList.Count > 0)
            newTarget = newTarget / playerCount;

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
                    if(playerList[i].GetComponent<StatsController>().isAlive && playerList[j].GetComponent<StatsController>().isAlive) {
                        float aux = Vector3.Distance(playerList[i].transform.position, playerList[j].transform.position);
                        if(aux > longestDistance) {
                            longestDistance = aux;
                        }
                    }
                }
            }
        }

        return longestDistance;
    }

    // Calculates the height difference between the lowest and the heighest players
    public float calculatePlayerHeightDifference() {
        float lowestHeight = float.MaxValue;
        float highestHeight = float.MinValue;

        foreach(GameObject player in playerList) {
            if(player.transform.position.y < lowestHeight)
                lowestHeight = player.transform.position.y;
            else if(player.transform.position.y > highestHeight)
                highestHeight = player.transform.position.y;
        }

        return highestHeight - lowestHeight;
    }

    // Adds Player to list when joined, set SinglePlayer flag on CameraController
    public void OnPlayerJoined(PlayerInput pI) {
        playerList.Add(pI.gameObject);

        // Place-holder for player model
        pI.gameObject.GetComponent<MeshRenderer>().material = playerMaterials[pI.playerIndex];
        pI.gameObject.GetComponent<PlayerController>().indicator = playerIndicators[pI.playerIndex];
        pI.gameObject.GetComponent<PlayerController>().playerMenu = playerUI[pI.playerIndex].transform.GetChild(0).gameObject;
        
        // Activate player's UI
        playerUI[pI.playerIndex].SetActive(true);

        // Update player bars
        pI.gameObject.GetComponent<StatsController>().hpBar = playerUI[pI.playerIndex].transform.GetChild(2).GetChild(0).GetComponent<BarController>();
        pI.gameObject.GetComponent<StatsController>().mpBar = playerUI[pI.playerIndex].transform.GetChild(2).GetChild(1).GetComponent<BarController>();
        pI.gameObject.GetComponent<StatsController>().expBar = playerUI[pI.playerIndex].transform.GetChild(2).GetChild(2).GetComponent<BarController>();

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

        // Dectivate player's UI
        playerUI[pI.playerIndex].SetActive(false);

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
