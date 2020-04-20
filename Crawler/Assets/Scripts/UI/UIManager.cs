﻿using System.Collections;
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
	float eraseTextTime;

	public Image powerup;
	// powerupBG is active if player has stacked powerups
	public Image powerupBG;
	public TextMeshProUGUI powerupLevelText;
	float powerUpTime;
	float powerUpTimer;
	bool powerUpTimerStarted;
	int powerupLevel;

	PhotonView photonView;

	void Start() {
		Instance = this;
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		keysUI = GameObject.Find("Keys");
		potion = GameObject.Find("PotionUI");
		potion.SetActive(false);
		//powerup = GameObject.Find("Powerup").GetComponent<Image>();
		powerUpTimerStarted = false;
		powerupLevel = 0;
		infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();
		infoText.text = "";
		photonView = GetComponent<PhotonView>();
		CreateUIBoxes();
	}

	private void Update() {
		if (Time.time >= eraseTextTime) {
			infoText.text = "";
			eraseTextTime = 0;
		}

		#region powerup UI handling
		if (powerupLevel > 0) {
			// Enable powerup level text if any powerup is active
			powerupLevelText.text = powerupLevel.ToString();
			// bg enabled if player has stacked powerups
			if (powerupLevel > 1) {
				powerupBG.enabled = true;
			} else {
				powerupBG.enabled = false;
			}
		} else {
			powerupBG.enabled = false;
			powerupLevelText.text = "";
		}



		if (powerUpTimerStarted) {

			if (powerUpTimer - Time.deltaTime < 0) {
				powerUpTimer = 0;
			} else {
				powerUpTimer -= Time.deltaTime;
			}

			powerup.fillAmount = powerUpTimer / powerUpTime;

			if (powerUpTime <= 0) {
				powerUpTimerStarted = false;
			}
		} else {
			powerup.fillAmount = 0;
		}
		#endregion
	}

	public void setPowerupUITimer(float time, int weaponlevel) {
		if (time > 0) {
			powerUpTimerStarted = true;
			powerUpTime = time;
			powerUpTimer = powerUpTime;
			powerupLevel = weaponlevel;
		} else {
			powerUpTimerStarted = false;
			powerupLevel = 0;
		}
	}

	[PunRPC]
	public void UpdateKeys(int keyNmbr, string playerName) {
		GameObject keyUI = keysUI.transform.GetChild(keyNmbr).gameObject; //KeyUI canvasissa
		string key = keyUI.name.Trim('U', 'I'); //poimittava Key -gameobjekti
		if (PhotonNetwork.room.CustomProperties[key] != null) {
			SetInfoText(names[GetKeyPickerName(playerName)].text + " Picked up " + key, 10);
			keyUI.GetComponent<Image>().color = Color.white;
		}
	}

	int GetKeyPickerName(string picker) {
		for (int i = 0; i < names.Length; i++) {
			if (names[i].text == picker) {
				return i;
			}
		}
		return 0;
	}

	public void UpdatePotion() {
		if (potion.activeSelf) {
			potion.SetActive(false);
		} else {
			potion.SetActive(true);
			SetInfoText("Picked up a health potion!", 2);
		}
	}

	public void SetInfoText(string text, float textTime) {
		eraseTextTime = Time.time + textTime;
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
