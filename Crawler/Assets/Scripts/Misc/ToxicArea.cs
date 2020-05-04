using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicArea : MonoBehaviour {
    Dictionary<IDamageable<int>, float> players = new Dictionary<IDamageable<int>, float>();
    public float damageInterval = 1f;
    public int damage = 2;

    private void FixedUpdate() {
        foreach(var player in players) {
            if(player.Value < Time.time) {
                player.Key.TakeDamage(damage, Vector3.zero);
                players[player.Key] = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //var photonView = collision.GetComponent<PhotonView>();
        //if(photonView != null)
        //    photonView.RPC("Choking", photonView.owner, true);
        //if(PhotonNetwork.isMasterClient) {
        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(iDamageable != null) {
            if(!players.ContainsKey(iDamageable)) {
                players[iDamageable] = 0f;
            }
        }
        //}
    }

    private void OnTriggerExit2D(Collider2D collision) {
        //var photonView = collision.GetComponent<PhotonView>();
        //if(photonView != null)
        //    photonView.RPC("Choking", photonView.owner, false);
        //if(PhotonNetwork.isMasterClient) {
        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(players.ContainsKey(iDamageable))
            players.Remove(iDamageable);
        //}
    }
}
