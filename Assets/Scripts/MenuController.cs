using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    
    public void OnCharacterPicked(int characterId) {
        if(PlayerInfo.PI != null) {
            PlayerInfo.PI.selectedCharacter = characterId;
            PlayerPrefs.SetInt("SelectedCharacter", characterId);
        }
    }
}
