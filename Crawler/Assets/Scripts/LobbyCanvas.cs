using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour {

    public RoomLayoutGroup roomLayoutGroup;

    public void OnClickJoinRoom(string roomName) {
        if(PhotonNetwork.JoinRoom(roomName)) {

        } else {
            print("Joining room failed");
        }
    }
}
