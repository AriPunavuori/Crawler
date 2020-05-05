using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHand : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.name);
        IDamageable<int> iDamageable = collider.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if (iDamageable != null)
        {
            // Not accurate, but something
            Vector3 recoilVector = new Vector3(collider.gameObject.transform.position.x - transform.position.x, collider.gameObject.transform.position.y - transform.position.y, 0f).normalized;
            iDamageable.TakeDamage(damage, recoilVector);
            return;
        }
    }
}

