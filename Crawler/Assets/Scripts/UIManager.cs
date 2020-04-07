using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
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

	float counter;
	bool triggerUIUpdate;
	void Start() {
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		keysUI = GameObject.Find("Keys").GetComponent<Text>();
		currentRoom = PhotonNetwork.room;
		photonPlayers = PhotonNetwork.playerList;
		triggerUIUpdate = true;
	}

	private void Update() {
		if(useUIBoxes) {

			if(PhotonNetwork.playerList != photonPlayers || PhotonNetwork.room != currentRoom) { // Jos pelaajien määrä tai huone vaihtuu
				print("Käytiin täällä");
				triggerUIUpdate = true;
			}
			photonPlayers = PhotonNetwork.playerList;
			currentRoom = PhotonNetwork.room;
		}
		keysUI.text = "Keys: " + keys.ToString();
		if(triggerUIUpdate) {
			if(counter > 0.25f) {
				counter = 0;
				UpdatePlayerUIBoxes(); // Päivitä UIBoxit
				print("Päivitettiin UI-Boksit");
				triggerUIUpdate = false;
			} else {
				counter += Time.deltaTime;
			}
		}
	}
	public void UpdatePlayerUIBoxes() {
		players = GameObject.FindGameObjectsWithTag("Player");
		foreach(Transform child in canvas.transform.GetChild(0).transform) {
			Destroy(child.gameObject);  // Tuhoa vanhat UIBoxit
		}
		playerUIBoxes = new GameObject[4]; // Taulukon koko on aina 4
		for(int i = 0; i < 4; i++) {
			if(i < players.Length) {
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
