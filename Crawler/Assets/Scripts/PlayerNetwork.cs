using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerNetwork : MonoBehaviour
{
    public static PlayerNetwork Instance;
    public string playerName;
    public InputField input;
    PhotonView PhotonView;
    int PlayersInGame = 0;

    public bool joined;

    PlayerMovement CurrentPlayer;

    void Awake()
    {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }
    void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    }
    [PunRPC]
    void RPC_LoadGameOthers()
    {
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    void RPC_LoadedGameScene(PhotonPlayer photonPlayer)
    {
        Debug.Log(PlayerManagement.Instance);
        PlayerManagement.Instance.AddPlayerStats(photonPlayer);
        PlayersInGame++;
        if (PlayersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players in game scene");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }

    public void NewHealth(PhotonPlayer photonPlayer, int health)
    {
        PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
    }

    [PunRPC]
    void RPC_NewHealth(int health)
    {
        if (CurrentPlayer == null)
            return;
        if (health <= 0)
            PhotonNetwork.Destroy(CurrentPlayer.gameObject);
        else
            CurrentPlayer.Health = health;
    }

    [PunRPC]
    void RPC_CreatePlayer()
    {
        int id = PhotonNetwork.player.ID;
        GameObject obj = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0 + id, 0.5f, 0), Quaternion.identity, 0);
        obj.name = playerName;
        CurrentPlayer = obj.GetComponent<PlayerMovement>();
        joined = true;
    }

    public bool joinedGame()
    {
        if (joined == true)
        {
            return true;

        }
        return false;
    }
    public void OnClickStartButton()
    {
        if (input.text != null)
        {
            playerName = input.text;
            print("Syötetty pelaajan nimi" + playerName);
        }
        else
            playerName = "Player#" + Random.Range(1000, 9999);
        PhotonNetwork.LoadLevel(1);
    }
}
