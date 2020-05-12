using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : Photon.MonoBehaviour {
	public static GameManager Instance;
	int keys = 0;
	UIManager um;
	public PlayerCharacter pc;
	float counter;
	bool triggerUIUpdate;
	bool gameOver;
	bool gameReady = false;
	bool bossSpawned = false;
	public bool bossFightStarted = false;
	bool doorClosed = false;
	public bool bossDefeated = false;
	public bool gameWon = false;
	GameObject[] players;
	GameObject BossDoor;
	GameObject Portal;
	bool portalSpawned = false;


	private void Awake() {
		Destroy(GameObject.Find("DynamicBG"));
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

	}

	void resetKeys()
	{

		// Imagine doing all this.. 

		//if(PhotonNetwork.room.CustomProperties["keyCount"] != null)
		//{
		//	//Debug.Log("not null");
		//	int count = (int)PhotonNetwork.room.CustomProperties["keyCount"];
		//	//Debug.Log(count);
		//	for (int i = 1; i <= count; i++)
		//	{
		//		string name = (string)PhotonNetwork.room.CustomProperties["keyTable" + i.ToString()];
		//		//Debug.Log(name);
		//		Hashtable h = new Hashtable();
		//		h.Add(name, false);
		//		PhotonNetwork.room.SetCustomProperties(h);
		//	}
		//} 
		//(still works though..)

		// When it was this simple ;(

		Hashtable redKey = new Hashtable();
		redKey.Add("Red Key", false);
		PhotonNetwork.room.SetCustomProperties(redKey);

		Hashtable blueKey = new Hashtable();
		blueKey.Add("Blue Key", false);
		PhotonNetwork.room.SetCustomProperties(blueKey);

		Hashtable greenKey = new Hashtable();
		greenKey.Add("Green Key", false);
		PhotonNetwork.room.SetCustomProperties(greenKey);

	}
	private void Start() {
		resetKeys();
		Portal = Resources.Load("Portal") as GameObject;
		BossDoor = Resources.Load("BossArenaDoor") as GameObject;
		Instance = this;
		um = FindObjectOfType<UIManager>();
		AudioFW.StopAllSounds();
		//AudioFW.PlayLoop("GameLoop");
	}

	[PunRPC]
	public void RPC_instBossDoor()
	{
		BossDoor = Instantiate(BossDoor, new Vector3(71.8f, 136.5f, 0f), Quaternion.Euler(0, 0, 90));
	}

	
	public void instBossDoor()
	{
		photonView.RPC("RPC_instBossDoor", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	public void RPC_instPortal()
	{
		BossDoor = Instantiate(Portal, new Vector3(68.5f, 147.5f, 0f), Quaternion.identity);
		portalSpawned = true;
	}


	public void instPortal()
	{
		photonView.RPC("RPC_instPortal", PhotonTargets.AllViaServer);
	}


	private void Update() {
		if(bossFightStarted && !doorClosed)
		{
			BossDoor.GetComponent<BossDoorScript>().SlideBossDoor();
			doorClosed = true;
		}
		if (PhotonNetwork.isMasterClient) {
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			int alivePlayers = 0;

			foreach (GameObject i in players) {
				if (i.GetComponent<PlayerCharacter>().alive) {
					alivePlayers++;
					if(gameReady && i.transform.position.y > 124 && !bossSpawned)
					{
						PhotonNetwork.Instantiate("BossEnemy", new Vector3(68.5f, 147.5f, 0f), Quaternion.identity, 0);
						instBossDoor();
						bossSpawned = true;
					}
				}
			}
			if (alivePlayers == PhotonNetwork.playerList.Length) {
				gameReady = true;
			}
			if(bossDefeated == true && !portalSpawned)
			{
				portalSpawned = true;
				Invoke("instPortal", 1.5f);
				//instPortal();
			}
			//Debug.Log("Players alive: " + alivePlayers);
			//Debug.Log(gameReady);
			if (alivePlayers == 0 && gameReady) {
				//Playercharacter RPC_GameWon
				gameReady = false;
				print("PlayerCharacter.GameLost");
				pc.GameLost();
			}
			if(gameWon) {
				//Playercharacter RPC_GameWon
				gameWon = false;
				print("PlayerCharacter.GameWon");
				pc.GameWon();
			}
		}
	}

	public void FoundKey(string keyName, string playerName) {
		AudioFW.Play("KeyPickup");
		keys += 1;
		//Debug.Log(keys);
		Hashtable hash = new Hashtable();
		hash.Add(keyName, true);
		//Hashtable keyTable = new Hashtable();
		//Debug.Log("keyTable" + keys.ToString());
		//Debug.Log("keyname:" + keyName);
		//Debug.Log("count: " + keys);
		//keyTable.Add("keyTable" + keys.ToString(), keyName);
		//Hashtable keyCountTable = new Hashtable();
		//keyCountTable.Add("keyCount", keys);
		PhotonNetwork.room.SetCustomProperties(hash);
		// For resetting keys(redundant now)
		//PhotonNetwork.room.SetCustomProperties(keyTable);
		//PhotonNetwork.room.SetCustomProperties(keyCountTable);
		um.UpdateKeys(keys - 1, playerName);
		//Debug.Log(PhotonNetwork.room.CustomProperties["Key"]);
	}



}
