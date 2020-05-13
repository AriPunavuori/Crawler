using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerNetwork : MonoBehaviour {
    public RectTransform nameInteractables;
    public Button skipIntro;
    public Button nameEntryButton;
    public GameObject Intro;
    public GameObject nameEntry;
    public RectTransform introText;
    public static PlayerNetwork Instance;
    public string playerName;
    public InputField input;
    public PhotonView PhotonView;
    public int selectedCharacter;
    int playersSelectedCharacter = 0;
    public int numberOfPlayers;
    int PlayersInGame = 0;
    PlayerCharacter pc;
    bool[] selectedCharacters = new bool[4];
    Action watchedIntro;
    Action loadMenu;
    Action menuMusic;


    void Awake() {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void Start() {
        watchedIntro += WatchIntro;
        watchedIntro += NameEntry;
        loadMenu += LoadMenu;
        menuMusic += PlayMenuMusic;
        input.text = PlayerPrefs.GetString("Name");
        //PlayerPrefs.SetInt("IntroSeen", 0);
        if(PlayerPrefs.GetInt("IntroSeen") == 1) {
            skipIntro.interactable = true;
            skipIntro.Select();
        }
        LeanTween.move(introText, Vector2.up * 1900, 50f).setOnComplete(watchedIntro);
        AudioFW.PlayLoop("IntroLoop");
        AudioFW.AdjustLoopVolume("IntroLoop", .25f, 1f);
    }

    void WatchIntro() {
        PlayerPrefs.SetInt("IntroSeen", 1);
        skipIntro.interactable = true;
        skipIntro.Select();
    }
    void PlayMenuMusic() {
        AudioFW.PlayLoop("MenuLoop");
        AudioFW.AdjustLoopVolume("MenuLoop", .25f, 2.5f);
    }

    public void NameEntry() {
        Intro.SetActive(false);
        nameEntry.SetActive(true);
        nameEntryButton.Select();
        LeanTween.move(nameInteractables, Vector3.zero, .5f).setEaseOutBack().setOnComplete(menuMusic);
        AudioFW.StopLoop("IntroLoop");
        //AudioFW.AdjustLoopVolume("IntroLoop", 0f, .5f);
        PlayMenuMusic();
        AudioFW.Play("Whip");
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        if(scene.name == "CharacterSelection") {
            if(PhotonNetwork.isMasterClient)
                PhotonView.RPC("RPC_LoadCharacterSelection", PhotonTargets.Others);
        }
        if(scene.name == "Level1") {
            PlayersInGame = 0;
            if(PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
        if(scene.name == "Credits") {
            if(PhotonNetwork.isMasterClient) {
                MasterLoadedCredits();
            }
        }
    }

    void MasterLoadedCredits() {
        PhotonView.RPC("RPC_LoadCreditsOthers", PhotonTargets.Others);
    }

    [PunRPC]
    void RPC_LoadCreditsOthers() {
        PhotonNetwork.LoadLevel(4);
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
        if(health <= 0) {
            print("Should die already");
            pc.Die();
        } else
            pc.health = health;
    }

    [PunRPC]
    void RPC_DisableButton(int selected) {
        var cs = FindObjectOfType<CharacterSelection>();
        var im = FindObjectOfType<InfoManager>();
        if(selected >= 4) {
            for(int i = 0; i < 4; i++) {
                cs.buttons[i].gameObject.SetActive(false);
            }
            cs.AfterSelection();
            im.SelectedCharacter();
        } else {
            cs.buttons[selected].interactable = false;
            foreach(var b in cs.buttons) {
                if(b.interactable == true) {
                    b.Select();
                    return;
                }
            }
        }
    }

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
                Invoke("LoadLevel", 5.5f);
            }
        }
    }

    void LoadLevel() {
        PhotonNetwork.LoadLevel(3);
    }

    [PunRPC]
    void RPC_CreatePlayer() {
        int id = PhotonNetwork.player.ID;
        string[] heroType = new string[] { "NetworkPlayer0", "NetworkPlayer1", "NetworkPlayer2", "NetworkPlayer3" };
        GameObject obj = PhotonNetwork.Instantiate(heroType[selectedCharacter], new Vector3(0 + id, 0.5f, 0), Quaternion.identity, 0);
        obj.name = playerName;
        pc = obj.GetComponent<PlayerCharacter>();
    }


    public void OnClickStartButton() {

        if(input.text != "") {
            PlayerPrefs.SetString("Name", input.text.ToUpper());
            playerName = input.text.ToUpper();
        } else
            playerName = ("Player " + UnityEngine.Random.Range(1000, 9999)).ToUpper();
        AudioFW.Play("Whip");
        LeanTween.move(nameInteractables, Vector3.right * 2500, .5f).setEaseOutQuart().setOnComplete(loadMenu);
    }

    public void LoadMenu() {
        selectedCharacter = -1;
        playersSelectedCharacter = 0;
        numberOfPlayers = 0;
        PlayersInGame = 0;
        pc = null;
        for(int i = 0; i < selectedCharacters.Length; i++) {
            selectedCharacters[i] = false;
        }
        PhotonNetwork.LoadLevel(1);
    }

    public void OnValueChanged() {
        input.text = input.text.ToUpper();
    }
}
