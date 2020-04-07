using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage;
    private float timer = 1.0f;
    public bool shotByNPC;
    public bool explosive;
    public bool reflective;

    public void LaunchProjectile(int d, float s, bool npc, Vector2 dir) {
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();
        // Set damage of the projectile
        damage = d;
        // Set shooter type
        shotByNPC = npc;
        rb2D.AddForce(dir * s, ForceMode2D.Impulse);
    }

    void Update() {
        timer -= Time.deltaTime;
        if(timer < 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(!reflective)
            Destroy(gameObject);
        else {
            // Reflect from colliding object with proper angle
        }
    }
}
