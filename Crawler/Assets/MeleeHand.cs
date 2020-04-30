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
            iDamageable.TakeDamage(damage, Vector3.zero);
            return;
        }
    }
}

