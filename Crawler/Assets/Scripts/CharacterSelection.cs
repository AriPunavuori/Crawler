using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour {

    int playersSelectedCharacter = 0;
    int numberOfPlayers;


    private void Start() {
        PlayerNetwork.Instance.numberOfPlayers = PhotonNetwork.playerList.Length;
    }


    public void OnClickPickCharacter(int c) {
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PhotonView.RPC("RPC_PickedCharacter", PhotonTargets.MasterClient);
    }
}
