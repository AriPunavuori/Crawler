using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Door : MonoBehaviour {
	GameManager gm;
	BoxCollider2D bc;
	UIManager uim;
	PhotonView photonView;
	public bool opened;
	public GameObject doorOpenParticles;

	float openingSpeed;
	void Start() {
		gm = FindObjectOfType<GameManager>();
		bc = GetComponent<BoxCollider2D>();
		uim = GameObject.Find("UIManager").GetComponent<UIManager>();
		photonView = GetComponent<PhotonView>();
	}
	void OnCollisionEnter2D(Collision2D collision) {
		string keyName = gameObject.name.Trim('D', 'o', 'o', 'r') + "Key";

		if (collision.gameObject.CompareTag("Player") && !opened) {
			Debug.Log("This door requires " + keyName + " to open");
			AudioFW.Play("DoorLocked");
			uim.SetInfoText("This door requires " + keyName + " to open", 2);
		}

		if (PhotonNetwork.room.CustomProperties[keyName] != null) {

			if (collision.gameObject.CompareTag("Player") && (bool)PhotonNetwork.room.CustomProperties[keyName] && !opened) {
				Debug.Log(gameObject.name + " opened");
				AudioFW.Play("DoorOpen");
				photonView.RPC("OpenDoorAll", PhotonTargets.All, collision.gameObject.name);
			}

		}
	}
	IEnumerator OpenMe(Vector3 byAngles, float inTime) {
		var fromPosition = transform.position;
		var toPosition = transform.position + transform.right * 2.75f;
		for (var t = 0f; t < 1; t += Time.deltaTime / inTime) {
			transform.position = Vector3.Lerp(fromPosition, toPosition, t);
			yield return null;
		}
	}



	[PunRPC]
	public void OpenDoorAll(string openerName) {
		//bc.enabled = false;
		opened = true;
		uim.SetInfoText(openerName + " opened " + gameObject.name, 2);
		GameObject particles = Instantiate(doorOpenParticles, transform.GetChild(1).transform.position, transform.rotation, transform.GetChild(1));
		Destroy(particles, 5);
		StartCoroutine(OpenMe(Vector3.forward * 90, 2));
	}
}
