﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCanvasManager : MonoBehaviour {
    
    public static MenuCanvasManager Instance;

    public LobbyCanvas lobbyCanvas;

    public CurrentRoomCanvas currentRoomCanvas;

    private void Awake() {
        Instance = this;
    }
}