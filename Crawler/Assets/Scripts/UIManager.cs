using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    //int keys = 0;
    Canvas canvas;
    public bool useUIBoxes = true;
    public GameObject[] UIBoxes;
    //public GameObject playerUIBox;
    //public GameObject emptyUIBox;
    public GameObject UIBox;
    public List<Text> names;
    public List<Text> healths;

    //GameObject[] players;
    //PhotonPlayer[] photonPlayers;
    //Room currentRoom;
    Text keysUI;

    float counter;
    //bool triggerUIUpdate;
    void Start() {
        Instance = this;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        keysUI = GameObject.Find("Keys").GetComponent<Text>();
        //currentRoom = PhotonNetwork.room;
        //photonPlayers = PhotonNetwork.playerList;
        //triggerUIUpdate = true;
    }

    //private void Update() {
    //	if(useUIBoxes) {

    //		if(PhotonNetwork.playerList != photonPlayers || PhotonNetwork.room != currentRoom) { // Jos pelaajien määrä tai huone vaihtuu
    //			print("Käytiin täällä");
    //			triggerUIUpdate = true;
    //		}
    //		photonPlayers = PhotonNetwork.playerList;
    //		currentRoom = PhotonNetwork.room;

    //		keysUI.text = "Keys: " + keys.ToString();
    //		if(triggerUIUpdate) {
    //			if(counter > 0.25f) {
    //				counter = 0;
    //				UpdatePlayerUIBoxes(); // Päivitä UIBoxit
    //				print("Päivitettiin UI-Boksit");
    //				triggerUIUpdate = false;
    //			} else {
    //				counter += Time.deltaTime;
    //			}
    //		}
    //	}
    //}
    [PunRPC]
    public void UpdateKeys(int keys) {
        keysUI.text = "" + keys;
    }
    [PunRPC]
    public void UpdateUIBoxContent(string n, string h, int c) {
        names[c].text = n;
        healths[c].text = h;
    }
    [PunRPC]
    public void UpdateUIBoxes() {
        //players = GameObject.FindGameObjectsWithTag("Player");
        foreach(Transform child in canvas.transform.GetChild(0).transform) {
            Destroy(child.gameObject);  // Tuhoa vanhat UIBoxit
        }
        UIBoxes = new GameObject[4]; // Taulukon koko on aina 4
        for(int i = 0; i < UIBoxes.Length; i++) {
            //if(i < players.Length) {
            GameObject newUIBox = Instantiate(UIBox.gameObject); // Tee uusi UIBox
            //newUIBox.GetComponent<PlayerUIBox>().myPlayer = players[i]; // Anna boxin playeriksi listan i:es pelaaja
            //players[i].GetComponent<PlayerCharacter>().myUIBox = newUIBox; // Anna i:en pelaajan boxiksi uusi boxi
            UIBoxes[i] = newUIBox;    // Aseta uusi boxi taulukkoon
            names[i] = newUIBox.transform.GetChild(0).GetComponent<Text>();
            healths[i] = newUIBox.transform.GetChild(1).GetComponent<Text>();
            UIBoxes[i].transform.SetParent(canvas.transform.GetChild(0)); // laita boxi canvasin "players" -elementin lapseksi
            //	GameObject newEmptyUIBox = Instantiate(UIBox.gameObject); // Tee uusi tyhjä UIBox
            //	UIBoxes[i] = newEmptyUIBox;    // Aseta tyhjä boxi taulukkoon
            //	UIBoxes[i].transform.SetParent(canvas.transform.GetChild(0)); // laita tyhjä boxi canvasin "players" -elementin lapseksi
        }
    }
}
