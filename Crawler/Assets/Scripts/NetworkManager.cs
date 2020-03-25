using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Photon.PunBehaviour {
    private const string roomName = "RoomName";
    private RoomInfo[] roomList;
    public List<PhotonPlayer> currentPlayersInRoom = new List<PhotonPlayer>();
    public bool joined;

    void Start() {
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
        PhotonNetwork.ConnectUsingSettings("0,1");
    }

    private void OnGUI() {
        // Ruudun vasemmassa ylä nurkassa tietoa
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        //Jos ei olla missään huoneessa, eli ollaan lobyssa, näytä nappuloita huoneista
        if(PhotonNetwork.room == null) {
            if(GUI.Button(new Rect(100, 100, 250, 100), "Start Server (Create Room)")) {
                //PhotonNetwork.CreateRoom()
                // Luodaan jokaiselle huoneelle satunnainen nimi
                PhotonNetwork.CreateRoom(roomName + System.Guid.NewGuid().ToString("N"));
            }
            if(roomList != null) {
                for(int i = 0; i < roomList.Length; i++) {
                    if(GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomList[i].Name + "\n\nMax: " + roomList[i].PlayerCount)) {
                        PhotonNetwork.JoinRoom(roomList[i].Name);
                    }
                }
            }
        }
    }
    public override void OnConnectedToPhoton() {
        Debug.Log("Yhteys Photoniin");
    }

    public override void OnJoinedLobby() {
        Debug.Log("Tultiin lobbyyn");
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Masteryhteys");
    }
    public override void OnReceivedRoomListUpdate() {
        roomList = PhotonNetwork.GetRoomList();
    }

    public override void OnCreatedRoom() {
        Debug.Log("Huone tehty");
    }

    public override void OnJoinedRoom() {

    PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        //PhotonNetwork.Instantiate("MagicalGirl", new Vector3(0, 0, 0), Quaternion.identity, 0);
        joined = true;
    }

    private void OnConnectedToServer() {
    }
}

