using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonCollection : MonoBehaviour
{
    // Säilyy läpi koko prosessin 

    //void Awake() {
    //    DontDestroyOnLoad(this);
    //}


    // PlayerNetwork ----------------------------------------------------------------------------------------

    //void Awake() {
    //    Instance = this;
    //    PhotonView = GetComponent<PhotonView>();
    //    PlayerName = "Player#" + Random.Range(1000, 9999);
    //    PhotonNetwork.sendRate = 60;
    //    PhotonNetwork.sendRateOnSerialize = 30;
    //    SceneManager.sceneLoaded += OnSceneFinishedLoading;
    //}
    //void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
    //    if(scene.name == "Game") {
    //        if(PhotonNetwork.isMasterClient)
    //            MasterLoadedGame();
    //        else
    //            NonMasterLoadedGame();
    //    }
    //}

    //void MasterLoadedGame() {
    //    PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    //    PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    //}
    //void NonMasterLoadedGame() {
    //    PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
    //}
    //[PunRPC]
    //void RPC_LoadGameOthers() {
    //    PhotonNetwork.LoadLevel(2);
    //}

    //[PunRPC]
    //void RPC_LoadedGameScene(PhotonPlayer photonPlayer) {
    //    PlayerManagement.Instance.AddPlayerStats(photonPlayer);
    //    PlayersInGame++;
    //    if(PlayersInGame == PhotonNetwork.playerList.Length) {
    //        print("All players in game scene");
    //        PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
    //    }
    //}

    //public void NewHealth(PhotonPlayer photonPlayer, int health) {
    //    PhotonView.RPC("RPC_NewHealth", photonPlayer, health);
    //}

    //[PunRPC]
    //void RPC_NewHealth(int health) {
    //    if(CurrentPlayer == null)
    //        return;
    //    if(health <= 0)
    //        PhotonNetwork.Destroy(CurrentPlayer.gameObject);
    //    else
    //        CurrentPlayer.Health = health;
    //}

    //[PunRPC]
    //void RPC_CreatePlayer() {
    //    float randomValue = Random.Range(0f, 5f);
    //    GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "NewPlayer"), Vector3.up * randomValue, Quaternion.identity, 0);
    //    CurrentPlayer = obj.GetComponent<PlayerMovement>();
    //}

    // PlayerNetwork ----------------------------------------------------------------------------------------


}
