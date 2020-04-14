using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelection : MonoBehaviour {

    public Button[] buttons;


    private void Start() {
        PlayerNetwork.Instance.numberOfPlayers = PhotonNetwork.playerList.Length;
        AudioFW.StopLoop("MenuLoop");
        AudioFW.PlayLoop("CharaterSelectionLoop");
        //AudioFW.StopLoop("LobbyMenuLoop");

    }


    public void OnClickPickCharacter(int c) {
        AudioFW.StopLoop("CharaterSelectionLoop");
        AudioFW.Play("CharacterSelected");
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PickedCharacter(c);
    }
}
