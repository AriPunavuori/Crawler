using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Door : MonoBehaviour {
    GameManager gm;
    BoxCollider2D bc;
    PhotonView photonView;
    public bool opened;

    float openingSpeed;
    void Start() {
        gm = FindObjectOfType<GameManager>();
        bc = GetComponent<BoxCollider2D>();
        photonView = GetComponent<PhotonView>();
    }
    void OnCollisionEnter2D(Collision2D collision) {
        string keyName = "Key" + gameObject.name.Trim('D', 'o', 'o', 'r');
        Debug.Log("This door requires " + keyName + " to open");
        if(collision.gameObject.CompareTag("Player") && (bool)PhotonNetwork.room.CustomProperties[keyName]) {
            //if (gm.UseKey())
            //{
            Debug.Log(gameObject.name + " opened");
            photonView.RPC("OpenDoorAll", PhotonTargets.All);
            //}
        }
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime) {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for(var t = 0f; t < 1; t += Time.deltaTime / inTime) {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
    }

    [PunRPC]
    public void OpenDoorAll()
    {
        bc.enabled = false;
        opened = true;
        StartCoroutine(RotateMe(Vector3.forward * 90, 5));
    }
}
