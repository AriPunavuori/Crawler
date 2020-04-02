using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelection : MonoBehaviour {

    public Button[] buttons;


    private void Start() {
        PlayerNetwork.Instance.numberOfPlayers = PhotonNetwork.playerList.Length;
    }


    public void OnClickPickCharacter(int c) {
        foreach(var button in buttons) {
            button.interactable = false;
        }
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PhotonView.RPC("RPC_PickedCharacter", PhotonTargets.MasterClient);
        if(c == 0)
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton0", PhotonTargets.All);
        if(c == 1)
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton1", PhotonTargets.All);
        if(c == 2)
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton2", PhotonTargets.All);
        if(c == 3)
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton3", PhotonTargets.All);
    }
}
