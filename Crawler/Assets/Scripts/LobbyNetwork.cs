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
        PhotonNetwork.playerName = PlayerNetwork.Instance.playerName;
        print(PhotonNetwork.playerName);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    void OnJoinedLobby() {
        print("Joined lobby");
        if(!PhotonNetwork.inRoom)
        MenuCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }
}
