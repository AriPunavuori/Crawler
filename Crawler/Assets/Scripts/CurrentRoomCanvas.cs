using UnityEngine;
using UnityEngine.UI;

public class CurrentRoomCanvas : MonoBehaviour {
    public Button startButton;
    void OnJoinedRoom() {
        if(PhotonNetwork.isMasterClient)
            startButton.interactable = true;
        else
            startButton.interactable = false;
    }
    public void OnClickStartSync() {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonNetwork.LoadLevel(2);
    }
    public void OnClickStartDelayed() {
        if(!PhotonNetwork.isMasterClient)
            return;

        PhotonNetwork.LoadLevel(2);
    }
}
