using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour {

    public void OnClickStartDelayed() {
        Invoke("StartCharacterSelection", .5f);
    }

    void StartCharacterSelection() {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.room.IsOpen = false;
        PhotonNetwork.room.IsVisible = false;
        PhotonNetwork.LoadLevel(2);
    }

    public void OnClickRoomState() {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;
    }
    public void OnClickLeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}