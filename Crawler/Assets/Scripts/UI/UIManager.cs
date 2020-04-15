using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	public static UIManager Instance;
	Canvas canvas;
	public bool useUIBoxes = true;
	public GameObject[] UIBoxes;
	public Text[] names;
	public Text[] healths;
	public GameObject UIBox;

	GameObject keysUI;
	GameObject potion;
	public TextMeshProUGUI infoText;
	float eraseTextTimer;

	PhotonView photonView;

	void Start() {
		Instance = this;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		keysUI = GameObject.Find("Keys");
		potion = GameObject.Find("PotionUI");
		potion.SetActive(false);
		infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();
		infoText.text = "";
		photonView = GetComponent<PhotonView>();
		CreateUIBoxes();
	}

	[PunRPC]
	public void UpdateKeys(int keyNmbr) {
		GameObject keyUI = keysUI.transform.GetChild(keyNmbr).gameObject; //KeyUI canvasissa
		string key = "Key" + keyUI.name.Trim('K', 'e', 'y', 'U', 'I'); //poimittava Key -gameobjekti
		if (PhotonNetwork.room.CustomProperties[key] != null) {
			print("Picked up " + key);
			keyUI.GetComponent<Image>().color = Color.white;
		}
	}

	public void UpdatePotion() {
		if (potion.activeSelf) {
			potion.SetActive(false);
		} else {
			potion.SetActive(true);
		}
	}

	public void SetInfoText(string text, float textTime) {
		eraseTextTimer = Time.time + textTime;
		infoText.text = text;
	}

	public void UpdateUIContent(string name, int health, int selected) {
		photonView.RPC("RPC_UpdateUIBoxContent", PhotonTargets.All, name, health, selected);
	}
	public void CreateUIBoxes() {
		photonView.RPC("RPC_CreateUIBoxes", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void RPC_UpdateUIBoxContent(string name, int health, int selected) {
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
