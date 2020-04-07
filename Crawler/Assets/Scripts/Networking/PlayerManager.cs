using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager Instance;
    PhotonView photonView;
    List<PlayerStats> PlayerStats = new List<PlayerStats>();
    void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void AddPlayerStats(PhotonPlayer photonPlayer, int selectedCharacter) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index == -1) {           
            PlayerStats.Add(new PlayerStats(photonPlayer, photonPlayer.NickName, 0, selectedCharacter));
            // Add UIBox to UIManager   
            photonView.RPC("UpdateUIBoxes", PhotonTargets.All);
            photonView.RPC("UpdateUIBoxContent", PhotonTargets.All,
                                                                PlayerStats[index].Name,
                                                                PlayerStats[index].Health,
                                                                PlayerStats[index].SelectedCharacter);
        }
    }

    public void ModifyHealth(PhotonPlayer photonPlayer, int value) {
        int index = PlayerStats.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if(index != -1) {
            PlayerStats playerStats = PlayerStats[index];
            //print("Original health: " + playerStats.Health);
            playerStats.Health += value;
            //print(photonPlayer.NickName + " Health changed " + value+"!");
            PlayerNetwork.Instance.NewHealth(photonPlayer, playerStats.Health);
            photonView.RPC("UpdateUIBoxContent", PhotonTargets.All, PlayerStats[index].Name,
                                                                    PlayerStats[index].Health,
                                                                    PlayerStats[index].SelectedCharacter);
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