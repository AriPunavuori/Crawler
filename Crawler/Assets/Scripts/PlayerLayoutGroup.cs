using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour {

    public GameObject playerListingPrefab;

    List<PlayerListing> playerListings = new List<PlayerListing>();

    // Called by Photon when Master Client is switched
    void OnMasterClientSwitched() {
        print("Master switched");
        PhotonNetwork.LeaveRoom();
    }

    void OnJoinedRoom() {
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        MenuCanvasManager.Instance.currentRoomCanvas.transform.SetAsLastSibling();
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for(int i = 0; i < photonPlayers.Length; i++) {
            PlayerJoinedRoom(photonPlayers[i]);
        }
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
        PlayerLeftRoom(photonPlayer);
        GameObject playerListingsObj = Instantiate(playerListingPrefab);
        playerListingsObj.transform.SetParent(transform, false);
        PlayerListing playerListing = playerListingsObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListings.Add(playerListing);
    }
    void PlayerLeftRoom(PhotonPlayer photonPlayer) {
        int index = playerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index != -1) {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }


}
