using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }
[RequireComponent(typeof(PhotonView))]
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    // Attributes of characters (No need for public variables after testing)
    public bool npc;
    public bool ranged;
    public float projectileSpeed;
    public int projectilesPerAttack;
    public int weaponLevel;
    public float attackAngle;
    public int attackRange;
    public int damage;
    public float attackInterval;
    public float attackTimer;
    public float speed;

    [SerializeField]
    public int health;

    public GameObject projectileSpawn;
    public GameObject projectilePrefab;

    public Vector2 movement;

    public void TakeDamage(int dmg) {
        if (npc)
        {
            health -= dmg;
        }
        else{

                health -= dmg;

        }

        print("Health after damage " + health);
    }
    [PunRPC]
    public void Destroy()
    {
        if(gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (health < 0)
            {
                photonView.TransferOwnership(1);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        // Check if collision is projectile and type of shooter
        if(collision.gameObject.CompareTag("Projectile")) {
            var projectile = collision.gameObject.GetComponent<Projectile>();
            if(npc != projectile.shotByNPC)
                TakeDamage(projectile.damage);
                //photonView.RPC("TakeDamage", PhotonTargets.Others, projectile.damage);
        }
    }

    public void Attack() {
        if(ranged) {
            Shoot(projectilesPerAttack);
            photonView.RPC("Shoot", PhotonTargets.Others, projectilesPerAttack);
        } else {
            Melee();
            photonView.RPC("Melee", PhotonTargets.Others);
        }
        attackTimer = attackInterval; 
    }



    [PunRPC]
    public void Shoot(int amount) {


        if (amount % 2 == 0)
            {
                float startOffset = 0.25f * (amount - 1);
                for (int i = 0; i < amount; i++)
                {
                    GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
                    projectileClone.transform.parent = projectileSpawn.transform;
                    projectileClone.transform.localPosition = new Vector3(0f, startOffset - (i * 0.50f), 0f);
                    projectileClone.transform.parent = null;
                    Projectile projectile = projectileClone.GetComponent<Projectile>();
                    projectile.LaunchProjectile(damage, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
                }
            }
            else
            {
                float leftOffset = 0.50f;
                float rightOffset = 0.50f;
                for(int i = 0; i < amount; i++)
                {
                    if(i == 0)
                    {
                        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
                        projectileClone.transform.parent = projectileSpawn.transform;
                        projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
                        projectileClone.transform.parent = null;
                        Projectile projectile = projectileClone.GetComponent<Projectile>();
                        projectile.LaunchProjectile(damage, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
                    }
                    else if(i % 2 == 0)
                    {
                        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
                        projectileClone.transform.parent = projectileSpawn.transform;
                        projectileClone.transform.localPosition = new Vector3(0f, leftOffset, 0f);
                        projectileClone.transform.parent = null;
                        Projectile projectile = projectileClone.GetComponent<Projectile>();
                        projectile.LaunchProjectile(damage, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
                        leftOffset += 0.50f;
                    }
                    else
                    {
                        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
                        projectileClone.transform.parent = projectileSpawn.transform;
                        projectileClone.transform.localPosition = new Vector3(0f, -rightOffset, 0f);
                        projectileClone.transform.parent = null;
                        Projectile projectile = projectileClone.GetComponent<Projectile>();
                        projectile.LaunchProjectile(damage, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
                        rightOffset += 0.50f;
                     } 
                }
            }
        }

    

    [PunRPC]
    public void Melee() {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach(var hit in hits) {
            //print(hit.gameObject);

        }
        foreach(var hit in hits) {
            var c = hit.gameObject.GetComponent<Character>();
            if (c != null && !c.npc)
            {
                Debug.Log(hit.gameObject);
            }
            if (c != null && c.npc != npc) {
                if(npc) {
                    //print("Player should take damage!");
                } else {
                    //print("NPC should take damage!");
                }
                    c.TakeDamage(damage);
            }
        }
        //if(npc)
            //print("NPC meleeing!");
        //else
            //print("Player meleeing!");
    }

    #region Set attributes
    public void SetCharacterAttributes() {
        if(characterType == EntityType.Hero0) {
            ranged = true;
            damage = 20;
            attackRange = 20;
            weaponLevel = 0;
            projectileSpeed = 5f;
            projectilesPerAttack = 1;
            attackAngle = 0;
            attackInterval = .5f;
            speed = 10;
            health = 150;
        }
        if(characterType == EntityType.Hero1) {
            ranged = true;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 5f;
            projectilesPerAttack = 1;
            attackAngle = 0f;
            attackInterval = 0.5f;
            speed = 7.5f;
            health = 200;
        }
        if(characterType == EntityType.Hero2) {
            ranged = false;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 0f;
            attackAngle = 90;
            attackInterval = 0.5f;
            speed = 25;
            health = 250;
        }
        if(characterType == EntityType.Hero3) {
            ranged = false;
            damage = 100;
            attackRange = 5;
            projectileSpeed = 0f;
            attackAngle = 90;
            attackInterval = 1f;
            speed = 10;
            health = 300;
        }
        if(characterType == EntityType.Enemy0) {
            ranged = false;
            damage = 20;
            attackRange = 3;
            projectileSpeed = 0;
            projectilesPerAttack = 0;
            attackAngle = 0;
            attackInterval = .5f;
            speed = 6.5f;
            health = 20;
        }
        if(characterType == EntityType.Enemy1) {
            ranged = true;
            damage = 50;
            attackRange = 10;
            projectileSpeed = 5f;
            projectilesPerAttack = 1;
            attackAngle = 0f;
            attackInterval = 0.5f;
            speed = 7.5f;
            health = 60;
        }
        if(characterType == EntityType.Enemy2) {
            ranged = false;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 0f;
            projectilesPerAttack = 1;
            attackAngle = 90;
            attackInterval = 0.5f;
            speed = 25;
            health = 250;
        }
        if(characterType == EntityType.Enemy3) {
            ranged = false;
            damage = 100;
            attackRange = 5;
            projectileSpeed = 0f;
            attackAngle = 90;
            attackInterval = 1f;
            speed = 10;
            health = 300;
        }
    }

    #endregion
}