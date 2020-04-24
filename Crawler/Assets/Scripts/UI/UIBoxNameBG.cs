using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIBoxNameBG : MonoBehaviour {
	public TextMeshProUGUI playerName;
	public TextMeshProUGUI textField;
	void Start() {
		playerName = transform.parent.gameObject.GetComponent<TextMeshProUGUI>();
		textField = transform.GetComponent<TextMeshProUGUI>();
	}

	private void FixedUpdate() {
		if (playerName.text != textField.text) {
			textField.text = playerName.text;
		}
	}
}
