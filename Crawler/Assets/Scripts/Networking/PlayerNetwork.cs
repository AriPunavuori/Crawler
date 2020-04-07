using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerNetwork : MonoBehaviour {
    public static PlayerNetwork Instance;
    public string playerName;
    public InputField input;
    public PhotonView PhotonView;
    public int selectedCharacter;
    int playersSelectedCharacter = 0;
    public int numberOfPlayers;
    int PlayersInGame = 0;
    public bool joined;
    PlayerCharacter currentPlayer;
    PlayerCharacter pc;

    void Awake() {

        Instance = this;
        PhotonView = GetComponent<PhotonView>();
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        if(scene.name == "CharacterSelection") {
            if(PhotonNetwork.isMasterClient)
                PhotonView.RPC("RPC_LoadCharacterSelection", PhotonTargets.Others);
        }
        if(scene.name == "Level1") {
            if(PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }
    void MasterLoadedCharacterSelection() {
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    [PunRPC]
    void RPC_LoadCharacterSelection() {
        PhotonNetwork.LoadLevel(2);
    }
    void MasterLoadedGame() {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    void NonMasterLoadedGame() {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }
    [PunRPC]
    void RPC_LoadGameOthers() {
        PhotonNetwork.LoadLevel(3);
    }

    [PunRPC]
    void RPC_LoadedGameScene(PhotonPlayer photonPlayer) {
        PlayerManager.Instance.AddPlayerStats(photonPlayer, selectedCharacter);
        PlayersInGame++;
        if(PlayersInGame == PhotonNetwork.playerList.Length) {
            print("All players in game scene");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }


    public void NewHealth(PhotonPlayer photonPlayer, int health) {
        PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
    }

    [PunRPC]
    void RPC_NewHealth(int health) {

        if(currentPlayer == null)
            return;
        if(currentPlayer.health <= 0) {
            //print("Should die already");
            //var pc = GetComponent<PlayerCharacter>();
            //pc.Die();
            //PhotonNetwork.Destroy(currentPlayer.gameObject);
        } else
            currentPlayer.health = health;
    }


    [PunRPC]
    void RPC_DisableButton0() {
        var cs = FindObjectOfType<CharacterSelection>();
        cs.buttons[0].interactable = false;
    }
    [PunRPC]
    void RPC_DisableButton1() {
        var cs = FindObjectOfType<CharacterSelection>();
        cs.buttons[1].interactable = false;
    }
    [PunRPC]
    void RPC_DisableButton2() {
        var cs = FindObjectOfType<CharacterSelection>();
        cs.buttons[2].interactable = false;
    }
    [PunRPC]
    void RPC_DisableButton3() {
        var cs = FindObjectOfType<CharacterSelection>();
        cs.buttons[3].interactable = false;
    }

    [PunRPC]
    void RPC_PickedCharacter() {
        if(!PhotonNetwork.isMasterClient)
            return;
        playersSelectedCharacter++;
        print("Player picked a character");
        if(playersSelectedCharacter >= numberOfPlayers) {
            PhotonNetwork.LoadLevel(3);
        }
    }

    [PunRPC]
    void RPC_CreatePlayer() {
        int id = PhotonNetwork.player.ID;
        GameObject obj = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0 + id, 0.5f, 0), Quaternion.identity, 0);
        obj.name = playerName;
        currentPlayer = obj.GetComponent<PlayerCharacter>();
        currentPlayer.characterType = (EntityType)selectedCharacter;
        joined = true;
    }

    public bool joinedGame() {
        if(joined == true) {
            return true;
        }
        return false;
    }
    public void OnClickStartButton() {
        if(input.text != null) {
            playerName = input.text;
        } else
            playerName = "Player#" + Random.Range(1000, 9999);
        PhotonNetwork.LoadLevel(1);
    }
}
