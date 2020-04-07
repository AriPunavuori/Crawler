using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour {

    public GameObject roomListingPrefab;

    public List<RoomListing> roomListingButtons = new List<RoomListing>();

    void OnReceivedRoomListUpdate() {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach(var room in rooms) {
            RoomReceived(room);
        }
        RemoveOldRooms();
    }
    void RoomReceived(RoomInfo room) {
        int index = roomListingButtons.FindIndex(x => x.roomName == room.Name);
        if(index == -1) {
            if(room.IsVisible && room.PlayerCount < room.MaxPlayers) {
                GameObject roomListingObj = Instantiate(roomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);
                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                roomListingButtons.Add(roomListing);

                index = (roomListingButtons.Count - 1);
            }
        }
        if(index != -1) {
            RoomListing roomListing = roomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);
            roomListing.updated = true;
        }
    }
    void RemoveOldRooms() {
        List<RoomListing> removeRooms = new List<RoomListing>();
        foreach(var roomListing in roomListingButtons) {
            if(!roomListing.updated)
                removeRooms.Add(roomListing);
            else
                roomListing.updated = false;
        }
        foreach(var roomListing in removeRooms) {
            GameObject roomListingObj = roomListing.gameObject;
            roomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}
