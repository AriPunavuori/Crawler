using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
	public static UIManager Instance;
	GameObject players;
	public bool useUIBoxes = true;
	public GameObject[] otherPlayerBoxes;
	public TextMeshProUGUI[] names;
	public TextMeshProUGUI[] healths;
	public Image[] healthBars;
	public Sprite[] boxBackgrounds = new Sprite[4];
	public Sprite[] origBoxBGs = new Sprite[4];
	public GameObject otherPlayerBox;
	public GameObject SpecialCoolDownUI;
	public Image SpecialBar;
	GameObject playerBoxPlace;
	GameObject healthAmountUI;
	GameObject playerName;

	GameObject keysUI;
	GameObject potion;
	public TextMeshProUGUI infoText;
	float eraseTextTime;

	public Image powerup;
	public Image powerupBG; // powerupBG is active if player has stacked powerups
	public TextMeshProUGUI powerupLevelText;

	public Image speedBoost;
	public Image speedBoostBG;
	public TextMeshProUGUI speedBoostLevelText;

	float speedBoostTime;
	float speedBoostTimer;
	bool speedBoostTimerStarted;
	int speedBoostLevel;

	float powerUpTime;
	float powerUpTimer;
	bool powerUpTimerStarted;
	int powerupLevel;

	float cooldownFinishTime;
	float cooldownTime;
	bool specialBarReduced;

	PhotonView photonView;

	void Start() {
		Instance = this;
		players = GameObject.Find("Players");
		keysUI = GameObject.Find("Keys");
		potion = GameObject.Find("PotionUI");
		potion.SetActive(false);
		playerBoxPlace = GameObject.Find("PlayerBoxPlace");
		healthAmountUI = GameObject.Find("HealthAmountUI");
		playerName = GameObject.Find("PlayerName");
		speedBoostTimerStarted = false;
		speedBoostLevel = 0;
		powerUpTimerStarted = false;
		powerupLevel = 0;
		infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();
		infoText.text = "";
		SpecialCoolDownUI = GameObject.Find("SpecialCooldownUI");
		SpecialBar = SpecialCoolDownUI.transform.GetChild(1).GetComponent<Image>();
		specialBarReduced = false;
		photonView = GetComponent<PhotonView>();
		CreateUIBoxes();
		print(otherPlayerBoxes[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
		Invoke("UpdateBoxColors", 1f);
	}

	private void Update() {
		if (Time.time >= eraseTextTime) {
			infoText.text = "";
			eraseTextTime = 0;
		}

		if (Time.time < cooldownFinishTime && specialBarReduced) {
			SpecialBar.fillAmount = -((cooldownFinishTime - Time.time) - cooldownTime) / cooldownTime;
		}

		#region speedboost UI handling
		if (speedBoostLevel > 0) {
			// Enable powerup level text if any powerup is active
			speedBoostLevelText.text = speedBoostLevel.ToString();
			// bg enabled if player has stacked powerups
			if (speedBoostLevel > 1) {
				speedBoostBG.enabled = true;
			} else {
				speedBoostBG.enabled = false;
			}
		} else {
			speedBoostBG.enabled = false;
			speedBoostLevelText.text = "";
		}

		if (speedBoostTimerStarted) {

			if (speedBoostTimer - Time.deltaTime < 0) {
				speedBoostTimer = 0;
			} else {
				speedBoostTimer -= Time.deltaTime;
			}

			speedBoost.fillAmount = speedBoostTimer / speedBoostTime;

			if (speedBoostTime <= 0) {
				speedBoostTimerStarted = false;
			}
		} else {
			speedBoost.fillAmount = 0;
		}
		#endregion

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

	public void setSpecialCooldownTimer(float finishTime, float specialCooldownTime) {
		StartCoroutine(reduceSpecialBar(1, 0, 0.25f));
		cooldownFinishTime = finishTime;
		cooldownTime = specialCooldownTime;
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

	public void setSpeedBoostUITimer(float time, int speedlevel) {
		if (time > 0) {
			speedBoostTimerStarted = true;
			speedBoostTime = time;
			speedBoostTimer = speedBoostTime;
			speedBoostLevel = speedlevel;
		} else {
			speedBoostTimerStarted = false;
			speedBoostLevel = 0;
		}
	}

	[PunRPC]
	public void UpdateKeys(int keyNmbr, string playerName) {
		GameObject keyUI = keysUI.transform.GetChild(keyNmbr).gameObject; //KeyUI canvasissa
		string key = keyUI.name.Trim('K', 'e', 'y', 'U', 'I') + "Key"; //poimittava Key -gameobjekti
		if (PhotonNetwork.room.CustomProperties[key] != null) {
			SetInfoText(names[GetRealName(playerName)].text + " Picked up " + key, 2);
			keyUI.GetComponent<Image>().color = Color.white;
		}
	}

	public void UpdateDeath(string playerName) {
		SetInfoText(names[GetRealName(playerName)].text + " Has Died", 2);
	}

	public void UpdateRespawn(string playerName) {
		SetInfoText(names[GetRealName(playerName)].text + " Has Respawned", 2);
	}

	int GetRealName(string playerName) {
		if (playerName == "NetworkPlayer0(Clone)") {
			playerName = names[0].text;
		}
		if (playerName == "NetworkPlayer1(Clone)") {
			playerName = names[1].text;
		}
		if (playerName == "NetworkPlayer2(Clone)") {
			playerName = names[2].text;
		}
		if (playerName == "NetworkPlayer3(Clone)") {
			playerName = names[3].text;
		}
		for (int i = 0; i < names.Length; i++) {
			if (names[i].text == playerName) {
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
			SetInfoText("Picked up a health potion", 2);
		}
	}

	public void SetInfoText(string text, float textTime) {
		eraseTextTime = Time.time + textTime;
		infoText.text = text;
	}

	public void UpdateUIContent(string name, int health, int selected, int baseHealth) {
		photonView.RPC("RPC_UpdateUIBoxContent", PhotonTargets.All, name, health, selected, baseHealth);
	}
	public void CreateUIBoxes() {
		photonView.RPC("RPC_CreateUIBoxes", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void RPC_UpdateUIBoxContent(string name, int health, int selected, int baseHealth) {
		names[selected].text = name;
		StartCoroutine(updateHealthBar(health, 0.1f, selected, baseHealth));
		healths[selected].text = "" + health;
		for (int i = 0; i < names.Length; i++) {
			if (names[i].text == PhotonNetwork.player.NickName) {
				playerName.GetComponent<TextMeshProUGUI>().text = names[i].text;
				healthAmountUI.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = healths[i].text;

				GameObject playerBox = otherPlayerBoxes[i];
				playerBox.transform.SetParent(playerBoxPlace.transform);
				playerBox.transform.localPosition = Vector3.zero;
				playerBox.transform.localScale = new Vector3(1, 1, 1);
				playerBox.GetComponent<Image>().sprite = origBoxBGs[i];
				playerBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
				playerBox.transform.GetChild(1).GetComponent<Image>().color = new Color(0, 0, 0, 0);
				playerBox.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
				playerBox.transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 0);
			}
		}
	}

	IEnumerator updateHealthBar(int newHealth, float updateTime, int selected, int baseHealth) {
		float elapsedTime = 0;
		// only gets local player base stats
		//int baseHealth = Character.Instance.CheckCharacterHealt(Character.Instance.characterType);
		//int baseHealth = baseHealths[selected];
		int currentHealth = int.Parse(healths[selected].text);


		//bool filled = false;

		while (elapsedTime < updateTime) {
			healthBars[selected].fillAmount = Mathf.Lerp(((float)currentHealth / baseHealth), ((float)newHealth / baseHealth), (elapsedTime / updateTime));

			elapsedTime += Time.deltaTime;

			// Stop when filled with 2 decimal points of accuracy
			//if (System.Math.Round(healthBars[selected].fillAmount, 2) == System.Math.Round((float)newHealth / baseHealth, 2))
			//{
			//	filled = true;
			//	//Debug.Log("fillAmount: " + healthBars[selected].fillAmount);
			//	Debug.Log("Filled");
			//}
			yield return null;
		}

		// Lazy fallback fix if bar doesnt get filled in time
		if (System.Math.Round(healthBars[selected].fillAmount, 2) != System.Math.Round((float)newHealth / baseHealth, 2)) {
			healthBars[selected].fillAmount = (float)newHealth / baseHealth;
		}

	}

	IEnumerator reduceSpecialBar(float startAmount, float endAmount, float updateTime) {
		float elapsedTime = 0;
		specialBarReduced = false;

		while (elapsedTime < updateTime) {
			SpecialBar.fillAmount = Mathf.Lerp(startAmount, endAmount, (elapsedTime / updateTime));

			elapsedTime += Time.deltaTime;

			yield return null;
		}
		specialBarReduced = true;
	}

	[PunRPC]
	void RPC_UpdateBoxColors() {
		for (int i = 0; i < otherPlayerBoxes.Length; i++) {
			if (names[i].text != "No Player") {
				otherPlayerBoxes[i].GetComponent<Image>().color = Color.white;
			}
		}
	}

	public void UpdateBoxColors() {
		photonView.RPC("RPC_UpdateBoxColors", PhotonTargets.All);
	}

	[PunRPC]
	public void RPC_CreateUIBoxes() {
		if (otherPlayerBoxes != null)
			foreach (var box in otherPlayerBoxes) {
				Destroy(box);
			}
		otherPlayerBoxes = new GameObject[4]; // Taulukon koko on aina 4
		names = new TextMeshProUGUI[4];
		healths = new TextMeshProUGUI[4];
		healthBars = new Image[4];
		for (int i = 0; i < otherPlayerBoxes.Length; i++) {
			GameObject NewOtherPBox = Instantiate(otherPlayerBox.gameObject); // Tee uusi UIBox
			NewOtherPBox.GetComponent<Image>().sprite = boxBackgrounds[i];
			otherPlayerBoxes[i] = NewOtherPBox;    // Aseta uusi boxi taulukkoon
			names[i] = NewOtherPBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			healths[i] = NewOtherPBox.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
			healthBars[i] = NewOtherPBox.transform.GetChild(1).GetChild(0).GetComponent<Image>();
			otherPlayerBoxes[i].transform.SetParent(players.transform, false); // laita boxi canvasin "players" -elementin lapseksi
		}
	}
}
