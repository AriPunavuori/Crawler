﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager Instance;
    PhotonView photonView;
    List<PlayerStats> PlayerStats = new List<PlayerStats>();
    Character baseHealth;

    void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        baseHealth = GameObject.Find("BaseHealth").GetComponent<Character>();
    }

    public void AddPlayerStats(PhotonPlayer photonPlayer, int selectedCharacter) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index == -1) {
            PlayerStats.Add(new PlayerStats(photonPlayer, photonPlayer.NickName, 0, selectedCharacter));
        }
    }

    public void ModifyHealth(PhotonPlayer photonPlayer, int value) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index != -1) {
            PlayerStats playerStats = PlayerStats[index];
            if(playerStats.Health + value > baseHealth.CheckCharacterHealt((EntityType)playerStats.SelectedCharacter))
                playerStats.Health = baseHealth.CheckCharacterHealt((EntityType)playerStats.SelectedCharacter);
            else if(playerStats.Health + value <= 0)
                playerStats.Health = 0;
            else
                playerStats.Health += value;
            PlayerNetwork.Instance.NewHealth(photonPlayer, playerStats.Health);
            print("Changing health for hero: " + PlayerStats[index].SelectedCharacter + " " + value);
            UIManager.Instance.UpdateUIContent(PlayerStats[index].Name,
                                                PlayerStats[index].Health,
                                                PlayerStats[index].SelectedCharacter,
                                                baseHealth.CheckCharacterHealt((EntityType)playerStats.SelectedCharacter));
        }
    }

    public string GetName(PhotonPlayer photonPlayer) {
        return photonPlayer.NickName;
    }

    public int GetHealth(PhotonPlayer photonPlayer) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index != -1) {
            PlayerStats playerStats = PlayerStats[index];
            return playerStats.Health;
        }
        return 0;
    }
}

public class PlayerStats {
    public PlayerStats(PhotonPlayer photonPlayer, string name, int healt, int selectedCharacter) {
        PhotonPlayer = photonPlayer;
        Name = name;
        Health = healt;
        SelectedCharacter = selectedCharacter;
    }
    public readonly PhotonPlayer PhotonPlayer;
    public readonly string Name;
    public int Health;
    public readonly int SelectedCharacter;
}