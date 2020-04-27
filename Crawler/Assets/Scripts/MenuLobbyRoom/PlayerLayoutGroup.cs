using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour {

    public GameObject playerListingPrefab;
    public List<PlayerListing> playerListings = new List<PlayerListing>();
    
    public Button startGame;

    // Called by Photon when Master Client is switched
    void OnMasterClientSwitched() {
        print("Master switched");
        PhotonNetwork.LeaveRoom();
    }

    void OnJoinedRoom() {
        if(PhotonNetwork.isMasterClient)
            startGame.interactable = true;
        else
            startGame.interactable = false;

        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        Invoke("SwitchCanvas", .5f);
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i = 0; i < photonPlayers.Length; i++) {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    void SwitchCanvas() {
        MenuCanvasManager.Instance.playerList.GetComponent<UIElementJuicer>().enabled = true;
        MenuCanvasManager.Instance.currentRoomCanvas.transform.SetAsLastSibling();
    }
    
    // Called by photon when a player joins the room
    void OnPhotonPlayerConnected(PhotonPlayer photonPlayer) {
        PlayerJoinedRoom(photonPlayer);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
        PlayerLeftRoom(photonPlayer);
    }

    void PlayerJoinedRoom(PhotonPlayer photonPlayer) {
        if(photonPlayer == null) {
            return;
        }
        GameObject playerListingsObj = Instantiate(playerListingPrefab);
        playerListingsObj.transform.SetParent(transform, false);
        PlayerListing playerListing = playerListingsObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListings.Add(playerListing);
    }

    void PlayerLeftRoom(PhotonPlayer photonPlayer) {
        print(photonPlayer.NickName + " left room");
        print(playerListings.FindIndex(x => x.PhotonPlayer == photonPlayer)); 
        int index = playerListings.FindIndex(x => x.PhotonPlayer == photonPlayer); // Does not find proper index on the masterclient returns -1 and bypasses deletion
        if(index != -1) {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }
}
