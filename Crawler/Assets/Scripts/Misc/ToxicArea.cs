using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicArea : MonoBehaviour {
    Dictionary<PhotonView, float> players = new Dictionary<PhotonView, float>();
    public float damageInterval = 1f;
    public int damage = 2;
    private void OnTriggerStay2D(Collider2D collision) {
        if(!PhotonNetwork.isMasterClient)
            return;
        PhotonView photonView = collision.GetComponent<PhotonView>();
        if(photonView != null) {
            if(!players.ContainsKey(photonView)|| players[photonView] < Time.time) {
                IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
                if(iDamageable != null) {
                    iDamageable.TakeDamage(damage, Vector3.zero);
                }
                players[photonView] = Time.time + damageInterval;
            }
        }
    }
}
