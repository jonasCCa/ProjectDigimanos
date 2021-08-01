using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerHandler : MonoBehaviour {
    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Start() {
        PV = GetComponent<PhotonView>();
        //int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        if(PV.IsMine) {
            int spawnPicker = this.PV.Controller.ActorNumber -1;
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerObj"),
                                                 GameSetup.gs.spawnPoints[spawnPicker].position,
                                                 GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);

            spawnPicker++;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
