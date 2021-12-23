using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PlayerInputManager playerInputManager;
    public GameObject gameCamera;
    [SerializeField] private List<GameObject> playerList;

    [Header("Camera Configs")]
    public float cameraSmoothTime = 0.5f;
    public float cameraMaxSpeed = 50f;
    public Vector3 cameraOffset;
    public GameObject cameraTarget;
    
    [Header("Read Only")]
    [SerializeField] private Vector3 velocity = Vector3.zero;

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
        cameraTarget.transform.position = Vector3.SmoothDamp(cameraTarget.transform.position, calculateTarget() + cameraOffset,
                                                            ref velocity, cameraSmoothTime, cameraMaxSpeed, Time.deltaTime);

        //gameCamera.transform.position = cameraTarget.transform.position + cameraOffset;

        gameCamera.transform.LookAt(cameraTarget.transform);
    }

    // Calculates centroid between all players
    Vector3 calculateTarget() {
        Vector3 newTarget = new Vector3(0,0,0);

        foreach(GameObject player in playerList) {
            newTarget += player.transform.position;
        }
        if(playerList.Count > 0)
            newTarget = newTarget / playerList.Count;

        //Debug.Log(newTarget.ToString());
        return newTarget;
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
