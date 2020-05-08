using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasManager : MonoBehaviour {

    Action goLobby;
    Action goRoom;

    public Button createRoom;
    public Button startGame;
    public Button leaveGame;

    public static MenuCanvasManager Instance;

    public LobbyCanvas lobbyCanvas;

    public CurrentRoomCanvas currentRoomCanvas;

    public RectTransform roomInteractables;

    public RectTransform lobbyInteractables;

    private void Start() {
        goLobby += InLobby;
        goRoom += InRoom;
    }

    public void JoinedRoom() {
        LeanTween.move(lobbyInteractables, Vector3.right * 2500, .5f).setEaseInExpo().setOnComplete(goRoom);
    }

    public void JoinedLobby() {
        LeanTween.move(roomInteractables, Vector3.right * 2500, .5f).setEaseInExpo().setOnComplete(goLobby);
    }

    void InLobby() {
        var rb = roomInteractables.GetComponentsInChildren<Button>();
        foreach(var b in rb) {
            b.interactable = false;
        }
        var lb = lobbyInteractables.GetComponentsInChildren<Button>();
        foreach(var b in lb) {
            b.interactable = true;
        }
        MenuCanvasManager.Instance.lobbyCanvas.transform.SetAsLastSibling();
        LeanTween.move(lobbyInteractables, Vector3.zero, .5f).setEaseOutExpo();
        if(FindObjectOfType<RoomLayoutGroup>().roomListingButtons.Count<1)
        createRoom.Select();
    }

    void InRoom() {
        var rb = roomInteractables.GetComponentsInChildren<Button>();
        foreach(var b in rb) {
            b.interactable = true;
        }
        var lb = lobbyInteractables.GetComponentsInChildren<Button>();
        foreach(var b in lb) {
            b.interactable = false;
        }
        MenuCanvasManager.Instance.currentRoomCanvas.transform.SetAsLastSibling();
        LeanTween.move(roomInteractables, Vector3.zero, .5f).setEaseOutExpo();
        if(PhotonNetwork.isMasterClient)
            startGame.Select();
        else
            leaveGame.Select();
    }


    private void Awake() {
        Instance = this;
    }
}
