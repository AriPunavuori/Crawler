using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {
    bool beenToLobby;
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
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    void OnJoinedLobby() {
        print("Joined lobby");
        if(!PhotonNetwork.inRoom) {
            Invoke("SwitchCanvas", .5f);
            if(beenToLobby) {
                var playerList = GameObject.Find("PlayerList").GetComponent<UIElementJuicer>();
                playerList.MoveOut(false);
                var roomList = GameObject.Find("RoomList").GetComponent<UIElementJuicer>();
                roomList.MoveIn(true);
            }
        }
        beenToLobby = true;
    }

    void SwitchCanvas() {
        MenuCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
    }

}
