﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : Photon.MonoBehaviour {
	public static GameManager Instance;
	int keys = 0;
	UIManager um;

	float counter;
	bool triggerUIUpdate;
	bool gameOver;
	bool gameReady = false;
	bool bossSpawned = false;
	public bool gameWon;
	GameObject[] players;


	private void Start() {
		Instance = this;
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
					if(gameReady && i.transform.position.y > 137 && !bossSpawned)
					{
						PhotonNetwork.Instantiate("BossEnemy", new Vector3(68.5f, 147.5f, 0f), Quaternion.identity, 0);
						bossSpawned = true;
					}
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
			if(gameWon)
				PhotonNetwork.LoadLevel(4);
		}
	}

	public void FoundKey(string keyName, string playerName) {
		AudioFW.Play("KeyPickup");
		keys += 1;
		//Debug.Log(keys);
		Hashtable hash = new Hashtable();
		hash.Add(keyName, true);
		PhotonNetwork.room.SetCustomProperties(hash);
		um.UpdateKeys(keys - 1, playerName);
		//Debug.Log(PhotonNetwork.room.CustomProperties["Key"]);
	}



}
