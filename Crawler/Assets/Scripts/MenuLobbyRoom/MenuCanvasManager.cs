using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasManager : MonoBehaviour {

    Action goLobby;
    Action goRoom;

    bool firstTimeInLobby = true;

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
        HideLobby();
    }

    public void JoinedLobby() {
        if(!firstTimeInLobby) {
            HideRoom(true);
        } else {
            firstTimeInLobby = false;
            InLobby();
        }
    }

    public void HideLobby() {
        AudioFW.Play("Whip");
        LeanTween.move(lobbyInteractables, Vector3.right * 2500, .5f).setEaseOutQuart().setOnComplete(goRoom);
    }

    public void HideRoom(bool inLobby) {
        AudioFW.Play("Whip");
        if(inLobby)
            LeanTween.move(roomInteractables, Vector3.right * 2500, .5f).setEaseOutQuart().setOnComplete(goLobby);
        else
            LeanTween.move(roomInteractables, Vector3.right * 2500, .5f).setEaseOutQuart();
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
        AudioFW.Play("Whip");
        LeanTween.move(lobbyInteractables, Vector3.zero, .5f).setEaseOutQuart();
        if(FindObjectOfType<RoomLayoutGroup>().roomListingButtons.Count < 1)
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
        AudioFW.Play("Whip");
        LeanTween.move(roomInteractables, Vector3.zero, .5f).setEaseOutQuart();
        if(PhotonNetwork.isMasterClient) {
            startGame.interactable = true;
            startGame.Select();
        } else {
            startGame.interactable = false;
            leaveGame.Select();
        }
    }


    private void Awake() {
        Instance = this;
    }
}
