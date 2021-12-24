using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages multiple player intances
public class MultiplayerManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PlayerInputManager playerInputManager;
    public GameObject gameCamera;
    [SerializeField] private List<GameObject> playerList;
    [SerializeField] private float longestPlayerDistance;

    [Header("Camera Configs")]
    public float distanceRatio = 1f;
    public float maxCameraDistance = 15f;
    public float cameraSmoothTime = 0.05f;
    public float cameraMaxSpeed = 50f;
    public Vector3 cameraOffset = new Vector3(0,3.15f,-3.99f);
    public GameObject cameraTarget;
    
    [Header("Read Only")]
    [SerializeField] private Vector3 targetVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraOffsettedPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        gameCamera = GameObject.Find("MainCamera");

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        // Calculates camera target position
        cameraOffsettedPosition = calculateTargetPosition();
        // Calculates the longest distance between players
        longestPlayerDistance = calculatePlayerDistance();

        // Smooths camera target position
        cameraTarget.transform.position = Vector3.SmoothDamp(cameraTarget.transform.position, calculateTargetPosition(),
                                                            ref targetVelocity, cameraSmoothTime, cameraMaxSpeed, Time.deltaTime);
        // Points camera to target
        gameCamera.transform.LookAt(cameraTarget.transform);
        // Smooths camera position
        gameCamera.transform.position = Vector3.SmoothDamp(gameCamera.transform.position, calculateCameraPosition() + cameraOffset,
                                                            ref cameraVelocity, cameraSmoothTime, cameraMaxSpeed, Time.deltaTime);

    }

    Vector3 calculateCameraPosition() {
        Vector3 auxResult = new Vector3();

        auxResult = Vector3.ClampMagnitude(gameCamera.transform.forward*(calculatePlayerDistance() * distanceRatio), maxCameraDistance);

        return cameraOffsettedPosition - auxResult;
    }

    // Calculates centroid between all players
    Vector3 calculateTargetPosition() {
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
    float calculatePlayerDistance() {
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

    // Adds Player to list when joined
    public void OnPlayerJoined(PlayerInput pI) {
        playerList.Add(pI.gameObject);

        Debug.Log("Player Joined; ID: " + pI.playerIndex);
    }

    // Removes Player to list when left
    public void OnPlayerLeft(PlayerInput pI) {
        for(int i=0; i<playerList.Count; i++) {
            if(playerList[i].GetComponent<PlayerInput>().playerIndex == pI.playerIndex) {
                playerList.RemoveAt(i);
            }
        }

        Debug.Log("Player Left; ID: " + pI.playerIndex);
    }

    // Draws lines between centroid and players
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach(GameObject player in playerList) {
            Gizmos.DrawLine(cameraTarget.transform.position, player.transform.position);
        }
    }
}
