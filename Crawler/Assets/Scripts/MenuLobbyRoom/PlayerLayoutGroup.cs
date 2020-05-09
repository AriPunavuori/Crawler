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
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        MenuCanvasManager.Instance.JoinedRoom();
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
        AudioFW.Play("Joined");
        GameObject playerListingsObj = Instantiate(playerListingPrefab);
        playerListingsObj.transform.SetParent(transform, false);
        PlayerListing playerListing = playerListingsObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListings.Add(playerListing);
    }

    void PlayerLeftRoom(PhotonPlayer photonPlayer) {
        AudioFW.Play("Left");
        int index = playerListings.FindIndex(x => x.photonPlayer == photonPlayer);
        if(index != -1) {
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);
        }
    }
}
