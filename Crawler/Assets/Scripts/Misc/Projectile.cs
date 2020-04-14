using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage;
    public float speed;
    private float range;

    Vector2 origPos;
    Vector2 direction;

    public bool impulse;
    public bool explosive;
    public bool reflective;

    Rigidbody2D rb2D;
    public GameObject particles;
    public GameObject graphics;

    private void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
    }
    public void LaunchProjectile(int d, float r, float s, Vector2 dir) {
        // Set original position
        origPos = transform.position;
        // Set damage of the projectile
        if(PhotonNetwork.isMasterClient) {
            damage = d;
        } else
            damage = 0;
        // Set range
        range = r;
        // Set speed
        speed = s;
        // Set direction
        direction = dir;

        if(impulse)
        rb2D.AddForce(dir * s, ForceMode2D.Impulse);
        AudioFW.Play("Shot");
    }

    void Update() {
        if(Vector2.Distance(transform.position, origPos) > range) {
            DestroyProjectile();
        }
        if(!impulse) {
            rb2D.velocity = Vector2.zero;
            rb2D.velocity = direction * Time.deltaTime * speed * 250;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(iDamageable != null) {
            iDamageable.TakeDamage(damage);
            DestroyProjectile();
        }


        if(!reflective)
            DestroyProjectile();
        else {
            // Reflect from colliding object with proper angle
            var contact = collision.GetContact(0);
            float dn = 2 * Vector2.Dot(direction, contact.normal);
            var reflection = direction - contact.normal * dn;
            direction = reflection;
            transform.right = direction;
            reflective = false;
            impulse = false;
        }
    }

    public void DestroyProjectile() {
        //AudioFW.Play("Hit");
        particles.SetActive(true);
        graphics.SetActive(false);
        Destroy(gameObject, 0.5f);
    }
}
