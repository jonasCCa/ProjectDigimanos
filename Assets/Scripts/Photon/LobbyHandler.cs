using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyHandler : MonoBehaviourPunCallbacks {
    public static LobbyHandler lobby;
    public static int roomName;

    public GameObject startButton;
    public GameObject cancelButton;
    public GameObject offlineButton;

    private void Awake() {
        lobby = this;
        roomName = 0;
    }

    // Start is called before the first frame update
    void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update() {
        if(PlayerInfo.PI.selectedCharacter > 0) {
            startButton.GetComponent<Button>().interactable = true;
        } else {
            startButton.GetComponent<Button>().interactable = false;
        }
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
        offlineButton.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log(cause);
        offlineButton.SetActive(true);
    }

    public void onStartButtonClick() {
        startButton.SetActive(false);
        cancelButton.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
    }

    public void onCancelButtonClick() {
        Debug.Log("Cancelled connection");
        cancelButton.SetActive(false);
        startButton.SetActive(true);

        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed joining a radom Room; Creating new Room...");
        CreateRoom();
    }

    void CreateRoom() {
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        PhotonNetwork.CreateRoom("Room#"+roomName, roomOps);
        roomName++;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Failed to create a new Room; Creating a new Room...");
        CreateRoom();
    }
}
