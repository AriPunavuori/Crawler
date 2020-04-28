﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    public Text roomName;
    public Button startGame;
    public void CreateRoomOnClick() {

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        if(PhotonNetwork.CreateRoom(roomName.text, roomOptions, TypedLobby.Default)) {
            print("Create room named " + (string)roomName.text + " sent");
        } else {
            print("Create room failed to send");
        }
        startGame.interactable = true;
    }
    void OnCreateRoomFailed(object[] codeAndMessage) {
        print("Create room failed: " + codeAndMessage[1]);
    }
    void OnCreatedRoom() {
        var buttonStart = GameObject.Find("ButtonStart").GetComponent<Button>();
        buttonStart.Select();
        print("Created room successfully");
    }
}
