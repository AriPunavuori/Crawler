using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage;
    public float speed;
    public float range;
    public bool npc;

    Vector2 origPos;
    Vector2 direction;

    public bool impulse;
    public bool explosive;
    public bool reflective;
    public bool homing;
    public GameObject target;

    Rigidbody2D rb2D;
    public GameObject particles;
    public GameObject graphics;

    float timeToDestroy = 3.5f;
    float launchTime;
    bool launched = false;

    private void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="d">Damage</param>
    /// <param name="r">Range</param>
    /// <param name="s">Speed</param>
    /// <param name="dir">Direction</param>
    /// <param name="shotByNPC">True if shot by npc</param>
    public void LaunchProjectile(int d, float r, float s, Vector2 dir, bool shotByNPC) {
        // Set original position
        origPos = transform.position;
        // Set damage of the projectile
        damage = d;
        // Set range
        range = r;
        // Set speed
        speed = s;
        // Set direction
        direction = dir;
        // Set npc
        npc = shotByNPC;
        // Set Launchtime
        launchTime = Time.time;
        if(impulse)
            rb2D.AddForce(dir * s, ForceMode2D.Impulse);
        if(npc) {
            var random = Random.Range(0, 4);
            AudioFW.Play("EnemyShoot" + random);
        } else {
            var random = Random.Range(0, 4);
            AudioFW.Play("PlayerShoot" + random);
        }
        launched = true;
        Debug.Log("launched");
    }

    void FixedUpdate() {

        if(launched)
        {
            if (Vector2.Distance(transform.position, origPos) > range || Time.time > launchTime + timeToDestroy)
            {
                DestroyProjectile();
            }
        }
        
        if(!impulse) {
            rb2D.velocity = Vector2.zero;
            rb2D.velocity = direction * Time.fixedDeltaTime * speed * 50;
        }
        if(homing)
        {
            direction = (target.transform.position - transform.position).normalized;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        IDamageable<int> iDamageable = collision.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
        if(iDamageable != null) {
            Vector3 recoilVector = rb2D.velocity.normalized;
            iDamageable.TakeDamage(damage, recoilVector);
            DestroyProjectile();
            return;
        }


        if(!reflective) {
            DestroyProjectile();
        } else {
            // Reflect from colliding object with proper angle
            var contact = collision.GetContact(0);
            float dn = 2 * Vector2.Dot(direction, contact.normal);
            var reflection = direction - contact.normal * dn;
            direction = reflection;
            transform.right = direction;
            //reflective = false;
            impulse = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "BossShield")
        {
            DestroyProjectile();
        }
    }

    public void DestroyProjectile() {
        if(gameObject != null) {
            var random = Random.Range(0, 4);
            AudioFW.Play("Hit" + random);
            if(particles != null) {
                particles.SetActive(true);
                particles.transform.parent = null;
            }
            graphics.SetActive(false);
            Destroy(gameObject);
        }
    }
}
