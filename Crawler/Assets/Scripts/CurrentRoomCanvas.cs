using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour {
    public void OnClickStartSync() {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.LoadLevel(2);

    }

    [PunRPC]
    public void OnClickStartDelayed() {
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