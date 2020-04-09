using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager Instance;
	Canvas canvas;
	public bool useUIBoxes = true;
	public GameObject[] UIBoxes;
	public Text[] names;
	public Text[] healths;
	public GameObject UIBox;

	GameObject keyUI;
	GameObject keyUI1;
	GameObject keyUI2;

	PhotonView photonView;
	Text keysUI;

	void Start() {
		Instance = this;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		keysUI = GameObject.Find("Keys").GetComponent<Text>();
		photonView = GetComponent<PhotonView>();
		CreateUIBoxes();

		keyUI = GameObject.Find("KeyUI");
		keyUI1 = GameObject.Find("KeyUI (1)");
		keyUI2 = GameObject.Find("KeyUI (2)");
	}

	[PunRPC]
	public void UpdateKeys() {
		if (PhotonNetwork.room.CustomProperties["Key"] != null) {
			print("Key hallussa");
			keyUI.GetComponent<Image>().color = Color.white;
		}
		if (PhotonNetwork.room.CustomProperties["Key (1)"] != null) {
			print("Key (1) hallussa");
			keyUI1.GetComponent<Image>().color = Color.white;
		}
		if (PhotonNetwork.room.CustomProperties["Key (2)"] != null) {
			print("Key (2) hallussa");
			keyUI2.GetComponent<Image>().color = Color.white;
		}
	}

	public void UpdateUIContent(string name, int health, int selected) {
		photonView.RPC("RPC_UpdateUIBoxContent", PhotonTargets.All, name, health, selected);
	}
	public void CreateUIBoxes() {
		photonView.RPC("RPC_CreateUIBoxes", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void RPC_UpdateUIBoxContent(string name, int health, int selected) {
		print("Updating data in UI index: " + selected);
		names[selected].text = name;
		healths[selected].text = "" + health;
	}

	[PunRPC]
	public void RPC_CreateUIBoxes() {
		if (UIBoxes != null)
			foreach (var box in UIBoxes) {
				Destroy(box);
			}
		UIBoxes = new GameObject[4]; // Taulukon koko on aina 4
		names = new Text[4];
		healths = new Text[4];
		for (int i = 0; i < UIBoxes.Length; i++) {
			GameObject newUIBox = Instantiate(UIBox.gameObject); // Tee uusi UIBox
			UIBoxes[i] = newUIBox;    // Aseta uusi boxi taulukkoon
			names[i] = newUIBox.transform.GetChild(0).GetComponent<Text>();
			healths[i] = newUIBox.transform.GetChild(1).GetComponent<Text>();
			UIBoxes[i].transform.SetParent(canvas.transform.GetChild(0)); // laita boxi canvasin "players" -elementin lapseksi
		}
	}
}
