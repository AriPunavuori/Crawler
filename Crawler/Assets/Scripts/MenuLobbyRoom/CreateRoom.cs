using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    public Text roomNameInput;
    public Button startGame;
    public void CreateRoomOnClick() {

        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        string roomName;
        if(roomNameInput.text != "") {
            roomName = roomNameInput.text.ToUpper();
            PlayerPrefs.SetString("RoomName", roomNameInput.text.ToUpper());
        } else {
            roomName = ("Room#" + Random.Range(1000, 9999)).ToUpper();
        }

        if(PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default)) {
            print("Create room named " + roomNameInput.text.ToUpper() + " sent");
        } else {
            print("Create room failed to send");
        }
        startGame.interactable = true;
    }
    void OnCreateRoomFailed(object[] codeAndMessage) {
        print("Create room failed: " + codeAndMessage[1]);
    }
    void OnCreatedRoom() {
        print("Created room successfully");
    }
}
