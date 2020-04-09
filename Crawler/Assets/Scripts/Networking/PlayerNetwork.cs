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
    PlayerCharacter pc;
    bool[] selectedCharacters = new bool[4];
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
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player, selectedCharacter);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    void NonMasterLoadedGame() {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player, selectedCharacter);
    }
    [PunRPC]
    void RPC_LoadGameOthers() {
        PhotonNetwork.LoadLevel(3);
    }

    [PunRPC]
    void RPC_LoadedGameScene(PhotonPlayer photonPlayer, int selected) {
        PlayerManager.Instance.AddPlayerStats(photonPlayer, selected);
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

        if(pc == null)
            return;
        if(pc.health <= 0) {
            print("Should die already");
            pc.Die();
        } else
            pc.health = health;
    }


    [PunRPC]
    void RPC_DisableButton(int selected) {
        var cs = FindObjectOfType<CharacterSelection>();
        if(selected >= 4) {
            for(int i = 0; i < 4; i++) {
                cs.buttons[i].interactable = false;
            }
        } else {
            cs.buttons[selected].interactable = false;
        }
    }
    //[PunRPC]
    //void RPC_DisableButton1() {
    //    var cs = FindObjectOfType<CharacterSelection>();
    //    cs.buttons[1].interactable = false;
    //}
    //[PunRPC]
    //void RPC_DisableButton2() {
    //    var cs = FindObjectOfType<CharacterSelection>();
    //    cs.buttons[2].interactable = false;
    //}
    //[PunRPC]
    //void RPC_DisableButton3() {
    //    var cs = FindObjectOfType<CharacterSelection>();
    //    cs.buttons[3].interactable = false;
    //}
    public void PickedCharacter(int c) {
        PlayerNetwork.Instance.PhotonView.RPC("RPC_PickedCharacter", PhotonTargets.MasterClient, PhotonNetwork.player, c);
    }

    [PunRPC]
    void RPC_PickedCharacter(PhotonPlayer photonPlayer, int selected) {
        if(!PhotonNetwork.isMasterClient)
            return;
        if(selectedCharacters[selected] == false) {
            selectedCharacters[selected] = true;
            playersSelectedCharacter++;
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton", photonPlayer, 4);
            PlayerNetwork.Instance.PhotonView.RPC("RPC_DisableButton", PhotonTargets.All, selected);
            print("Player picked a character");
            if(playersSelectedCharacter >= numberOfPlayers) {
                PhotonNetwork.LoadLevel(3);
            }
        }
    }

    [PunRPC]
    void RPC_CreatePlayer() {
        int id = PhotonNetwork.player.ID;
        string[] heroType = new string[] { "NetworkPlayer0", "NetworkPlayer1", "NetworkPlayer2", "NetworkPlayer3" };
        GameObject obj = PhotonNetwork.Instantiate(heroType[selectedCharacter], new Vector3(0 + id, 0.5f, 0), Quaternion.identity, 0);
        obj.name = playerName;
        pc = obj.GetComponent<PlayerCharacter>();
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
