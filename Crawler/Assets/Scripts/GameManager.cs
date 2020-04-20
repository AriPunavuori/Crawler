using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : Photon.MonoBehaviour {
	int keys = 0;
	UIManager um;

	float counter;
	bool triggerUIUpdate;
	bool gameOver;
	bool gameReady = false;
	GameObject[] players;


	private void Start() {
		um = FindObjectOfType<UIManager>();
		AudioFW.StopAllSounds();
		AudioFW.PlayLoop("GameLoop");
	}

	private void Update() {
		if (PhotonNetwork.isMasterClient) {
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			int alivePlayers = 0;

			foreach (GameObject i in players) {
				if (i.GetComponent<PlayerCharacter>().alive) {
					alivePlayers++;
				}
			}
			if (alivePlayers == PhotonNetwork.playerList.Length) {
				gameReady = true;
			}
			//Debug.Log("Players alive: " + alivePlayers);
			//Debug.Log(gameReady);
			if (alivePlayers == 0 && gameReady) {
				Debug.Log("Restarting game");
				PhotonNetwork.LoadLevel(3);
			}
		}
	}

	public bool UseKeyRPC() {
		Debug.Log(keys);

		if (keys > 0) {
			photonView.RPC("DecraseKeyAll", PhotonTargets.Others);
			keys -= 1;

			Debug.Log(keys);
			return true;
		}
		Debug.Log(keys);
		return false;
	}

	[PunRPC]
	public void DecreaseKeyAll() {
		//um.UpdateKeys(keys);
		keys -= 1;
	}
	public void FoundKey(string keyName, string playerName) {
		keys += 1;
		//Debug.Log(keys);
		Hashtable hash = new Hashtable();
		hash.Add(keyName, true);
		PhotonNetwork.room.SetCustomProperties(hash);
		um.UpdateKeys(keys - 1, playerName);
		//Debug.Log(PhotonNetwork.room.CustomProperties["Key"]);
	}
	public bool UseKey() {
		return UseKeyRPC();
	}


}
