using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyNetwork : MonoBehaviour {
    bool beenToLobby;
    Button buttonCreate;
    public InputField roomInput;
    void Start() {
        buttonCreate = GameObject.Find("ButtonCreate").GetComponent<Button>();
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
        buttonCreate.interactable = true;
        if(PlayerPrefs.GetString("RoomName") != "") {
            roomInput.text = PlayerPrefs.GetString("RoomName");
        }
        
        if(!PhotonNetwork.inRoom) {
            MenuCanvasManager.Instance.JoinedLobby();
        }
    }

    public void OnValueChanged() {
        roomInput.text = roomInput.text.ToUpper();
    }
}
