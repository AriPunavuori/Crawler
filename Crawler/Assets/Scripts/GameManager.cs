using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class GameManager : Photon.MonoBehaviour {
	int keys = 0;
	Canvas canvas;
	public bool useUIBoxes = true;
	public GameObject[] playerUIBoxes;
	public GameObject playerUIBox;
	public GameObject emptyUIBox;
	GameObject[] players;
	PhotonPlayer[] photonPlayers;
	Room currentRoom;
	Text keysUI;

	void Start() {
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		keysUI = GameObject.Find("Keys").GetComponent<Text>();
		currentRoom = PhotonNetwork.room;
		photonPlayers = PhotonNetwork.playerList;
	}

	private void Update() {
		if (useUIBoxes) {
			Debug.Log(photonPlayers.Length);

			if (PhotonNetwork.playerList != photonPlayers || PhotonNetwork.room != currentRoom) { // Jos pelaajien määrä tai huone vaihtuu
				UpdatePlayerUIBoxes(); // Päivitä UIBoxit
			}
			photonPlayers = PhotonNetwork.playerList;
			currentRoom = PhotonNetwork.room;
		}
		keysUI.text = "Keys: " + keys.ToString();
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
	public void DecraseKeyAll() {
		keys -= 1;
	}
	public void FoundKey(string name) {
		keys += 1;
		//Debug.Log(keys);
		Hashtable hash = new Hashtable();
		hash.Add(name, true);
		PhotonNetwork.room.SetCustomProperties(hash);
		//Debug.Log(PhotonNetwork.room.CustomProperties["Key"]);
	}
	public bool UseKey() {
		return UseKeyRPC();
	}

	public void UpdatePlayerUIBoxes() {
		players = GameObject.FindGameObjectsWithTag("Player");
		foreach (Transform child in canvas.transform.GetChild(0).transform) {
			Destroy(child.gameObject);  // Tuhoa vanhat UIBoxit
		}
		playerUIBoxes = new GameObject[4]; // Taulukon koko on aina 4
		for (int i = 0; i < 4; i++) {
			if (i < players.Length) {
				GameObject newUIBox = Instantiate(playerUIBox.gameObject); // Tee uusi UIBox
				newUIBox.GetComponent<PlayerUIBox>().myPlayer = players[i]; // Anna boxin playeriksi listan i:es pelaaja
				players[i].GetComponent<PlayerCharacter>().myUIBox = newUIBox; // Anna i:en pelaajan boxiksi uusi boxi
				playerUIBoxes[i] = newUIBox;    // Aseta uusi boxi taulukkoon
				playerUIBoxes[i].transform.SetParent(canvas.transform.GetChild(0)); // laita boxi canvasin "players" -elementin lapseksi
			} else {
				GameObject newEmptyUIBox = Instantiate(emptyUIBox.gameObject); // Tee uusi tyhjä UIBox
				playerUIBoxes[i] = newEmptyUIBox;    // Aseta tyhjä boxi taulukkoon
				playerUIBoxes[i].transform.SetParent(canvas.transform.GetChild(0)); // laita tyhjä boxi canvasin "players" -elementin lapseksi
			}
		}
	}
}
