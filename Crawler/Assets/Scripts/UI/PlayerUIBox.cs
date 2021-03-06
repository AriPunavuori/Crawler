﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIBox : MonoBehaviour {
	public GameObject myPlayer;
	Text hpText;
	Text nameText;
	PhotonView photonView;

	PlayerCharacter pc;
	void Start() {
		pc = myPlayer.GetComponent<PlayerCharacter>();
		nameText = transform.GetChild(0).gameObject.GetComponent<Text>();
		hpText = transform.GetChild(1).gameObject.GetComponent<Text>();
		photonView = myPlayer.GetComponent<PhotonView>();
		nameText.text = PlayerManager.Instance.GetName(photonView.owner);
		if(pc.characterType == EntityType.Hero0) {
			transform.GetComponent<Image>().color = Color.blue;
		} else if(pc.characterType == EntityType.Hero1) {
			transform.GetComponent<Image>().color = Color.green;
		} else if(pc.characterType == EntityType.Hero2) {
			transform.GetComponent<Image>().color = Color.yellow;
		} else if(pc.characterType == EntityType.Hero3) {
			transform.GetComponent<Image>().color = Color.red;
		}
	}

	public void UpdateUIBox(int health) {
		if(myPlayer != null) {
			hpText.text = "HP = " + health;
		}
	}
}
