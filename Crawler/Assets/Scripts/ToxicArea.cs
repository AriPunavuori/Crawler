using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicArea : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonView photonView = collision.GetComponent<PhotonView>();
        if(photonView != null)
            PlayerManagement.Instance.ModifyHealth(photonView.owner, -10);
    }
}
