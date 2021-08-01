using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviour {
    private PhotonView PV;
    public int characterId;
    public GameObject thisCharacterObj;

    // Start is called before the first frame update
    void Start() {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine) {
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.selectedCharacter-1);
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int selectedCharacter){
        characterId = selectedCharacter;
        thisCharacterObj = Instantiate(PlayerInfo.PI.availableCharacters[selectedCharacter], transform.position, transform.rotation, transform);
    }
}
