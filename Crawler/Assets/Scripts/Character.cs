using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, enemy0, enemy1, enemy2, enemy3, env }
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    // Attributes of characters (No need for public variables after testing)
    public bool npc;
    public bool ranged;
    public float projectileSpeed;
    public float attackAngle;
    public int attackRange;
    public int damage;
    public float attackInterval;
    public float attackTimer;
    public float speed;
    public int health;

    public GameObject projectileSpawn;
    public GameObject projectilePrefab;

    public Vector2 movement;

    [PunRPC]
    public void TakeDamage(int dmg) {
        print(gameObject);
        print("Health before damage " + health);
        if(npc)
            print("NPS is taking " + dmg + " damage!");
        else
            print("Player is taking " + dmg + " damage!");
        if(health - dmg <= 0) {
            if(npc) {
                Destroy(gameObject); // Does it show to all players?
            } else {
                print("Player should die!");
                // Do something to player
            }
        } else {
            // Still alive
            health -= dmg;
        }
        print("Health after damage " + health);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Check if collision is projectile and type of shooter
        if(collision.gameObject.CompareTag("Projectile")) {
            var projectile = collision.gameObject.GetComponent<Projectile>();
            if(npc != projectile.shotByNPC)
                TakeDamage(projectile.damage);
        }
    }

    [PunRPC]
    public void Shoot() {
        // Instantiate projectilePrefab clone
        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
        // Get projectile component of clone
        Projectile projectile = projectileClone.GetComponent<Projectile>();
        // Set projectile on its way
        projectile.LaunchProjectile(damage, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
    }

    [PunRPC]
    public void Melee() {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach(var hit in hits) {
            print(hit.gameObject);
        }
        foreach(var hit in hits) {
            var c = hit.gameObject.GetComponent<Character>();
            if(c != null && c.npc != npc) {
                if(npc) {
                    print("Player should take damage!");
                } else {
                    print("NPC should take damage!");
                }
                c.TakeDamage(damage);
            }
        }
        if(npc)
            print("NPC meleeing!");
        else
            print("Player meleeing!");
    }
}