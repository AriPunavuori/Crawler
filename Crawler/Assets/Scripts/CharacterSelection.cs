using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour {

    int playersSelectedCharacter = 0;
    int numberOfPlayers;

    private void Start() {
        numberOfPlayers = PhotonNetwork.playerList.Length;
    }

    public void OnClickPickCharacter(int c) {
        playersSelectedCharacter++;
        PlayerNetwork.Instance.selectedCharacter = c;
        if(playersSelectedCharacter >= numberOfPlayers) {
            if(!PhotonNetwork.isMasterClient)
                return;
            PhotonNetwork.LoadLevel(3);
        }
    }
}
