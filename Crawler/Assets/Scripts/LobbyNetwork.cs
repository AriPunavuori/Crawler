using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {

    void Start() {
        if(!PhotonNetwork.connected) {
            print("Connecting to server");
            PhotonNetwork.ConnectUsingSettings("0.0.0");
        }
    }
    void OnConnectedToMaster() {
        print("Connected to master");
        PhotonNetwork.automaticallySyncScene = false;
        print(PlayerNetwork.Instance.playerName + " is PlayerNetwork.playerName");
        PhotonNetwork.playerName = PlayerNetwork.Instance.playerName;
        print(PhotonNetwork.playerName + " is PhotonNetwork.playerName");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    void OnJoinedLobby() {
        print("Joined lobby");
        if(!PhotonNetwork.inRoom)
        MenuCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }

}
