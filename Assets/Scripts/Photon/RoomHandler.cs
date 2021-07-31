using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class RoomHandler : MonoBehaviourPunCallbacks, IInRoomCallbacks {

    public static RoomHandler room;
    private PhotonView PV;
    public int pickedScene;
    public int currentScene;

    public Player[] players;
    public int roomPlayers;
    public int currentIDInRoom;

    private void Awake() {
        if(RoomHandler.room == null)
            room = this;
        else {
            if(RoomHandler.room != this) {
                Destroy(RoomHandler.room);
                RoomHandler.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable() {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable() {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        Debug.Log("Successfully joind a Room");
        // players = PhotonNetwork.PlayerList;
        // roomPlayers = players.Length;
        // currentIDInRoom = roomPlayers;
        // PhotonNetwork.NickName = currentIDInRoom.ToString();
        if(!PhotonNetwork.IsMasterClient) {
            Debug.Log("Non-Master client tried to Start a game; returned");
            return;
        }
        StartGame();

    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.buildIndex;
        if(currentScene == pickedScene)
            CreatePlayer();
    }

    private void CreatePlayer() {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    void Update() {
        
    }

    void StartGame() {
        Debug.Log("Starting a game...");
        PhotonNetwork.LoadLevel(pickedScene);
    }
}
