using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToxicArea : MonoBehaviour {
    Dictionary<IDamageable<int>, float> players = new Dictionary<IDamageable<int>, float>();
    float damageInterval = .5f;
    int damage = 2;

    private void FixedUpdate() {
        IDamageable<int>[] iDamageables = players.Keys.ToArray<IDamageable<int>>();
        for(int i = 0; i < iDamageables.Length; i++) {
            if(players[iDamageables[i]]< Time.time) {
                iDamageables[i].TakeDamage(damage, Vector3.zero);
                players[iDamageables[i]] = Time.time + damageInterval;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(iDamageable != null) {
            if(!players.ContainsKey(iDamageable)) {
                players[iDamageable] = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(iDamageable != null) {
            if(players.ContainsKey(iDamageable)) {
                players.Remove(iDamageable);
            }
        }
    }
}
