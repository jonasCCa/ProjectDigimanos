using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour {
    public static PlayerInfo PI;
    public int selectedCharacter;
    public GameObject[] availableCharacters;

    [SerializeField] Dropdown dropdown;


    void OnEnable() {
        if(PlayerInfo.PI==null)
            PlayerInfo.PI = this;
        else {
            if(PlayerInfo.PI != this) {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start() {
        if(PlayerPrefs.HasKey("SelectedCharacter")) {
            selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
        } else {
            selectedCharacter = 0;
            PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        }
        dropdown.SetValueWithoutNotify(selectedCharacter);
    }
}
