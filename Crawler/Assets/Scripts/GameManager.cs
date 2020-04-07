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
	UIManager um;

	float counter;
	bool triggerUIUpdate;



	private void Start() {
		um = FindObjectOfType<UIManager>();
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
		um.UpdateKeys(keys);
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

	
}
